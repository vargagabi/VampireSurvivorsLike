using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VampireSurvivorsLike.Weapons;
using Object = System.Object;

class KeyVal
{
    public string Message { get; }
    public float Value { get; set; } = -1;
    public CircleShape2D Shape { get; set; }
    private float BonusModifier { get; }

    public KeyVal(string msg, float val, float modify)
    {
        this.Message = msg;
        this.Value = val;
        BonusModifier = (100f + modify) / 100f;
    }

    public KeyVal(string msg, CircleShape2D val, float modify)
    {
        this.Message = msg;
        this.Shape = val;
        BonusModifier = (100f + modify) / 100f;
    }

    public float GetValue()
    {
        return Value;
    }

    public void Increase()
    {
        if (Value > -1)
        {
            Value *= BonusModifier;
        }
        else if (Shape != null)
        {
            GD.Print("SHAPEEEE");
            Shape.Radius *= BonusModifier;
        }
    }
}

public class Player : KinematicBody2D
{
    //Player attributes
    private float _currentHealth = 200;
    private KeyVal _maxHealth = new KeyVal("Increase max health", 200f, 10f);
    private KeyVal _healthRegen =
        new KeyVal("Increase health regeneration", 1.0f, 10f);
    private int _healthCounter = 0;

    private KeyVal _speed = new KeyVal("Increase speed", 100.0f, 100f);
    private KeyVal _pickupRange; 
    private List<KeyVal> _upgradeableStats = new List<KeyVal>();
    private Vector2 _direction { get; set; }

    private int _immunityTime = 25;
    private int _damageCounter = 0;
    private float _damageValue = 0;

    //The function to calculate the required xp between levels: f(x) = 200x , where x->the level
    private int _experience = 0;
    private int _currentLevel = 0;


    private List<Node2D> _allWeapons = new List<Node2D>();
    private int _weaponCount = 4;
    private List<Node2D> _equippedWeapons = new List<Node2D>();


    private AnimatedSprite _animatedSprite;
    private TextureProgress _healthBar;
    private CircleShape2D _pickupArea;
    private Texture[] _textures = new Texture[3];
    private Sprite _directionArrow;
    private int _rewardIndex = -1;

    //Signals
    [Signal] public delegate void GameOver();
    [Signal] public delegate void CurrentHealth(float currentHealth);
    [Signal] public delegate void CurrentExperience(float exp, int level);
    [Signal] public delegate void ChooseReward(string opt0, string opt1, string opt2, string opt3);


