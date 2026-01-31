using System;
using System.ComponentModel;
using Godot;

public partial class Level : Node
{
	const int Width = 10;
	const int Height = 8;

	const int BlockSize = 2;
	const int TileTypes = 2; // 0, 1, 2

	private int[,] level = new int[Width, Height];

	private PackedScene _tileScene;

	public override void _Ready()
	{
		_tileScene = GD.Load<PackedScene>("res://Tile.tscn");

		GenerateLevel();
		PrintLevel();

		Texture2D grass = GD.Load<Texture2D>("res://art/terrain/tile_0000.png");
		Texture2D water = GD.Load<Texture2D>("res://art/terrain/tile_0037.png");

		for (int i = 0; i < Width; i++)
		{
			for (int j = 0; j < Height; j++)
			{
				Tile tile = _tileScene.Instantiate<Tile>();
				GD.Print(tile);
				tile.Position = new Vector2(i * 64, j * 64);
				AddChild(tile);

				// _Ready is run after AddChild, so _sprite is initialized
				// so SetTexture wasn't working earlier
				if (level[i, j] == 0)
					tile.SetTexture(grass);
				else
					tile.SetTexture(water);
			}
		}
	}

	void GenerateLevel()
	{
		for (int x = 0; x < Width; x += BlockSize)
		{
			for (int y = 0; y < Height; y += BlockSize)
			{
				int tileType = Random.Shared.Next(TileTypes);

				// Fill 2x2 block
				for (int dx = 0; dx < BlockSize; dx++)
				{
					for (int dy = 0; dy < BlockSize; dy++)
					{
						level[x + dx, y + dy] = tileType;
					}
				}
			}
		}
	}

	void PrintLevel()
	{
		for (int y = 0; y < Height; y++)
		{
			string row = "";
			for (int x = 0; x < Width; x++)
			{
				row += level[x, y] + " ";
			}
			GD.Print(row);
		}
	}

	[Export]
	public PackedScene BasicTile { get; set; }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) { }
}
