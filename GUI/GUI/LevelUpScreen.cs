using Godot;

public class LevelUpScreen : CenterContainer {

    private ItemList itemList;
    internal bool IsCurrentlyVisible { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        this.itemList = GetChild(1).GetChild<ItemList>(1);
        this.IsCurrentlyVisible = false;
        this.Connect("visibility_changed", this, nameof(OnVisibilityChanged));
    }

    public void OnVisibilityChanged() {
        if (this.Visible) {
            this.itemList.GrabFocus();
        }
    }

    public void SetRewards(string[] options) {
        this.itemList.Clear();
        for (int i = 0; i < options.Length; i++) {
            this.itemList.AddItem(options[i]);
        }
        this.ChangeVisibility();
    }

    public void ChangeVisibility() {
        this.IsCurrentlyVisible = !this.IsCurrentlyVisible;
        this.Visible = this.IsCurrentlyVisible;
    }

}