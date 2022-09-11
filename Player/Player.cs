using Godot;
using System;
using System.Collections.Generic;
using Object = System.Object;

public class Player : KinematicBody2D
{
    //Player attributes
    private float _currentHealth = 200;
    private KeyValuePair<string, float> _maxHealth = new KeyValuePair<string, float>("Increase max health", 200f);
    private KeyValuePair<string, float> _healthRegen =
        new KeyValuePair<string, float>("Increase health regeneration", 1.0f);
    private int _healthCounter = 0;

    private KeyValuePair<string, float> _speed = new KeyValuePair<string, float>("Increase speed", 100.0f);
    private KeyValuePair<string, float> _pickupRange = new KeyValuePair<string, float>("Increase pickup range", 10.0f);
    private List<KeyValuePair<string, float>> _upgradeableStats = new List<KeyValuePair<string, float>>();
    private Vector2 _direction { get; set; }

    private int _immunityTime = 25;
    private int _damageCounter = 0;
    private float _damageValue = 0;

    //The function to calculate the required xp between levels: f(x) = 200x , where x->the level
    private int _experience = 0;
    private int _currentLevel = 0;

    private enum Rewards
    {
        StatBonus, //Add bonus to one stat
        UpgradeWeapon, //Upgrade one of the equipped weapon
        ChooseWeapon, //Choose one new weapon if there is slot for it
        Consumables, //Choose one one-time use bonus, like heal 10 hp, or get 100 coins
    }

    private enum Stats
    {
        Health,
        Regeneration,
        Speed,
        Damage,
    }

    private List<Node2D> _allWeapons = new List<Node2D>();
    private int _weaponCount = 4;
    private List<Node2D> _equippedWeapons = new List<Node2D>();


    private AnimatedSprite _animatedSprite;
    private TextureProgress _healthBar;
    private Texture[] _textures = new Texture[3];

    //Signals
    [Signal] public delegate void GameOver();
    [Signal] public delegate void CurrentHealth(float currentHealth);
    [Signal] public delegate void CurrentExperience(float exp, int level);
    [Signal] public delegate void ChooseReward(Object options);


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("Player Ready...");
        _direction = Vector2.Right;
        _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        _healthBar = GetNode<TextureProgress>("Node2D/HealthBar");
        _textures[0] = ResourceLoader.Load("res://Textures/bar_green_mini.png") as Texture;
        _textures[1] = ResourceLoader.Load("res://Textures/bar_yellow_mini.png") as Texture;
        _textures[2] = ResourceLoader.Load("res://Textures/bar_red_mini.png") as Texture;
        _upgradeableStats.Add(_maxHealth);
        _upgradeableStats.Add(_healthRegen);
        _upgradeableStats.Add(_speed);
        _upgradeableStats.Add(_pickupRange);

        //Add the weapons the player can choose
        _allWeapons.Add((ResourceLoader.Load<PackedScene>("res://Weapons/Gun/Gun.tscn")).Instance() as Node2D);

        EquipWeapon(_allWeapons[0]);

        //Emit signals to set the HUD health and level bars
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

        if (_currentHealth < _maxHealth.Value)
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
        MoveAndSlide(velocity.Normalized() * _speed.Value);
    }

    //Update the health bar depending on the current health
    private void UpdateHealthBar()
    {
        _healthBar.Value = _currentHealth / _maxHealth.Value * 100;
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
            GetTree().Paused = true;
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
            _currentHealth = _healthRegen.Value + _currentHealth > _maxHealth.Value
                ? _maxHealth.Value
                : _healthRegen.Value + _currentHealth;
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

        //If leveled up, choose rewards depending on the number of level ups
        if (_currentLevel < LvlToExp(_experience))
        {
            SelectRewardOnLevelUp(LvlToExp(_experience) - _currentLevel);
            _currentLevel = LvlToExp(_experience);
        }

        float currentExpInLevel = 100 * (_experience - (float)ExpToLvl(_currentLevel)) /
                                  ((float)ExpToLvl(_currentLevel + 1) - ExpToLvl(_currentLevel));

        // GD.Print("EXP THIS LEVEL: " + (ExpToLvl(_currentLevel + 1) - ExpToLvl(_currentLevel)));
        // GD.Print("Level: " + _currentLevel);
        // GD.Print("Exp: " + _experience);
        // GD.Print("Current exp: " + currentExpInLevel);
        EmitSignal(nameof(CurrentExperience), currentExpInLevel, _currentLevel);
    }

    private void SelectRewardOnLevelUp(int levels)
    {
        GD.Print("CONGRATULATIONS YOU LEVELED UP YOU BASTARD :D");
        for (int i = 0; i < levels; i++)
        {
            //For every level the player can choose a reward,
            //1. Randomly choose the options to the rewards
            GD.Print("Choose a reward" + i);
            Object[] options = new Object[4];
            List<Object> rewards = new List<object>();
            foreach (var keyValuePair in _upgradeableStats)
            {
                rewards.Add(keyValuePair);
            }

            rewards.AddRange(_equippedWeapons);
            rewards.AddRange(_allWeapons);
            GD.Print("Count: " + rewards.Count);
            foreach (var reward in rewards)
            {
                if (reward is KeyValuePair<string, float>)
                {
                    GD.Print(((KeyValuePair<string, float>)reward).Key);
                }
                else if (reward is Node2D)
                {
                    GD.Print(((Node2D)reward).ToString());
                    ((Gun)reward).Set("NumberOfBullets",5);
                }
            }

            GD.Print("\n");
            //Select 4 options from the rewards list
            for (int j = 0; j < 4; j++)
            {
                int rand = (int)GD.RandRange(0, rewards.Count);
                options[j] = rewards[rand];
                rewards.Remove(rewards[rand]);
            }

            GD.Print("Options: ");
            foreach (var reward in options)
            {
                if (reward is KeyValuePair<string, float>)
                {
                    GD.Print(((KeyValuePair<string, float>)reward).Key);
                }
                else if (reward is Node2D)
                {
                    GD.Print(((Node2D)reward).ToString());
                }
            }

            //2. Send the list to the HUD to display it to the player
            //Wait for the player to choose, pause the game while waiting

            EmitSignal(nameof(ChooseReward), options);

            //3. Get the choosen reward and add it to the character
        }
    }

    private string RandomNewWeapon()
    {
        throw new NotImplementedException();
    }

    private void EquipWeapon(Node2D weapon)
    {
        AddChild(weapon);
        _equippedWeapons.Add(weapon);
        _allWeapons.Remove(weapon);
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