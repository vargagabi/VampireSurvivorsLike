using Godot;
using System;

namespace VampireSurvivorsLike {

    public class Gold : Node2D {

        private AnimationPlayer animationPlayer;
        private Area2D player;
        private bool isMoving = false;
        private int speed = 100;
        private Tween tween;

        [Signal] public delegate void GoldPickedUp(int value);

        public override void _Ready() {
            this.animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            this.animationPlayer.Play("Hover");
            Connect(nameof(GoldPickedUp), this.GetTree().Root.GetNode<CanvasLayer>("Main/GUI"), "OnGoldPickedUp");

            //Create tween when gold drops
            this.tween = GetNode<Tween>("Tween");
            int degree = (int)GD.RandRange(0,360);
            double distance = GD.RandRange(30,40);
            Vector2 newPos = Vector2.Zero;
            newPos.y = (float)(Math.Sin(degree * 3.1415 / 180) * distance); 
            newPos.x = (float)(Math.Cos(degree * 3.1415 / 180) * distance); 
            this.tween.InterpolateProperty(this,"position",this.GlobalPosition, this.GlobalPosition + newPos, 0.18f);
            this.tween.Start();
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
                this.player = body;
                this.isMoving = true;
            }
        }

    }

}