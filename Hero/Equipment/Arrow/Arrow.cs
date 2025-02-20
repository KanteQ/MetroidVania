using Godot;
using System;

public class Arrow : KinematicBody2D
{
    [Export]
    private float Speed = 7.0f;                 // The speed of the arrow
    [Export]
    private float Mass = 0.30f;                 // The mass of the arrow
    [Export]
    public float ArrowGravity = 10.0f;          // The gravity for the arrow
    private Vector2 Velocity = Vector2.Zero;    // The arrow velocity
    private Timer LifeTimer;                    // Life timer for the arrow
    private bool collided = false;              // If the arrow has collided
    private AudioStreamPlayer ArrowImpactSfx;   // The arrow impact sound effect

    private Hitbox ArrowHitBox;                 // The arrow hitbox        

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        LifeTimer = GetNode<Timer>("LifeTimer");                        // Get thte life timer for the arrow
        LifeTimer.Start();                                              // Start the life timer
        ArrowImpactSfx = GetNode<AudioStreamPlayer>("ArrowImpactSfx");  // Ge the arrow impact sound effect reference      
        ArrowHitBox = GetNode<Hitbox>("Hitbox");                        // Get the arrow hitbox node          
    }

    public void SetAngleAndPosition(float angle, Vector2 spawnPosition, bool FlipHorizontal)
    {
        this.SetAsToplevel(true);                                       // Set arrow as top level, so it won't inherit transforms from parent
        var angleInRadians = Mathf.Deg2Rad(angle);                      // Convert degrees to radians
        this.Velocity = new Vector2(Speed, 0).Rotated(angleInRadians);  // Set the direction the arrow is going to fly
        // if the hero is facing left
        if (FlipHorizontal)
        {
            // Inverse the direction of the arrow
            this.Velocity = new Vector2(-this.Velocity.x, -this.Velocity.y);
            // Rotate the arrow arround to face the right direction
            angle += 180;
        }
        this.RotationDegrees = angle;                                   // Set the rotation angle
        this.GlobalPosition = spawnPosition;                            // Set the global position to the spawn position
    }
    private void OnLifeTimerTimeout()
    {
        this.QueueFree();   // Free the arrow from memory when the life timer times out
    }
    public override void _PhysicsProcess(float delta)
    {
        // If the arrow has not collided yet
        if (!collided)
        {
            Velocity += Vector2.Down * ArrowGravity * Mass * delta; // update the arrow velocity
            this.Rotation = Velocity.Angle();                       // update the arrow rotation

            var collision = MoveAndCollide(Velocity);               // check if the arrow has collided

            // If the arrow collided
            if (collision != null)
            {
                collided = true;                                    // flag that the arrow has collided
                ArrowImpactSfx.Play();                              // play the arrow impact sound effect
                ArrowHitBox.Monitorable = false;                    // Turn off hitbox monitorable  

                // If the arrow collided with something other than the tilemap
                if (!collision.Collider.GetType().ToString().Equals("Godot.TileMap"))
                {
                    this.QueueFree();   // Free the arrow from memory
                }                                              
            }
        }
    }
}
