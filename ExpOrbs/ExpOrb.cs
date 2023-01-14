using Godot;

public class ExpOrb : Node2D
{
    private int Experience { get; set; }
    private AnimationPlayer _animationPlayer;
    private Node2D _player;
    private bool _move = false;
    private int _speed = 100;
    [Signal] public delegate void PickedUp(int exp);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _animationPlayer.Play("Hover");

        // _player = GetNode<KinematicBody2D>("../Player");
        // Connect(nameof(PickedUp), _player, "OnPickUp");
    }

    public void OnAreaEntered(Node2D body)
    {
        if (!_move)
        {
            Connect(nameof(PickedUp), body.GetParent(), "OnPickUp");
            _player = body;
            _move = true;
        }
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (_move)
        {
            GlobalPosition += (_player.GlobalPosition - GlobalPosition).Normalized() * _speed * delta;
            _speed += 1;
            if (_player.GlobalPosition.DistanceTo(GlobalPosition) <= 10)
            {
                EmitSignal(nameof(PickedUp), Experience);
                CallDeferred("queue_free");
            }
        }
    }
}