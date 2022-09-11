using Godot;
using System;

//Slime monster
public class Enemy1 : KinematicBody2D
{
    private float _health ;
    private float _speed;
    private int _expValue;
    private float _strength { get; set; }

    private KinematicBody2D _player;
    private AnimatedSprite _animatedSprite;

    [Signal]
    public delegate void OnDeath(int value);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _health = 10;
        _speed = 50;
        _strength = 10;
        _player = GetNode<KinematicBody2D>("../../Player");
        _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        _animatedSprite.Play("Walk");
        _expValue = 1000;
        this.Connect(nameof(OnDeath), _player, "OnEnemyKilled");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        Vector2 velocity = Vector2.Zero;
        velocity = (_player.GlobalPosition - GlobalPosition).Normalized();
        if (_animatedSprite.Frame >= 2 && _animatedSprite.Frame <= 4)
        {
            MoveAndSlide(velocity * _speed);
        }
        
        _animatedSprite.FlipH = velocity.x < 0;
    }

    public void OnHit(float damage)
    {
        _health -= damage;
        if (_health <= 0)
        {
            QueueFree();
            EmitSignal(nameof(OnDeath),_expValue);            
        }
    }
}