using Godot;

namespace VampireSurvivorsLike {

    public class PauseScreen : CenterContainer {

        private LevelUpScreen levelUpScreen;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready() {
            this.levelUpScreen = GetNode<LevelUpScreen>("../LevelUpScreen");
        }

        public void TogglePauseGame(bool isPaused) {
            this.Visible = isPaused;
            if (isPaused) {
                this.GetNode<Button>("VBoxContainer/ResumeButton").GrabFocus();
            }
        }

        public void OnResumeButtonPressed() {
            if (GameStateManagerSingleton.Instance.IsMultiplayer) {
                this.GetTree().Root.GetNode<Main>("Main").Rpc(nameof(Main.TogglePauseGame), false);
                return;
            }
            this.GetTree().Root.GetNode<Main>("Main").TogglePauseGame(false);
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