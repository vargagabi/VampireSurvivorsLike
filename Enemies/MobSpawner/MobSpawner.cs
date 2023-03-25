using System;
using System.Collections.Generic;
using Godot;

namespace VampireSurvivorsLike {

    public class MobSpawner : Node2D {

        private int SpawnCounter { get; set; }
        public Player PlayerOne { get; set; }
        public Player PlayerTwo { get; set; }
        private List<PackedScene> enemies = new List<PackedScene>();
        private YSort ySort;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready() {
            GD.Randomize();
            this.SpawnCounter = 0;

            // this.enemies.Add(GD.Load<PackedScene>("res://Enemies/Slime/Slime.tscn"));
            this.enemies.Add(GD.Load<PackedScene>("res://Enemies/Skeleton/Skeleton.tscn"));
            this.ySort = this.GetParent().GetNode<YSort>("Entities");
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta) {
            if ((!GameStateManagerSingleton.Instance.IsMultiplayer || this.IsNetworkMaster()) && PlayerOne != null) {
                this.SpawnEnemy();
            }
        }

        /*
         * Spawns an enemy around the player in a circle every spawnRate time.
         */
        private void SpawnEnemy() {
            this.SpawnCounter++;
            Vector2 position = this.PlayerOne.Position;
            if (this.PlayerTwo != null) {
                position = new Random().Next(0, 2) == 0 ? this.PlayerOne.GlobalPosition : this.PlayerTwo.GlobalPosition;
            }
            for (int i = 0; i < this.enemies.Count; i++) {
                Enemy enemy = this.enemies[i].Instance<Enemy>();
                if (this.SpawnCounter % enemy.SpawnRate != 0) {
                    continue;
                }
                this.SpawnCounter = 0;
                enemy.PlayerOne = this.PlayerOne;
                enemy.PlayerTwo = this.PlayerTwo;
                enemy.Name = enemy.GetClass() + Enemy.mobCount;
                enemy.GlobalPosition = position +
                                       new Vector2(enemy.SpawnDistance, 0).Rotated(
                                           (float)GD.RandRange(0, Mathf.Tau));
                this.ySort.AddChild(enemy, true);
                if (GameStateManagerSingleton.Instance.IsMultiplayer) {
                    this.Rpc(nameof(this.SpawnPuppetEnemy), i, enemy.GlobalPosition, enemy.Name);
                }
            }
        }

        [Puppet]
        public void SpawnPuppetEnemy(int enemyIndex, Vector2 globalPosition, string name) {
            Enemy enemy = this.enemies[enemyIndex].Instance<Enemy>();
            enemy.PlayerOne = this.PlayerOne;
            enemy.PlayerTwo = this.PlayerTwo;
            enemy.Name = name;
            enemy.GlobalPosition = globalPosition;
            this.ySort.AddChild(enemy, true);
        }

    }

}