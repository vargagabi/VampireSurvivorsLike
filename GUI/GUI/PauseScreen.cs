using Godot;

namespace VampireSurvivorsLike {

    public class PauseScreen : CenterContainer {

        private bool IsPaused { get; set; }
        private LevelUpScreen levelUpScreen;

        internal bool IsPlaying { get; set; }

        // Called when the node enters the scene tree for the first time.
        public override void _Ready() {
            this.IsPaused = false;
            this.IsPlaying = true;
            this.levelUpScreen = GetNode<LevelUpScreen>("../LevelUpScreen");
        }

        public override void _UnhandledKeyInput(InputEventKey @event) {
            base._UnhandledKeyInput(@event);
            if (@event.IsActionReleased("ui_cancel") && this.IsPlaying) {
                GD.Print("STOPPED");
                this.PauseGame();
                if (this.IsPaused) {
                    this.GetNode<Button>("VBoxContainer/ResumeButton").GrabFocus();
                }
            }
        }

        public void OnQuitButtonPressed() {
            GetTree().Paused = false;
            GetTree().ChangeScene("res://GUI/Menu/Menu.tscn");
        }

        public void PauseGame() {
            this.IsPaused = !this.IsPaused;
            if (this.levelUpScreen.IsCurrentlyVisible) {
                this.levelUpScreen.Visible = !this.IsPaused;
            } else {
                GetTree().Paused = this.IsPaused;
            }
            this.Visible = this.IsPaused;
        }

    }

}