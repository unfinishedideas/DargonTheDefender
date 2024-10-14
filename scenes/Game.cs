using Godot;
using System;

public partial class Game : Node2D
{
    private float _currentTreasure = 999.0f;    // default to 999 so I can see it 
    private float _maxTreasure = 999.0f;        // change when the signal comes

    private bool _firstTreasureReport = true;

    // Nodes
    public RichTextLabel TreasureCounter;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        TreasureCounter = GetNode<RichTextLabel>("%TreasureCounter");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

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
}
