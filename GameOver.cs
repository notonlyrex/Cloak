using System;
using Godot;

public partial class GameOver : Node
{
	[Signal]
	public delegate void RestartEventHandler();

	public int FinalScore { get; set; } = 0;

	public override void _Ready()
	{
		GetNode<Label>("Score").Text = $"Final Score: {FinalScore} (levels completed)";
	}

	public override void _Process(double delta) { }

	public void OnButtonPressed()
	{
		EmitSignal(SignalName.Restart);
	}
}
