using System;
using Godot;
using VampireSurvivorsLike.ItemDrops;

namespace VampireSurvivorsLike {

    public class Player : KinematicBody2D {

        public Vector2 Direction;
        public bool IsDead { get; private set; } = false;
        public ItemManager ItemManager { get; private set; }

        private int currentHealth;
        private int healthCounter = 0;
        private int damageCounter = 0;
        private const int ImmunityTime = 25;
        private const int PassiveHealTime = 100;
        private int takenDamageValue = 0;

        private AnimatedSprite animatedSprite;
        private TextureProgress healthBar;
        private readonly Texture[] textures = new Texture[3];
        private Sprite directionArrow;
        private GUI gui;


        public GUI Gui {
            get => this.gui;
            set {
                this.gui = value;
                this.AddChild(this.gui);
            }
        }

        [Signal] public delegate void OnPlayerDeath();

        public override void _Ready() {
            GD.Print("Player Ready...");
            this.animatedSprite = this.GetNode<AnimatedSprite>("AnimatedSprite");
            this.healthBar = this.GetNode<TextureProgress>("Node2D/HealthBar");
            this.textures[0] = ResourceLoader.Load("res://Textures/bar_green_mini.png") as Texture;
            this.textures[1] = ResourceLoader.Load("res://Textures/bar_yellow_mini.png") as Texture;
            this.textures[2] = ResourceLoader.Load("res://Textures/bar_red_mini.png") as Texture;
            this.directionArrow = this.GetNode<Sprite>("Arrow");
            this.ItemManager = GetNode<ItemManager>("ItemManager");

            this.currentHealth = AttributeManagerSingleton.Instance.MaxHealth.GetCurrentValue();
            this.Direction = Vector2.Right;
            this.directionArrow.Position = new Vector2(0, -8) + this.Direction * 20;

            if (!GameStateManagerSingleton.Instance.IsMultiplayer || this.IsNetworkMaster()) {
                this.ItemManager.EquipOrUpgradeItem(0);
                if (GameStateManagerSingleton.Instance.IsMultiplayer) {
                    this.GetNode<Label>("Label").Text = "You";
                }

                //Set hud initial values
                this.gui.SetCurrentHealth(this.currentHealth);
                this.gui.SetCurrentExperience(0);
                this.gui.SetCurrentLevel(0);
                this.gui.SetGoldCount(0);
            }
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta) {
            if (GameStateManagerSingleton.Instance.IsMultiplayer && !this.IsNetworkMaster()) {
                return;
            }
            this.Move();
            if (this.takenDamageValue > 0 && !this.IsDead) {
                this.TakeDamage();
            }

            if (GameStateManagerSingleton.Instance.IsMultiplayer && this.IsNetworkMaster()) {
                this.RpcUnreliable(nameof(this.MovePuppet), this.GlobalPosition, this.Direction);
            }
            if (this.currentHealth < AttributeManagerSingleton.Instance.MaxHealth.GetCurrentValue() && !this.IsDead) {
                this.PassiveHeal();
            }
        }

        /*
         * Get the movement direction from the keyboard and set the velocity and animation
         * according to the direction.
         */
        private void Move() {
            AnimationsEnum animation = AnimationsEnum.Idle;
            int xInput = (Input.IsActionPressed("ui_right") ? 1 : 0) - (Input.IsActionPressed("ui_left") ? 1 : 0);
            int yInput = (Input.IsActionPressed("ui_down") ? 1 : 0) - (Input.IsActionPressed("ui_up") ? 1 : 0);
            Vector2 velocity = new Vector2(xInput, yInput).Normalized();
            if (!Input.IsActionPressed("ui_hold") && !velocity.Equals(Vector2.Zero)) {
                this.Direction = velocity;
                this.directionArrow.Rotation = this.Direction.Angle();
                this.directionArrow.Position = new Vector2(0, -8) + this.Direction * 20;
            }

            if (xInput != 0) {
                animation = xInput < 0 ? AnimationsEnum.Left : AnimationsEnum.Right;
            } else if (yInput != 0) {
                animation = yInput < 0 ? AnimationsEnum.Up : AnimationsEnum.Down;
            } else if (this.Direction.x != 0) {
                animation = this.Direction.x < 0 ? AnimationsEnum.IdleLeft : AnimationsEnum.IdleRight;
            } else if (this.Direction.y != 0) {
                animation = this.Direction.y < 0 ? AnimationsEnum.IdleUp : AnimationsEnum.IdleDown;
            }
            this.animatedSprite.Play(animation.ToString());
            this.MoveAndSlide(velocity.Normalized() * AttributeManagerSingleton.Instance.Speed.GetCurrentValue());
        }

