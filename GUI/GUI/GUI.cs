using Godot;

public class GUI : CanvasLayer {
    private HUD hud;
    private GameOverScreen gameOverScreen;
    private float elapsedTime;
    private LevelUpScreen levelUpScreen;

    [Signal] public delegate void RewardSelected(int index);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        GD.Print("GUI Ready...");
        this.hud = GetNode<HUD>("Control/HUD");
        this.gameOverScreen = GetNode<GameOverScreen>("Control/GameOverScreen");
        this.levelUpScreen = GetNode<LevelUpScreen>("Control/LevelUpScreen");
        this.elapsedTime = 0;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta) {
        this.elapsedTime += delta;
        this.hud.SetTimeLabel(this.elapsedTime);
    }

    /*
     * Sets the current health when receiving the value.
     * If the health is less than or equals 0 show the Game Over screen and pause the game
     */
    public void OnPlayerCurrentHealth(float currentHealth) {
        this.hud.SetHealthLabel(currentHealth);
        if (currentHealth <= 0) {
            GetTree().Paused = true;
            this.gameOverScreen.SetScoreLabel(this.elapsedTime);
        }
    }

    public void OnExpEarned(float exp, int level) {
        this.hud.SetExpBar(exp, level);
    }

    /*
     * Sets up the rewards the player can choose on level up
     */
    public void OnPlayerChooseReward(string opt0, string opt1, string opt2, string opt3) {
        this.levelUpScreen.SetRewards(new string[] { opt0, opt1, opt2, opt3 });
    }

    /*
     * Emits a signal of the index of the chosen reward to the Player.
     */
    public void OnRewardSelected(int index) {
        this.levelUpScreen.Visible = false;
        EmitSignal(nameof(RewardSelected), index);
    }
}