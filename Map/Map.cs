using Godot;

namespace VampireSurvivorsLike {

    public class Map : Node2D {

        private TileMap ground;
        private TileMap tree;
        private TileMap props;
        private OpenSimplexNoise noise;
        private int renderDistance;
        private KinematicBody2D player;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready() {
            GD.Randomize();
            this.ground = GetChild<TileMap>(0);
            this.tree = GetChild<TileMap>(1);
            this.props = GetChild<TileMap>(2);
            this.renderDistance = 20;
            this.noise = new OpenSimplexNoise();
            this.noise.Seed = 0; //(int)GD.Randi();
            this.noise.Octaves = 2;
            this.noise.Period = 60f;
            this.noise.Persistence = 1f;
            this.noise.Lacunarity = 2f;
            this.player = GetNode<KinematicBody2D>("../Player");
        }

        //  // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta) {
            GenerateTiles();
        }

        private void GenerateTiles() {
            Vector2 pos = this.ground.WorldToMap(this.player.GlobalPosition);
            for (int x = -this.renderDistance; x < this.renderDistance; x++) {
                for (int y = -this.renderDistance; y < this.renderDistance; y++) {
                    int posX = (int)(pos.x + x);
                    int posY = (int)(pos.y + y);
                    float noise = this.noise.GetNoise2d(posX, posY);
                    int groundTile = this.ground.GetCell(posX, posY);
                    if (groundTile != TileMap.InvalidCell) continue;

                    //Places the grass, flowers etc. tiles
                    if (noise > 0.2) {
                        if (GD.Randf() < 0.2f)

                            this.tree.SetCell(posX, posY, (int)GD.RandRange(0, 3));
                    }
                    this.ground.SetCell(posX, posY, 0, false, false, false,
                        new Vector2((int)GD.RandRange(0, 8), (int)GD.RandRange(0, 4)));
                    if (GD.Randf() < 0.01 && noise < -0.4 && noise > -0.6)
                        this.props.SetCell(posX, posY, (int)GD.RandRange(0, 14));
                }
            }
            this.ground.UpdateDirtyQuadrants();
        }

    }

}