    // Called when the node enters the scene tree for the first time.
    public override async void _Ready()
    {
        GD.Print("Player Ready...");
        _direction = Vector2.Right;
        _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        _healthBar = GetNode<TextureProgress>("Node2D/HealthBar");
        _textures[0] = ResourceLoader.Load("res://Textures/bar_green_mini.png") as Texture;
        _textures[1] = ResourceLoader.Load("res://Textures/bar_yellow_mini.png") as Texture;
        _textures[2] = ResourceLoader.Load("res://Textures/bar_red_mini.png") as Texture;
        _directionArrow = GetNode<Sprite>("Arrow");
        _pickupArea = GetNode<Area2D>("PickupArea").GetChild<CollisionShape2D>(0).Shape as CircleShape2D;
        _pickupRange = new KeyVal("test", _pickupArea, 30.0f);
        GD.Print(_pickupRange.Shape);
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
        CheckLevelUp();
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

        _directionArrow.Rotation = _direction.Normalized().Angle();
        _directionArrow.Position = _direction.Normalized() * 15;
        _direction = _direction.Normalized();
        _animatedSprite.Play(animation);
        MoveAndSlide(velocity.Normalized() * _speed.GetValue());
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

    // public async void OnEnemyKilled(int exp)
    // {
    //     _experience += exp;
    //     GD.Print("-------------------------------");
    //     GD.Print("EXP REQUIRED THIS LEVEL: " + (LvlToExp(_currentLevel + 1) - LvlToExp(_currentLevel)));
    //     GD.Print("EXP REQUIRED TO NEXT LEVEL" + LvlToExp(_currentLevel + 1));
    //     GD.Print("Level: " + _currentLevel);
    //     GD.Print("All Exp: " + _experience);
    //
    //     //If leveled up, choose rewards depending on the number of level ups
    //     float currentExpInLevel = 100 * (_experience - (float)LvlToExp(_currentLevel)) /
    //                               ((float)LvlToExp(_currentLevel + 1) - LvlToExp(_currentLevel));
    //
    //
    //     GD.Print("Current exp in %: " + currentExpInLevel);
    //     EmitSignal(nameof(CurrentExperience), currentExpInLevel, _currentLevel);
    // }

    private async void CheckLevelUp()
    {
        if (ExpToLvl(_experience) > _currentLevel)
        {
            for (int i = 0; i < ExpToLvl(_experience) - _currentLevel; i++)
            {
                GetTree().Paused = true;
                Object[] options = new object[4];
                _rewardIndex = -1;
                SelectRewardOnLevelUp(options);
                await ToSignal(GetNode("../GUI"), "RewardSelected");
                GD.Print("------------------");
                GD.Print(_rewardIndex);
                GD.Print(options[_rewardIndex]);
                if (options[_rewardIndex] is Item)
                {
                    ((Item)options[_rewardIndex]).Upgrade();
                }
                else if (options[_rewardIndex] is KeyVal)
                {
                    ((KeyVal)options[_rewardIndex]).Increase();
                }

                GetTree().Paused = false;
            }

            _currentLevel = ExpToLvl(_experience);
            float currentExpInLevel = 100 * (_experience - (float)LvlToExp(_currentLevel)) /
                                      ((float)LvlToExp(_currentLevel + 1) - LvlToExp(_currentLevel));
            EmitSignal(nameof(CurrentExperience), currentExpInLevel, _currentLevel);
        }
    }

    //Makee a list of rewards for the player to choose
    private void SelectRewardOnLevelUp(Object[] options)
    {
        GD.Print("SelectRewardOnLevelUp()");

        List<Object> rewards = new List<object>();
        foreach (var keyValuePair in _upgradeableStats)
        {
            rewards.Add(keyValuePair);
        }

        rewards.AddRange(_equippedWeapons);
        rewards.AddRange(_allWeapons);

        //Select 4 options from the rewards list
        for (int j = 0; j < 4; j++)
        {
            int rand = (int)GD.RandRange(0, rewards.Count);
            options[j] = rewards[rand];
            rewards.Remove(rewards[rand]);
        }

        // GD.Print("Options: ");
        string[] optionsString = new string[4];
        for (int j = 0; j < 4; j++)
        {
            if (options[j] is KeyVal)
            {
                // GD.Print(((KeyValuePair<string, float>)options[j]).Key);
                optionsString[j] = ((KeyVal)options[j]).Message;
            }
            else if (options[j] is Node2D)
            {
                // GD.Print(((Node2D)options[j]).ToString());
                optionsString[j] = ((Node2D)options[j]).ToString();
            }
        }

        //2. Send the list to the HUD to display it to the player
        //Wait for the player to choose, pause the game while waiting

        EmitSignal(nameof(ChooseReward), optionsString[0], optionsString[1], optionsString[2], optionsString[3]);

        //3. Get the chosen reward and add it to the character
    }

    //Equips the given weapon, removes it from the _allWeapons and adds it to the _equippedWeapon so it doesn't appear twice in the rewards list
    private void EquipWeapon(Node2D weapon)
    {
        AddChild(weapon);
        _equippedWeapons.Add(weapon);
        _allWeapons.Remove(weapon);
    }

    private int ExpToLvl(int exp)
    {
        return (int)(Math.Sqrt(exp + 4) - 2);
    }

    private int LvlToExp(int lvl)
    {
        return (int)(4 * lvl + Math.Pow(lvl, 2));
    }

    //Signal receiver method
    public void OnRewardSelected(int index)
    {
        // GetTree().Paused = false;
        _rewardIndex = index;
    }

    public void OnPickUp(int exp)
    {
        GD.Print(exp);
        _experience += exp;
        GD.Print("-------------------------------");
        GD.Print("EXP REQUIRED THIS LEVEL: " + (LvlToExp(_currentLevel + 1) - LvlToExp(_currentLevel)));
        GD.Print("EXP REQUIRED TO NEXT LEVEL" + LvlToExp(_currentLevel + 1));
        GD.Print("Level: " + _currentLevel);
        GD.Print("All Exp: " + _experience);

        float currentExpInLevel = 100 * (_experience - (float)LvlToExp(_currentLevel)) /
                                  ((float)LvlToExp(_currentLevel + 1) - LvlToExp(_currentLevel));


        GD.Print("Current exp in %: " + currentExpInLevel);
        EmitSignal(nameof(CurrentExperience), currentExpInLevel, _currentLevel);
    }
}