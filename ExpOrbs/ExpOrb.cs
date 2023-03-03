using System;
using Godot;

namespace VampireSurvivorsLike {

    public class ExpOrb : Node2D {

        private float Experience { get; set; }
        private AnimationPlayer animationPlayer;
        private Node2D player;
        private bool isMoving = false;
        private int speed = 100;
        private Tween tween;
        [Signal] public delegate void PickedUp(int exp);

        // Called when the node enters the scene tree for the first time.
        public override void _Ready() {
            this.animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            this.animationPlayer.Play("Hover");
            this.tween = GetNode<Tween>("Tween");
            int degree = (int)GD.RandRange(0,360);
            double distance = GD.RandRange(30,40);
            Vector2 newPos = Vector2.Zero;
            newPos.y = (float)(Math.Sin(degree * 3.1415 / 180) * distance); 
            newPos.x = (float)(Math.Cos(degree * 3.1415 / 180) * distance); 
            this.tween.InterpolateProperty(this,"position",this.GlobalPosition, this.GlobalPosition + newPos, 0.18f);
            this.tween.Start();
        }

        /*
         * When the player picked up the ord set the player field to the player.
         * Also Connect [PickedUp] to the player's OnPickUp method.
         */
        public void OnAreaEntered(Area2D body) {
            if (!this.isMoving) {
                Connect(nameof(PickedUp), body.GetParent(), "OnPickUp");
                this.player = body;
                this.isMoving = true;
            }
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta) {
            if (this.isMoving) {
                GlobalPosition += (this.player.GlobalPosition - GlobalPosition).Normalized() * this.speed * delta;
                this.speed += 1;
                if (this.player.GlobalPosition.DistanceTo(GlobalPosition) <= 10) {
                    EmitSignal(nameof(PickedUp), Experience);
                    CallDeferred("queue_free");
                }
            }
        }

    }

}