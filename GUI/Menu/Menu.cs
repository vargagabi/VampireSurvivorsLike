using System;
using Godot;

namespace VampireSurvivorsLike {

    public class Menu : Control {

        private CenterContainer _mainMenu;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready() {
            _mainMenu = GetNode<CenterContainer>("MainMenu");
            ((Button)_mainMenu.GetChild(0).GetChild(0)).GrabFocus();
        }

        //  // Called every frame. 'delta' is the elapsed time since the previous frame.
        //  public override void _Process(float delta)
        //  {
        //      
        //  }

        public void OnStartButtonPressed() {
            GetTree().ChangeScene("res://Main/Main.tscn");
        }

        public void OnQuitButtonPressed() {
            GetTree().Quit();
        }

        public void OnSettingsButtonPressed() {
            Console.WriteLine("Settings button pressed");
        }

    }

}