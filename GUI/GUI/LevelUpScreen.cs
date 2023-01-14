using Godot;

public class LevelUpScreen : CenterContainer
{
    private ItemList _itemList;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

        // _optionButton = GetChild(1).GetChild<OptionButton>(1);
        _itemList = GetChild(1).GetChild<ItemList>(1);

    }

    public void SetRewards(string[] options)
    {
        // _optionButton.Clear();
        _itemList.Clear();
        _itemList.GrabFocus();
        for (int i = 0; i < options.Length; i++)
        {
            // _optionButton.AddItem(options[i],i);
            _itemList.AddItem(options[i]);
            // GD.Print(i + ", " + options[i]);
        }
        Visible = true;

    }

    // public void OnSelected(int index)
    // {
    //     // Visible = false;
    // }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
