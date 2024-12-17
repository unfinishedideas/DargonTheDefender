using Godot;
using System;

public partial class EnemySpawner : Node2D
{
	[Export]
	public PackedScene RobberScene {get; set;}
	public int maxRobbers = 4;
	public int currentRobbers = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SpawnRobber(GoldPile pile)
	{
	   Robber robber = RobberScene.Instantiate<Robber>();
	   robber.Initialize(this.GlobalPosition, pile);
	   AddChild(robber);
	}
}
