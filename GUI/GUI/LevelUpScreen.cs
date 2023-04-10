using System.Collections.Generic;
using Godot;

namespace VampireSurvivorsLike {

    public class LevelUpScreen : Control {

        private Control itemContainer;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready() {
            LevelUpManagerSingleton.Instance.LevelUpScreen = this;
            this.itemContainer = GetNode<Control>("VBoxContainer/RewardList");
        }

        public void SetRewards(List<object> options) {
            for (int i = 0; i < options.Count; i++) {
                this.CreateItemContainer(options[i], i);
            }
            this.Visible = true;
        }

        private void CreateItemContainer(object option, int index) {
            RewardItemContainer container = ResourceLoader.Load<PackedScene>("res://GUI/GUI/RewardItemContainer.tscn")
                .Instance<RewardItemContainer>();
            this.itemContainer.AddChild(container);
            if (option is Item item) {
                container.SetItemContainer(index, item.Icon, item.ToString(), this);
            } else if (option is Attribute attribute) {
                container.SetItemContainer(index, attribute.Icon, attribute.ToString(), this);
            }
        }

        public void OnRewardSelected(int index) {
            foreach (Node child in this.itemContainer.GetChildren()) {
                child.QueueFree();
            }
            LevelUpManagerSingleton.Instance.OnRewardSelected(index);
            this.Visible = false;
        }

        public void OnVisibilityChanged() {
            if (this.Visible) {
                this.itemContainer.GetChild<RewardItemContainer>(0).GetChild<Button>(0).GrabFocus();
            }
        }

    }

}