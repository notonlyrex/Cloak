using System;
using Godot;

public partial class Camouflage : Node
{
    [Signal]
    public delegate void EnterEventHandler(float contrast);

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
        }

        if (Input.IsActionPressed("camouflage_left"))
        {
            area.Color.ToHsv(out float h, out float s, out float v);
            Color newColor = Color.FromHsv(h - ChangeSpeed, 1, 1, Alpha);
            area.Color = newColor;
        }

        if (Input.IsActionJustReleased("camouflage_toggle"))
        {
            GD.Print($"Contrast: {Contrast}");
            IsActive = !IsActive;
            if (!IsActive)
            {
                Contrast = 1;
            }
        }

        area.Visible = IsActive;
    }

    private void OnTimerTimeout()
    {
        if (IsActive && Energy > 0)
        {
            Energy -= 1;
        }

        if (!IsActive && Energy < 10)
        {
            Energy += 1;
        }

        if (Energy <= 0)
        {
            IsActive = false;
            Contrast = 1;
        }
    }

    private void OnCamouflageCollision(Area2D body)
    {
        if (IsActive)
        {
            var t = body.GetNodeOrNull<GreenTile>(".");

            if (t == null)
                return;

            // Calculate contrast between tile and camouflage area
            // based on hue difference
            t.TileColor.ToHsv(out float h1, out float s1, out float v1);
            area.Color.ToHsv(out float h2, out float s2, out float v2);
            Contrast = MathF.Abs(h1 - h2);

            GD.Print($"Contrast: {Contrast}");
        }
    }
}
