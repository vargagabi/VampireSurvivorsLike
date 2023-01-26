using Godot;
using System;

namespace VampireSurvivorsLike {

    public class HUD : Control {

        private Label timeLabel;
        private Label HpNumber { get; set; }
        private TextureProgress expBar;
        private Label LevelNumber { get; set; }
        internal float ElapsedTime { get; set; }

        // Called when the node enters the scene tree for the first time.
        public override void _Ready() {
            GD.Print("HUD Ready...");
            this.ElapsedTime = 0;
            this.timeLabel = GetNode<Label>("TimerControl/TimeLabel");
            this.HpNumber = GetNode<Label>("HpControl/HpNumber");
            this.LevelNumber = GetNode<Label>("LevelControl/LevelNumber");
            this.expBar = GetNode<TextureProgress>("ExpBar");
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta) {
            this.ElapsedTime += delta;
            this.timeLabel.Text = TimeSpan.FromSeconds(this.ElapsedTime).ToString("hh':'mm':'ss");
        }

        public void SetHealthLabel(float currentHealth) {
            this.HpNumber.Text = currentHealth.ToString();
        }

        public void SetExpBar(float value, int level) {
            this.expBar.Value = value;
            this.LevelNumber.Text = level.ToString();
        }

    }

}