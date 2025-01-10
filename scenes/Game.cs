using Godot;
using System;

public partial class Game : Node2D
{
	private float _currentTreasure = 999.0f;    // default to 999 so I can see it 
	private float _maxTreasure = 999.0f;        // change when the signal comes
	private bool _firstTreasureReport = true;   // used to get the initial value of treasure
	private int _numSpawners;

	// Nodes
	public RichTextLabel TreasureCounter;
	public RichTextLabel LevelTimerDisplay;
	public Timer LevelTimer;
	public Timer EnemySpawnTimer;
	private Node2D _spawnerParent;
	public EnemySpawner[] EnemySpawners;
	public GoldPile GoldHoard;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		TreasureCounter = GetNode<RichTextLabel>("%TreasureCounter");
		LevelTimerDisplay = GetNode<RichTextLabel>("%LevelTimerDisplay");
		LevelTimer = GetNode<Timer>("%LevelTimer");
		_spawnerParent = GetNode<Node2D>("%EnemySpawners");
		GoldHoard = GetNode<GoldPile>("%GoldPile");
		EnemySpawnTimer = GetNode<Timer>("%EnemySpawnTimer");

		// Add the enemy spawners
		Godot.Collections.Array<Node> points = _spawnerParent.GetChildren();
		_numSpawners = points.Count;
		EnemySpawners = new EnemySpawner[_numSpawners];

		// populate the active spawner list
		int i = 0;
		foreach(Node point in points)
		{
			EnemySpawners[i] = (EnemySpawner)point;
			i++;
		}
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

	// TODO: Have this actually check to see how many enemies there are first instead of just random :D
	public void CheckForSpawns()
	{
		SpawnEnemy();
		/*
		var rand = new Random();
		int choice = rand.Next(0, 2);
		if (choice == 1)
		{
			GD.Print("SPAWN");
			SpawnEnemy();
		}
		*/
	}

	// TODO: Make a generic enemy type that can be spawned with this
	// make enemy spawning more intelligent
	public void SpawnEnemy()
	{
		//EnemySpawners[0].SpawnRobber(GoldHoard);  // used for debugging just the first spawner
		var rand = new Random();
		EnemySpawners[rand.Next(0,_numSpawners)].SpawnRobber(GoldHoard);
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

	public void _on_enemy_spawn_timer_timeout()
	{
		CheckForSpawns();
		EnemySpawnTimer.Start();
	}

}
