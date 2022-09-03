using Godot;
using System;

public class GUI : CanvasLayer
{
    private HUD _hud;
    private GameOverScreen _gameOverScreen;
    private float _elapsedTime;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("GUI Ready...");
        _hud = GetNode<HUD>("Control/HUD");
        _gameOverScreen = GetNode<GameOverScreen>("Control/GameOverScreen");
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
        // _HPLabel.Text = "HP: " + currentHealth;
        _hud.SetHealthLabel(currentHealth);
        if (currentHealth <= 0)
        {
            GetTree().Paused = true;
           _gameOverScreen.SetScoreLabel(_elapsedTime); 
        }
    }
}