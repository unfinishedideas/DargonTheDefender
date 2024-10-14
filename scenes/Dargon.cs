using Godot;
using System;

public partial class Dargon : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;
    public bool FacingRight = true;

    private bool _isAttacking = false;
    private bool _readyToAttack = true;

    // Nodes
    public AnimatedSprite2D AnimPlayer;
    public Area2D AttackArea;
    public Timer AttackTimer;
    public Timer AttackCooldown;
    public GpuParticles2D AttackParticles;

    public override void _Ready()
    {
        base._Ready();
        AnimPlayer = GetNode<AnimatedSprite2D>("%AnimatedSprite2D");
        AttackArea = GetNode<Area2D>("%AttackArea");
        AttackTimer = GetNode<Timer>("%AttackTimer");
        AttackParticles = GetNode<GpuParticles2D>("%AttackParticles");
        AttackCooldown = GetNode<Timer>("%AttackCooldown");
        AttackTimer.Timeout += () => StopAttack();
        AttackCooldown.Timeout += () => { _readyToAttack = true; };
    }

	public override void _PhysicsProcess(double delta)
	{
        InputCheck(delta);
		MoveAndSlide();
	}

    private void InputCheck(double delta)
    {
        // Movement related stuff ----------------------------------------------
		Vector2 velocity = Velocity;

		// Get the input direction and handle the movement/deceleration.
		Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		if (direction != Vector2.Zero)
		{
            if (!_isAttacking)
            {
                AnimPlayer.Play("walk");
            }
			velocity.X = direction.X * Speed;
            velocity.Y = direction.Y * Speed;
		}
		else
		{
            if (!_isAttacking)
            {
                AnimPlayer.Play("idle");
            }
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

        // Flip things based on FacingRight
        switch(FacingRight)
        {
            case true:
                AttackArea.Scale = new Vector2(1, AttackArea.Scale.Y);
                FlipGPUParticles();
                break;
            case false:
                AttackArea.Scale = new Vector2(-1, AttackArea.Scale.Y);
                FlipGPUParticles();
                break;
        }

		Velocity = velocity;

        // Attack related stuff ------------------------------------------------
        if (Input.IsActionPressed("attack") && !_isAttacking && _readyToAttack)
        {
            Attack(delta);
        }
    }

    private void Attack(double delta)
    {
        GD.Print("ATTAK");
        _isAttacking = true;
        _readyToAttack = false;
        AttackTimer.Start();
        AttackParticles.Emitting = true;
        AttackArea.Monitoring = true;
        AnimPlayer.Play("attack");
    }

    private void StopAttack()
    {
        AttackCooldown.Start();
        _isAttacking = false;
        AttackParticles.Emitting = false;
        AttackArea.Monitoring = false;
    }

    // Flip the fire effects horizontally based on FacingRight
    private void FlipGPUParticles()
    {
        ParticleProcessMaterial partyMaterial = (ParticleProcessMaterial)AttackParticles.ProcessMaterial;

        Vector3 newEmissionShapeOffset = partyMaterial.EmissionShapeOffset; 
        newEmissionShapeOffset.X = FacingRight ? 20 : -20;

        Vector3 newEmissionDirection = partyMaterial.Direction;
        newEmissionDirection.X = FacingRight ? 1 : -1;

        partyMaterial.EmissionShapeOffset = newEmissionShapeOffset;
        partyMaterial.Direction = newEmissionDirection;

        AttackParticles.ProcessMaterial = partyMaterial;
    }
}
