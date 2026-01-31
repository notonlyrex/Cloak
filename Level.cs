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

		Texture2D grass = GD.Load<Texture2D>("res://art/terrain/tile_0000.png");
		Texture2D grass2 = GD.Load<Texture2D>("res://art/terrain/tile_0001.png");
		Texture2D grass3 = GD.Load<Texture2D>("res://art/terrain/tile_0002.png");

		Texture2D water = GD.Load<Texture2D>("res://art/terrain/tile_0037.png");

		Texture2D urban = GD.Load<Texture2D>("res://art/terrain/tile_0110.png");

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
						tile.SetTexture(grass);
						tile.TileColor = Colors.Green;
						break;
					case 1:
						tile.SetTexture(water);
						tile.TileColor = Colors.Blue;
						break;
					case 2:
						tile.SetTexture(grass2);
						tile.TileColor = Colors.LightGreen;
						break;
					case 3:
						tile.SetTexture(grass3);
						tile.TileColor = Colors.DarkGreen;
						break;
					case 4:
						tile.SetTexture(urban);
						tile.TileColor = Colors.Gray;
						break;
					default:
						tile.SetTexture(grass);
						tile.TileColor = Colors.Green;
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
		// up to a max of 5 types
		var tileTypes = 2;
		if (LevelNumber > 1)
		{
			tileTypes = Math.Min(2 + LevelNumber - 1, 5);
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
