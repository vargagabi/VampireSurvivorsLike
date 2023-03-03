using Godot;

namespace VampireSurvivorsLike {

    public class Shop : Control {

        private Label Explanation { get; set; }
        private GridContainer gridContainer { get; set; }
        private Label GoldLabel { get; set; }

        public override void _Ready() {
            this.Explanation = GetNode<Label>("Explanation/Label");
            this.gridContainer = GetNode<GridContainer>("ScrollContainer/GridContainer");
            this.GoldLabel = GetNode<Label>("MoneyContainer/Label");
            this.GoldLabel.Text = AttributeManagerSingleton.Instance.Gold.ToString();
            FillAttributes(this.gridContainer);
        }

        private void FillAttributes(GridContainer container) {
            PackedScene cardScene = ResourceLoader.Load<PackedScene>("res://Shop/AttributeCard.tscn");
            foreach (Attribute attribute in AttributeManagerSingleton.Instance.GetAttributes()) {
                if (attribute == null) {
                    continue;
                }
                AttributeCard card = cardScene.Instance<AttributeCard>();
                container.AddChild(card);
                card.init(attribute.Name, attribute.BaseLevel, attribute.MaxBaseLevel, attribute.Icon,
                    attribute.GetCurrentCost(), this);
            }
        }

        public void OnPurchase(string name) {
            this.GoldLabel.Text = AttributeManagerSingleton.Instance.Gold.ToString();
        }

        public void OnVisibilityChanged() {
            if (this.Visible) {
                this.GetNode<Button>("BackButton").GrabFocus();
            }
        }

        public void OnResetButtonPressed() {
            AttributeManagerSingleton.Instance.ResetAttributes();
            this.GoldLabel.Text = AttributeManagerSingleton.Instance.Gold.ToString();
            foreach (AttributeCard card in this.gridContainer.GetChildren()) {
                card.RefreshInfo();
            }
        }

    }

}