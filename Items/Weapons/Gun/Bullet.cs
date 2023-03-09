using Godot;

namespace VampireSurvivorsLike {

    public class Bullet : Node2D {

        public Vector2 Direction { set; get; }
        public float Speed { get; set; }
        public float Damage { get; set; }
        private int Counter { get; set; }
        private int LifeSpan { get; set; }
        public int Piercing { get; set; }

        // Called when the node enters the scene tree for the first time.
        public override void _Ready() {
            this.Counter = 0;
            this.LifeSpan = 5000; // original 500
        }

        //  // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta) {
            this.Counter++;
            if (this.Counter % this.LifeSpan == 0) {
                QueueFree();
                if (GameStateManagerSingleton.Instance.IsMultiplayer && this.IsNetworkMaster()) {
                    Rpc(nameof(this.PuppetDestroyed));
                }
                return;
            }
            GlobalPosition += Direction * this.Speed * delta;
        }

        public void OnBodyEntered(Node2D body) {
            if (GameStateManagerSingleton.Instance.IsMultiplayer && !this.IsNetworkMaster()) {
                return;
            }
            if (body.HasMethod("OnHit") && body.GetClass() == "KinematicBody2D") {
                ((Enemy)body).OnHit(this.Damage, this.GetParent<Weapon>());
                this.Piercing--;
            }
            if (this.Piercing <= 0) {
                if (GameStateManagerSingleton.Instance.IsMultiplayer) {
                    this.Rpc(nameof(this.PuppetDestroyed));
                }
                this.QueueFree();
            }
        }

        [Puppet]
        public void PuppetDestroyed() {
            QueueFree();
        }

    }

}