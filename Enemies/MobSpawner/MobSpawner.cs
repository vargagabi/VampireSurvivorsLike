using Godot;
using System;
using System.Drawing.Imaging;

public class MobSpawner : Node2D
{
    private int _spawnCounter;
    private int _spawnRate;
    private KinematicBody2D _player;
    private PackedScene[] _mobs = new PackedScene[1];
    private int _spawnDistance;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // _player = GetNode<KinematicBody2D>("Player");
        GD.Randomize();
        _spawnRate = 100;
        _spawnCounter = 0;
        _spawnDistance = 100;
        _player = GetNode<KinematicBody2D>("../Player");
        _mobs[0] = GD.Load<PackedScene>("res://Enemies/Enemy1/Enemy1.tscn");
        Position = Vector2.Zero;
        GlobalPosition = Vector2.Zero;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        SpawnEnemy(delta);
    }

    private void SpawnEnemy(float delta)
    {
        _spawnCounter++;
        if (_spawnCounter % _spawnRate == 0)
        {
            KinematicBody2D enemyInstance = (KinematicBody2D)_mobs[0].Instance();
            enemyInstance.Scale *= (float)GD.RandRange(1, 3);
            float rand = GD.Randf()*delta;
            ((KinematicBody2D)enemyInstance).GlobalPosition =
                _player.GlobalPosition + new Vector2(_spawnDistance,0).Rotated((float)GD.RandRange(0,Mathf.Tau));
            AddChild(enemyInstance);
        }
        
    }
}