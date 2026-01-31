using System;
using Godot;

public partial class Camouflage : Node
{
	[Signal]
	public delegate void CamouflageUpdatedEventHandler(float contrast, int energy, bool active);

	[Export]
	public float Alpha { get; set; } = 0.3f;

	[Export]
	public float ChangeSpeed { get; set; } = 0.005f;

	[Export]
	public bool IsActive { get; set; } = false;

	[Export]
	public int Energy { get; set; } = 10;

	public float Contrast { get; set; } = 1f;

	private Polygon2D area;
	private Area2D? collider = null;

	public override void _Ready()
	{
		area = GetNode<Polygon2D>("Area1");
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("camouflage_right"))
		{
			area.Color.ToHsv(out float h, out float s, out float v);
			Color newColor = Color.FromHsv(h + ChangeSpeed, 1, 1, Alpha);
			area.Color = newColor;

			if (collider != null)
			{
				UpdateContrast(collider);
			}
		}

		if (Input.IsActionPressed("camouflage_left"))
		{
			area.Color.ToHsv(out float h, out float s, out float v);
			Color newColor = Color.FromHsv(h - ChangeSpeed, 1, 1, Alpha);
			area.Color = newColor;

			if (collider != null)
			{
				UpdateContrast(collider);
			}
		}

		if (Input.IsActionJustReleased("camouflage_toggle"))
		{
			GD.Print($"Contrast: {Contrast}");
			IsActive = !IsActive;
			EmitSignal(SignalName.CamouflageUpdated, Contrast, Energy, IsActive);
			if (!IsActive)
			{
				Contrast = 1;

				if (collider != null)
				{
					UpdateContrast(collider);
				}
			}
		}

		area.Visible = IsActive;
	}

	private void OnTimerTimeout()
	{
		if (IsActive && Energy > 0)
		{
			Energy -= 1;
			EmitSignal(SignalName.CamouflageUpdated, Contrast, Energy, IsActive);
		}

		if (!IsActive && Energy < 10)
		{
			Energy += 1;
			EmitSignal(SignalName.CamouflageUpdated, Contrast, Energy, IsActive);
		}

		if (Energy <= 0)
		{
			IsActive = false;
			Contrast = 1;

			if (collider != null)
			{
				UpdateContrast(collider);
			}
		}
	}

	private void OnCamouflageCollision(Area2D body)
	{
		if (IsActive)
		{
			collider = body;
			bool isTile = UpdateContrast(body);
			if (!isTile)
			{
				return;
			}
		}
	}

	private bool UpdateContrast(Area2D body)
	{
		var t = body.GetNodeOrNull<GreenTile>(".");
		if (t == null)
			return false;

		// Calculate contrast between tile and camouflage area
		// based on hue difference
		t.TileColor.ToHsv(out float h1, out float s1, out float v1);
		area.Color.ToHsv(out float h2, out float s2, out float v2);
		Contrast = MathF.Abs(h1 - h2);

		GD.Print($"Contrast: {Contrast}");

		EmitSignal(SignalName.CamouflageUpdated, Contrast, Energy, IsActive);

		return true;
	}
}
