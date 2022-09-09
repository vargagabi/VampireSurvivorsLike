using Godot;
using System;
using Object = System.Object;

public class Player : KinematicBody2D
{
    //Player attributes
    private float _currentHealth;
    private float _maxHealth;
    private float _healthRegen;
    private int _healthCounter;

    private float _speed;
    private float _pickupRange;
    private Vector2 _direction { get; set; }

    private int _immunityTime;
    private int _damageCounter;
    private float _damageValue;

    //The function to calculate the required xp between levels: f(x) = 200x , where x->the level
    private int _experience;
    private int _currentLevel;


    private AnimatedSprite _animatedSprite;
    private TextureProgress _healthBar;
    private Texture[] _textures = new Texture[3];


    [Signal]
    public delegate void GameOver();

    [Signal]
    public delegate void CurrentHealth(float currentHealth);

    [Signal]
    public delegate void CurrentExperience(float exp, int level);

    [Signal]
    public delegate void LevelUp(int level);


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("Player Ready...");
        _currentHealth = 200;
        _maxHealth = 200;
        _healthRegen = 1.0f;
        _speed = 100;
        _pickupRange = 10;
        _immunityTime = 25;
        _currentLevel = 0;
        _experience = 0;
        _damageCounter = 0;
        _damageValue = 0;
        _direction = Vector2.Right;
        _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        _healthBar = GetNode<TextureProgress>("Node2D/HealthBar");
        _textures[0] = ResourceLoader.Load("res://Textures/bar_green_mini.png") as Texture;
        _textures[1] = ResourceLoader.Load("res://Textures/bar_yellow_mini.png") as Texture;
        _textures[2] = ResourceLoader.Load("res://Textures/bar_red_mini.png") as Texture;

        EmitSignal(nameof(CurrentHealth), _currentHealth);
        EmitSignal(nameof(CurrentExperience), _experience, _currentLevel);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        Move();
        if (_damageValue > 0)
        {
            TakeDamage();
        }

        if (_currentHealth < _maxHealth)
        {
            PassiveHeal();
        }
    }

    //The input related things
    private void Move()
    {
        string animation = "Idle";
        bool isSpacePressed = Input.IsActionPressed("ui_space");
        Vector2 velocity = Vector2.Zero;
        if (Input.IsActionPressed("ui_down"))
        {
            velocity += Vector2.Down;
            _direction += isSpacePressed ? Vector2.Zero : Vector2.Down;
        }

        if (Input.IsActionPressed("ui_up"))
        {
            velocity += Vector2.Up;
            _direction += isSpacePressed ? Vector2.Zero : Vector2.Up;
        }

        if (Input.IsActionPressed("ui_left"))
        {
            velocity += Vector2.Left;
            _direction += isSpacePressed ? Vector2.Zero : Vector2.Left;
        }

        if (Input.IsActionPressed("ui_right"))
        {
            velocity += Vector2.Right;
            _direction += isSpacePressed ? Vector2.Zero : Vector2.Right;
        }

        if (velocity.x == 0 && velocity.y != 0)
        {
            animation = (velocity.y > 0 ? "Down" : "Up");
        }
        else if (velocity.x != 0)
        {
            animation = (velocity.x > 0 ? "Right" : "Left");
        }

        _direction = _direction.Normalized();
        _animatedSprite.Play(animation);
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
        _damageCounter++;
        if (_damageCounter % _immunityTime == 0)
        {
            // GD.Print("Damage taken: " + _damageValue);
            _damageCounter = 0;
            _currentHealth = _currentHealth < (int)_damageValue ? 0 : _currentHealth - (int)_damageValue;
            EmitSignal(nameof(CurrentHealth), _currentHealth);
            UpdateHealthBar();
        }

        if (_currentHealth <= 0)
        {
            // EmitSignal(nameof(GameOver));
            // GetTree().Paused = true;
        }
    }

    //Every x seconds heal
    private void PassiveHeal()
    {
        _healthCounter++;
        if (_healthCounter % _immunityTime == 0)
        {
            // GD.Print("Healed: " + _healthRegen + ", current: " + _currentHealth);
            _healthCounter = 0;
            _currentHealth = _healthRegen + _currentHealth > _maxHealth ? _maxHealth : _healthRegen + _currentHealth;
            EmitSignal(nameof(CurrentHealth), _currentHealth);
            UpdateHealthBar();
        }
    }

    //When an enemy overlaps with the player, the _strength of the enemy is added to the damage received by the player
    public void OnBodyEntered(Node body)
    {
        // _damageValue += damage;
        float dmg = (float)(body.Get("_strength"));
        _damageValue += dmg;
    }

    //When an enemy leave the player, its _strength is substracted from the damage taken by the player
    public void OnBodyExited(Node body)
    {
        float dmg = (float)(body.Get("_strength"));
        _damageValue -= dmg;
    }

    public void OnEnemyKilled(int exp)
    {
        _experience += exp;
        if (_currentLevel < LvlToExp(_experience))
        {
            _currentLevel = LvlToExp(_experience);
            EmitSignal(nameof(LevelUp), _currentLevel);
        }

        float currentExpInLevel = 100 * (_experience - (float)ExpToLvl(_currentLevel)) /
                                  ((float)ExpToLvl(_currentLevel + 1) - ExpToLvl(_currentLevel));

        // GD.Print("EXP THIS LEVEL: " + (ExpToLvl(_currentLevel + 1) - ExpToLvl(_currentLevel)));
        // GD.Print("Level: " + _currentLevel);
        // GD.Print("Exp: " + _experience);
        // GD.Print("Current exp: " + currentExpInLevel);
        EmitSignal(nameof(CurrentExperience), currentExpInLevel, _currentLevel);
    }

    private int ExpToLvl(int lvl)
    {
        return (int)Mathf.Pow((lvl) * 5, 2);
    }

    private int LvlToExp(int exp)
    {
        return (int)Mathf.Sqrt(exp) / 5;
    }
}