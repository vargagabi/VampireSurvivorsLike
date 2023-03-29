using Godot;
using System;

namespace VampireSurvivorsLike {

    public class AttributeCard : TextureButton {

        //Gui elements
        private TextureRect Icon { get; set; }
        private Label NameLabel { get; set; }
        private Label CostLabel { get; set; }
        private TextureProgress LevelProgress { get; set; }
        private Sprite Outline { get; set; }

        //Info, other elements
        private Shop Receiver { get; set; }
        private int maxLevel;


        [Signal] public delegate void Purchase(string name);

        public void init(String name, int progress, int maxLevel, Texture iconTexture, int cost, Shop receiver) {
            this.NameLabel.Text = name;
            this.CostLabel.Text = progress >= maxLevel? "Max" : cost.ToString();
            this.LevelProgress.Value = progress;
            this.Icon.Texture = iconTexture;
            this.Receiver = receiver;
            this.maxLevel = maxLevel;
            this.Disabled = this.LevelProgress.Value >= this.maxLevel;
            if (this.Receiver != null) {
                Connect(nameof(Purchase), this.Receiver, nameof(this.Receiver.OnPurchase));
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
            this.LevelProgress.Value = AttributeManagerSingleton.Instance.GetAttributeBaseLevel(this.NameLabel.Text);
            this.CostLabel.Text = AttributeManagerSingleton.Instance.GetAttributeCost(this.NameLabel.Text).ToString();
        }

        public void OnButtonPressed() {
            GD.Print("Purchase Button pressed");
            if (AttributeManagerSingleton.Instance.IncreaseBaseLevel(this.NameLabel.Text)) {
                this.LevelProgress.Value =
                    AttributeManagerSingleton.Instance.GetAttributeBaseLevel(this.NameLabel.Text);
                if (this.LevelProgress.Value >= this.maxLevel) {
                    CostLabel.Text = "Max";
                } else {
                    this.CostLabel.Text = AttributeManagerSingleton.Instance.GetAttributeCost(this.NameLabel.Text)
                        .ToString();
                }
                this.EmitSignal(nameof(Purchase), this.NameLabel.Text);
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