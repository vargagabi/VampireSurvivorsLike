using System.Collections.Generic;
using System.Threading;
using Godot;

namespace VampireSurvivorsLike {

    public class LevelUpScreen : Control {

        // private ItemList itemList;
        internal bool IsCurrentlyVisible { get; set; }
        private Control ItemContainer { get; set; }


        // Called when the node enters the scene tree for the first time.
        public override void _Ready() {
            LevelUpManagerSingleton.Instance.LevelUpScreen = this;

            // this.itemList = GetChild(1).GetChild<ItemList>(1);
            this.IsCurrentlyVisible = false;
            this.Connect("visibility_changed", this, nameof(OnVisibilityChanged));
            this.ItemContainer = GetNode<Control>("VBoxContainer/RewardList");
        }

        public void OnVisibilityChanged() {
            if (this.Visible) {
                // this.itemList.GrabFocus();
            }
        }

        public void SetRewards(List<object> options) {
            // this.itemList.Clear();
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
                container.SetItemContainer(index, item.Icon, item.ToString(),this);
            } else if (option is Attribute attribute) {
                container.SetItemContainer(index, attribute.Icon, attribute.ToString(),this);
            }
        }

        public void ChangeVisibility() {
            this.IsCurrentlyVisible = !this.IsCurrentlyVisible;
            this.Visible = this.IsCurrentlyVisible;
        }

        public void OnRewardSelected(int index) {
            GD.Print($"Signal received with index {index}");
            foreach (Node child in this.ItemContainer.GetChildren()) {
                child.QueueFree(); 
            }
            LevelUpManagerSingleton.Instance.OnRewardSelected(index);
            this.ChangeVisibility();
        }

    }

}