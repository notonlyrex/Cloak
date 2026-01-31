using System;
using Godot;

public partial class Intro : Node
{
	[Signal]
	public delegate void RestartEventHandler();

	public int FinalScore { get; set; } = 0;

	public override void _Ready() { }

	public override void _Process(double delta) { }

	public void OnButtonPressed()
	{
		EmitSignal(SignalName.Restart);
	}
}
