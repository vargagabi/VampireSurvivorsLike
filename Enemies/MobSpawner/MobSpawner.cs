using Godot;

public class MobSpawner : Node2D {
    private int spawnCounter = 0;
    private int spawnRate = 100;
    private KinematicBody2D player;
    private PackedScene[] mobs = new PackedScene[1];
    private YSort ySort;
    private int spawnDistance = 500;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        // this.player = GetNode<KinematicBody2D>("Player");
        GD.Randomize();
        this.player = this.GetNode<KinematicBody2D>("../Player");
        this.mobs[0] = GD.Load<PackedScene>("res://Enemies/Enemy1/Enemy1.tscn");
        this.ySort = this.GetChild<YSort>(0);
        this.Position = Vector2.Zero;
        this.GlobalPosition = Vector2.Zero;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta) {
        this.SpawnEnemy(delta);
    }

    /*
     * Spawns an enemy around the player in a circle every spawnRate time.
     */
    private void SpawnEnemy(float delta) {
        this.spawnCounter++;
        if (this.spawnCounter % this.spawnRate == 0) {
            this.spawnCounter = 0;
            KinematicBody2D enemyInstance = (KinematicBody2D)this.mobs[0].Instance();
            enemyInstance.Scale *= (float)GD.RandRange(1, 3);
            ((KinematicBody2D)enemyInstance).GlobalPosition =
                this.player.GlobalPosition +
                new Vector2(this.spawnDistance, 0).Rotated((float)GD.RandRange(0, Mathf.Tau));
            this.ySort.AddChild(enemyInstance);
        }
    }
}