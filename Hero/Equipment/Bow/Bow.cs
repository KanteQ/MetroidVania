using Godot;
using System;

public class Bow : Node2D
{
    private AnimatedSprite BowAnimations;               // The animated sprite node for the bow animations
    private Sprite HeroHead;                            // The hero head
    public bool FlipHorizontal = false;                 // If the bow should be drawed flipped horizontally
    private float BowAimAngleSpeed = 120f;              // The bow aim angle speed
    private float HeroHeadAimSpeed = 70f;               // The hero head aim speed
    private PackedScene Arrow;                          // The Arrow Packed Scene
    private Position2D SpawnArrowPosition;              // Where to spawn the arrow when fired
    private AudioStreamPlayer DrawBowSfx;               // The sound effect for drawing the bow
    private AudioStreamPlayer FireBowSfx;               // The sound effect for firing the bow

    public override void _Ready()
    {
        BowAnimations = GetNode<AnimatedSprite>("BowAnimations");                       // Get the sprite animations node
        HeroHead = GetNode<Sprite>("HeroHead");                                         // Get the hero head sprite node
        // Load the arrow as a packed scene
        Arrow = ResourceLoader.Load<PackedScene>("res://Scenes/Hero/Equipment/Arrow/Arrow.tscn");
        SpawnArrowPosition = GetNode<Position2D>("BowAnimations/ArrowSpawnPosition");   // Load the arrow spawn position node
        DrawBowSfx = GetNode<AudioStreamPlayer>("DrawBowSfx");                          // Get the sound effect for drawing the bow
        FireBowSfx = GetNode<AudioStreamPlayer>("FireBowSfx");                          // Get the sound effect for firing the bow
    }

    public void FireArrow(HeroStateMachine hero)
    {
        FireBowSfx.Play();                          // Play the fire bow sound effect
        Arrow newArrow = Arrow.Instance() as Arrow; // Instantiate a new arrow

        // Set the angle and position of the new arrow
        newArrow.SetAngleAndPosition(BowAnimations.RotationDegrees, SpawnArrowPosition.GlobalPosition, FlipHorizontal);

        hero.AddChild(newArrow);                    // Add the arrow as a child to the hero scene
        BowAnimations.Play("BowFire");              // Play the BowFire animation
    }
    public void PlayDrawBowSound()
    {
        if (!DrawBowSfx.Playing)
        {
            DrawBowSfx.Play();
        }
    }

    public void AimHigher(Node2D node, float aimAngleSpeed, float maxAngle)
    {
        // If the hero is flipped horizontally
        if (FlipHorizontal)
        {
            node.RotationDegrees += aimAngleSpeed;  // Rotate the target node with the aim angle speed

            // If the max rotation degrees is passed
            if (node.RotationDegrees > maxAngle)
            {
                node.RotationDegrees = maxAngle;    // Set the rotation degrees to the max angle
            }
        }
        // If the hero is not flipped horizontally
        else
        {
            node.RotationDegrees -= aimAngleSpeed;  // Rotate the target node with the aim angle speed

            // If the max rotation degrees is passed
            if (node.RotationDegrees < -maxAngle)
            {
                node.RotationDegrees = -maxAngle;   // Set the rotation degrees to the max angle
            }
        }
    }
    public void AimLower(Node2D node, float aimAngleSpeed, float maxAngle)
    {
        // If the hero is flipped horizontally
        if (FlipHorizontal)
        {
            node.RotationDegrees -= aimAngleSpeed;  // Rotate the target node with the aim angle speed

            // If the max rotation degrees is passed
            if (node.RotationDegrees < -maxAngle)
            {
                node.RotationDegrees = -maxAngle;   // Set the rotation degrees to the max angle
            }
        }
        // If the hero is not flipped horizontally
        else
        {
            node.RotationDegrees += aimAngleSpeed;  // Rotate the target node with the aim angle speed

            // If the max rotation degrees is passed
            if (node.RotationDegrees >= maxAngle)
            {
                node.RotationDegrees = maxAngle;    // Set the rotation degrees to the max angle
            }
        }
    }

    public void ResetAimAngle()
    {
        BowAnimations.Frame = 0;                // Reset animation frame to 0
        this.Visible = true;                    // Make the bow visible
        this.BowAnimations.Playing = false;     // Stop any animations from playing
        BowAnimations.RotationDegrees = 0;      // Reset bow rotation angle to 0
        HeroHead.RotationDegrees = 0;           // Rest hero head rotation angle to 0
    }
    public void FlipBowDirection()
    {
        BowAnimations.RotationDegrees = BowAnimations.RotationDegrees * -1; // Flip the bow rotation degrees
        HeroHead.RotationDegrees = HeroHead.RotationDegrees * -1;           // Flip the head rotation degrees

        // If the hero is facing left
        if (FlipHorizontal)
        {
            BowAnimations.Offset = new Vector2(-5, -3);          // Move the offset of the bow
            BowAnimations.Position = new Vector2(5, 3);          // Move the position of the bow
            HeroHead.Position = new Vector2(2, -1);              // Move the hero head position
            SpawnArrowPosition.Position = new Vector2(-14, 0);   // Move the arrow spawn position
        }
        // If the hero is facing right
        else
        {
            BowAnimations.Offset = new Vector2(5, -3);           // Move the offset of the bow
            BowAnimations.Position = new Vector2(-5, 3);         // Move the position of the bow
            HeroHead.Position = new Vector2(-2, -1);             // Move the hero head position
            SpawnArrowPosition.Position = new Vector2(14, 0);    // Move the arrow spawn position
        }
    }
    private void UpdateFacingDirection()
    {
        // If the hero direction changed
        if (BowAnimations.FlipH != FlipHorizontal)
        {
            FlipBowDirection();                    // Flip the direction of the bow
        }
        BowAnimations.FlipH = FlipHorizontal;       // Update the direction for the bow
        HeroHead.FlipH = FlipHorizontal;            // Update the direction for the hero head
    }

    public override void _Process(float delta)
    {
        UpdateFacingDirection();
        // If the Up action is just pressed
        if (Input.IsActionPressed("Up"))
        {
            AimHigher(BowAnimations, BowAimAngleSpeed * delta, 90);
            AimHigher(HeroHead, HeroHeadAimSpeed * delta, 40);
        }
        // If the Down action is just pressed
        if (Input.IsActionPressed("Down"))
        {
            AimLower(BowAnimations, BowAimAngleSpeed * delta, 60);
            AimLower(HeroHead, HeroHeadAimSpeed * delta, 30);
        }
    }
}
