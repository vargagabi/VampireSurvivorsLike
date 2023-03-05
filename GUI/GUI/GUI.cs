using Godot;

namespace VampireSurvivorsLike {

    public class GUI : CanvasLayer {

        private HUD hud;
        private PauseScreen pauseScreen;
        private GameFinishedScreen gameFinishedScreen;

        private int gold = 0;

        [Signal] public delegate void RewardSelected(int index);

        // Called when the node enters the scene tree for the first time.
        public override void _Ready() {
            GD.Print("GUI Ready...");
            this.hud = this.GetNode<HUD>("Control/HUD");
            this.pauseScreen = this.GetNode<PauseScreen>("Control/PauseScreen");
            this.gameFinishedScreen = this.GetNode<GameFinishedScreen>("Control/GameFinishedScreen");
        }

        internal void GameEnded(bool victory, int enemiesDefeated = 0, int gold = -1) {
            this.gameFinishedScreen.GameFinished(victory, enemiesDefeated, gold);
        }

        /*
         * Sets the current health when receiving the value.
         * If the health is less than or equals 0 show the Game Over screen and pause the game
         */
        public void OnPlayerCurrentHealth(float currentHealth) {
            // this.hud.SetHealthLabel(currentHealth);
            // if (currentHealth <= 0) {
            // this.gameOverScreen.SetScoreLabel(this.hud.ElapsedTime);
            //     this.pauseScreen.IsPlaying = false;
            // }
        }

        public void OnExperienceEarned(int percent) {
            // this.hud.SetExpBar(percent);
        }

        public void OnGoldPickedUp(int value) {
            // this.gold += value;
            // this.hud.SetGold(this.gold);
        }

        public void OnGameWon() {
            // this.GetTree().Paused = true;
            // this.gameFinishedScreen.SetGold(this.gold);
            // this.gameFinishedScreen.Visible = true;
        }

    }

}