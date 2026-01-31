using System;
using Godot;

public partial class Player : Area2D
{
	[Signal]
	public delegate void HitEventHandler();

	[Signal]
	public delegate void CamouflageUpdatedEventHandler(float contrast, int energy, bool active);

	[Export]
	public int Speed { get; set; } = 400; // How fast the player will move (pixels/sec).

	public Vector2 ScreenSize;

	private float contrast = 1f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var velocity = Vector2.Zero; // The player's movement vector.

		if (Input.IsActionPressed("move_right"))
		{
			velocity.X += 1;
		}

		if (Input.IsActionPressed("move_left"))
		{
			velocity.X -= 1;
		}

		if (Input.IsActionPressed("move_down"))
		{
			velocity.Y += 1;
		}

		if (Input.IsActionPressed("move_up"))
		{
			velocity.Y -= 1;
		}

		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		if (velocity.Length() > 0)
		{
			velocity = velocity.Normalized() * Speed;
			animatedSprite2D.Play();
		}
		else
		{
			animatedSprite2D.Stop();
		}

		Position += velocity * (float)delta;
		Position = new Vector2(
			x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
			y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
		);

		if (velocity.X != 0)
		{
			//animatedSprite2D.Animation = "walk";
			//animatedSprite2D.FlipV = false;
			// See the note below about the following boolean assignment.
			//animatedSprite2D.FlipH = velocity.X < 0;
		}
		else if (velocity.Y != 0)
		{
			//animatedSprite2D.Animation = "up";
			//animatedSprite2D.FlipV = velocity.Y > 0;
		}

		CalculateContrast();

		EmitSignal(
			SignalName.CamouflageUpdated,
			contrast,
			GetNode<Camouflage>("Camouflage").Energy,
			GetNode<Camouflage>("Camouflage").IsActive
		);
	}

	private void CheckAllHits()
	{
		var area2DList = GetOverlappingAreas();
		foreach (var area in area2DList)
		{
			if (area is Enemy enemy)
			{
				CheckHit();
			}
		}
	}

	private void CalculateContrast()
	{
		// player checking collisions with background tiles
		// and calculating average contrast between itself and tiles
		var area2DList = GetOverlappingAreas();

		float averageContrast = 0f;
		int tileCount = 0;
		foreach (var area in area2DList)
		{
			if (area is Tile tile)
			{
				var camouflage = GetNode<Camouflage>("Camouflage");
				contrast = camouflage.CalculateContrast(tile);
				averageContrast += contrast;
				tileCount++;
			}
		}

		if (tileCount > 0)
		{
			averageContrast /= tileCount;
			contrast = averageContrast;
		}
		else
		{
			contrast = 1f;
		}
	}

	private void CheckHit()
	{
		if (contrast > 0.2f)
		{
			GD.Print("Player is visible and takes damage.");
			EmitSignal(SignalName.Hit);
		}
		else
		{
			GD.Print("Player is camouflaged and avoids damage.");
		}
	}

	private void OnBodyEntered(Area2D body)
	{
		if (body is Enemy)
		{
			GD.Print("Player hit an enemy!");
			CheckHit();
		}

		// Hide(); // Player disappears after being hit.
		// EmitSignal(SignalName.Hit);
		// // Must be deferred as we can't change physics properties on a physics callback.
		// GetNode<CollisionShape2D>("CollisionShape2D")
		// 	.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
	}

	private void OnCamouflageUpdated(int energy, bool active)
	{
		CalculateContrast();
		CheckAllHits();
		EmitSignal(SignalName.CamouflageUpdated, contrast, energy, active);
	}
}
