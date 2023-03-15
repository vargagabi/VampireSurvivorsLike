using Godot;
using System;

namespace VampireSurvivorsLike {

    public class HUD : Control {

        private Label timeLabel;

        // private Label HpNumber { get; set; }
        // private TextureProgress expBar;
        // private Label LevelNumber { get; set; }
        internal float ElapsedTime { get; set; }
        private HBoxContainer ItemControl { get; set; }
        private int nextEmpyItemFrame = 0;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready() {
            GD.Print("HUD Ready...");

            // ItemManagerSingleton.Instance.Hud = this;
            this.ElapsedTime = 0;
            this.timeLabel = GetNode<Label>("TimerControl/TimeLabel");

            // this.HpNumber = GetNode<Label>("HpControl/HpNumber");
            // this.LevelNumber = GetNode<Label>("LevelControl/LevelNumber");
            // this.expBar = GetNode<TextureProgress>("ExpBar");
            this.ItemControl = GetNode<HBoxContainer>("ItemControl");

            // this.expBar.Value = 0;
            // this.LevelNumber.Text = "0";
            GetNode<Label>("GoldContainer/Label").Text = "0";
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta) {
            this.ElapsedTime += delta;
            this.timeLabel.Text = TimeSpan.FromSeconds(this.ElapsedTime).ToString("hh':'mm':'ss");
        }

        public void SetItem(int index, Texture icon, int level) {
            //Not new  item
            foreach (Control frame in this.ItemControl.GetChildren()) {
                if (frame.GetChild<TextureRect>(1).Texture != null &&
                    frame.GetChild<TextureRect>(1).Texture.Equals(icon)) {
                    frame.GetChild<Label>(2).Text = level.ToString();
                    return;
                }
            }

            //New item
            Control itemFrame = this.ItemControl.GetChild<Control>(this.nextEmpyItemFrame++);
            TextureRect iconTexture = itemFrame.GetChild<TextureRect>(1);
            iconTexture.Texture = icon;
            itemFrame.GetChild<Label>(2).Text = level.ToString();
        }

        public void SetHealthLabel(int currentHealth) {
            this.GetNode<Label>("HpControl/HpNumber").Text = currentHealth.ToString();
        }

        public void SetExpBar(int percent) {
            this.GetNode<TextureProgress>("ExpBar").Value = percent;
        }

        public void SetLevel(int value) {
            this.GetNode<Label>("LevelControl/LevelNumber").Text = value.ToString();
        }

        public void OnRewardSelected(int index) {
            // this.IncreaseLevel(null);
        }

        public void SetGold(int value) {
            GetNode<Label>("GoldContainer/Label").Text = value.ToString();
        }

    }

}