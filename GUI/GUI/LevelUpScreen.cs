using System.Collections.Generic;
using System.Threading;
using Godot;

namespace VampireSurvivorsLike {

    public class LevelUpScreen : CenterContainer {

        private ItemList itemList;
        internal bool IsCurrentlyVisible { get; set; }
        

        // Called when the node enters the scene tree for the first time.
        public override void _Ready() {
            LevelUpManagerSingleton.Instance.LevelUpScreen = this;
            this.itemList = GetChild(1).GetChild<ItemList>(1);
            this.IsCurrentlyVisible = false;
            this.Connect("visibility_changed", this, nameof(OnVisibilityChanged));
        }

        public void OnVisibilityChanged() {
            if (this.Visible) {
                this.itemList.GrabFocus();
            }
        }

        public void SetRewards(List<string> options) {
            this.itemList.Clear();
            foreach (string item in options) {
                this.itemList.AddItem(item);
            }
            this.ChangeVisibility();
        }

        public void ChangeVisibility() {
            this.IsCurrentlyVisible = !this.IsCurrentlyVisible;
            this.Visible = this.IsCurrentlyVisible;
        }

        public void OnRewardSelected(int index) {
           LevelUpManagerSingleton.Instance.OnRewardSelected(index); 
           this.ChangeVisibility();
           // this.GetTree().Paused = false;
        }
    }

}