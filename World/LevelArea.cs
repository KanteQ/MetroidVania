using Godot;
using System;

public class LevelArea : Node2D
{
    private CustomSignals CustomSignals;    // Custom signals singleton

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Get the custom signals singleton
        CustomSignals = GetNode<CustomSignals>("/root/CustomSignals");
        // Connect to the pause game signal
        CustomSignals.Connect(nameof(CustomSignals.PauseGameSignal), this, nameof(PauseGame));                
    }

    private void PauseGame(bool paused, string pauseName)
    {
        // If the game should pause
        if (paused)
        {
            GetTree().Paused = true;    // Pause the level area node and it's children
        }
        // if the game shoud resume
        else
        {
            GetTree().Paused = false;   // Un-pause the level area node and it's children
        }
    }
}
