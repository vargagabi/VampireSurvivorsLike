using Godot;

namespace VampireSurvivorsLike {

    public class Skeleton : Enemy {

        public Skeleton() {
            this.health = 20;
            this.Strength = 10;
            this.expValue = 5;
            this.speed = 50;
            this.SpawnRate = 250;
            this.SpawnDistance = 600;
            this.SpawnTime = new Vector2(60f, 300f);
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