using Godot;
using System.Collections.Generic;

public class Rope : Node2D
{
    [Export]
    public bool StaticRopeEnd = false;                  // If the end of the rope is attached to a wall
    private float SegmentLength = 4f;                   // The length of one rope segment (see RopeSegment.tscn for the length )
    private PackedScene RopeSegment;                    // Variable to hold the rope piece scene
    private RopeSegment RopeStart;                      // The rope start segment
    private RopeSegment RopeEnd;                        // The rope end segment
    private PinJoint2D RopeStartPinJoint;               // The pinjoint for the rope start segment
    private PinJoint2D RopeEndPinJoint;                 // The pinjoint for the rope end segment
    private List<Vector2> RopePointsLine2D;             // The Line2D points for drawing the rope
    private Line2D Line2DNode;                          // The Line2D node
    private float MinDistanceToRopeEndSegment = 4.0f;   // The minimum distance to the final rope segment
    private AudioStreamPlayer RopeSwingLeftSfx;         // The rope swing left sound effect
    private AudioStreamPlayer RopeSwingRightSfx;        // The rope swing right sound effect    
    public bool HasEntityAttached = false;              // Flag to keep track of it an entity is attached to it    

    // List with all the rope segments
    public List<RopeSegment> RopeSegments = new List<RopeSegment>();

    // The actual number of rope segments in the rope
    public int ActualNumRopeSegments = 0;
    public override void _Ready()
    {
        RopePointsLine2D = new List<Vector2>();                                 // Create a new list for the Line2D points
        // Load the rope segment scene
        RopeSegment = GD.Load<PackedScene>("res://Scenes/World/InteractableObjects/Rope/RopeSegment.tscn");
        Line2DNode = GetNode<Line2D>("Line2D");                                 // Get the Line2D node
        RopeStart = GetNode<RopeSegment>("RopeStart");                          // Get the rope start node
        RopeEnd = GetNode<RopeSegment>("RopeEnd");                              // Get the rope end node
        RopeStart.Rope = this;                                                  // Set the parent node for the start node to this node
        RopeEnd.Rope = this;                                                    // Set the parent node for the end node to this node
        RopeStartPinJoint = GetNode<PinJoint2D>("RopeStart/Collision/PinJoint");// Get the rope start pinjoint node
        RopeEndPinJoint = GetNode<PinJoint2D>("RopeEnd/Collision/PinJoint");    // Get the rope end pinjoint node
        SpawnRope(RopeStart.GlobalPosition, RopeEnd.GlobalPosition);            // Spawn a new rope between the start and end nodes
        RopeSwingLeftSfx = GetNode<AudioStreamPlayer>("RopeSwingLeftSfx");      // Get the rope swing left sound effect
        RopeSwingRightSfx = GetNode<AudioStreamPlayer>("RopeSwingRightSfx");    // Get the rope swing left sound effect        
    }

    public void SpawnRope(Vector2 ropeStartPos, Vector2 ropeEndPos)
    {
        // If the setting for the rope has a static end
        if (StaticRopeEnd)
        {
            RopeEnd.Mode = RigidBody2D.ModeEnum.Static;                     // Set the rope end rigidbody 2d node mode to static
        }
        // If it's a loose hanging rope (the end is not static)
        else
        {
            RopeEnd.Mode = RigidBody2D.ModeEnum.Rigid;                      // Set the rope end rigidbody 2d node mode to rigid
        }
        var dist = ropeStartPos.DistanceTo(ropeEndPos);                     // Get the distance between the start and end node
        var numSegmentsEstimate = (int)Mathf.Ceil(dist / SegmentLength);      // Esitmate the number of segments on the rope
        var rotationAngle = (ropeEndPos - ropeStartPos).Angle() - Mathf.Pi / 2; // Get the rotation angle

        RopeStart.IndexInArray = 0;                                         // Set the start node index in the array to 0
        ropeStartPos = RopeStartPinJoint.GlobalPosition;                    // Set the rope start position to the pinjoint position
        ropeEndPos = RopeEndPinJoint.GlobalPosition;                        // Set the rope end position to the pinjoint position

        // Create the rope
        CreateRope(numSegmentsEstimate, RopeStart, ropeEndPos, rotationAngle);

        // Finally set the rope end index to the actual number of rope segments in the rope
        RopeEnd.IndexInArray = ActualNumRopeSegments;
    }
    private void CreateRope(int numSegmentsEstimate, RigidBody2D parent, Vector2 RopeEnd, float rotationAngle)
    {
        // Clear the rope segments list
        RopeSegments.Clear();

        // Loop through all the rope segments
        for (int i = 0; i < numSegmentsEstimate; ++i)
        {
            // Add a new segment to the rope (parent will now reference the latest segment)
            parent = AddRopeSegment(parent, i, rotationAngle);
            // Give the rope segment a name with the current index
            parent.Name = "RopePiece" + i;
            // Add the rope segment to the list of rope segments
            RopeSegments.Add((RopeSegment)parent);
            // Get the joint position of the parent
            var jointPos = parent.GetNode<PinJoint2D>("Collision/PinJoint").GlobalPosition;

            // If the distance between the joint position and the rope end is less than the minimum distance to the end segment
            if (jointPos.DistanceTo(RopeEnd) < MinDistanceToRopeEndSegment)
            {
                // Break out of the for loop
                break;
            }
        }
        // Get the actual number of rope segments the rope has
        ActualNumRopeSegments = RopeSegments.Count - 1;
        // Connect the end pin joint NodeA to the rope end
        RopeEndPinJoint.NodeA = this.RopeEnd.GetPath();
        // Connect the end joint NodeB to the last rope piece
        RopeEndPinJoint.NodeB = RopeSegments[ActualNumRopeSegments].GetPath();
    }

