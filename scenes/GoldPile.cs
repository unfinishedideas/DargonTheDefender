using Godot;
using System;

public partial class GoldPile : StaticBody2D
{
    [Export]
    public float MaxTreasure = 1000.0f;
    private float _currentTreasure;

    private bool emittedOnce = false;

    // Signals
    [Signal]
    public delegate void UpdateTreasureCountEventHandler(float newTreasure);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        _currentTreasure = MaxTreasure;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        // TODO: this is a little jank, but it was emitting too soon in _Ready()
        if (emittedOnce == false)
        {
            EmitSignal(SignalName.UpdateTreasureCount, _currentTreasure);
            emittedOnce = true;
        }

	}

    // To be called by other classes
    public void ReduceTreasure(float amount)
    {
        _currentTreasure = Math.Clamp(_currentTreasure - amount, 0, MaxTreasure);
        EmitSignal(SignalName.UpdateTreasureCount, _currentTreasure);
    }

    public void IncreaseTreasure(float amount)
    {
        _currentTreasure = Math.Clamp(_currentTreasure + amount, 0, MaxTreasure);
        EmitSignal(SignalName.UpdateTreasureCount, _currentTreasure);
    }
}
