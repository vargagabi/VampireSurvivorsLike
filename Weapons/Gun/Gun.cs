using Godot;
using System;

public class Gun : Node2D
{
    private KinematicBody2D _player;
    private int _counter;
    private int _attackSpeed;
    private int NumberOfBullets { get; set; }
    private PackedScene _bullet;
    private int _level;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _player = GetNode<KinematicBody2D>("../../Player");
        _level = 0;
        _counter = 0;
        _attackSpeed = 100;
        NumberOfBullets = 3;
        _bullet = (PackedScene)ResourceLoader.Load("res://Weapons/Gun/Bullet.tscn");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        _counter++;
        if (_counter % _attackSpeed == 0)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        for (int i = 0; i < NumberOfBullets; i++)
        {
            Bullet bulletInst = (Bullet)_bullet.Instance();
            bulletInst.Set("_direction",
                ((Vector2)_player.Get("_direction"))
                .Rotated((i * Mathf.Pi / 12) -  (Mathf.Pi /12)*(NumberOfBullets-1)/2.0f).Normalized());
            bulletInst.GlobalPosition =
                _player.GlobalPosition + ((Vector2)_player.Get("_direction") ).Normalized() * 10;
            bulletInst.Visible = true;
            AddChild(bulletInst);
            bulletInst.SetAsToplevel(true);
        }
    }

    public void Upgrade()
    {
        _level++;
        NumberOfBullets++;
        _attackSpeed--;

    }

    public override string ToString()
    {
        return "This is a shotgun, deals basic damage.";
    }
}