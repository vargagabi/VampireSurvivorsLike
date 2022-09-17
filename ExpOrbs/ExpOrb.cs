using Godot;
using System;

public class ExpOrb : Node2D
{

    private int Experience { get; set; } 
    private AnimationPlayer _animationPlayer;
    private KinematicBody2D _player;
    [Signal] public delegate void PickedUp(int exp);

// Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _animationPlayer.Play("Hover");
        _player = GetNode<KinematicBody2D>("../Player");
        Connect(nameof(PickedUp), _player, "OnPickUp");
    }

    public void OnAreaEntered(Node2D body)
    {
        EmitSignal(nameof(PickedUp), Experience);
        QueueFree();
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
