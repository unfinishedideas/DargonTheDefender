using Godot;
using System;

public partial class Dargon : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;
    public bool FacingRight = true;

    // Nodes
    public AnimatedSprite2D AnimPlayer;
    public Area2D AttackArea;

    public override void _Ready()
    {
        base._Ready();
        AnimPlayer = GetNode<AnimatedSprite2D>("%AnimatedSprite2D");
        AttackArea = GetNode<Area2D>("%AttackArea");
    }

	public override void _PhysicsProcess(double delta)
	{
        InputCheck(delta);
		MoveAndSlide();
	}

    private void InputCheck(double delta)
    {
		Vector2 velocity = Velocity;

		// Get the input direction and handle the movement/deceleration.
		Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		if (direction != Vector2.Zero)
		{
            AnimPlayer.Play("walk");
			velocity.X = direction.X * Speed;
            velocity.Y = direction.Y * Speed;
		}
		else
		{
            AnimPlayer.Play("idle");
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);
		}

        // Check facing and flip sprite and attack area location
        FacingRight = velocity.X switch 
        {
            > 0 => true,
            < 0 => false,
            0   => FacingRight,
            _   => FacingRight,
        };
        AnimPlayer.FlipH = !FacingRight;

        AttackArea.Scale = FacingRight switch
        {
            true => new Vector2(1, AttackArea.Scale.Y),
            false => new Vector2(-1, AttackArea.Scale.Y),
        };

		Velocity = velocity;
    }
}
