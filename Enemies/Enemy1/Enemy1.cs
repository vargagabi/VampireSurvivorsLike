using Godot;

//Slime monster
public class Enemy1 : KinematicBody2D
{
    private float _health = 10;
    private float _speed = 50;
    private int _expValue = 1;
    private float _strength { get; set; }

    private KinematicBody2D _player;
    private AnimatedSprite _animatedSprite;
    private PackedScene _expOrb;

    // [Signal]
    // public delegate void OnDeath(int value);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _strength = 5;
        _player = GetNode<KinematicBody2D>("../../../Player");
        _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        _expOrb = ResourceLoader.Load<PackedScene>("res://ExpOrbs/ExpOrb.tscn");
        _animatedSprite.Play("Walk");
        // this.Connect(nameof(OnDeath), _player, "OnEnemyKilled");
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        Vector2 velocity = Vector2.Zero;
        velocity = ((_player.GlobalPosition + Vector2.Down * 15) - GlobalPosition).Normalized();
        if (_animatedSprite.Frame >= 2 && _animatedSprite.Frame <= 4)
        {
            MoveAndSlide(velocity * _speed);
        }

        _animatedSprite.FlipH = velocity.x < 0;
    }

    public void OnHit(float damage)
    {
        _health -= damage;
        if (_health <= 0)
        {
            // CallDeferred("QueueFree()");
            QueueFree();

            // EmitSignal(nameof(OnDeath), _expValue);

            Node2D expOrb = _expOrb.Instance<Node2D>();
            expOrb.GlobalPosition = GlobalPosition;
            expOrb.Set("Experience", _expValue);
            var viewport = GetTree().Root.GetChild(0);
            viewport.CallDeferred("add_child", expOrb);
        }
    }
}