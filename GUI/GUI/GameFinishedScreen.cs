using Godot;

namespace VampireSurvivorsLike {

    public class GameFinishedScreen : CenterContainer {

        public void GameFinished(bool isVictory, int gold) {
            this.Visible = true;
            if (isVictory) {
                GetNode<Label>("VBoxContainer/Label").Text = "Congratulations!\nYou Win!";
                GetChild(1).GetNode<Label>("GoldContainer/GoldLabel").Text = gold.ToString();
            } else {
                GetNode<Label>("VBoxContainer/Label").Text = "Game Over!";
                GetNode<HBoxContainer>("VBoxContainer/GoldContainer").Visible = false;
            }
        }


        public void OnVisibilityChanged() {
            if (this.Visible) {
                this.GetNode<Button>("VBoxContainer/MainMenuButton").GrabFocus();
            }
        }

        public void OnMainMenuButtonPressed() {
            GetTree().Paused = false;
            this.GetTree().Root.GetNode<Network>("Network").ConnectionClosed();
            this.GetTree().Root.GetNode("Main").QueueFree();
            GetTree().ChangeScene("res://GUI/Menu/Menu.tscn");
        }

    }

}