using Godot;

namespace VampireSurvivorsLike {

    public class PauseScreen : CenterContainer {

        private bool IsPaused { get; set; }
        private LevelUpScreen levelUpScreen;

        internal bool IsPlaying { get; set; }

        // Called when the node enters the scene tree for the first time.
        public override void _Ready() {
            // this.IsPaused = false;
            // this.IsPlaying = true;
            this.levelUpScreen = GetNode<LevelUpScreen>("../LevelUpScreen");
        }

        public void TogglePauseGame(bool isPaused) {
            this.Visible = isPaused;
            if (isPaused) {
                this.GetNode<Button>("VBoxContainer/ResumeButton").GrabFocus();
            }
        }

        // public override void _UnhandledKeyInput(InputEventKey @event) {
        //     base._UnhandledKeyInput(@event);
        //     GameStateEnum state = GameStateManagerSingleton.Instance.GameState;
        //     if (@event.IsActionReleased("ui_cancel") && this.IsPlaying &&
        //         (state.Equals(GameStateEnum.Playing) || state.Equals(GameStateEnum.Paused))) {
        //         GD.Print("STOPPED");
        //         this.PauseGame();
        //     }
        // }


        // public void PauseGame() {
        //     this.IsPaused = !this.IsPaused;
        //     if (this.levelUpScreen.IsCurrentlyVisible) {
        //         this.levelUpScreen.Visible = !this.IsPaused;
        //     } else {
        //         GetTree().Paused = this.IsPaused;
        //     }
        //     this.Visible = this.IsPaused;
        //     if (this.IsPaused) {
        //         this.GetNode<Button>("VBoxContainer/ResumeButton").GrabFocus();
        //         AudioPlayerSingleton.Instance.SwitchToAmbient();
        //         GameStateManagerSingleton.Instance.GameState = GameStateEnum.Paused;
        //     } else {
        //         AudioPlayerSingleton.Instance.SwitchToAction();
        //         GameStateManagerSingleton.Instance.GameState = GameStateEnum.Playing;
        //     }
        // }

        public void OnResumeButtonPressed() {
            if (GameStateManagerSingleton.Instance.IsMultiplayer) {
                this.GetTree().Root.GetNode<Main>("Main").Rpc(nameof(Main.TogglePauseGame), false);
                return;
            }
            this.GetTree().Root.GetNode<Main>("Main").TogglePauseGame(false);
        }

        public void OnQuitButtonPressed() {
            GetTree().Paused = false;
            GetTree().ChangeScene("res://GUI/Menu/Menu.tscn");
        }

    }

}