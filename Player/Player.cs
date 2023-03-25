using System;
using System.Threading.Tasks;
using Godot;
using VampireSurvivorsLike.ItemDrops;
using Thread = System.Threading.Thread;

namespace VampireSurvivorsLike {

    public class Player : KinematicBody2D {

        private int currentHealth;

        //Counters
        private int healthCounter = 0;
        private int damageCounter = 0;
        private const int ImmunityTime = 25;
        private const int PassiveHealTime = 100;
        private Vector2 Direction { get; set; }
        private float takenDamageValue = 0;

        private float experience = 0;
        private int currentLevel = 0;

        private AnimatedSprite animatedSprite;
        private TextureProgress healthBar;
        private CircleShape2D pickupArea;
        private Texture[] textures = new Texture[3];
        private Sprite directionArrow;

        // private PackedScene FloatingValue { get; set; }
        private GUI gui;
        public int Gold { get; private set; } = 0;
        public int EnemiesDefeated { get; private set; } = 0;
        public bool IsDead { get; private set; } = false;
        public ItemManager ItemManager { get; private set; }

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
            this.currentHealth = AttributeManagerSingleton.Instance.MaxHealth.GetCurrentValue();
            this.Direction = Vector2.Right;
            this.animatedSprite = this.GetNode<AnimatedSprite>("AnimatedSprite");
            this.healthBar = this.GetNode<TextureProgress>("Node2D/HealthBar");
            this.textures[0] = ResourceLoader.Load("res://Textures/bar_green_mini.png") as Texture;
            this.textures[1] = ResourceLoader.Load("res://Textures/bar_yellow_mini.png") as Texture;
            this.textures[2] = ResourceLoader.Load("res://Textures/bar_red_mini.png") as Texture;

            // this.FloatingValue = ResourceLoader.Load<PackedScene>("res://GUI/GUI/FloatingValue.tscn");
            this.directionArrow = this.GetNode<Sprite>("Arrow");
            GetNode<Label>("Label").Text = this.Name;
            this.ItemManager = GetNode<ItemManager>("ItemManager");

            if (!GameStateManagerSingleton.Instance.IsMultiplayer || this.IsNetworkMaster()) {
                this.ItemManager.EquipOrUpgradeItem(1);
                this.ItemManager.EquipOrUpgradeItem(0);

                //Set hud initial values
                this.gui.SetCurrentHealth(this.currentHealth);
                this.gui.SetCurrentExperience(0);
                this.gui.SetCurrentLevel(0);
                this.gui.SetGoldCount(0);

                // AttributeManagerSingleton.Instance.SetPickupArea(this.GetNode<Area2D>("PickupArea").GetChild<CollisionShape2D>(0).Shape as CircleShape2D);
            }

