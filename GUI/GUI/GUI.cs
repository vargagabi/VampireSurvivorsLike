using Godot;
using System;

public class GUI : CanvasLayer
{
    private Label _HPLabel;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _HPLabel = GetNode<Label>("Control/HUD/HPLabel");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {

   }

    public void OnPlayerCurrentHealth(float currentHealth)
    {
        _HPLabel.Text = "HP: " + currentHealth;
        GD.Print("CURRENT HEALTH SIGNAL RECEIVED");
    }

}