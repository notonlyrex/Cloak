using System;
using System.ComponentModel;
using Godot;

public partial class Level : Node
{
	[Signal]
	public delegate void LevelFinishedEventHandler();

	int mapWidth = 10;
	int mapHeight = 8;

	const int BlockSize = 2;

	int tileSize = 64;

	private int[,] map;

	private PackedScene _tileScene;

	public int LevelNumber { get; set; }

	public override void _Ready()
	{
		_tileScene = GD.Load<PackedScene>("res://Tile.tscn");

		GenerateLevel();
		PrintLevel();

		Texture2D grass = GD.Load<Texture2D>("res://art/terrain/tile_0000.png");
		Texture2D water = GD.Load<Texture2D>("res://art/terrain/tile_0037.png");

		for (int i = 0; i < mapWidth; i++)
		{
			for (int j = 0; j < mapHeight; j++)
			{
				Tile tile = _tileScene.Instantiate<Tile>();
				tile.TileColor = (map[i, j] == 0) ? Colors.Green : Colors.Blue;

				tile.Position = new Vector2(i * tileSize, j * tileSize);
				AddChild(tile);

				// _Ready is run after AddChild, so _sprite is initialized
				// so SetTexture wasn't working earlier
				if (map[i, j] == 0)
					tile.SetTexture(grass);
				else
					tile.SetTexture(water);
			}
		}
	}

	void GenerateLevel()
	{
		// tile types are generated from level number
		// first level has 2 types, second has 3, etc.
		// up to a max of 5 types
		var tileTypes = 2;
		// if (LevelNumber > 1)
		// {
		//     tileTypes = Math.Min(2 + LevelNumber - 1, 5);
		// }

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

	[Export]
	public PackedScene BasicTile { get; set; }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) { }
}
