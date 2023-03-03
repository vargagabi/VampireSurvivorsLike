using System;
using System.Threading.Tasks;
using Godot;
using Thread = System.Threading.Thread;

namespace VampireSurvivorsLike {

    public class Player : KinematicBody2D {

        private float currentHealth;

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
        private PackedScene FloatingValue { get; set; }


        [Signal] public delegate void CurrentHealth(float currentHealth);
        [Signal] public delegate void CurrentExperience(float exp, int level);

        [Signal] public delegate void ExperienceInPercent(int percent);

        public override void _Ready() {
            GD.Print("Player Ready...");
            this.currentHealth = AttributeManagerSingleton.Instance.MaxHealth.GetCurrentValue();
            this.Direction = Vector2.Right;
            this.animatedSprite = this.GetNode<AnimatedSprite>("AnimatedSprite");
            this.healthBar = this.GetNode<TextureProgress>("Node2D/HealthBar");
            this.textures[0] = ResourceLoader.Load("res://Textures/bar_green_mini.png") as Texture;
            this.textures[1] = ResourceLoader.Load("res://Textures/bar_yellow_mini.png") as Texture;
            this.textures[2] = ResourceLoader.Load("res://Textures/bar_red_mini.png") as Texture;
            this.directionArrow = this.GetNode<Sprite>("Arrow");

            AttributeManagerSingleton.Instance.SetPickupArea(
                this.GetNode<Area2D>("PickupArea").GetChild<CollisionShape2D>(0).Shape as CircleShape2D);
            this.FloatingValue = ResourceLoader.Load<PackedScene>("res://GUI/GUI/FloatingValue.tscn");

            ItemManagerSingleton.Instance.Player = this;
            ItemManagerSingleton.Instance.EquipOrUpgradeItem(ItemManagerSingleton.Instance.GetUnequippedItems()[0]);
            ItemManagerSingleton.Instance.EquipOrUpgradeItem(ItemManagerSingleton.Instance.GetUnequippedItems()[0]);
            ItemManagerSingleton.Instance.EquipOrUpgradeItem(ItemManagerSingleton.Instance.GetUnequippedItems()[0]);

            //Emit signals to set the HUD health and level bars
            this.EmitSignal(nameof(CurrentHealth), this.currentHealth);
            this.EmitSignal(nameof(CurrentExperience), this.experience, this.currentLevel);
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta) {
            this.Move();
            if (this.takenDamageValue > 0) {
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
            bool isSpacePressed = Input.IsActionPressed("ui_space");
            Vector2 velocity = Vector2.Zero;
            if (Input.IsActionPressed("ui_down")) {
                velocity += Vector2.Down;
                this.Direction += isSpacePressed ? Vector2.Zero : Vector2.Down;
            }
            if (Input.IsActionPressed("ui_up")) {
                velocity += Vector2.Up;
                this.Direction += isSpacePressed ? Vector2.Zero : Vector2.Up;
            }
            if (Input.IsActionPressed("ui_left")) {
                velocity += Vector2.Left;
                this.Direction += isSpacePressed ? Vector2.Zero : Vector2.Left;
            }
            if (Input.IsActionPressed("ui_right")) {
                velocity += Vector2.Right;
                this.Direction += isSpacePressed ? Vector2.Zero : Vector2.Right;
            }
            if (velocity.x == 0 && velocity.y != 0) {
                animation = (velocity.y > 0 ? AnimationsEnum.Down : AnimationsEnum.Up);
            } else if (velocity.x != 0) {
                animation = (velocity.x > 0 ? AnimationsEnum.Right : AnimationsEnum.Left);
            }

            this.directionArrow.Rotation = this.Direction.Normalized().Angle();
            this.directionArrow.Position = new Vector2(0f, -6f) + this.Direction.Normalized() * 15;
            this.Direction = this.Direction.Normalized();
            this.animatedSprite.Play(animation.ToString());
            this.MoveAndSlide(velocity.Normalized() * AttributeManagerSingleton.Instance.Speed.GetCurrentValue());
        }

        /*
         * Update the health bar depending on the current health
         * Change the health bar color according to its value
         */
        private void UpdateHealthBar() {
            this.healthBar.Value =
                (this.currentHealth / AttributeManagerSingleton.Instance.MaxHealth.GetCurrentValue()) * 100;
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
                FloatingValue damageInd = this.FloatingValue.Instance<FloatingValue>();
                damageInd.SetValues(this.GlobalPosition, new Color(0.96f, 0.14f, 0.14f), (int)this.takenDamageValue);
                this.GetTree().Root.GetNode("Main").CallDeferred("add_child", damageInd);
                this.damageCounter = 0;
                this.currentHealth = Math.Max(0, this.currentHealth - (int)this.takenDamageValue);
                this.EmitSignal(nameof(CurrentHealth), this.currentHealth);
                this.UpdateHealthBar();
            }

            if (this.currentHealth <= 0) {
                this.GetTree().Paused = true;
                AudioPlayerSingleton.Instance.SwitchToAmbient();
                AudioPlayerSingleton.Instance.PlayEffect(AudioPlayerSingleton.EffectEnum.Death);
                GameStateManagerSingleton.Instance.GameState = GameStateEnum.GameEnd;
            }
        }

