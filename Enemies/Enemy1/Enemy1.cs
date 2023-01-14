using Godot;

//Slime monster
public class Enemy1 : KinematicBody2D {
    private float health = 10;
    private float speed = 50;
    private int expValue = 1;
    private float Strength { get; set; }

    private KinematicBody2D player;
    private AnimatedSprite animatedSprite;
    private PackedScene expOrb;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        this.Strength = 5;
        this.player = this.GetNode<KinematicBody2D>("../../../Player");
        this.animatedSprite = this.GetNode<AnimatedSprite>("AnimatedSprite");
        this.expOrb = ResourceLoader.Load<PackedScene>("res://ExpOrbs/ExpOrb.tscn");
        this.animatedSprite.Play("Walk");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta) {
        Vector2 velocity = ((this.player.GlobalPosition + Vector2.Down * 15) - this.GlobalPosition).Normalized();
        if (this.animatedSprite.Frame >= 2 && this.animatedSprite.Frame <= 4) {
            this.MoveAndSlide(velocity * this.speed);
        }

        this.animatedSprite.FlipH = velocity.x < 0;
    }

    /*
     * When receiving damage reduce the Health by damage amount.
     * If the Health is less than or equals 0 remove the enemy and add an
     * instance of an ExpOrb at its place.
     */
    public void OnHit(float damage) {
        this.health -= damage;
        if (this.health <= 0) {
            // CallDeferred("QueueFree()");
            this.QueueFree();

            // EmitSignal(nameof(OnDeath), this.expValue);
            Node2D expOrb = this.expOrb.Instance<Node2D>();
            expOrb.GlobalPosition = this.GlobalPosition;
            expOrb.Set("Experience", this.expValue);
            Node viewport = this.GetTree().Root.GetChild(0);
            viewport.CallDeferred("add_child", expOrb);
        }
    }
}