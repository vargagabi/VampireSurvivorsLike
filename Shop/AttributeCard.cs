using Godot;
using System;

namespace VampireSurvivorsLike {

    public class AttributeCard : TextureButton {

        private int maxLevel;
        private TextureRect Icon { get; set; }
        private Label NameLabel { get; set; }
        private Label CostLabel { get; set; }
        private TextureProgress LevelProgress { get; set; }
        private Sprite Outline { get; set; }
        private Shop Receiver { get; set; }
        private string attributeName;


        [Signal] public delegate void Purchase(string name);

        public void init(string name, string nameText, int progress, int maxLevel, Texture iconTexture, int cost, Shop receiver) {
            this.attributeName = name;
            this.NameLabel.Text = nameText;
            this.CostLabel.Text = progress >= maxLevel? "Max" : cost.ToString();
            this.LevelProgress.Value = progress;
            this.Icon.Texture = iconTexture;
            this.Receiver = receiver;
            this.maxLevel = maxLevel;
            this.Disabled = this.LevelProgress.Value >= this.maxLevel;
            if (this.Receiver != null) {
                this.Connect(nameof(Purchase), this.Receiver, nameof(this.Receiver.OnPurchase));
            }
        }

        public override void _Ready() {
            this.Icon = GetNode<TextureRect>("CenterContainer/TextureRect");
            this.NameLabel = GetNode<Label>("NameLabel");
            this.LevelProgress = GetNode<TextureProgress>("CenterContainer2/TextureProgress");
            this.CostLabel = GetNode<Label>("CostLabel");
            this.Outline = GetNode<Sprite>("Sprite");
        }

        public void RefreshInfo() {
            this.LevelProgress.Value = AttributeManagerSingleton.Instance.GetAttributeBaseLevel(this.attributeName);
            this.CostLabel.Text = AttributeManagerSingleton.Instance.GetAttributeCost(this.attributeName).ToString();
            this.Disabled = false;
        }

        public void OnButtonPressed() {
            if (AttributeManagerSingleton.Instance.IncreaseBaseLevel(this.attributeName)) {
                this.LevelProgress.Value =
                    AttributeManagerSingleton.Instance.GetAttributeBaseLevel(this.attributeName);
                if (this.LevelProgress.Value >= this.maxLevel) {
                    this.CostLabel.Text = "Max";
                } else {
                    this.CostLabel.Text = AttributeManagerSingleton.Instance.GetAttributeCost(this.attributeName)
                        .ToString();
                }
                this.EmitSignal(nameof(Purchase), this.attributeName);
            }
        }

        public void OnFocusEntered() {
            this.Outline.Visible = true;
        }

        public void OnFocusExited() {
            this.Outline.Visible = false;
        }

    }

}