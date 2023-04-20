using Godot;

namespace VampireSurvivorsLike {

    public class GUI : CanvasLayer {

        private HUD hud;

        [Signal] public delegate void RewardSelected(int index);

        // Called when the node enters the scene tree for the first time.
        public override void _Ready() {
            this.hud = this.GetNode<HUD>("Control/HUD");
        }

        public void GameFinished(bool isVictory, int gold, int score) {
            this.GetNode<GameFinishedScreen>("Control/GameFinishedScreen").GameFinished(isVictory, gold, score);
            this.GetNode<LevelUpScreen>("Control/LevelUpScreen").Visible = false;
        }

        public void SetItemOnHud(Texture icon, int level) {
            this.hud.SetItem(icon, level);
        }

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
            if (!isPaused) {
                this.GetNode<Settings>("Control/Settings").Visible = false;
            }
        }

        public bool GetSettingsVisible() {
            bool visible = this.GetNode<Settings>("Control/Settings").Visible;
            this.GetNode<Settings>("Control/Settings").Visible = false;
            return visible;
        }

    }

}