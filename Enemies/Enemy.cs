using System;
using Godot;

namespace VampireSurvivorsLike {

    public class Enemy : KinematicBody2D {

        public static int mobCount = 0;
        protected float Health { get; set; }
        protected float Strength { get; set; }
        protected float ExpValue { get; set; }
        protected float Speed { get; set; }
        public int SpawnRate { get; set; }
        public float SpawnDistance { get; set; }
        public Vector2 SpawnTime { get; protected set; }
        public Player PlayerOne { get; set; }
        public Player PlayerTwo { get; set; }
        private PackedScene ExpOrb { get; set; }
        private PackedScene Gold { get; set; }
        protected AnimatedSprite AnimatedSprite { get; set; }
        private PackedScene DamageIndicator { get; set; }
        public int spawnAfterMinute;


        protected Enemy() {
            this.ExpOrb = ResourceLoader.Load<PackedScene>("res://ExpOrbs/ExpOrb.tscn");
            this.Gold = ResourceLoader.Load<PackedScene>("res://Gold/Gold.tscn");
            this.DamageIndicator = ResourceLoader.Load<PackedScene>("res://GUI/GUI/FloatingValue.tscn");
        }

        public override void _Ready() {
            base._Ready();
            GD.Randomize();
            this.AnimatedSprite = this.GetNode<AnimatedSprite>("AnimatedSprite");
            this.AnimatedSprite.Connect("animation_finished", this, nameof(this.OnDeath));
            this.AnimationPlay(EnemyAnimationsEnum.Walk);
        }

        protected Vector2 GetTargetPosition() {
            if (this.PlayerTwo == null) {
                return this.PlayerOne.GlobalPosition;
            }
            if (this.PlayerOne.IsDead) {
                return this.PlayerTwo.GlobalPosition;
            }
            if (this.PlayerTwo.IsDead) {
                return this.PlayerOne.GlobalPosition;
            }
            return this.GlobalPosition.DistanceTo(this.PlayerOne.GlobalPosition) <
                   this.GlobalPosition.DistanceTo(this.PlayerTwo.GlobalPosition)
                ? this.PlayerOne.GlobalPosition
                : this.PlayerTwo.GlobalPosition;
        }

        [Puppet]
        protected void SetPuppetPosition(Vector2 globalPosition, int frame = 0) {
            Vector2 velocity = (globalPosition - this.GlobalPosition).Normalized();
            this.GlobalPosition = globalPosition;
            this.AnimatedSprite.Frame = frame;
            if (this.GetTargetPosition().DistanceTo(this.GlobalPosition) < 20) {
                this.AnimationPlay(EnemyAnimationsEnum.Attack);
            } else if (this.AnimatedSprite.Frame >= 2 && this.AnimatedSprite.Frame <= 4) {
                this.AnimatedSprite.FlipH = velocity.x < 0;
                this.AnimationPlay(EnemyAnimationsEnum.Walk);
            }
        }

        protected void AnimationPlay(EnemyAnimationsEnum enemyAnimations) {
            this.AnimatedSprite.Play(enemyAnimations.ToString());
        }

        public void OnDeath() {
            if (this.AnimatedSprite.Animation == EnemyAnimationsEnum.Death.ToString()) {
                this.QueueFree();
                Node2D expOrb = this.ExpOrb.Instance<Node2D>();
                expOrb.GlobalPosition = this.GlobalPosition;
                expOrb.Set("Experience", this.ExpValue);
                Node viewport = this.GetTree().Root.GetNode("Main");
                viewport.CallDeferred("add_child", expOrb);
                if (true) {
                    Node2D gold = this.Gold.Instance<Node2D>();
                    gold.GlobalPosition = this.GlobalPosition;
                    viewport.CallDeferred("add_child", gold);
                }
            }
        }

        /*
         * When receiving damage reduce the Health by damage amount.
         * If the Health is less than or equals 0 remove the enemy and add an
         * instance of an ExpOrb at its place.
         */
        public void OnHit(float damage, Weapon weapon) {
            if (this.Health <= 0) {
                return;
            }
            FloatingValue damageInd = this.DamageIndicator.Instance<FloatingValue>();
            damageInd.SetValues(this.GlobalPosition, new Color(0.96f, 0.24f, 0.24f), (int)damage);
            this.GetTree().Root.GetNode("Main").CallDeferred("add_child", damageInd);

            this.Health -= damage;
            if (this.Health <= 0) {
                if (weapon is Aura aura) {
                    this.ExpValue += this.ExpValue * aura.BonusExperience;
                }
                this.CollisionMask = 0;
                this.AnimationPlay(EnemyAnimationsEnum.Death);
            }
        }

    }

}