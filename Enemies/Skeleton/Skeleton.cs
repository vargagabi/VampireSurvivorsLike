using Godot;

namespace VampireSurvivorsLike {

    public class Skeleton : Enemy {

        public Skeleton() {
            this.SpawnTime = new Vector2(2f, 20f);
            this.health = 25 + (int)(Main.MinutesPassed - this.SpawnTime.x) * 2;
            this.Strength = 20 + (int)(Main.MinutesPassed - this.SpawnTime.x);
            this.expValue = 3;
            this.speed = 80 + (int)(Main.MinutesPassed - this.SpawnTime.x) * 4;
            this.SpawnRate = 250;
            this.SpawnDistance = 600;
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta) {
            if (this.health <= 0) {
                return;
            }
            Vector2 velocity = this.GlobalPosition.DirectionTo(this.GetTargetPosition());
            if (this.GetTargetPosition().DistanceTo(this.GlobalPosition) <= 20f) {
                this.AnimationPlay(EnemyAnimationsEnum.Attack);
            } else {
                this.MoveAndSlide(velocity * this.speed);
                this.AnimationPlay(EnemyAnimationsEnum.Walk);
            }
            this.AnimatedSprite.FlipH = velocity.x < 0;
        }

    }

}