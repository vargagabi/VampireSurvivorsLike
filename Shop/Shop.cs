using Godot;
using System;
using System.Runtime.CompilerServices;

namespace VampireSurvivorsLike {

    public class Shop : CanvasLayer {

        private Label Explanation { get; set; }
        private GridContainer gridContainer { get; set; }
        private Label GoldLabel { get; set; }

        public override void _Ready() {
            this.Explanation = GetNode<Label>("Control/Explanation/Label");
            this.gridContainer = GetNode<GridContainer>("Control/ScrollContainer/GridContainer");
            this.GoldLabel = GetNode<Label>("Control/MoneyContainer/Label");
            this.GoldLabel.Text = AttributeManagerSingleton.Instance.Gold.ToString();
            FillAttributes(this.gridContainer);
        }

        public override void _Input(InputEvent @event) {
            if (@event.IsActionReleased("ui_cancel")) {
                this.OnBackButtonPressed();
            }
        }

        private void FillAttributes(GridContainer container) {
            PackedScene cardScene = ResourceLoader.Load<PackedScene>("res://Shop/AttributeCard.tscn");
            foreach (Attribute attribute in AttributeManagerSingleton.Instance.GetAttributes()) {
                if (attribute == null) {
                    continue;
                }
                AttributeCard card = cardScene.Instance<AttributeCard>();
                card.AttributeName = attribute.Name;
                card.Progress = attribute.BaseLevel;
                card.IconTexture = attribute.Icon;
                card.Cost = 111;
                card.Receiver = this;
                card.Disabled = attribute.BaseLevel >= AttributeManagerSingleton.Instance.levelRange.y;
                container.AddChild(card);
            }
        }

        public void OnBackButtonPressed() {
            AttributeManagerSingleton.Instance.Save();
            this.GetTree().ChangeScene("res://GUI/Menu/Menu.tscn");
        }

        public void OnPurchase(string name) {
            GD.Print($"First Name: {name}");
            this.GoldLabel.Text = AttributeManagerSingleton.Instance.Gold.ToString();
        }

        public void OnVisibilityChanged() {
            GD.Print("hell");
            if (this.Visible) {
                this.GetNode<GridContainer>("Control/ScrollContainer/GridContainer").GetChild<Control>(0).GrabFocus();
            }
        }

    }

}