        /*
         * After every ImmunityTime seconds increase the Health by HealthRegen value while
         * the MaxHealth is higher than the CurrentHealth.
         */
        private void PassiveHeal() {
            this.healthCounter++;
            if (this.healthCounter % PassiveHealTime == 0) {
                FloatingValue healingInd = this.FloatingValue.Instance<FloatingValue>();
                healingInd.SetValues(this.GlobalPosition, new Color(0.53f, 0.88f, 0.38f, 1f),
                    (int)AttributeManagerSingleton.Instance.HealthRegen.GetCurrentValue());
                this.GetTree().Root.GetNode("Main").CallDeferred("add_child", healingInd);
                this.healthCounter = 0;
                this.currentHealth = Math.Min(AttributeManagerSingleton.Instance.MaxHealth.GetCurrentValue(),
                    AttributeManagerSingleton.Instance.HealthRegen.GetCurrentValue() + this.currentHealth);
                this.EmitSignal(nameof(CurrentHealth), this.currentHealth);
                this.UpdateHealthBar();
            }
        }

        /*
         * When an enemy overlaps with the player, the Strength value of the enemy is added to the TakenDamageValue field.
         * This field's value is then subtracted from the player's health, thus hurting the player.
         */
        public void OnBodyEntered(Node body) {
            float dmg = (float)(body.Get("Strength"));
            this.takenDamageValue += dmg;
        }

        /*
         * When an enemy leave the player, its Strength value is subtracted from the TakenDamageValue field.
         */
        public void OnBodyExited(Node body) {
            float dmg = (float)(body.Get("Strength"));
            this.takenDamageValue -= dmg;
        }

        /*
         * This method checks if a new level is reached. If the Player gains enough experience to level multiple
         * times the level's rewards are given after each other. The Player can select between two types of rewards
         * Either upgrade an item or increase one Status.
         * After successfully leveling up the CurrentLevel and the XP bar are set to the correct values
         */
        private async void CheckLevelUp() {
            if (this.ExpToLvl(this.experience) > this.currentLevel) {
                int levelIncrease = this.ExpToLvl(this.experience) - this.currentLevel;
                this.GetTree().Paused = true;

                await LevelUpManagerSingleton.Instance.OnLevelUp(levelIncrease);
                this.currentLevel += levelIncrease;

                this.GetTree().Paused = false;
            }
            float currentExpInLevel = 100 * (this.experience - (float)this.LvlToExp(this.currentLevel)) /
                                      ((float)this.LvlToExp(this.currentLevel + 1) -
                                       this.LvlToExp(this.currentLevel));
            this.EmitSignal(nameof(ExperienceInPercent), currentExpInLevel);
        }

        /*
         * Calculates the current level depending on the experience.
         */
        private int ExpToLvl(float exp) {
            return (int)(Math.Sqrt(exp + 4) - 2);
        }

        /*
         * Calculates the experience required to reach the level.
         */
        private float LvlToExp(int lvl) {
            return (float)(4 * lvl + Math.Pow(lvl, 2));
        }

        /*
         * After picking up an ExpOrb the xp of the orb is added to the player's xp.
         * Refreshes the xp using [CurrentExperience]
         */
        public void OnPickUp(float exp) {
            this.experience += exp;
            this.CheckLevelUp();
        }

    }

}