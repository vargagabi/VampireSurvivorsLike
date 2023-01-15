using System.Collections.Specialized;
using Godot;
using Godot.Collections;
using VampireSurvivorsLike.Enums;
using VampireSurvivorsLike.Weapons;

namespace VampireSurvivorsLike.Enemies {

    public class Enemy : KinematicBody2D {

        protected float Health { get; set; }
        protected float Strength { get; set; }
        protected float ExpValue { get; set; }
        protected float Speed { get; set; }
        public int SpawnRate { get; set; }
        public float SpawnDistance { get; set; }
        public Vector2 SpawnTime { get; set; }
        protected KinematicBody2D Player { get; set; }
        private PackedScene ExpOrb { get; set; }
        protected AnimatedSprite AnimatedSprite { get; set; }


        protected Enemy() {
            this.ExpOrb = ResourceLoader.Load<PackedScene>("res://ExpOrbs/ExpOrb.tscn");
        }

        public override void _Ready() {
            base._Ready();
            GD.Randomize();
            this.AnimatedSprite = this.GetNode<AnimatedSprite>("AnimatedSprite");
            this.AnimatedSprite.Connect("animation_finished", this, nameof(this.OnDeath));
            this.AnimationPlay(EnemyAnimationsEnum.Walk);
        }

        public void OnDeath() {
            if (this.AnimatedSprite.Animation == EnemyAnimationsEnum.Death.ToString()) {
                this.QueueFree();
                Node2D expOrb = this.ExpOrb.Instance<Node2D>();
                expOrb.GlobalPosition = this.GlobalPosition;
                expOrb.Set("Experience", this.ExpValue);
                Node viewport = this.GetTree().Root.GetChild(0);
                viewport.CallDeferred("add_child", expOrb);
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
            this.Health -= damage;
            if (this.Health <= 0) {
                if (weapon is Aura aura) {
                    this.ExpValue += this.ExpValue * aura.BonusExperience;
                }
                this.CollisionMask = 0;
                this.AnimationPlay(EnemyAnimationsEnum.Death);
            }
        }

        protected void AnimationPlay(EnemyAnimationsEnum enemyAnimations) {
            this.AnimatedSprite.Play(enemyAnimations.ToString());
        }

    }

}