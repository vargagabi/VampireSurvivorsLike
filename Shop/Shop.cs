using Godot;
using System;
using System.Runtime.CompilerServices;

namespace VampireSurvivorsLike {

    public class Shop : CanvasLayer {

        private Label Explanation { get; set; }
        private GridContainer Tab1 { get; set; }
        private GridContainer Tab2 { get; set; }
        private Label GoldLabel { get; set; }

        public override void _Ready() {
            this.Explanation = GetNode<Label>("Control/Explanation/Label");
            this.Tab1 = GetNode<TabContainer>("Control/TabContainer").GetChild<GridContainer>(0);
            this.Tab2 = GetNode<TabContainer>("Control/TabContainer").GetChild<GridContainer>(1);
            this.GoldLabel = GetNode<Label>("Control/MoneyContainer/Label");
            this.GoldLabel.Text = AttributeManagerSingleton.Instance.Gold.ToString();
            FillAttributes(this.Tab1);
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
                card.Disabled = attribute.BaseLevel >=AttributeManagerSingleton.Instance.levelRange.y;
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

    }

}