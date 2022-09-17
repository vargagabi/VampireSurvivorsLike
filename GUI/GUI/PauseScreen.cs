using Godot;
using System;

public class PauseScreen : CenterContainer
{
    private bool isPaused;
    private ColorRect _colorRect;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        isPaused = false;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (Input.IsActionJustReleased("ui_cancel"))
        {
            GD.Print("STOPPED");
            isPaused = !isPaused;
            GetTree().Paused = isPaused;
            if (isPaused)
            {
                GetNode<Button>("VBoxContainer/ResumeButton").GrabFocus();
            }
        }

        this.Visible = isPaused;
    }

    public void OnResumeButtonPressed()
    {
        isPaused = false;
        GetTree().Paused = isPaused;
        Visible = false;
    }

    public void OnQuitButtonPressed()
    {
        GetTree().Paused = false;        
        GetTree().ChangeScene("res://GUI/Menu/Menu.tscn");
        
    }
    
}