using Godot;
using System;

public class Player : KinematicBody2D
{
    //Player attributes
    private int _health;
    private float _healthRegen;
    private float _speed;
    private float _pickupRange;

    private AnimatedSprite _animatedSprite;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _health = 100;
        _healthRegen = 1.0f;
        _speed = 100;
        _pickupRange = 10;
        _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        Vector2 velocity = Vector2.Zero;
        if (Input.IsActionPressed("ui_down"))
        {
            velocity = Vector2.Down;
            _animatedSprite.Play("Down");
        }

        else if (Input.IsActionPressed("ui_up"))
        {
            velocity = Vector2.Up;
            _animatedSprite.Play("Up");
        }

        else if (Input.IsActionPressed("ui_left"))
        {
            velocity = Vector2.Left;
            _animatedSprite.Play("Left");
        }

        else if (Input.IsActionPressed("ui_right"))
        {
            velocity = Vector2.Right;
            _animatedSprite.Play("Right");
        }
        else
        {
            _animatedSprite.Play("Idle");
        }

        MoveAndSlide(velocity * _speed);
    }
}