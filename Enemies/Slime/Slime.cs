using Godot;
using VampireSurvivorsLike.Enemies;
using VampireSurvivorsLike.Enums;

//Slime monster
public class Slime : Enemy {

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        base._Ready();
        this.Player = this.GetNode<KinematicBody2D>("../../../Player");
    }

    public Slime() {
        this.Health = 10;
        this.Strength = 5;
        this.ExpValue = 1;
        this.Speed = 50;
        this.SpawnRate = 100;
        this.SpawnDistance = 500;
        this.SpawnTime = new Vector2(0f,60f);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta) {
        if (this.Health <= 0) {
            return;
        }
        Vector2 velocity = ((this.Player.GlobalPosition + Vector2.Down * 15) - this.GlobalPosition).Normalized();
        if (this.AnimatedSprite.Frame >= 2 && this.AnimatedSprite.Frame <= 4) {
            this.MoveAndSlide(velocity * this.Speed);
        }
        this.AnimatedSprite.FlipH = velocity.x < 0;
    }

}