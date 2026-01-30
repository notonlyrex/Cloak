using System;
using System.ComponentModel;
using Godot;

public partial class Level : Node
{
	[Export]
	public PackedScene BasicTile { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < 1; j++)
			{
				var tile = BasicTile.Instantiate<GreenTile>();
				tile.Position = new Vector2(i * 128, j * 128);
				AddChild(tile);
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) { }
}
