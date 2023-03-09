using Godot;

namespace VampireSurvivorsLike {

    public class Slime : Enemy {

        // Called when the node enters the scene tree for the first time.
        public override void _Ready() {
            base._Ready();
            
            // if (GameStateManagerSingleton.Instance.IsMultiplayer && !this.IsNetworkMaster()) {
            //     return;
            // }
            // this.Scale *= (float)GD.RandRange(1, 3);
            // if (GameStateManagerSingleton.Instance.IsMultiplayer) {
            //     // this.Rpc(nameof(this.SetPuppetScale),this.Scale);
            // }
        }

        public Slime() {
            this.Health = 10;
            this.Strength = 5;
            this.ExpValue = 1;
            // this.Speed = 50;
            this.Speed = 10;
            this.SpawnRate = 222;
            this.SpawnDistance = 500;
            this.SpawnTime = new Vector2(0f, 60f);
            this.spawnAfterMinute = 0;
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta) {
            if (!GameStateManagerSingleton.Instance.IsMultiplayer || this.IsNetworkMaster()) {
                if (this.Health <= 0) {
                    return;
                }
                Vector2 velocity = ((this.GetTargetPosition() + Vector2.Down * 15) - this.GlobalPosition).Normalized();
                if (this.GetTargetPosition().DistanceTo(this.GlobalPosition) < 20) {
                    this.AnimationPlay(EnemyAnimationsEnum.Attack);
                } else if (this.AnimatedSprite.Frame >= 2 && this.AnimatedSprite.Frame <= 4) {
                    this.AnimatedSprite.FlipH = velocity.x < 0;
                    this.AnimationPlay(EnemyAnimationsEnum.Walk);
                    this.MoveAndSlide(velocity * this.Speed);
                }
                if (GameStateManagerSingleton.Instance.IsMultiplayer) {
                    Rpc(nameof(this.SetPuppetPosition), this.GlobalPosition, this.AnimatedSprite.Frame);
                }
            }
        }

    }

}