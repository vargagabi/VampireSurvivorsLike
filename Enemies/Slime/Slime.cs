using Godot;

namespace VampireSurvivorsLike {

    public class Slime : Enemy {

        public Slime() {
            this.SpawnTime = new Vector2(0f, 10f);
            this.health = 10 + (int)(Main.MinutesPassed - this.SpawnTime.x) * 2;
            this.Strength = 8 + (int)(Main.MinutesPassed - this.SpawnTime.x);
            this.expValue = 1 + (int)(Main.MinutesPassed - this.SpawnTime.x);
            this.speed = 60 + (Main.MinutesPassed - this.SpawnTime.x) * 4;
            this.SpawnRate = 150;
            this.SpawnDistance = 400;
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta) {
            if (this.health <= 0) {
                return;
            }
            Vector2 velocity = this.GlobalPosition.DirectionTo(this.GetTargetPosition() + Vector2.Down * 8);
            if (this.GlobalPosition.DistanceTo(this.GetTargetPosition()) < 10) {
                this.AnimationPlay(EnemyAnimationsEnum.Attack);
            } else if (this.AnimatedSprite.Frame >= 2 && this.AnimatedSprite.Frame <= 4) {
                this.AnimationPlay(EnemyAnimationsEnum.Walk);
                this.MoveAndSlide(velocity * this.speed);
                this.AnimatedSprite.FlipH = velocity.x < 0;
            }
        }

    }

}