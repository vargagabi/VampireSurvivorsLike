using Godot;

namespace VampireSurvivorsLike {

    public class Shop : Control {

        private GridContainer gridContainer;
        private Label goldLabel;

        public override void _Ready() {
            this.gridContainer = GetNode<GridContainer>("ScrollContainer/GridContainer");
            this.goldLabel = GetNode<Label>("MoneyContainer/Label");
            this.goldLabel.Text = AttributeManagerSingleton.Instance.Gold.ToString();
            this.FillAttributes(this.gridContainer);
        }

        private void FillAttributes(GridContainer container) {
            PackedScene cardScene = ResourceLoader.Load<PackedScene>("res://Shop/AttributeCard.tscn");
            foreach (Attribute attribute in AttributeManagerSingleton.Instance.GetAttributes()) {
                if (attribute == null) {
                    continue;
                }
                AttributeCard card = cardScene.Instance<AttributeCard>();
                container.AddChild(card);
                card.init(attribute.Name, attribute.NameText, attribute.BaseLevel, attribute.MaxBaseLevel,
                    attribute.Icon,
                    attribute.GetCurrentCost(), this);
            }
        }

        public void OnPurchase(string name) {
            this.goldLabel.Text = AttributeManagerSingleton.Instance.Gold.ToString();
        }

        public void OnVisibilityChanged() {
            if (this.Visible) {
                this.GetNode<Button>("HBoxContainer/BackButton").GrabFocus();
            }
        }

        public void OnResetButtonPressed() {
            AttributeManagerSingleton.Instance.ResetAttributes();
            this.goldLabel.Text = AttributeManagerSingleton.Instance.Gold.ToString();
            foreach (AttributeCard card in this.gridContainer.GetChildren()) {
                card.RefreshInfo();
            }
        }

        public void OnBackButtonPressed() {
            AttributeManagerSingleton.Instance.Save();
            this.Visible = false;
        }

    }

}