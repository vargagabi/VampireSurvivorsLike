using Godot;

namespace VampireSurvivorsLike {

    public class GUI : CanvasLayer {

        private HUD hud;

        [Signal] public delegate void RewardSelected(int index);

        // Called when the node enters the scene tree for the first time.
        public override void _Ready() {
            GD.Print("GUI Ready...");
            this.hud = this.GetNode<HUD>("Control/HUD");
        }

        public void GameFinished(bool isVictory, int gold = -1) {
            this.GetNode<GameFinishedScreen>("Control/GameFinishedScreen").GameFinished(isVictory, gold);
            this.GetNode<LevelUpScreen>("Control/LevelUpScreen").Visible = false;
        }

        public void SetItemOnHud(Texture icon, int level) {
            this.hud.SetItem(icon, level);
        }

        /*
         * Sets the current health when receiving the value.
         * If the health is less than or equals 0 show the Game Over screen and pause the game
         */
        public void SetCurrentHealth(int currentHealth) {
            this.hud.SetHealthLabel(currentHealth);
        }

        public void SetCurrentExperience(int percent) {
            this.hud.SetExpBar(percent);
        }

        public void SetCurrentLevel(int level) {
            this.hud.SetLevel(level);
        }

        public void SetGoldCount(int value) {
            this.hud.SetGold(value);
        }

        public void TogglePauseGame(bool isPaused) {
           this.GetNode<PauseScreen>("Control/PauseScreen").TogglePauseGame(isPaused); 
        }

    }

}