using Godot;
using System;

public class MainMenuBackground : ParallaxBackground
{
    [Export]
    public float ScrollSpeed = 100;
    public override void _Process(float delta)
    {
        // Scroll the parallax background to the left
        ScrollOffset -= new Vector2(ScrollSpeed * delta, 0);
    }
}
