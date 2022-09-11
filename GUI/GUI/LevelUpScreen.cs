using Godot;
using System;
using Object = System.Object;

public class LevelUpScreen : CenterContainer
{
    private OptionButton _optionButton;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

        _optionButton = GetChild(1).GetChild<OptionButton>(1);
    }

    public async void SetRewards(string[] options)
    {
        _optionButton.Clear();
        for (int i = 0; i < options.Length; i++)
        {
            _optionButton.AddItem(options[i],i);
            GD.Print(i + ", " + options[i]);
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
