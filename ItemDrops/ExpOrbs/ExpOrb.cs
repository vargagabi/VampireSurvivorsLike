using System;
using Godot;
using VampireSurvivorsLike.ItemDrops;

namespace VampireSurvivorsLike {

    public class ExpOrb : Node2D {

        public float Experience { get; set; }
        private int direction;
        private int distance;
        private int speed = 100;
        private Player player;
        private bool isMoving = false;
        private Tween tween;
        private AnimationPlayer animationPlayer;


        [Signal] public delegate void PickedUp(int exp);

        // Called when the node enters the scene tree for the first time.
        public override void _Ready() {
            // base._Ready();
            this.animationPlayer = this.GetNode<AnimationPlayer>("AnimationPlayer");
            this.animationPlayer.Play("Hover");
            this.tween = this.GetNode<Tween>("Tween");
            if (!GameStateManagerSingleton.Instance.IsMultiplayer || this.IsNetworkMaster()) {
                // this.CreateItem();
            }
        }

        private void SpawnOrb() {
            int degree = (int)GD.RandRange(0, 360);
            int distance = (int)GD.RandRange(30, 40);
            Vector2 newPos = Vector2.Zero;
            newPos.y = (float)(Math.Sin(degree * 3.1415 / 180) * distance);
            newPos.x = (float)(Math.Cos(degree * 3.1415 / 180) * distance);
            this.tween.InterpolateProperty(this, "position", this.GlobalPosition, this.GlobalPosition + newPos,
                0.18f);
            this.tween.Start();
            if (GameStateManagerSingleton.Instance.IsMultiplayer) {
                Rpc(nameof(this.PuppetSpawnOrb), degree, distance);
            }
        }

        [Puppet]
        public void PuppetSpawnOrb(int degree, int distance) {
            GD.Print("Spawn orb");
            Vector2 newPos = Vector2.Zero;
            newPos.y = (float)(Math.Sin(degree * 3.1415 / 180) * distance);
            newPos.x = (float)(Math.Cos(degree * 3.1415 / 180) * distance);
            this.tween.InterpolateProperty(this, "position", this.GlobalPosition, this.GlobalPosition + newPos,
                0.18f);
            this.tween.Start();
        }

        /*
         * When the player picked up the ord set the player field to the player.
         * Also Connect [PickedUp] to the player's OnPickUp method.
         */
        // public void OnAreaEntered(Area2D body) {
        // if (!this.isMoving) {
        // Connect(nameof(PickedUp), body.GetParent(), "OnPickUp");
        // this.player = body;
        // this.isMoving = true;
        // }
        // }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta) {
            base._Process(delta);

            // if (this.isMoving) {
            //     GlobalPosition += (this.player.GlobalPosition - GlobalPosition).Normalized() * this.speed * delta;
            //     this.speed += 1;
            //     if (this.player.GlobalPosition.DistanceTo(GlobalPosition) <= 10) {
            //         EmitSignal(nameof(PickedUp), Experience);
            //         CallDeferred("queue_free");
            //     }
            // }
        }

    }

}