using System;
using Godot;

public partial class SceneManager : Node2D
{
	private int levelIndex = 1;
	private Level current;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		LoadNewLevel();
	}

	private void LoadNewLevel()
	{
		current = ResourceLoader.Load<PackedScene>("res://Level.tscn").Instantiate<Level>();
		current.LevelNumber = levelIndex;
		current.Connect(
			Level.SignalName.LevelFinished,
			new Callable(this, nameof(OnLevelFinished))
		);
		current.Connect(Level.SignalName.GameOver, new Callable(this, nameof(OnGameOver)));
		AddChild(current);
	}

	private void LoadGameOver()
	{
		var current = ResourceLoader
			.Load<PackedScene>("res://GameOver.tscn")
			.Instantiate<GameOver>();
		current.FinalScore = levelIndex - 1;
		// current.Connect(
		//     GameOver.SignalName.LevelFinished,
		//     new Callable(this, nameof(OnLevelFinished))
		// );
		AddChild(current);
	}

	private void OnGameOver()
	{
		current.Disconnect(
			Level.SignalName.LevelFinished,
			new Callable(this, nameof(OnLevelFinished))
		);
		current.QueueFree();

		CallDeferred(nameof(LoadGameOver));
	}

	public override void _Process(double delta) { }

	private void OnLevelFinished()
	{
		GD.Print("On level finished!");
		current.Disconnect(
			Level.SignalName.LevelFinished,
			new Callable(this, nameof(OnLevelFinished))
		);
		current.QueueFree();

		levelIndex += 1;
		GD.Print("Loading level " + levelIndex);
		CallDeferred(nameof(LoadNewLevel));
	}
}
