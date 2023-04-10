using Godot;

namespace VampireSurvivorsLike {

    public class PauseScreen : CenterContainer {

        public void TogglePauseGame(bool isPaused) {
            this.Visible = isPaused;
            if (isPaused) {
                this.GetNode<Button>("VBoxContainer/ResumeButton").GrabFocus();
            }
        }

        public void OnResumeButtonPressed() {
            if (GameStateManagerSingleton.Instance.IsMultiplayer) {
                this.GetTree().Root.GetNode<Main>("Main").Rpc(nameof(Main.TogglePauseGame), false);
            } else {
                this.GetTree().Root.GetNode<Main>("Main").TogglePauseGame(false);
            }
        }

        public void OnSettingsButtonPressed() {
            this.Visible = false;
        }

        public void OnQuitButtonPressed() {
            GetTree().Paused = false;
            this.GetTree().Root.GetNode<Network>("Network").ConnectionClosed();
            this.GetTree().Root.GetNode("Main").QueueFree();
            GetTree().ChangeScene("res://GUI/Menu/Menu.tscn");
        }

    }

}