using System;
using Godot;

public partial class Tile : Area2D
{
	[Export]
	public Texture2D Texture;
	private Sprite2D _sprite;

	[Export]
	public Color TileColor { get; set; } = Colors.Green;

	public override void _Ready()
	{
		_sprite = GetNode<Sprite2D>("Sprite2D");
		GD.Print("Sprite1: " + _sprite);

		if (Texture != null)
			_sprite.Texture = Texture;
	}

	public void SetTexture(Texture2D texture)
	{
		Texture = texture;
		GD.Print(texture);
		GD.Print("Sprite2: " + _sprite);

		_sprite.Texture = texture;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) { }
}
