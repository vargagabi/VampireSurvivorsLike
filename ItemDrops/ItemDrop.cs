using System;
using Godot;

namespace VampireSurvivorsLike.ItemDrops {

    public class ItemDrop : Node2D {

        private int dropDirection;
        private int dropDistance;
        private int speed;
        private bool isMoving = false;
        private int value;
        private ItemDropsEnum type;

        private Tween tween;
        private AnimationPlayer animationPlayer;
        private Player player;

        [Signal] public delegate void OnPickUp(int value, ItemDropsEnum type);

        public override void _Ready() {
            this.animationPlayer = this.GetNode<AnimationPlayer>("AnimationPlayer");
            this.animationPlayer.Play("Hover");
            this.tween = this.GetNode<Tween>("Tween");
            
            double degree = this.dropDirection * 3.1415 / 180;
            Vector2 newPos = Vector2.Zero;
            newPos.y = (float)(Math.Sin(degree) * this.dropDistance);
            newPos.x = (float)(Math.Cos(degree) * this.dropDistance);
            this.tween.InterpolateProperty(this, "position", this.GlobalPosition, this.GlobalPosition + newPos,
                0.18f);
            this.tween.Start();
        }

        public override void _Process(float delta) {
            if (!this.isMoving) {
                return;
            }
            GlobalPosition += (this.player.GlobalPosition - GlobalPosition).Normalized() * this.speed * delta;
            this.speed += 4;
            if (this.player.GlobalPosition.DistanceTo(this.GlobalPosition) <= 10) {
                EmitSignal(nameof(OnPickUp), this.value, this.type);
                CallDeferred("queue_free");
            }
        }

        public void Init(Vector2 globalPosition, int value, int dropDirection, int dropDistance, ItemDropsEnum type,
            int speed = 100) {
            this.GlobalPosition = globalPosition;
            this.value = value;
            this.dropDirection = dropDirection;
            this.dropDistance = dropDistance;
            this.speed = speed;
            this.type = type;
        }


        //Signal connection
        public void OnAreaEntered(Area2D body) {
            if (!this.isMoving && body.GetParent() is Player playerBody) {
                Connect(nameof(OnPickUp), playerBody, "OnPickedUp");
                this.player = playerBody;
                this.isMoving = true;
            }
        }

    }

}