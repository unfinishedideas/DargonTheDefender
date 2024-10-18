using Godot;
using System;

public partial class Dargon : CharacterBody2D
{
	[Export]
	public float Speed = 650.0f;            // movement speed while walking
	[Export]
	public float BurninatingSpeed = 450.0f; // movement speed while attacking
	[Export]
	public float BurnoutSpeed = 375.0f;     // movement speed in burnout
	[Export]
	public float FireRechargeRate = 0.01f;  // how fast fire recharges
	[Export]
	public float FireDrainRate = 0.01f;     // how fast fire drains
	[Export]
	public float MaxSprintMultiplier = 1.5f; // multiplier to use for sprinting
	[Export]
	public float SprintStaminaDrainRate = 0.01f; // host fast stamina drains while sprinting
	[Export]
	public float SprintStaminaRechargeRate = 0.01f; // host fast stamina recharges when not sprinting

	private bool _facingRight = true;
	private bool _isAttacking = false;
	private bool _burnOut = false;       // used too much fire, must cooldown
	private float _amountOfFuel = 1.0f;  // 0-1% how much fire left
	private float _amountOfStamina = 1.0f; // how much stamina left for sprinting
	private float _currentSprintMultiplier = 1.5f;

	// Nodes
	public AnimatedSprite2D AnimPlayer;
	public Area2D AttackArea;
	public GpuParticles2D AttackParticles;
	public ProgressBar FireAmmoBar;
	public ProgressBar StaminaBar;
	public Timer HurtboxAfterTimer; // how long after the player stops attacking until the hurtbox turns off
	public AnimationPlayer ColorAnimator;

	public override void _Ready()
	{
		base._Ready();
		AnimPlayer = GetNode<AnimatedSprite2D>("%AnimatedSprite2D");
		AttackArea = GetNode<Area2D>("%AttackArea");
		AttackParticles = GetNode<GpuParticles2D>("%AttackParticles");
		FireAmmoBar = GetNode<ProgressBar>("%FireAmmoBar");
		StaminaBar = GetNode<ProgressBar>("%StaminaBar");
		HurtboxAfterTimer = GetNode<Timer>("%HurtboxAfterTimer");
		HurtboxAfterTimer.Timeout += () => { AttackArea.Monitoring = false; };
		ColorAnimator = GetNode<AnimationPlayer>("%ColorAnimator");
	}

	public override void _PhysicsProcess(double delta)
	{
		InputCheck(delta);
		UpdateUI(delta);
		MoveAndSlide();
	}

	// Perform input checks for movement and attack, etc ----------------------
	private void InputCheck(double delta)
	{
		// Movement related stuff
		Vector2 velocity = Velocity;

		if (!Input.IsActionPressed("sprint") || _burnOut == true)
		{
			_amountOfStamina = Math.Clamp(_amountOfStamina + SprintStaminaRechargeRate, 0, 1.0f);
		}

		// Get the input direction and handle the movement/deceleration.
		Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		if (direction != Vector2.Zero)
		{
			// Handle Sprinting Speed
			if (Input.IsActionPressed("sprint") && _burnOut == false && _amountOfStamina > 0)
			{
				_currentSprintMultiplier = MaxSprintMultiplier;
				_amountOfStamina -= SprintStaminaDrainRate;
				if (_amountOfStamina <= 0)
				{
					EnterBurnout();
				}
			}
			else
			{
				_currentSprintMultiplier = 1.0f;
			}

			// Handle Regular Speed
			if (!_isAttacking)
			{
				AnimPlayer.Play("walk");
			}
			float currentSpeed = 0f;
			if (_burnOut)
			{
				currentSpeed = BurnoutSpeed;
			}
			else if (_isAttacking)
			{
				currentSpeed = BurninatingSpeed;
			}
			else
			{
				currentSpeed = Speed;
			}
			velocity.X = direction.X * currentSpeed * _currentSprintMultiplier;
			velocity.Y = direction.Y * currentSpeed * _currentSprintMultiplier;
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

		Velocity = velocity;

		UpdatePlayerFacing(velocity.X);
		Attacking(delta);
	}

	// Flip things based on _facingRight --------------------------------------
	private void UpdatePlayerFacing(float velocityX)
	{
		// Check facing and flip sprite and attack area location
		_facingRight = velocityX switch
		{
			> 0 => true,
			< 0 => false,
			0 => _facingRight,
			_ => _facingRight,
		};
		AnimPlayer.FlipH = !_facingRight;
		switch (_facingRight)
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
	}

	// Flip the fire effects horizontally based on _facingRight ---------------
	private void FlipGPUParticles()
	{
		ParticleProcessMaterial partyMaterial = (ParticleProcessMaterial)AttackParticles.ProcessMaterial;

		Vector3 newEmissionShapeOffset = partyMaterial.EmissionShapeOffset;
		newEmissionShapeOffset.X = _facingRight ? 20 : -20;

		Vector3 newEmissionDirection = partyMaterial.Direction;
		newEmissionDirection.X = _facingRight ? 1 : -1;

		partyMaterial.EmissionShapeOffset = newEmissionShapeOffset;
		partyMaterial.Direction = newEmissionDirection;

		AttackParticles.ProcessMaterial = partyMaterial;
	}

	// Attack related stuff ---------------------------------------------------
	private void Attacking(double delta)
	{
		// Attacking, not burnt out
		if (Input.IsActionPressed("attack") && !_burnOut)
		{
			if (HurtboxAfterTimer.IsStopped() != true)
			{
				HurtboxAfterTimer.Stop();
			}
			_amountOfFuel = Math.Clamp(_amountOfFuel - FireDrainRate, 0, 1);
			if (_amountOfFuel <= 0)
			{
				EnterBurnout();
			}
			else
			{
				_isAttacking = true;
				AnimPlayer.Play("attack");
				AttackParticles.Emitting = true;
				AttackArea.Monitoring = true;
			}
		}
		// Not Attacking (recharge)
		else
		{
			if (_isAttacking == true)
			{
				HurtboxAfterTimer.Start();
			}
			_isAttacking = false;
			AttackParticles.Emitting = false;
			_amountOfFuel = Math.Clamp(_amountOfFuel + FireRechargeRate, 0, 1);
			if (_amountOfFuel == 1 && _amountOfStamina == 1)
			{
				_burnOut = false;
				ColorAnimator.Play("RESET");
			}
		}
	}

	// Update the Player UI (health, fuel etc) --------------------------------
	private void UpdateUI(double delta)
	{
		// Refresh Stamina bar UI
		if (_amountOfStamina < 1)
		{
			StaminaBar.Visible = true;
			StaminaBar.Value = _amountOfStamina;
		}
		else
		{
			StaminaBar.Visible = false;
		}
		// Refresh Ammo bar UI
		if (_amountOfFuel < 1)
		{
			FireAmmoBar.Visible = true;
			FireAmmoBar.Value = _amountOfFuel;
		}
		else
		{
			FireAmmoBar.Visible = false;
		}
	}

	// Dargon enters the burnout state ----------------------------------------
	private void EnterBurnout()
	{
		_burnOut = true;
		ColorAnimator.Play("burnout");
	}
}