    public RopeSegment AddRopeSegment(Node parent, int id, float rotationAngle)
    {
        PinJoint2D pinJoint = parent.GetNode("Collision/PinJoint") as PinJoint2D;   // Get the pinJoint of the previous rope segment
        var segment = RopeSegment.Instance() as RopeSegment;                        // Instance a new rope segment
        segment.GlobalPosition = pinJoint.GlobalPosition;                           // Set the start of it to the joint position
        segment.Rotation = rotationAngle;                                           // Set the rotation angle
        segment.Rope = this;                                                        // Set the rope the segment is part of
        segment.IndexInArray = id;                                                  // Set which index in the array the segment has

        // If the rope is attached in both start end end
        if (StaticRopeEnd)
        {
            segment.Mass = 30.0f;           // lower the mass of the rope segment
            segment.GravityScale = 2.0f;    // lower the gravity scale
        }

        AddChild(segment);                  // Add the segment as a childnode to the rope node
        pinJoint.NodeA = parent.GetPath();  // Connect the pin joint node A to the parent
        pinJoint.NodeB = segment.GetPath(); // Connect the pin joint node B to the new segment
        pinJoint.Bias = 0.9f;               // Set the pin joint bias to 0.9
        pinJoint.Softness = 0.003f;         // Set the rope softness to 0.003
        return segment;                     // Return the new rope segment
    }
    private void GetLine2DRopePoints()
    {
        RopePointsLine2D.Clear();                                   // Clear the Line2D points

        RopePointsLine2D.Add(RopeStartPinJoint.GlobalPosition);     // Add the rope start position

        // Loop through the rope segments
        foreach (var segment in RopeSegments)
        {
            RopePointsLine2D.Add(segment.GlobalPosition);           // Add the segment position to the rope line points list
        }
        RopePointsLine2D.Add(RopeEndPinJoint.GlobalPosition);       // add the rope end position
        Line2DNode.Points = RopePointsLine2D.ToArray();             // Set the Line2D points
    }
    public void PlayRopeSound()
    {
        // If it's a static rope or doesn't have an entity attached to it, return out of the method
        if (StaticRopeEnd || !HasEntityAttached) return;

        // If the end pin joint is to the left of the start pin joint
        if (RopeEndPinJoint.GlobalPosition < RopeStartPinJoint.GlobalPosition)
        {
            // And the RopeSwingLeftSfx sound effect isn' already playing 
            if (!RopeSwingLeftSfx.Playing)
            {
                RopeSwingLeftSfx.Play();    // Play the RopeSwingLeftSfx sound effect
            }
        }
        // If the end pin joint is to the right of the start pin joint
        else if (RopeEndPinJoint.GlobalPosition > RopeStartPinJoint.GlobalPosition)
        {
            // And the RopeSwingRightSfx sound effect isn' already playing 
            if (!RopeSwingRightSfx.Playing)
            {
                RopeSwingRightSfx.Play();   // Play the RopeSwingRightSfx sound effect
            }
        }
    }
    public override void _Process(float delta)
    {
        PlayRopeSound();            // Play the rope sound
        GetLine2DRopePoints();      // Update the Line2D rope point positions
    }
}
