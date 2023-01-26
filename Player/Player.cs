using System;
using System.Collections.Generic;
using Godot;
using Object = System.Object;

namespace VampireSurvivorsLike {

      class KeyVal {

        public string Message { get; }
        private readonly float initialValue;
        private CircleShape2D shape;
        private readonly float bonusModifier;
        private int level;

        public KeyVal(string message, float initialValue, float modify) {
            this.Message = message;
            this.initialValue = initialValue;

            // this.bonusModifier = (100f + modify) / 100f;
            this.bonusModifier = modify / 100f;
        }

        public KeyVal(string message, CircleShape2D shape, float modifier) {
            this.Message = message;
            this.shape = shape;
            this.initialValue = shape.Radius;
            this.bonusModifier = modifier / 100f;
        }

        public float GetCurrentValue() {
            return this.initialValue + (this.initialValue * this.bonusModifier) * this.level;
        }

        public void Increase() {
            this.level++;
            if (this.shape != null) {
                this.shape.Radius = this.initialValue + this.initialValue * this.bonusModifier * this.level;
                GD.Print("Radius: " + this.shape.Radius);
                GD.Print("Modifier " + this.bonusModifier);
            }
        }

    }

    public class Player : KinematicBody2D {

        //Player attributes
        private KeyVal maxHealth = new KeyVal("Increase max health", 200f, 10f);
        private KeyVal healthRegen = new KeyVal("Increase health regeneration", 5.0f, 10f);
        private KeyVal speed = new KeyVal("Increase speed", 100.0f, 20f);
        private KeyVal pickupRange;
        private List<KeyVal> upgradeableStats = new List<KeyVal>();
        private float currentHealth;

        //Counters
        private int healthCounter = 0;
        private int damageCounter = 0;
        private const int ImmunityTime = 25;
        private const int PassiveHealTime = 100;
        private Vector2 Direction { get; set; }
        private float takenDamageValue = 0;

        //The function to calculate the required xp between levels: f(x) = 200x , where x->the level
        private float experience = 0;
        private int currentLevel = 0;

        private bool IsRewardSelection { get; set; }

        private AnimatedSprite animatedSprite;
        private TextureProgress healthBar;
        private CircleShape2D pickupArea;
        private Texture[] textures = new Texture[3];
        private Sprite directionArrow;
        private PackedScene FloatingValue { get; set; }


        [Signal] public delegate void CurrentHealth(float currentHealth);
        [Signal] public delegate void CurrentExperience(float exp, int level);
        [Signal] public delegate void ChooseReward(string opt0, string opt1, string opt2, string opt3);

        //New ItemManager methods
        [Signal] public delegate void LevelUp();
        [Signal] public delegate void CurrentLevelSignal(int level);
        [Signal] public delegate void CurrentExperienceSignal(int experienceInPercentage);

// Called when the node enters the scene tree for the first time.
        public override void _Ready() {
            GD.Print("Player Ready...");
            this.IsRewardSelection = false;
            this.currentHealth = this.maxHealth.GetCurrentValue();
            this.Direction = Vector2.Right;
            this.animatedSprite = this.GetNode<AnimatedSprite>("AnimatedSprite");
            this.healthBar = this.GetNode<TextureProgress>("Node2D/HealthBar");
            this.textures[0] = ResourceLoader.Load("res://Textures/bar_green_mini.png") as Texture;
            this.textures[1] = ResourceLoader.Load("res://Textures/bar_yellow_mini.png") as Texture;
            this.textures[2] = ResourceLoader.Load("res://Textures/bar_red_mini.png") as Texture;
            this.directionArrow = this.GetNode<Sprite>("Arrow");
            this.pickupArea = this.GetNode<Area2D>("PickupArea").GetChild<CollisionShape2D>(0).Shape as CircleShape2D;
            this.pickupRange = new KeyVal("Increase the pickup range by 10%", this.pickupArea, 10.0f);
            this.FloatingValue = ResourceLoader.Load<PackedScene>("res://GUI/GUI/FloatingValue.tscn");
            this.upgradeableStats.Add(this.maxHealth);
            this.upgradeableStats.Add(this.healthRegen);
            this.upgradeableStats.Add(this.speed);
            this.upgradeableStats.Add(this.pickupRange);

            //Add the player to the manager scripts

            // ItemManagerSingleton.Instance.Player = this;
            // ItemManagerSingleton.Instance.EquipItem(ItemManagerSingleton.Instance.GetUnequippedItems()[1]);


            //Emit signals to set the HUD health and level bars
            this.EmitSignal(nameof(CurrentHealth), this.currentHealth);
            this.EmitSignal(nameof(CurrentExperience), this.experience, this.currentLevel);
        }

        //  // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta) {
            if (this.IsRewardSelection) {
                this.GetTree().Paused = true;
            }
            this.Move();

            // this.CheckLevelUp();
            if (this.takenDamageValue > 0) {
                this.TakeDamage();
            }

            if (this.currentHealth < this.maxHealth.GetCurrentValue()) {
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
            this.directionArrow.Position = this.Direction.Normalized() * 15;
            this.Direction = this.Direction.Normalized();
            this.animatedSprite.Play(animation.ToString());
            this.MoveAndSlide(velocity.Normalized() * this.speed.GetCurrentValue());
        }

