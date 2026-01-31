using System;
using Godot;

public partial class Hud : Node2D
{
    private Label camouflageLabel;

    private int energy = 10;
    private bool isActive = false;
    private float camouflageLevel = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        camouflageLabel = GetNode<Label>("HUD/Camouflage");
        camouflageLabel.Text = GetCamouflageStatus();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }

    private void OnCamouflageUpdated(float contrast, int energy, bool active)
    {
        this.energy = energy;
        this.isActive = active;
        this.camouflageLevel = (1 - contrast) * 100f;

        camouflageLabel.Text = GetCamouflageStatus();
    }

    private string GetCamouflageStatus()
    {
        return $"Camouflage: {(isActive ? "ON" : "OFF")} (Level: {camouflageLevel:F1}%, Energy: {energy} s)";
    }
}
