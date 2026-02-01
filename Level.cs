using System;
using System.ComponentModel;
using Godot;

public partial class Level : Node
{
    [Signal]
    public delegate void LevelFinishedEventHandler();

    [Signal]
    public delegate void GameOverEventHandler();

    int mapWidth = 10;
    int mapHeight = 8;

    const int BlockSize = 2;

    int tileSize = 64;

    private int[,] map;

    private PackedScene _tileScene;

    public int LevelNumber { get; set; } = 1;

    public override void _Ready()
    {
        _tileScene = GD.Load<PackedScene>("res://Tile.tscn");

        GenerateLevel();
        GenerateEnemies();
        //PrintLevel();

        Texture2D tile1 = GD.Load<Texture2D>("res://art/terrain/tile_00.PNG");
        Texture2D tile2 = GD.Load<Texture2D>("res://art/terrain/tile_01.PNG");
        Texture2D tile3 = GD.Load<Texture2D>("res://art/terrain/tile_02.PNG");
        Texture2D tile4 = GD.Load<Texture2D>("res://art/terrain/tile_03.PNG");
        Texture2D tile5 = GD.Load<Texture2D>("res://art/terrain/tile_04.PNG");
        Texture2D tile6 = GD.Load<Texture2D>("res://art/terrain/tile_05.PNG");
        Texture2D tile7 = GD.Load<Texture2D>("res://art/terrain/tile_06.PNG");
        Texture2D tile8 = GD.Load<Texture2D>("res://art/terrain/tile_07.PNG");

        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                Tile tile = _tileScene.Instantiate<Tile>();
                tile.Position = new Vector2(i * tileSize, j * tileSize);
                AddChild(tile);

                // _Ready is run after AddChild, so _sprite is initialized
                // so SetTexture wasn't working earlier
                switch (map[i, j])
                {
                    case 0:
                        tile.SetTexture(tile1);
                        tile.TileColor = Colors.Gray;
                        break;
                    case 1:
                        tile.SetTexture(tile2);
                        tile.TileColor = Colors.Red;
                        break;
                    case 2:
                        tile.SetTexture(tile3);
                        tile.TileColor = Colors.Yellow;
                        break;
                    case 3:
                        tile.SetTexture(tile4);
                        tile.TileColor = Colors.LightGreen;
                        break;
                    case 4:
                        tile.SetTexture(tile5);
                        tile.TileColor = Colors.DarkGreen;
                        break;
                    case 5:
                        tile.SetTexture(tile6);
                        tile.TileColor = Colors.LightBlue;
                        break;
                    case 6:
                        tile.SetTexture(tile7);
                        tile.TileColor = Colors.DarkBlue;
                        break;
                    default:
                        tile.SetTexture(tile8);
                        tile.TileColor = Colors.Violet;
                        break;
                }
            }
        }

        GetNode<Hud>("Hud").SetLevelNumber(LevelNumber);
    }

    void GenerateLevel()
    {
        // tile types are generated from level number
        // first level has 2 types, second has 3, etc.
        // up to a max of 7 types
        var tileTypes = 2;
        if (LevelNumber > 1)
        {
            tileTypes = Math.Min(2 + LevelNumber - 1, 7);
        }

        // similarly, increase map size with level number
        // up to a max size
        mapWidth = Math.Min(10 + (LevelNumber - 1) * 2, 20);
        mapHeight = Math.Min(8 + (LevelNumber - 1) * 2, 16);

        // and decrease tile size to fit more on screen
        tileSize = Math.Max(64 - (LevelNumber - 1) * 4, 32);

        map = new int[mapWidth, mapHeight];

        for (int x = 0; x < mapWidth; x += BlockSize)
        {
            for (int y = 0; y < mapHeight; y += BlockSize)
            {
                int tileType = Random.Shared.Next(tileTypes);

                // Fill 2x2 block
                for (int dx = 0; dx < BlockSize; dx++)
                {
                    for (int dy = 0; dy < BlockSize; dy++)
                    {
                        map[x + dx, y + dy] = tileType;
                    }
                }
            }
        }
    }

    void GenerateEnemies()
    {
        var enemy = GD.Load<PackedScene>("res://Enemy.tscn");
        var rotenemy = GD.Load<PackedScene>("res://RotatingEnemy.tscn");

        if (LevelNumber == 1)
        {
            var enemyInstance = enemy.Instantiate<Enemy>();
            enemyInstance.Position = new Vector2(406, 31);

            AddChild(enemyInstance);
        }
        else if (LevelNumber > 1 && LevelNumber <= 3)
        {
            int enemyCount = LevelNumber + 2;
            for (int i = 0; i < enemyCount; i++)
            {
                var enemyInstance = enemy.Instantiate<Enemy>();
                enemyInstance.Position = new Vector2(
                    Random.Shared.Next(60, mapWidth * tileSize - 60),
                    Random.Shared.Next(60, mapHeight * tileSize - 120)
                );
                AddChild(enemyInstance);
            }
        }
        else if (LevelNumber > 3 && LevelNumber <= 8)
        {
            int enemyCount = LevelNumber + 2;
            for (int i = 0; i < enemyCount; i++)
            {
                var enemyInstance = enemy.Instantiate<Enemy>();
                enemyInstance.Position = new Vector2(
                    Random.Shared.Next(60, mapWidth * tileSize - 60),
                    Random.Shared.Next(60, mapHeight * tileSize - 120)
                );
                enemyInstance.RotationDegrees = Random.Shared.Next(
                    -LevelNumber * 10,
                    LevelNumber * 10
                );
                AddChild(enemyInstance);
            }
        }
        else if (LevelNumber == 9)
        {
            var enemyInstance = rotenemy.Instantiate<RotatingEnemy>();
            enemyInstance.RotationDegrees = 180;
            enemyInstance.Position = new Vector2(406, 31);

            AddChild(enemyInstance);
        }
        else if (LevelNumber > 9 && LevelNumber <= 12)
        {
            int enemyCount = LevelNumber / 2;
            for (int i = 0; i < enemyCount; i++)
            {
                var rotatingEnemyInstance = rotenemy.Instantiate<RotatingEnemy>();
                rotatingEnemyInstance.Position = new Vector2(
                    Random.Shared.Next(60, mapWidth * tileSize - 60),
                    Random.Shared.Next(60, mapHeight * tileSize - 120)
                );
                rotatingEnemyInstance.RotationDegrees = 180;
                AddChild(rotatingEnemyInstance);
            }
        }
        else
        {
            int enemyCount = LevelNumber;
            for (int i = 0; i < enemyCount; i++)
            {
                if (Random.Shared.NextDouble() > 0.5)
                {
                    var rotatingEnemyInstance = rotenemy.Instantiate<RotatingEnemy>();
                    rotatingEnemyInstance.Position = new Vector2(
                        Random.Shared.Next(60, mapWidth * tileSize - 60),
                        Random.Shared.Next(60, mapHeight * tileSize - 120)
                    );
                    rotatingEnemyInstance.RotationDegrees = Random.Shared.Next(
                        -LevelNumber * 15,
                        LevelNumber * 15
                    );
                    AddChild(rotatingEnemyInstance);
                }
                else
                {
                    var enemyInstance = enemy.Instantiate<Enemy>();
                    enemyInstance.Position = new Vector2(
                        Random.Shared.Next(60, mapWidth * tileSize - 60),
                        Random.Shared.Next(60, mapHeight * tileSize - 120)
                    );
                    enemyInstance.RotationDegrees = Random.Shared.Next(
                        -LevelNumber * 15,
                        LevelNumber * 15
                    );
                    AddChild(enemyInstance);
                }
            }
        }
    }

    void PrintLevel()
    {
        for (int y = 0; y < mapHeight; y++)
        {
            string row = "";
            for (int x = 0; x < mapWidth; x++)
            {
                row += map[x, y] + " ";
            }
            GD.Print(row);
        }
    }

    private void OnLevelCompleted()
    {
        EmitSignal(SignalName.LevelFinished);
    }

    private void OnGameOver()
    {
        EmitSignal(SignalName.GameOver);
    }

    [Export]
    public PackedScene BasicTile { get; set; }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }
}
