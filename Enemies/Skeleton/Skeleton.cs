using Godot;
using System;
using VampireSurvivorsLike.Enemies;
using VampireSurvivorsLike.Enums;

public class Skeleton : Enemy {

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        base._Ready();
        this.Player = this.GetNode<KinematicBody2D>("../../../Player");
    }

    public Skeleton() {
        this.Health = 20;
        this.Strength = 10;
        this.ExpValue = 5;
        this.Speed = 50;
        this.SpawnRate = 90;
        this.SpawnDistance = 250;
        this.SpawnTime = new Vector2(60f, 300f);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta) {
        if (this.Health <= 0) {
            return;
        }
        Vector2 velocity = ((this.Player.GlobalPosition + Vector2.Down * 15) - this.GlobalPosition).Normalized();
        if (this.Player.GlobalPosition.DistanceTo(this.GlobalPosition) > 25f) {
            this.MoveAndSlide(velocity * this.Speed);
            this.AnimationPlay(EnemyAnimationsEnum.Walk);
        } else {
            this.AnimationPlay(EnemyAnimationsEnum.Attack);
        }
        this.AnimatedSprite.FlipH = velocity.x < 0;
    }
}