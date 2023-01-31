using Godot;
using System;

namespace VampireSurvivorsLike {

    public class AttributeCard : TextureButton {

        private TextureRect Icon { get; set; }
        private Label AttributeLabel { get; set; }
        private Label CostLabel { get; set; }
        private TextureProgress LevelProgress { get; set; }

        public Shop Receiver { get; set; }
        public string AttributeName { get; set; }
        public int Cost { get; set; }
        public Texture IconTexture { get; set; }
        public int Progress { get; set; }

        [Signal] public delegate void Purchase(string name);

        public override void _Ready() {
            this.Icon = GetNode<TextureRect>("CenterContainer/TextureRect");
            this.AttributeLabel = GetNode<Label>("NameLabel");
            this.LevelProgress = GetNode<TextureProgress>("CenterContainer2/TextureProgress");
            this.CostLabel = GetNode<Label>("CostLabel");
            this.LevelProgress.Value = this.Progress;
            this.AttributeLabel.Text = this.AttributeName;
            this.Icon.Texture = this.IconTexture;
            this.CostLabel.Text = this.Cost.ToString();
            Connect(nameof(Purchase), this.Receiver, nameof(this.Receiver.OnPurchase));
        }

        public void OnButtonPressed() {
            GD.Print("Purchase Button pressed");
            if (AttributeManagerSingleton.Instance.DecreaseGold(this.Cost)) {
                this.LevelProgress.Value = AttributeManagerSingleton.Instance.IncreaseBaseLevel(this.AttributeName);
                this.EmitSignal(nameof(Purchase), this.AttributeName);
            }
        }

    }

}