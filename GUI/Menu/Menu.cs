using System;
using Godot;

namespace VampireSurvivorsLike {

    public class Menu : Control {

        private CenterContainer _mainMenu;

        public override void _Ready() {
            _mainMenu = GetNode<CenterContainer>("MainMenu");
            ((Button)_mainMenu.GetChild(0).GetChild(0)).GrabFocus();
        }

        public void OnStartButtonPressed() {
            GetTree().ChangeScene("res://Main/Main.tscn");
            LevelUpManagerSingleton.Instance.Reset();
            ItemManagerSingleton.Instance.Reset();
            // AttributeManagerSingleton.Instance.Reset();
        }

        public void OnQuitButtonPressed() {
            AttributeManagerSingleton.Instance.Save();
            GetTree().Quit();
        }

        public void OnSettingsButtonPressed() {
            Console.WriteLine("Settings button pressed");
        }

        public void OnShopButtonPressed() {
           GD.Print("Shop button pressed"); 
           this.GetTree().ChangeScene("res://Shop/Shop.tscn");
        }
    }

}