        [Puppet]
        public void MovePuppet(Vector2 globalPosition, Vector2 direction) {
            this.Direction = direction;
            this.directionArrow.Rotation = this.Direction.Angle();
            this.directionArrow.Position = new Vector2(0f, -6f) + this.Direction * 15;

            AnimationsEnum animation = AnimationsEnum.Idle;
            if (this.Direction.x != 0) {
                animation = this.Direction.x < 0 ? AnimationsEnum.IdleLeft : AnimationsEnum.IdleRight;
            } else if (this.Direction.y != 0) {
                animation = this.Direction.y < 0 ? AnimationsEnum.IdleUp : AnimationsEnum.IdleDown;
            }
            if (globalPosition - this.GlobalPosition != Vector2.Zero) {
                if (direction.x == 0 && direction.y != 0) {
                    animation = (direction.y > 0 ? AnimationsEnum.Down : AnimationsEnum.Up);
                } else if (direction.x != 0) {
                    animation = (direction.x > 0 ? AnimationsEnum.Right : AnimationsEnum.Left);
                }
            }
            this.animatedSprite.Play(animation.ToString());
            this.GlobalPosition = globalPosition;
        }

        /*
         * Update the health bar depending on the current health
         * Change the health bar color according to its value
         */
        private void UpdateHealth() {
            if (!GameStateManagerSingleton.Instance.IsMultiplayer || this.IsNetworkMaster()) {
                this.Gui.SetCurrentHealth(this.currentHealth);
            }
            this.healthBar.Value =
                Math.Round((float)this.currentHealth / AttributeManagerSingleton.Instance.MaxHealth.GetCurrentValue()
                           * 100, 2);
            if (this.healthBar.Value < 100 && this.healthBar.TextureProgress_ != this.textures[0]) {
                this.healthBar.TextureProgress_ = this.textures[0];
            }
            if (this.healthBar.Value < 50 && this.healthBar.TextureProgress_ != this.textures[1]) {
                this.healthBar.TextureProgress_ = this.textures[1];
            }
            if (this.healthBar.Value < 25 && this.healthBar.TextureProgress_ != this.textures[2]) {
                this.healthBar.TextureProgress_ = this.textures[2];
            }
        }

        /*
         * If an enemy overlaps with the player the enemy's damage value is removed from
         * the player's health. If there are multiple enemies their damage values are added
         * together.
         */
        private void TakeDamage() {
            if (this.damageCounter++ > ImmunityTime) {
                this.damageCounter = 0;
                if (GameStateManagerSingleton.Instance.IsMultiplayer) {
                    this.Rpc(nameof(this.SyncTakeDamage), this.takenDamageValue);
                } else {
                    this.SyncTakeDamage(this.takenDamageValue);
                }
            }
            if (this.currentHealth > 0) {
                return;
            }
            if (!GameStateManagerSingleton.Instance.IsMultiplayer) {
                AudioPlayerSingleton.Instance.PlayEffect(AudioEffectEnum.Death);
                this.IsDead = true;
                this.EmitSignal(nameof(OnPlayerDeath));
            } else {
                this.Rpc(nameof(this.SyncDeath));
            }
        }

