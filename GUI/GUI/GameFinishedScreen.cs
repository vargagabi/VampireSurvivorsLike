using Godot;

namespace VampireSurvivorsLike {

    public class GameFinishedScreen : CenterContainer {

        public void GameFinished(bool isVictory, int gold, int score) {
            this.Visible = true;
            if (isVictory) {
                this.GetNode<Label>("VBoxContainer/Label").Text = "Congratulations!\nYou Win!";
                this.GetChild(1).GetNode<Label>("GoldContainer/GoldLabel").Text = gold.ToString();
            } else {
                this.GetNode<Label>("VBoxContainer/Label").Text = "Game Over!";
                this.GetNode<HBoxContainer>("VBoxContainer/GoldContainer").Visible = false;
            }
            this.GetNode<Label>("VBoxContainer/ScoreContainer/ScoreLabel").Text = score.ToString();
        }


        public void OnVisibilityChanged() {
            if (this.Visible) {
                this.GetNode<Button>("VBoxContainer/MainMenuButton").GrabFocus();
            }
        }

        public void OnMainMenuButtonPressed() {
            this.GetTree().Paused = false;
            this.GetTree().Root.GetNode<Network>("Network").ConnectionClosed();
            this.GetTree().Root.GetNode("Main").QueueFree();
            this.GetTree().ChangeScene("res://GUI/Menu/Menu.tscn");
        }

    }

}