using Godot;
using System;

public class RopeSegment : RigidBody2D
{
    public int IndexInArray;            // Which index in the rope segment array the segment has
    public Godot.Object Rope = null;    // The rope (which rope the segment is part of)
}
