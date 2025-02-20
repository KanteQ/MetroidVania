using Godot;
using System;

public class Particles : Particles2D
{
    public override void _Process(float delta)
    {
        // If the dust particles no longer are emitting
        if(!Emitting)
        {
            QueueFree();    // Free them from memory
        }
    }
}
