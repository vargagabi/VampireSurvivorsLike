using Godot;
using System;

public class HUD : Control {
    private Label timeLabel;
    private Label HPLabel;
    private TextureProgress expBar;
    private Label levelLabel;
    internal float ElapsedTime {get; set;}
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        GD.Print("HUD Ready...");
        this.ElapsedTime = 0;
        this.timeLabel = GetNode<Label>("TimeLabel");
        this.HPLabel = GetNode<Label>("HPLabel");
        this.expBar = GetChild<TextureProgress>(2);
        this.levelLabel = GetChild<Label>(3);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta) {
        this.ElapsedTime += delta;
        this.timeLabel.Text = TimeSpan.FromSeconds(this.ElapsedTime).ToString("hh':'mm':'ss");
    }

    public void SetHealthLabel(float currentHealth) {
        this.HPLabel.Text = "HP: " + currentHealth;
    }

    public void SetExpBar(float value, int level) {
        this.expBar.Value = value;
        this.levelLabel.Text = "Lvl: " + level;
    }
}