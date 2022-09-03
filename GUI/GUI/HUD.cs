using Godot;
using System;

public class HUD : Control
{
    //HUD
    // private float _elapsedTime;
    private Label _timeLabel;
    private Label _HPLabel;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("HUD Ready...");
        // _elapsedTime = 0;
        _timeLabel = GetNode<Label>("TimeLabel");
        _HPLabel = GetNode<Label>("HPLabel");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        // _elapsedTime += delta;
    }

    public void SetHealthLabel(float currentHealth)
    {
        _HPLabel.Text = "HP: " + currentHealth;
    }

    public void SetTimeLabel(float elapsedTime)
    {
        _timeLabel.Text = TimeSpan.FromSeconds(elapsedTime).ToString("hh':'mm':'ss");
    }
}