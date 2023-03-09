using Godot;
using System;

namespace VampireSurvivorsLike {

    public class Skeleton : Enemy {

        // Called when the node enters the scene tree for the first time.
        public override void _Ready() {
            base._Ready();
        }

        public Skeleton() {
            this.Health = 20;
            this.Strength = 10;
            this.ExpValue = 5;
            // this.Speed = 50;
            this.Speed = 10;
            this.SpawnRate = 188;
            this.SpawnDistance = 400;
            this.SpawnTime = new Vector2(60f, 300f);
            this.spawnAfterMinute = 1;
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta) {
            if (this.Health <= 0) {
                return;
            }
            Vector2 velocity = ((this.GetTargetPosition() + Vector2.Down * 15) - this.GlobalPosition).Normalized();
            if (this.GetTargetPosition().DistanceTo(this.GlobalPosition) <= 20f) {
                this.AnimationPlay(EnemyAnimationsEnum.Attack);
            } else {
                this.MoveAndSlide(velocity * this.Speed);
                this.AnimationPlay(EnemyAnimationsEnum.Walk);
            }
            this.AnimatedSprite.FlipH = velocity.x < 0;
        }

    }

}