using Godot;
using System;

public partial class Game : Node2D
{
	private float _currentTreasure = 999.0f;    // default to 999 so I can see it 
	private float _maxTreasure = 999.0f;        // change when the signal comes
	private bool _firstTreasureReport = true;   // used to get the initial value of treasure

	// Nodes
	public RichTextLabel TreasureCounter;
	public RichTextLabel LevelTimerDisplay;
	public Timer LevelTimer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		TreasureCounter = GetNode<RichTextLabel>("%TreasureCounter");
		LevelTimerDisplay = GetNode<RichTextLabel>("%LevelTimerDisplay");
		LevelTimer = GetNode<Timer>("%LevelTimer");

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("force_quit"))
		{
			GetTree().Quit();
		}
		TimeSpan duration = TimeSpan.FromSeconds(LevelTimer.TimeLeft);
		LevelTimerDisplay.Text = $"[right]{duration.Minutes.ToString("D2")}:{duration.Seconds.ToString("D2")}[/right]";
	}

	// signals ----------------------------------------------------------------
	public void _on_gold_pile_update_treasure_count(float newTreasure)
	{
		_currentTreasure = newTreasure;
		TreasureCounter.Text = _currentTreasure.ToString();

		// Properly set the max treasure value from the first signal
		if (_firstTreasureReport == true)
		{
			_maxTreasure = _currentTreasure;
			_firstTreasureReport = false;
		}
	}
}
