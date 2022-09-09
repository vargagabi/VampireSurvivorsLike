using Godot;
using System;

public class Bullet : Node2D
{
    private Vector2 _direction { set; get; }
    private float _speed;
    private float _damage;
    private int _counter;
    private int _lifeSpan;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _speed = 200;
        _counter = 0;
        _lifeSpan = 500;
        _damage = 5;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        _counter++;
        if (_counter % _lifeSpan == 0)
        {
            QueueFree();
            return;
        }

        GlobalPosition += _direction * _speed * delta;
    }

    public void OnBodyEntered(Node body)
    {
        if (body.HasMethod("OnHit") && body.GetClass() == "KinematicBody2D")
        {
            ((Enemy1)body).OnHit(_damage);
        }
    }
}