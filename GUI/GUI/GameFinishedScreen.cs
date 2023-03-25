using Godot;
using System;

namespace VampireSurvivorsLike {

    public class GameFinishedScreen : CenterContainer {


        public void GameFinished(bool isVictory, int gold = -1) {
            this.Visible = true;
            if (isVictory) {
                GetNode<Label>("VBoxContainer/Label").Text = "Congratulations!\nYou Win!";
                if (gold >= 0) {
                    SetGold(gold);
                }
            } else {
                GetNode<Label>("VBoxContainer/Label").Text = "Game Over!";
                GetNode<HBoxContainer>("VBoxContainer/GoldContainer").Visible = false;
            }
        }

        private void SetGold(int value) {
            GetChild(1).GetNode<Label>("GoldContainer/GoldLabel").Text = value.ToString();
        }

        public void OnVisibilityChanged() {
            if (this.Visible) {
                this.GetNode<Button>("VBoxContainer/Button").GrabFocus();
            }
        }

        public void OnButtonPressed() {
            GetTree().Paused = false;
            this.GetTree().Root.GetNode<Network>("Network").ConnectionClosed();
            this.GetTree().Root.GetNode("Main").QueueFree();
            GetTree().ChangeScene("res://GUI/Menu/Menu.tscn");
        }

    }

}