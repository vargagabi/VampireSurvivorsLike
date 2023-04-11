using System;
using System.Collections.Generic;
using Godot;

namespace VampireSurvivorsLike {

    public class Map : Node2D {

        private TileMap ground;
        private TileMap tree;
        private TileMap props;
        private OpenSimplexNoise noise;
        private OpenSimplexNoise otherNoise;
        private int renderDistance;
        private List<Player> players = new List<Player>();

        // Called when the node enters the scene tree for the first time.
        public override void _Ready() {
            this.ground = this.GetChild<TileMap>(0);
            this.tree = this.GetChild(1).GetChild<TileMap>(0);
            this.props = this.GetChild(1).GetChild<TileMap>(1);
            this.renderDistance = 25;

            this.noise = new OpenSimplexNoise();
            this.noise.Seed = new Random().Next();
            this.noise.Octaves = 2;
            this.noise.Period = 60f;
            this.noise.Persistence = 1f;
            this.noise.Lacunarity = 2f;

            this.otherNoise = new OpenSimplexNoise();
            this.otherNoise.Seed = new Random().Next();
            this.otherNoise.Octaves = 1;
            this.otherNoise.Period = 0.01f;
            this.otherNoise.Persistence = 0.8f;
            this.otherNoise.Lacunarity = 3f;
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta) {
            foreach (Player player in this.players) {
                this.GenerateTiles(player.GlobalPosition);
            }
        }

        public void AddPlayer(Player player) {
            this.players.Add(player);
            this.GetNode("Entities").AddChild(player);
            if (GameStateManagerSingleton.Instance.IsMultiplayer && !player.Name.Equals("1")) {
                this.otherNoise.Seed = this.noise.Seed = Convert.ToInt32(player.Name);
            }
        }

        private void GenerateTiles(Vector2 position) {
            position = this.ground.WorldToMap(position);
            for (int x = -this.renderDistance; x < this.renderDistance; x++) {
                for (int y = -this.renderDistance; y < this.renderDistance; y++) {
                    int posX = (int)(position.x + x);
                    int posY = (int)(position.y + y);
                    float groundNoiseValue = this.noise.GetNoise2d(posX, posY);
                    float otherNoiseValue = this.otherNoise.GetNoise2d(posX, posY);
                    int groundTile = this.ground.GetCell(posX, posY);
                    if (groundTile != TileMap.InvalidCell) continue;

                    //Places the grass, flowers etc. tiles
                    if (groundNoiseValue > 0.2 && otherNoiseValue > 0 && this.tree.GetCell(posX, posY) == -1) {
                        this.tree.SetCell(posX, posY, (int)GD.RandRange(0, 3));
                    }
                    this.ground.SetCell(posX, posY, 0, false, false, false,
                        new Vector2((int)GD.RandRange(0, 8), (int)GD.RandRange(0, 4)));


                    if (otherNoiseValue < -0.78 && this.props.GetCell(posX, posY) == TileMap.InvalidCell) {
                        this.props.SetCell(posX, posY, (int)((groundNoiseValue * otherNoiseValue + 1) * 3.5));
                    }
                }
            }
            this.ground.UpdateDirtyQuadrants();
        }

    }

}