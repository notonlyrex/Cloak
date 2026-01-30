using System;
using Godot;

public partial class GreenTile : Area2D
{
	[Export]
	public Color TileColor { get; set; } = Colors.Green;

	public override void _Ready() { }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) { }
}
