using Godot;
using System;

public partial class Dargon : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;
    public bool FacingRight = true;

    [Export]
    public float FireRechargeRate = 0.01f;  // how fast fire recharges
    [Export]
    public float FireDrainRate = 0.01f;  // how fast fire drains

    private bool _isAttacking = false;
    private bool _burnOut = false;       // used too much fire, must cooldown
    private float _amountOfFuel = 1.0f;  // 0-1% how much fire left

    // Nodes
    public AnimatedSprite2D AnimPlayer;
    public Area2D AttackArea;
    public GpuParticles2D AttackParticles;
    public ProgressBar FireAmmoBar;
    public Timer HurtboxAfterTimer; // how long after the player stops attacking until the hurtbox turns off
    public AnimationPlayer ColorAnimator;

    public override void _Ready()
    {
        base._Ready();
        AnimPlayer = GetNode<AnimatedSprite2D>("%AnimatedSprite2D");
        AttackArea = GetNode<Area2D>("%AttackArea");
        AttackParticles = GetNode<GpuParticles2D>("%AttackParticles");
        FireAmmoBar = GetNode<ProgressBar>("%FireAmmoBar");
        HurtboxAfterTimer = GetNode<Timer>("%HurtboxAfterTimer");
        HurtboxAfterTimer.Timeout += () => { AttackArea.Monitoring = false; };
        ColorAnimator = GetNode<AnimationPlayer>("%ColorAnimator");
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
        // Attacking, not burnt out
        if (Input.IsActionPressed("attack") && !_burnOut)
        {
            _amountOfFuel = Math.Clamp(_amountOfFuel - FireDrainRate, 0, 1);
            if (_amountOfFuel <= 0)
            {
                _burnOut = true;
                ColorAnimator.Play("burnout");
            }
            else
            {
                _isAttacking = true;
                AnimPlayer.Play("attack");
                AttackParticles.Emitting = true;
            }
        }
        // Not Attacking (recharge)
        else
        {
            _isAttacking = false;
            AttackParticles.Emitting = false;
            _amountOfFuel = Math.Clamp(_amountOfFuel + FireRechargeRate, 0, 1);
            if(_amountOfFuel == 1)
            {
                _burnOut = false;
                ColorAnimator.Play("RESET");
            }
        }
        // Refresh Ammo bar graphic
        if (_amountOfFuel <= 1)
        {
            FireAmmoBar.Visible = true;
            FireAmmoBar.Value = _amountOfFuel;
        }
        else
        {
            FireAmmoBar.Visible = false;
        }

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
