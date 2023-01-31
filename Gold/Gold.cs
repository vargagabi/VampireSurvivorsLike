using Godot;
using System;

namespace VampireSurvivorsLike {

    public class Gold : Node2D {

        private AnimationPlayer animationPlayer;
        private Area2D player;
        private bool isMoving = false;
        private int speed = 100;

        [Signal] public delegate void GoldPickedUp(int value);
        public override void _Ready() {
            this.animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            this.animationPlayer.Play("Hover");
        }

        public override void _Process(float delta) {
            if (this.isMoving) {
                GlobalPosition += (this.player.GlobalPosition - GlobalPosition).Normalized() * this.speed * delta;
                this.speed += 1;
                if (this.player.GlobalPosition.DistanceTo(GlobalPosition) <= 10) {
                    EmitSignal(nameof(GoldPickedUp), (int)GD.RandRange(1, 5));
                    CallDeferred("queue_free");
                }
            }
        }

        public void OnAreaEntered(Area2D body) {
            if (!this.isMoving) {
                Connect(nameof(GoldPickedUp),body.GetParent(),"OnGoldPickedUp");
                this.player = body;
                this.isMoving = true;
            }
        }

    }

}