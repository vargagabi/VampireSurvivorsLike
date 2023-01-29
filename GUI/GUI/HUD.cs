using Godot;
using System;

namespace VampireSurvivorsLike {

    public class HUD : Control {

        private Label timeLabel;
        private Label HpNumber { get; set; }
        private TextureProgress expBar;
        private Label LevelNumber { get; set; }
        internal float ElapsedTime { get; set; }
        private HBoxContainer ItemControl { get; set; }

        // Called when the node enters the scene tree for the first time.
        public override void _Ready() {
            GD.Print("HUD Ready...");
            ItemManagerSingleton.Instance.Hud = this;
            this.ElapsedTime = 0;
            this.timeLabel = GetNode<Label>("TimerControl/TimeLabel");
            this.HpNumber = GetNode<Label>("HpControl/HpNumber");
            this.LevelNumber = GetNode<Label>("LevelControl/LevelNumber");
            this.expBar = GetNode<TextureProgress>("ExpBar");
            this.ItemControl = GetNode<HBoxContainer>("ItemControl");
            this.expBar.Value = 0;
            this.LevelNumber.Text = "0";
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta) {
            this.ElapsedTime += delta;
            this.timeLabel.Text = TimeSpan.FromSeconds(this.ElapsedTime).ToString("hh':'mm':'ss");
        }

        public void AddItem(Item item) {
            Control itemFrame = this.ItemControl.GetChild<Control>(ItemManagerSingleton.Instance.EquippedItemCount);
            itemFrame.GetChild<TextureRect>(0).Texture = item.Icon;
        }

        public void SetItemLevel(int index, int level) {
            Control itemFrame = this.ItemControl.GetChild<Control>(index);
            itemFrame.GetChild<Label>(1).Text = level.ToString();
        }

        public void SetHealthLabel(float currentHealth) {
            this.HpNumber.Text = currentHealth.ToString();
        }

        public void SetExpBar(int percent) {
            this.expBar.Value = percent;
        }

        private void IncreaseLevel(int? value) {
            this.LevelNumber.Text =
                value == null ? (int.Parse(this.LevelNumber.Text) + 1).ToString() : value.ToString();
        }

        public void OnRewardSelected(int index) {
            this.IncreaseLevel(null);
        }

    }

}