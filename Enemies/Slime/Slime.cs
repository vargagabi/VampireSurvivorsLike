using Godot;

namespace VampireSurvivorsLike {

    public class Slime : Enemy {

        public Slime() {
            this.health = 10;
            this.Strength = 5;
            this.expValue = 30;
            this.speed = 50;
            this.SpawnRate = 150;
            this.SpawnDistance = 400;
            this.SpawnTime = new Vector2(0f, 60f);
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta) {
            if (this.health <= 0) {
                return;
            }
            Vector2 velocity = this.GlobalPosition.DirectionTo(this.GetTargetPosition() + Vector2.Down * 15);
            if (this.GetTargetPosition().DistanceTo(this.GlobalPosition) < 10) {
                this.AnimationPlay(EnemyAnimationsEnum.Attack);
            } else if (this.AnimatedSprite.Frame >= 2 && this.AnimatedSprite.Frame <= 4) {
                this.AnimationPlay(EnemyAnimationsEnum.Walk);
                this.MoveAndSlide(velocity * this.speed);
            }
            this.AnimatedSprite.FlipH = velocity.x < 0;
        }

    }

}