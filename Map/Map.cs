using Godot;

public class Map : Node2D
{
    private TileMap _ground;
    private TileMap _tree;
    private TileMap _props;
    private OpenSimplexNoise _noise;
    private int _renderDistance;
    private KinematicBody2D _player;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Randomize();
        _ground = GetChild<TileMap>(0);
        _tree = GetChild<TileMap>(1);
        _props = GetChild<TileMap>(2);
        _renderDistance = 20;
        _noise = new OpenSimplexNoise();
        _noise.Seed = 0; //(int)GD.Randi();

        _noise.Octaves = 2;
        _noise.Period = 60f;
        _noise.Persistence = 1f;
        _noise.Lacunarity = 2f;

        _player = GetNode<KinematicBody2D>("../Player");
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        GenerateTiles();
    }

    private void GenerateTiles()
    {
        Vector2 pos = _ground.WorldToMap(_player.GlobalPosition);
        for (int x = -_renderDistance; x < _renderDistance; x++)
        {
            for (int y = -_renderDistance; y < _renderDistance; y++)
            {
                int posX = (int)(pos.x + x);
                int posY = (int)(pos.y + y);
                float noise = _noise.GetNoise2d(posX, posY);
                var groundTile = _ground.GetCell(posX, posY);
                if (groundTile != TileMap.InvalidCell) continue;

                //Places the grass, flowers etc. tiles

                if (noise > 0.2)
                {
                    if (GD.Randf() < 0.2f)

                        _tree.SetCell(posX, posY, (int)GD.RandRange(0, 3));
                }

                _ground.SetCell(posX, posY, 0, false, false, false,
                    new Vector2((int)GD.RandRange(0, 8), (int)GD.RandRange(0, 4)));

                if (GD.Randf() < 0.01 && noise < -0.4 && noise > -0.6)
                    _props.SetCell(posX, posY, (int)GD.RandRange(0, 14));

            }
        }

        // _ground.UpdateBitmaskRegion();
        _ground.UpdateDirtyQuadrants();
    }
}