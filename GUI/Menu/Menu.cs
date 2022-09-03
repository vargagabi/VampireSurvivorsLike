using Godot;
using System;

public class Menu : Control
{
    private CenterContainer _mainMenu;
    // private CenterContainer _GameOverScreen;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _mainMenu = GetNode<CenterContainer>("MainMenu");
        // _GameOverScreen = GetNode<CenterContainer>("GameOverScreen");

        // _GameOverScreen.Visible = false;
        ((Button)_mainMenu.GetChild(0).GetChild(0)).GrabFocus();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
    public void OnStartButtonPressed()
    {
        GetTree().ChangeScene("res://Main/Main.tscn");
    }
}
