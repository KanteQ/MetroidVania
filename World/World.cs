using Godot;
using System;

public class World : Node2D
{
    private Node2D Hero;
    private Camera2D Camera2D;
    private Minimap Minimap;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Hero = GetNode<Node2D>("Hero");
        Minimap = GetNode<Minimap>("Minimap");
        Camera2D = Hero.GetNode<Camera2D>("Camera2D");
        GetCameraLimits();
    }
    private void GetCameraLimits()
    {
        Rect2 AreaCameraLimits = Minimap.GetCameraAreaLimits();
        Camera2D.LimitLeft = (int)AreaCameraLimits.Position.x;
        Camera2D.LimitTop = (int)AreaCameraLimits.Position.y;
        Camera2D.LimitRight = (int)AreaCameraLimits.Size.x;
        Camera2D.LimitBottom = (int)AreaCameraLimits.Size.y;
    }
    public override void _Process(float delta)
    {
        Minimap.UpdateMinmapPosition(Hero.Position);
    }
}
