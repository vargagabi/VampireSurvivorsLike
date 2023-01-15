using System.Collections.Generic;
using Godot;
using VampireSurvivorsLike.Enemies;

public class MobSpawner : Node2D {

    private int SpawnCounter { get; set; }
    private KinematicBody2D Player { get; set; }
    private List<PackedScene> enemies = new List<PackedScene>();
    private YSort ySort;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        GD.Randomize();
        this.Player = this.GetNode<KinematicBody2D>("../Player");
        this.SpawnCounter = 0;
        // this.enemies.Add(GD.Load<PackedScene>("res://Enemies/Slime/Slime.tscn"));
        this.enemies.Add(GD.Load<PackedScene>("res://Enemies/Skeleton/Skeleton.tscn"));
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
        this.SpawnCounter++;
        foreach (PackedScene enemyScene in this.enemies) {
            Enemy enemy = enemyScene.Instance<Enemy>();
            if (this.SpawnCounter % enemy.SpawnRate == 0) {
                enemy.GlobalPosition = this.Player.GlobalPosition +
                                       new Vector2(enemy.SpawnDistance, 0).Rotated((float)GD.RandRange(0, Mathf.Tau));
                this.ySort.AddChild(enemy);
            }
        }
    }

}