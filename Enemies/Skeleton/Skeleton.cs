using Godot;
using System;

namespace VampireSurvivorsLike {

    public class Skeleton : Enemy {

        // Called when the node enters the scene tree for the first time.
        public override void _Ready() {
            base._Ready();
            this.Player = this.GetNode<KinematicBody2D>("../../../Player");
        }

        public Skeleton() {
            this.Health = 20;
            this.Strength = 10;
            this.ExpValue = 5;
            this.Speed = 50;
            this.SpawnRate = 188;
            this.SpawnDistance = 300;
            this.SpawnTime = new Vector2(60f, 300f);
        }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta) {
            if (this.Health <= 0) {
                return;
            }
            Vector2 velocity = ((this.Player.GlobalPosition + Vector2.Down * 15) - this.GlobalPosition).Normalized();
            if (this.Player.GlobalPosition.DistanceTo(this.GlobalPosition) <= 20f) {
                this.AnimationPlay(EnemyAnimationsEnum.Attack);
            } else {
                this.MoveAndSlide(velocity * this.Speed);
                this.AnimationPlay(EnemyAnimationsEnum.Walk);
            }
            this.AnimatedSprite.FlipH = velocity.x < 0;
        }

    }

}