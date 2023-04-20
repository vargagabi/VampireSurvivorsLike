using System;
using System.Collections.Generic;
using Godot;

namespace VampireSurvivorsLike {

    public class MobSpawner : Node2D {

        public Player PlayerOne { get; set; }
        public Player PlayerTwo { get; set; }
        private readonly List<PackedScene> enemies = new List<PackedScene>();
        private YSort entities;
        private int spawnCounter = 0;
        private int counterResetBoundary = 1;

        public override void _Ready() {
            GD.Randomize();
            this.enemies.Add(GD.Load<PackedScene>("res://Enemies/Slime/Slime.tscn"));
            this.enemies.Add(GD.Load<PackedScene>("res://Enemies/Skeleton/Skeleton.tscn"));
            this.entities = this.GetParent().GetNode<YSort>("World/Entities");
            foreach (PackedScene enemy in this.enemies) {
                this.counterResetBoundary *= enemy.Instance<Enemy>().SpawnRate;
            }
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta) {
            if ((!GameStateManagerSingleton.Instance.IsMultiplayer || this.IsNetworkMaster()) &&
                this.PlayerOne != null) {
                this.SpawnEnemy();
            }
        }

        /*
         * Spawns an enemy around the player in a circle every spawnRate time.
         */
        private void SpawnEnemy() {
            this.spawnCounter++;
            Vector2 position = this.PlayerOne.Position;
            if (this.PlayerTwo != null) {
                position = new Random().Next(0, 2) == 0 ? this.PlayerOne.GlobalPosition : this.PlayerTwo.GlobalPosition;
            }
            for (int i = 0; i < this.enemies.Count; i++) {
                Enemy enemy = this.enemies[i].Instance<Enemy>();
                if (this.spawnCounter % enemy.SpawnRate != 0 || enemy.SpawnTime.x > Main.MinutesPassed || enemy.SpawnTime.y < Main.MinutesPassed) {
                    continue;
                }
                position += new Vector2(enemy.SpawnDistance, 0).Rotated(
                    (float)GD.RandRange(0, Mathf.Tau));
                if (GameStateManagerSingleton.Instance.IsMultiplayer) {
                    this.Rpc(nameof(this.SyncSpawnEnemy), i, position, Enemy.MobCount++);
                } else {
                    this.SyncSpawnEnemy(i, position, Enemy.MobCount++);
                }
            }
            if (this.spawnCounter == this.counterResetBoundary || this.spawnCounter == int.MaxValue) {
                this.spawnCounter = 0;
            }
        }

        [PuppetSync]
        public void SyncSpawnEnemy(int enemyIndex, Vector2 globalPosition, int mobCount) {
            Enemy enemy = this.enemies[enemyIndex].Instance<Enemy>();
            enemy.PlayerOne = this.PlayerOne;
            enemy.PlayerTwo = this.PlayerTwo;
            enemy.Name = enemy.GetClass() + mobCount;
            enemy.GlobalPosition = globalPosition;
            this.entities.AddChild(enemy, true);
        }

    }

}