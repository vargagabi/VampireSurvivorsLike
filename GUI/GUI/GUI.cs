using Godot;
using System;
using System.Threading.Tasks;

public class GUI : CanvasLayer
{
    private HUD _hud;
    private GameOverScreen _gameOverScreen;
    private float _elapsedTime;
    private LevelUpScreen _levelUpScreen;

    [Signal] public delegate void RewardSelected(int index);
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("GUI Ready...");
        _hud = GetNode<HUD>("Control/HUD");
        _gameOverScreen = GetNode<GameOverScreen>("Control/GameOverScreen");
        _levelUpScreen = GetNode<LevelUpScreen>("Control/LevelUpScreen");
        _elapsedTime = 0;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        _elapsedTime += delta;
        _hud.SetTimeLabel(_elapsedTime);
    }

    public void OnPlayerCurrentHealth(float currentHealth)
    {
        _hud.SetHealthLabel(currentHealth);
        if (currentHealth <= 0)
        {
            GetTree().Paused = true;
           _gameOverScreen.SetScoreLabel(_elapsedTime); 
        }
    }

    public void OnExpEarned(float exp, int level)
    {
        _hud.SetExpbar(exp,level);
    }

    //Signal from the Player to set up the HUD for the rewards
    public void OnPlayerChooseReward(string opt0,string opt1,  string opt2, string opt3)
    {
       _levelUpScreen.SetRewards(new string[]{opt0,opt1,opt2,opt3}); 
    }

    //Signal from the HUD
    public void OnRewardSelected(int index)
    {
        _levelUpScreen.Visible = false;
        EmitSignal(nameof(RewardSelected),index);
    }
}