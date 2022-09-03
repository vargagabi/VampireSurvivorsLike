using Godot;
using System;

public class HUD : Control
{
    //HUD
    private float _elapsedTime;
    private Label _timeLabel;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _elapsedTime = 0;
        _timeLabel = GetNode<Label>("TimeLabel");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        _elapsedTime += delta;
        _timeLabel.Text = TimeSpan.FromSeconds(_elapsedTime).ToString("hh':'mm':'ss");
    }
}