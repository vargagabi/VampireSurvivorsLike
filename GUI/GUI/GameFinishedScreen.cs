using Godot;
using System;

namespace VampireSurvivorsLike {

    public class GameFinishedScreen : CenterContainer {

        // Called when the node enters the scene tree for the first time.
        public override void _Ready() {
        }

        public void SetGold(int value) {
            GetChild(1).GetNode<Label>("GoldContainer/GoldLabel").Text = value.ToString();
            AttributeManagerSingleton.Instance.Gold += value;
        }

        public void OnVisibilityChanged() {
            if (this.Visible) {
                this.GetNode<Button>("VBoxContainer/Button").GrabFocus();
            }
        }

        public void OnButtonPressed() {
            GetTree().ChangeScene("res://GUI/Menu/Menu.tscn");
            this.GetTree().Root.GetNode("Main").QueueFree();
        }

    }

}