        [PuppetSync]
        public void SyncDeath() {
            AudioPlayerSingleton.Instance.PlayEffect(AudioEffectEnum.Death);
            this.animatedSprite.SelfModulate = new Color(0, 0.91f, 1f, 0.28f);
            this.healthBar.Visible = false;
            this.IsDead = true;
            this.currentHealth = AttributeManagerSingleton.Instance.MaxHealth.GetCurrentValue();
            this.GetNode<Area2D>("PickupArea").QueueFree();
            this.GetNode<CollisionShape2D>("CollisionShape2D").QueueFree();
            this.GetNode<Area2D>("Area2D").QueueFree();
            this.GetNode<Area2D>("PickupArea").QueueFree();
            this.GetNode("ItemManager").QueueFree();
            EmitSignal(nameof(OnPlayerDeath));
        }

        [PuppetSync]
        public void SyncTakeDamage(int damage) {
            AudioPlayerSingleton.Instance.PlayEffect(AudioEffectEnum.PlayerHit);
            FloatingValue.CreateFloatingValue(this.GlobalPosition, new Color(0.96f, 0.14f, 0.14f), damage,
                this.GetTree().Root.GetNode("Main"));
            this.currentHealth = Math.Max(0, this.currentHealth - damage);
            this.UpdateHealth();
        }

        /*
         * After every ImmunityTime seconds increase the Health by HealthRegen value while
         * the MaxHealth is higher than the CurrentHealth.
         */
        private void PassiveHeal() {
            if (GameStateManagerSingleton.Instance.IsMultiplayer && !this.IsNetworkMaster() ||
                this.healthCounter++ < PassiveHealTime) {
                return;
            }
            this.healthCounter = 0;
            if (GameStateManagerSingleton.Instance.IsMultiplayer) {
                this.RpcUnreliable(nameof(this.SyncPassiveHeal),
                    AttributeManagerSingleton.Instance.HealthRegen.GetCurrentValue());
            } else {
                this.SyncPassiveHeal(AttributeManagerSingleton.Instance.HealthRegen.GetCurrentValue());
            }
        }

        [PuppetSync]
        public void SyncPassiveHeal(int healingValue) {
            FloatingValue.CreateFloatingValue(this.GlobalPosition, new Color(0.53f, 0.88f, 0.38f), healingValue,
                this.GetTree().Root.GetNode("Main"));
            this.currentHealth = Math.Min(AttributeManagerSingleton.Instance.MaxHealth.GetCurrentValue(),
                this.currentHealth + AttributeManagerSingleton.Instance.HealthRegen.GetCurrentValue());
            this.UpdateHealth();
        }

        /**
         * Connected from ItemDrop.cs
         */
        public void OnPickedUp(int value, ItemDropsEnum type) {
            if (GameStateManagerSingleton.Instance.IsMultiplayer && !this.GetTree().IsNetworkServer()) {
                return;
            }
            if (type.Equals(ItemDropsEnum.ExperienceOrb)) {
                if (GameStateManagerSingleton.Instance.IsMultiplayer) {
                    this.GetTree().Root.GetNode<Main>("Main").Rpc(nameof(Main.IncreaseExperience), value);
                } else {
                    this.GetTree().Root.GetNode<Main>("Main").IncreaseExperience(value);
                }
            } else if (type.Equals(ItemDropsEnum.Gold)) {
                if (GameStateManagerSingleton.Instance.IsMultiplayer) {
                    this.GetTree().Root.GetNode<Main>("Main").Rpc(nameof(Main.AddGold), value);
                } else {
                    this.GetTree().Root.GetNode<Main>("Main").AddGold(value);
                }
            }
        }

        /*
         * When an enemy overlaps with the player, the Strength value of the enemy is added to the TakenDamageValue field.
         * This field's value is then subtracted from the player's health, thus hurting the player.
         */
        public void OnBodyEntered(Node body) {
            if (!(body is Enemy enemy) ||
                (GameStateManagerSingleton.Instance.IsMultiplayer && !this.IsNetworkMaster())) {
                return;
            }
            this.takenDamageValue += enemy.Strength;
        }

        /*
         * When an enemy leave the player, its Strength value is subtracted from the TakenDamageValue field.
         */
        public void OnBodyExited(Node body) {
            if (!(body is Enemy enemy) ||
                (GameStateManagerSingleton.Instance.IsMultiplayer && !this.IsNetworkMaster())) {
                return;
            }
            this.takenDamageValue -= enemy.Strength;
        }

    }

}