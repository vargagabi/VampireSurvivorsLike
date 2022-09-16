using Godot;
using System;
using VampireSurvivorsLike.Weapons;

public class Gun : Item 
{
    private KinematicBody2D _player;
    private int _counter = 0;
    private int _attackSpeed = 100;
    private int NumberOfBullets { get; set; }
    private PackedScene _bullet;
    private Node2D _bulletNode;
    private int _piercing = 1;
    private int _level = 0;
    private float _bulletSpeed = 200;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _player = GetNode<KinematicBody2D>("../../Player");
        NumberOfBullets = 1;
        _bullet = (PackedScene)ResourceLoader.Load("res://Weapons/Gun/Bullet.tscn");
        _bulletNode = _bullet.Instance<Bullet>();
        
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
            bulletInst.Set("_piercing",_piercing);
            bulletInst.Set("Direction",
                ((Vector2)_player.Get("_direction"))
                .Rotated((i * Mathf.Pi / 12) -  (Mathf.Pi /12)*(NumberOfBullets-1)/2.0f).Normalized());
            bulletInst.GlobalPosition =
                _player.GlobalPosition + ((Vector2)_player.Get("_direction") ).Normalized() * 10;
            bulletInst.Visible = true;
            AddChild(bulletInst);
            bulletInst.SetAsToplevel(true);
        }
    }

    public override void Upgrade()
    {
        _level++;
        switch (_level)
        {
           case 1:
               _attackSpeed -= 20;
               break;
           case 2:
               NumberOfBullets++;
               break;
           case 3:
               _piercing++;
               break;
           case 4:
               _bulletSpeed += 50;
               break;
           case 5:
               NumberOfBullets++;
               break;
           case 6:
               _piercing++;
               break;
           case 7:
               _attackSpeed -= 10;
               break;
           default:
               NumberOfBullets++;
               break;
        }
    }

    public override string ToString()
    {
        switch (_level)
        {
            case 0:
                return "Increase Attack Speed Of Gun";
            case 1:
                return "Increase Number Of Bullets Of Gun By 1";
            case 2:
                return "Increase Piercing Of Bullets By 1";
            case 3:
                return "Increase Bullet Speed";
            case 4:
                return "Increase Number Of Bullets By 1";
            case 5:
                return "Increase Piercing Of Bullets By 1";
            case 6:
                return "Increase Attack Speed And Number Of Bullets";
            default:
                return "Upgrade A Random Attribute Of The Gun";
        }
    }
}