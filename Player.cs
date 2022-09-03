using Godot;
using System;
using Object = System.Object;

public class Player : KinematicBody2D
{
    //Player attributes
    private float _currentHealth;
    private float _maxHealth;
    private float _healthRegen;
    private int _healthTimer;
    
    private float _speed;
    private float _pickupRange;

    private int _immunityTime;
    private int _damageTimer;
    private float _damageValue;

    
    private AnimatedSprite _animatedSprite;
    private TextureProgress _healthBar;
    private Texture[] _textures = new Texture[3];

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _currentHealth = 200;
        _maxHealth = 200;
        _healthRegen = 1.0f;
        _speed = 100;
        _pickupRange = 10;
        _immunityTime = 50;
        _damageTimer = 0;
        _damageValue = 0;
        _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        _healthBar = GetNode<TextureProgress>("Node2D/HealthBar");
        _textures[0] = ResourceLoader.Load("res://Textures/bar_green_mini.png") as Texture;
        _textures[1] = ResourceLoader.Load("res://Textures/bar_yellow_mini.png") as Texture;
        _textures[2] = ResourceLoader.Load("res://Textures/bar_red_mini.png") as Texture;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        Move();
        TakeDamage();
        PassiveHeal();
    }

    //The input related things
    private void Move()
    {
        if (Input.IsActionPressed("ui_cancel"))
        {
            GetTree().Quit();
        }

        Vector2 velocity = Vector2.Zero;
        if (Input.IsActionPressed("ui_down"))
        {
            velocity += Vector2.Down;
            // _animatedSprite.Play("Down");
        }

        if (Input.IsActionPressed("ui_up"))
        {
            velocity += Vector2.Up;
            // _animatedSprite.Play("Up");
        }

        if (Input.IsActionPressed("ui_left"))
        {
            velocity += Vector2.Left;
            // _animatedSprite.Play("Left");
        }

        if (Input.IsActionPressed("ui_right"))
        {
            velocity += Vector2.Right;
            // _animatedSprite.Play("Right");
        }

        if (velocity.x == 0)
        {
            _animatedSprite.Play(velocity.y > 0?"Down":"Up");
        }
        else
        {
            _animatedSprite.Play(velocity.x > 0?"Right":"Left");
        }
        if(velocity == Vector2.Zero)
        {
            _animatedSprite.Play("Idle");
        }

        MoveAndSlide(velocity.Normalized() * _speed);
    }

    //Update the health bar depending on the current health
    private void UpdateHealthBar()
    {
        _healthBar.Value = _currentHealth / _maxHealth * 100;
        if (_healthBar.Value < 100)
        {
            _healthBar.TextureProgress_ = _textures[0];
        }
        if (_healthBar.Value < 50)
        {
            _healthBar.TextureProgress_ = _textures[1];
        }

        if (_healthBar.Value < 25)
        {
            _healthBar.TextureProgress_ = _textures[2];
        }
    }

    //Every x seconds take the added damage of the overlapping enemies
    private void TakeDamage()
    {
        _damageTimer++;
        if (_damageTimer % _immunityTime == 0)
        {
            GD.Print("Damage taken: " + _damageValue);
            _damageTimer = 0;
            _currentHealth = _currentHealth < (int)_damageValue ? 0 : _currentHealth - (int)_damageValue;
            UpdateHealthBar();
        }
    }
    
    //Every x seconds heal
    private void PassiveHeal()
    {
        _healthTimer++;
        if (_healthTimer % _immunityTime == 0)
        {
            GD.Print("Healed: " + _healthRegen + ", current: " + _currentHealth);
            _healthTimer = 0;
            _currentHealth = _healthRegen+_currentHealth>_maxHealth?_maxHealth:_healthRegen+_currentHealth;
            UpdateHealthBar();
        }
    }

    //When an enemy overlaps with the player, the _strength of the enemy is added to the damage received by the player
    public void OnBodyEntered(Node body)
    {
        // _damageValue += damage;
        GD.Print(body.Get("_strength"));
        _damageTimer = _immunityTime - 1;
        float dmg = (float)(body.Get("_strength"));
        _damageValue += dmg;
    }

    //When an enemy leave the player, its _strength is substracted from the damage taken by the player
    public void OnBodyExited(Node body)
    {
        float dmg = (float)(body.Get("_strength"));
        _damageValue -= dmg;
    }
}