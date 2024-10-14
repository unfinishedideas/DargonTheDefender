using Godot;
using System;

public partial class Robber : CharacterBody2D
{
    [Export]
	public float Speed = 280.0f;
    [Export]
    public float TakingDamageSpeed = 150.0f;

    private enum EnemyState { RunningToGold, Stealing, RunningFromLevel, TakingDamage, Death };
    private EnemyState _currentState;

    public override void _Ready()
    {
        _currentState = EnemyState.RunningToGold;
    }

	public override void _PhysicsProcess(double delta)
	{
        CheckState(delta);
        switch(_currentState)
        {
            case EnemyState.RunningToGold:
                RunToGold(delta);
                break;
            case EnemyState.Stealing:
                StealGold(delta);
                break;
            case EnemyState.RunningFromLevel:
                RunFromLevel(delta);
                break;
            case EnemyState.TakingDamage:
                TakeDamage(delta);
                break;
            case EnemyState.Death:
                Death(delta);
                break;
        }
	}

    // Determine what course of action to take next ---------------------------
    private void CheckState(double delta)
    {
    }

    // Get direction toward gold pile and run towards it ----------------------
    private void RunToGold(double delta)
    {
		Vector2 velocity = Velocity;


		Vector2 direction = Vector2.Zero;
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
		}
		Velocity = velocity;
		MoveAndSlide();
    }

    // Steal that gold! -------------------------------------------------------
    private void StealGold(double delta)
    {
    }

    // Get away with the loot! ------------------------------------------------
    private void RunFromLevel(double delta)
    {
    }

    // Ouch -------------------------------------------------------------------
    private void TakeDamage(double delta)
    {
    }

    // Bye bye ----------------------------------------------------------------
    private void Death(double delta)
    {
    }
}
