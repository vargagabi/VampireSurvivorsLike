using Godot;

namespace VampireSurvivorsLike {

    public class Bullet : Node2D {

        private Vector2 direction;
        private float speed;
        private int damage;
        private int piercing;
        private int counter;
        private int lifeSpan;

        public void Init(Vector2 direction, float speed, int damage, int piercing, string name,
            Vector2 globalPosition) {
            this.direction = direction;
            this.speed = speed;
            this.damage = damage;
            this.piercing = piercing;
            this.Name = name;
            this.GlobalPosition = globalPosition;
            this.Visible = true;
            this.SetAsToplevel(true);
        }

        // Called when the node enters the scene tree for the first time.
        public override void _Ready() {
            this.counter = 0;
            this.lifeSpan = 1000;
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta) {
            if (this.counter++ > this.lifeSpan) {
                this.counter = 0;
                if (GameStateManagerSingleton.Instance.IsMultiplayer && this.IsNetworkMaster()) {
                    this.Rpc(nameof(this.SyncQueueFree));
                } else {
                    this.QueueFree();
                }
                return;
            }
            this.GlobalPosition += this.direction * this.speed * delta;
            if (GameStateManagerSingleton.Instance.IsMultiplayer && this.IsNetworkMaster() && this.counter % 100 == 0) {
                this.RpcUnreliable(nameof(this.PuppetPosition), this.GlobalPosition);
            }
        }

        [Puppet]
        public void PuppetPosition(Vector2 position) {
            this.GlobalPosition = position;
        }


        [PuppetSync]
        public void SyncQueueFree() {
            this.QueueFree();
        }

        public void OnBodyEntered(Node2D body) {
            if (GameStateManagerSingleton.Instance.IsMultiplayer && !this.IsNetworkMaster()) {
                return;
            }
            if (body is Enemy enemy) {
                enemy.OnHit(this.damage, this.GetParent<Weapon>());
                this.piercing--;
            }
            if (this.piercing > 0) {
                return;
            }
            if (GameStateManagerSingleton.Instance.IsMultiplayer) {
                this.Rpc(nameof(this.SyncQueueFree));
            } else {
                this.QueueFree();
            }
        }

    }

}