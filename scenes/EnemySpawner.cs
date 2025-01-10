using Godot;
using System;
using System.Collections.Generic;

public partial class EnemySpawner : Node2D
{
	[Export]
	public bool SpawnerActive = true;
	[Export]
	public PackedScene RobberScene {get; set;}
	
	public int maxRobbers = 4;
	public int currentEnemyCount = 0;
	public Godot.Collections.Array<Godot.Node> activeEnemies;
	private Node2D _enemyList; // for Node organization, keeping track of child enemies

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_enemyList = GetNode<Node2D>("%EnemyList");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SpawnRobber(GoldPile pile)
	{
		// TODO: Replace currentRobbersCount with ActiveEnemies.Count()
		if (currentEnemyCount < maxRobbers && SpawnerActive)
		{
			Robber robber = RobberScene.Instantiate<Robber>();
			robber.Initialize(this, pile);
			_enemyList.AddChild(robber);
			UpdateActiveEnemies();
		}
	}
	
	private void UpdateActiveEnemies()
	{
		activeEnemies = _enemyList.GetChildren();

		// Godot Arrays suck at counting... so I'll count for you!!!
		currentEnemyCount = 0;
		foreach(Node enemy in activeEnemies)
		{
			currentEnemyCount++;
		}
	}
	
	// TODO: Update this for new enemy list style
	public void RemoveRobber(Robber toRemove)
	{
		UpdateActiveEnemies();
	}
}
