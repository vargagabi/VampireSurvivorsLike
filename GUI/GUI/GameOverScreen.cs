using Godot;
using Godot.Collections;

namespace VampireSurvivorsLike {

    public class GameOverScreen : CenterContainer {

        private Label scoreLabel;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready() {
            this.scoreLabel = GetChild(1).GetChild<Label>(0);
            Visible = false;
        }

        public void SetScoreLabel(float score) {
            Visible = true;
            this.scoreLabel.Text = "Your Score: " + (int)score;
            this.GetNode<Button>("VBoxContainer/MenuButton").GrabFocus();
        }

        public void OnMenuButtonPressed() {
            GetTree().Paused = false;
            GetTree().ChangeScene("res://GUI/Menu/Menu.tscn");
        }

    }

}