        /*
         * Update the health bar depending on the current health
         * Change the health bar color according to its value
         */
        private void UpdateHealthBar() {
            this.healthBar.Value = (this.currentHealth / this.maxHealth.GetCurrentValue()) * 100;
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
                this.GetTree().Root.GetChild(0).CallDeferred("add_child", damageInd);
                this.damageCounter = 0;
                this.currentHealth = Math.Max(0, this.currentHealth - (int)this.takenDamageValue);
                this.EmitSignal(nameof(CurrentHealth), this.currentHealth);
                this.UpdateHealthBar();
            }

            if (this.currentHealth <= 0) {
                this.GetTree().Paused = true;
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
                    (int)this.healthRegen.GetCurrentValue());
                this.GetTree().Root.GetChild(0).CallDeferred("add_child", healingInd);
                this.healthCounter = 0;
                this.currentHealth = Math.Min(this.maxHealth.GetCurrentValue(),
                    this.healthRegen.GetCurrentValue() + this.currentHealth);
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
        private void CheckLevelUp() {
            if (this.ExpToLvl(this.experience) > this.currentLevel) {
                for (int i = 0; i < this.ExpToLvl(this.experience) - this.currentLevel; i++) {
                    this.currentLevel++;
                    LevelUpManagerSingleton.Instance.OnLevelUp();
                    this.EmitSignal(nameof(CurrentLevelSignal), this.currentLevel);
                }
            }

            // if (this.ExpToLvl(this.experience) > this.currentLevel) {
            // this.IsRewardSelection = true;
            // for (int i = 0; i < this.ExpToLvl(this.experience) - this.currentLevel; i++) {
            // this.GetTree().Paused = true;

            // Object[] options = new object[4];
            // this.rewardIndex = -1;
            // this.SelectRewardOnLevelUp(options);
            // await this.ToSignal(this.GetNode("../GUI"), "RewardSelected");
            // if (options[this.rewardIndex] is Weapon) {
            // if (!this.equippedItems.Contains(options[this.rewardIndex] as Weapon)) {
            // this.EquipItem((Weapon)options[this.rewardIndex]);
            // } else {
            // ((Weapon)options[this.rewardIndex]).Upgrade();
            // }
            // } else if (options[this.rewardIndex] is KeyVal) {
            // ((KeyVal)options[this.rewardIndex]).Increase();
            // }
            // this.IsRewardSelection = false;
            // this.GetTree().Paused = false;
            // }

            // this.currentLevel = this.ExpToLvl(this.experience);
            // float currentExpInLevel = 100 * (this.experience - (float)this.LvlToExp(this.currentLevel)) /
            // ((float)this.LvlToExp(this.currentLevel + 1) -
            // this.LvlToExp(this.currentLevel));
            // this.EmitSignal(nameof(CurrentExperience), currentExpInLevel, this.currentLevel);
            // }
        }

        /*
         * Generates 4 reward options from the UpgradeableStats + EquippedWeapons + AllWeapons.
         * The two item lists do not have overlapping items.
         * After selecting 4 options the options string value is sent to the GUI screen via [ChooseReward]
         */
        private void SelectRewardOnLevelUp(Object[] options) {
            // List<Object> rewards = new List<object>();
            // foreach (KeyVal keyValuePair in this.upgradeableStats) {
            //     rewards.Add(keyValuePair);
            // }
            // IEnumerable<Item> equipped =
            //     from item in this.equippedItems
            //     where item.Level < item.MaxLevel
            //     select item;
            // rewards.AddRange(equipped.ToList());
            // rewards.AddRange(this.allItems);
            //
            // //Select 4 options from the rewards list
            // for (int j = 0; j < 4; j++) {
            //     int rand = (int)GD.RandRange(0, rewards.Count);
            //     options[j] = rewards[rand];
            //     rewards.Remove(rewards[rand]);
            // }
            // string[] optionsString = new string[4];
            // for (int j = 0; j < 4; j++) {
            //     if (options[j] is KeyVal) {
            //         optionsString[j] = ((KeyVal)options[j]).Message;
            //     } else if (options[j] is Weapon) {
            //         optionsString[j] = ((Weapon)options[j]).UpgradeMessage();
            //     }
            // }
            // this.EmitSignal(nameof(ChooseReward), optionsString[0], optionsString[1], optionsString[2], optionsString[3]);
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
         * After the player choose a reward sets the chosen rewardIndex. 
         */
        public void OnRewardSelected(int index) {
            // this.rewardIndex = index;
        }

        /*
         * After picking up an ExpOrb the xp of the orb is added to the player's xp.
         * Refreshes the xp using [CurrentExperience]
         */
        public void OnPickUp(float exp) {
            this.experience += exp;
            this.CheckLevelUp();
            float currentExpInLevel = 100 * (this.experience - (float)this.LvlToExp(this.currentLevel)) /
                                      ((float)this.LvlToExp(this.currentLevel + 1) - this.LvlToExp(this.currentLevel));
            this.EmitSignal(nameof(CurrentExperience), currentExpInLevel, this.currentLevel);
        }

    }

}