            //JUST FOR TESTING, REMOVE LATER: THIS FOLLOWS THE HOST PLAYER ON BOTH GAME INSTANCES
            if (GameStateManagerSingleton.Instance.IsMultiplayer && !this.GetTree().IsNetworkServer()) {
                if (this.IsNetworkMaster()) {
                    GetNode<Camera2D>("Camera2D").Current = false;
                    GetParent<Main>().playerTwo.GetNode<Camera2D>("Camera2D").Current = true;
                } else {
                    GetParent<Main>().playerTwo.GetNode<Camera2D>("Camera2D").Current = true;
                }
            }
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta) {
            if (GameStateManagerSingleton.Instance.IsMultiplayer && !this.IsNetworkMaster()) {
                return;
            }
            this.Move();
            if (GameStateManagerSingleton.Instance.IsMultiplayer && this.IsNetworkMaster()) {
                //Update puppets
                this.RpcUnreliable(nameof(this.MovePuppet), this.GlobalPosition, this.Direction);
            }
            if (this.takenDamageValue > 0 && !this.IsDead) {
                this.TakeDamage();
            }
            if (this.currentHealth < AttributeManagerSingleton.Instance.MaxHealth.GetCurrentValue()) {
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
            if (!Input.IsActionPressed("ui_space") && !velocity.Equals(Vector2.Zero)) {
                this.Direction = velocity;
            }
            if (velocity.x == 0 && velocity.y != 0) {
                animation = (velocity.y > 0 ? AnimationsEnum.Down : AnimationsEnum.Up);
            } else if (velocity.x != 0) {
                animation = (velocity.x > 0 ? AnimationsEnum.Right : AnimationsEnum.Left);
            }

            this.directionArrow.Rotation = this.Direction.Angle();
            this.directionArrow.Position = new Vector2(0f, -6f) + this.Direction * 15;
            this.animatedSprite.Play(animation.ToString());
            this.MoveAndSlide(velocity.Normalized() * AttributeManagerSingleton.Instance.Speed.GetCurrentValue());
        }

        [Puppet]
        public void MovePuppet(Vector2 globalPosition, Vector2 direction) {
            this.Direction = direction;
            this.directionArrow.Rotation = this.Direction.Angle();
            this.directionArrow.Position = new Vector2(0f, -6f) + this.Direction * 15;

            AnimationsEnum animation = AnimationsEnum.Idle;
            Vector2 velocity = (globalPosition - this.GlobalPosition).Normalized();
            if (velocity.x == 0 && velocity.y != 0) {
                animation = (velocity.y > 0 ? AnimationsEnum.Down : AnimationsEnum.Up);
            } else if (velocity.x != 0) {
                animation = (velocity.x > 0 ? AnimationsEnum.Right : AnimationsEnum.Left);
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
         * Emits a signal to refresh the Health Counter using [CurrentHealth] and updates the
         * Health Bar
         */
        private void TakeDamage() {
            this.damageCounter++;
            if (this.damageCounter % ImmunityTime == 0) {
                FloatingValue.CreateFloatingValue(this.GlobalPosition, new Color(0.96f, 0.14f, 0.14f),
                    (int)this.takenDamageValue, this.GetParent());
                this.damageCounter = 0;
                this.currentHealth = Math.Max(0, this.currentHealth - (int)this.takenDamageValue);
                this.UpdateHealth();
                if (GameStateManagerSingleton.Instance.IsMultiplayer) {
                    RpcUnreliable(nameof(this.PuppetTakeDamage), this.takenDamageValue, this.currentHealth);
                }
            }

            if (this.currentHealth <= 0) {
                AudioPlayerSingleton.Instance.PlayEffect(AudioPlayerSingleton.EffectEnum.Death);
                if (!GameStateManagerSingleton.Instance.IsMultiplayer) {
                    this.IsDead = true;
                    EmitSignal(nameof(OnPlayerDeath));
                } else {
                    Rpc(nameof(this.PuppetDeath));
                }
            }
        }

        [PuppetSync]
        public void PuppetDeath() {
            this.animatedSprite.SelfModulate = new Color(0, 0.91f, 1f, 0.28f);
            this.healthBar.Visible = false;
            this.IsDead = true;
            EmitSignal(nameof(OnPlayerDeath));
        }

        [Puppet]
        public void PuppetTakeDamage(float damage, int currentHealth) {
            FloatingValue.CreateFloatingValue(this.GlobalPosition, new Color(0.96f, 0.14f, 0.14f), (int)damage,
                this.GetParent());
            this.currentHealth = currentHealth;
            this.UpdateHealth();
        }

        /*
         * After every ImmunityTime seconds increase the Health by HealthRegen value while
         * the MaxHealth is higher than the CurrentHealth.
         */
        private void PassiveHeal() {
            if (GameStateManagerSingleton.Instance.IsMultiplayer && !this.IsNetworkMaster()) {
                return;
            }
            this.healthCounter++;
            if (this.healthCounter % PassiveHealTime != 0) {
                return;
            }
            this.healthCounter = 0;
            FloatingValue.CreateFloatingValue(this.GlobalPosition, new Color(0.53f, 0.88f, 0.38f),
                AttributeManagerSingleton.Instance.HealthRegen.GetCurrentValue(), this.GetParent());
            this.currentHealth = Math.Min(AttributeManagerSingleton.Instance.MaxHealth.GetCurrentValue(),
                AttributeManagerSingleton.Instance.HealthRegen.GetCurrentValue() + this.currentHealth);
            this.UpdateHealth();
            if (GameStateManagerSingleton.Instance.IsMultiplayer) {
                RpcUnreliable(nameof(this.PuppetPassiveHeal),
                    AttributeManagerSingleton.Instance.HealthRegen.GetCurrentValue(),
                    this.currentHealth);
            }
        }

        [Puppet]
        public void PuppetPassiveHeal(int healingValue, int currentHealth) {
            FloatingValue.CreateFloatingValue(this.GlobalPosition, new Color(0.53f, 0.88f, 0.38f), healingValue,
                this.GetParent());
            this.currentHealth = currentHealth;
            this.UpdateHealth();
        }


        /*
         * This method checks if a new level is reached. If the Player gains enough experience to level multiple
         * times the level's rewards are given after each other. The Player can select between two types of rewards
         * Either upgrade an item or increase one Status.
         * After successfully leveling up the CurrentLevel and the XP bar are set to the correct values
         */
        // private async void CheckLevelUp() {
        //     if (GameStateManagerSingleton.Instance.GameState.Equals(GameStateEnum.Leveling)) {
        //         return;
        //     }
        //     if (Main.ExpToLvl(this.experience) > this.currentLevel) {
        //         this.GetTree().Paused = true;
        //         int levelIncrease = Main.ExpToLvl(this.experience) - this.currentLevel;
        //         GameStateManagerSingleton.Instance.GameState = GameStateEnum.Leveling;
        //
        //         await LevelUpManagerSingleton.Instance.OnPlayerLevelUp(levelIncrease);
        //
        //         this.GetTree().Paused = false;
        //         this.currentLevel += levelIncrease;
        //         this.gui.SetCurrentLevel(this.currentLevel);
        //         GameStateManagerSingleton.Instance.GameState = GameStateEnum.Playing;
        //     }
        //     int currentExpInLevel = (int)(100 * (this.experience - Main.LvlToExp(this.currentLevel)) /
        //                                   (Main.LvlToExp(this.currentLevel + 1) -
        //                                    Main.LvlToExp(this.currentLevel)));
        //     this.gui.SetCurrentExperience(currentExpInLevel);
        // }

        /*
         * After picking up an ExpOrb the xp of the orb is added to the player's xp.
         * Refreshes the xp using [CurrentExperience]
         */
        public void OnPickedUp(int value, ItemDropsEnum type) {
            if (GameStateManagerSingleton.Instance.IsMultiplayer && !this.GetTree().IsNetworkServer()) {
                return;
            }
            if (type.Equals(ItemDropsEnum.ExperienceOrb)) {
                if (GameStateManagerSingleton.Instance.IsMultiplayer) {
                    this.GetParent<Main>().Rpc(nameof(Main.IncreaseExperience), value);
                } else {
                    this.GetParent<Main>().IncreaseExperience(value);
                }
            } else if (type.Equals(ItemDropsEnum.Gold)) {
                if (GameStateManagerSingleton.Instance.IsMultiplayer) {
                    this.GetParent<Main>().Rpc(nameof(Main.MultiplayerAddGold), value);
                } else {
                    this.GetParent<Main>().MultiplayerAddGold(value);
                }
            }
        }

        /*
         * When an enemy overlaps with the player, the Strength value of the enemy is added to the TakenDamageValue field.
         * This field's value is then subtracted from the player's health, thus hurting the player.
         */
        public void OnBodyEntered(Node body) {
            if (GameStateManagerSingleton.Instance.IsMultiplayer && !this.IsNetworkMaster()) {
                return;
            }
            if (body is Enemy enemy) {
                this.takenDamageValue += enemy.Strength;
            }
        }

        /*
         * When an enemy leave the player, its Strength value is subtracted from the TakenDamageValue field.
         */
        public void OnBodyExited(Node body) {
            if (GameStateManagerSingleton.Instance.IsMultiplayer && !this.IsNetworkMaster()) {
                return;
            }
            if (body is Enemy enemy) {
                this.takenDamageValue -= enemy.Strength;
            }
        }

    }

}