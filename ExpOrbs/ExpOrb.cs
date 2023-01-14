using Godot;

public class ExpOrb : Node2D {
    private int Experience { get; set; }
    private AnimationPlayer animationPlayer;
    private Node2D player;
    private bool isMoving = false;
    private int speed = 100;
    [Signal] public delegate void PickedUp(int exp);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        this.animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        this.animationPlayer.Play("Hover");
    }

    /*
     * When the player picked up the ord set the player field to the player.
     * Also Connect [PickedUp] to the player's OnPickUp method.
     */
    public void OnAreaEntered(Node2D body) {
        if (!this.isMoving) {
            Connect(nameof(PickedUp), body.GetParent(), "OnPickUp");
            this.player = body;
            this.isMoving = true;
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta) {
        if (this.isMoving) {
            GlobalPosition += (this.player.GlobalPosition - GlobalPosition).Normalized() * this.speed * delta;
            this.speed += 1;
            if (this.player.GlobalPosition.DistanceTo(GlobalPosition) <= 10) {
                EmitSignal(nameof(PickedUp), Experience);
                CallDeferred("queue_free");
            }
        }
    }
}