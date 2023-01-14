using Godot;

public class GameOverScreen : CenterContainer {
    private Label scoreLabel;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        this.scoreLabel = GetChild(1).GetChild<Label>(0);
        Visible = false;
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }

    public void SetScoreLabel(float score) {
        Visible = true;
        this.scoreLabel.Text = "Your Score: " + (int)score;
    }

    public void OnMenuButtonPressed() {
        GetTree().Paused = false;
        GetTree().ChangeScene("res://GUI/Menu/Menu.tscn");
    }
}