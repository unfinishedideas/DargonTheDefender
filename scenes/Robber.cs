using Godot;
using System;

// TODO: Create a generic Enemy class that Robber inherits
public partial class Robber : CharacterBody2D
{
	[Export]
	public float Speed = 1.0f;
	//public float Speed = 280.0f;
	[Export]
	public float TakeDamageSpeed = 150.0f;
	[Export]
	public float MaxHealth = 100.0f;
	[Export]
	public float MaxTreasure = 100.0f;

	private enum EnemyState { RunToGold, Steal, RunFromLevel, TakeDamage, Death };
	private EnemyState _currentState;
	private float _currentHealth = 100.0f;
	private float _currentTreasure = 0.0f;

	// Nodes
	private ProgressBar _healthBar;
	private ProgressBar _treasureStolenBar;
	private GoldPile _goldPile;

	public override void _Ready()
	{
		_currentHealth = MaxHealth;
		_currentState = EnemyState.RunToGold;
		_healthBar = GetNode<ProgressBar>("%HealthBar");
		_treasureStolenBar = GetNode<ProgressBar>("%TreasureStolenBar");
	}

	public override void _PhysicsProcess(double delta)
	{
		CheckState();
		switch(_currentState)
		{
			case EnemyState.RunToGold:
				RunToGold(delta);
				break;
			case EnemyState.Steal:
				StealGold();
				break;
			case EnemyState.RunFromLevel:
				RunFromLevel(delta);
				break;
			case EnemyState.TakeDamage:
				TakeDamage();
				break;
			case EnemyState.Death:
				Death();
				break;
		}
		UpdateUI();
	}

	public void Initialize(Vector2 spawnLocation, GoldPile pile)
	{
		this.GlobalPosition = spawnLocation;
		_goldPile = pile;
	}

	// Determine what course of action to take next ---------------------------
	private void CheckState()
	{
	}

	// Get direction toward gold pile and run towards it ----------------------
	private void RunToGold(double delta)
	{
		Vector2 direction = _goldPile.GlobalPosition - this.GlobalPosition;
		RunSomewhere(delta, direction);
	}

	// Get direction to nearest exit and run towards it -----------------------
	private void RunFromLevel(double delta)
	{
	}

	// Run in that direction! -------------------------------------------------
	private void RunSomewhere(double delta, Vector2 direction)
	{
		Vector2 velocity = Velocity;
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
		}
		Velocity = velocity;
		MoveAndSlide();
	}

	// Steal that gold! -------------------------------------------------------
	private void StealGold()
	{
	}

	// Ouch -------------------------------------------------------------------
	private void TakeDamage()
	{
	}

	// Bye bye ----------------------------------------------------------------
	private void Death()
	{
		// play death animation
		// drop treasure on the ground? Or just give back to hoard for now...
	}

	// Update UI for the guy --------------------------------------------------
	private void UpdateUI()
	{
		if (_currentHealth < MaxHealth)
		{
			_healthBar.Value = _currentHealth;
			_healthBar.Visible = true;
		}
		if (_currentTreasure > 0)
		{
			_treasureStolenBar.Value = _currentTreasure;
			_treasureStolenBar.Visible = true;
		}
	}
}
