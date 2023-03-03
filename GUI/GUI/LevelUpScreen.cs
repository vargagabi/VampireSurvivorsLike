using System.Collections.Generic;
using System.Threading;
using Godot;

namespace VampireSurvivorsLike {

    public class LevelUpScreen : Control {

        // private ItemList itemList;
        internal bool IsCurrentlyVisible { get; set; }
        private Control ItemContainer { get; set; }
        private int InFocus { get; set; }

        // Called when the node enters the scene tree for the first time.
        public override void _Ready() {
            this.InFocus = 0;
            LevelUpManagerSingleton.Instance.LevelUpScreen = this;
            this.IsCurrentlyVisible = false;
            this.ItemContainer = GetNode<Control>("VBoxContainer/RewardList");
        }

        public void OnVisibilityChanged() {
            if (this.Visible) {
                this.ItemContainer.GetChild<RewardItemContainer>(this.InFocus).GetChild<Button>(0).GrabFocus();
            } else {
                this.InFocus = this.GetFocusOwner().GetParent().GetIndex();
            }
        }

        public void SetRewards(List<object> options) {
            this.InFocus = 0;
            for (int i = 0; i < options.Count; i++) {
                createItemContainer(options[i], i);
            }
            this.ChangeVisibility();
        }

        private void createItemContainer(object option, int index) {
            RewardItemContainer container = ResourceLoader.Load<PackedScene>("res://GUI/GUI/RewardItemContainer.tscn")
                .Instance<RewardItemContainer>();
            this.ItemContainer.AddChild(container);
            if (option is Item item) {
                container.SetItemContainer(index, item.Icon, item.ToString(), this);
            } else if (option is Attribute attribute) {
                container.SetItemContainer(index, attribute.Icon, attribute.ToString(), this);
            }
        }

        private void ChangeVisibility() {
            this.IsCurrentlyVisible = !this.IsCurrentlyVisible;
            this.Visible = this.IsCurrentlyVisible;
        }

        public void OnRewardSelected(int index) {
            foreach (Node child in this.ItemContainer.GetChildren()) {
                child.QueueFree();
            }
            LevelUpManagerSingleton.Instance.OnRewardSelected(index);
            this.ChangeVisibility();
        }

    }

}