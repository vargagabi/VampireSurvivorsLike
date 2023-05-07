using Godot;

namespace VampireSurvivorsLike {

    public class Skeleton : Enemy {

        public Skeleton() {
            this.SpawnTime = new Vector2(2f, 20f);
            this.Health = 25 + (int)(Main.MinutesPassed - this.SpawnTime.x) * 2;
            this.Strength = 20 + (int)(Main.MinutesPassed - this.SpawnTime.x);
            this.ExpValue = 3;
            this.Speed = 80 + (int)(Main.MinutesPassed - this.SpawnTime.x) * 4;
            this.SpawnRate = 250;
            this.SpawnDistance = 600;
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta) {
            if (this.Health <= 0) {
                return;
            }
            Vector2 velocity = this.GlobalPosition.DirectionTo(this.GetTargetPosition());
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