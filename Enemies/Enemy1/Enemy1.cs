using Godot;
using VampireSurvivorsLike.Enemies;

//Slime monster
public class Enemy1 : Enemy {

    private AnimatedSprite animatedSprite;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        this.Health = 10;
        this.Strength = 5;
        this.ExpValue = 1;
        this.Speed = 50;
        this.Player = this.GetNode<KinematicBody2D>("../../../Player");
        this.animatedSprite = this.GetNode<AnimatedSprite>("AnimatedSprite");
        this.animatedSprite.Play("Walk");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta) {
        Vector2 velocity = ((this.Player.GlobalPosition + Vector2.Down * 15) - this.GlobalPosition).Normalized();
        if (this.animatedSprite.Frame >= 2 && this.animatedSprite.Frame <= 4) {
            this.MoveAndSlide(velocity * this.Speed);
        }
        this.animatedSprite.FlipH = velocity.x < 0;
    }

}