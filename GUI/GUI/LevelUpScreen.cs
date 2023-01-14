using Godot;

public class LevelUpScreen : CenterContainer {
    private ItemList itemList;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        // _optionButton = GetChild(1).GetChild<OptionButton>(1);
        this.itemList = GetChild(1).GetChild<ItemList>(1);
    }

    public void SetRewards(string[] options) {
        this.itemList.Clear();
        this.itemList.GrabFocus();
        for (int i = 0; i < options.Length; i++) {
            this.itemList.AddItem(options[i]);
        }
        Visible = true;
    }
}