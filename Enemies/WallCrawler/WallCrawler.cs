using Godot;
using System;

public class WallCrawler : KinematicBody2D, ITakeDamage
{
    [Export]
    private float Speed = 100.0f;                   // The speed of the crawler
    private Vector2 SnapVector = Vector2.Zero;      // The vector to snap the crawler to the wall with    
    private Vector2 Velocity = Vector2.Right;       // Initial direction is right, but will be overridden by the CrawlDirection
    private float TargetRotationAngle;              // The target rotation angle to rotate towards
    [Export]
    private float TurnSpeed = 15f;                  // How fast the crawler turns in the corners    
    [Export(PropertyHint.Enum, "Right, Left")]      // Create a drop-down for choosing direction
    private int CrawlDirection = 0;                 // The direction to crawl in. 0 = right, 1 = left    
    private AnimatedSprite Animation;               // The sprite animations
    private AudioStreamPlayer CrawlerTakeDamageSfx; // The crawler take damage sound effect 
    private AudioStreamPlayer CrawlerDieSfx;        // The crawler die sound effect    
    private AudioStreamPlayer SwordImpactSfx;       // The sword impact sound effect    
    private ShaderMaterial ShaderMaterial;          // The material shader for the crawler
    private Timer HurtTimer;                        // The hurt timer
    [Export]
    private float Health = 4;                       // The health of the crawler    
    private CloudExplosion CloudExplosionAnim;      // The cloud explosion animation, used when the crawler dies    

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        TargetRotationAngle = Velocity.Angle();                                     // Initialize the target rotation angle to the same as the velocity angle
        InitCrawlerDirection();                                                     // Initialize the crawler direction, based on what is selected in the drop-down
        Animation = GetNode<AnimatedSprite>("AnimatedSprite");                      // Get the animated sprite node
        CrawlerTakeDamageSfx = GetNode<AudioStreamPlayer>("CrawlerTakeDamageSfx");  // Get the crawler hurt sound effect reference
        CrawlerDieSfx = GetNode<AudioStreamPlayer>("CrawlerDieSfx");                // Get the crawler die sound effect reference        
        SwordImpactSfx = GetNode<AudioStreamPlayer>("SwordImpactSfx");              // Get the sword impact sound effect reference        
        ShaderMaterial = Animation.Material as ShaderMaterial;                      // Get the shader material
        HurtTimer = GetNode<Timer>("HurtTimer");                                    // Get the hurt timer        
        CloudExplosionAnim = GetNode<CloudExplosion>("CloudExplosion");             // Get the cloud explosion animation        
    }
    private void InitCrawlerDirection()
    {
        // If the crawler direction is set to Right
        if (CrawlDirection == 0)
        {
            Velocity = Vector2.Right;   // Set velocity vector to: Right
        }
        // If the crawler direction is set to Left
        else if (CrawlDirection == 1)
        {
            Velocity = Vector2.Left;    // Set velocity vector to: Left
        }
    }
    private void SetVelocitySnapAndRotation(KinematicCollision2D collision)
    {
        // If the crawler is moving to the right
        if (CrawlDirection == 0)
        {
            Velocity = collision.Normal.Rotated(Mathf.Pi / 2);  // Set velocity forward direction to point along the direction of the wall
            TargetRotationAngle = Velocity.Angle();             // Set target rotation to the Velocity angle
            Animation.FlipH = true;                             // Flip the animation so the legs move according to the direction            
        }
        // If the crawler is moving to the left
        else if (CrawlDirection == 1)
        {
            Velocity = -collision.Normal.Rotated(Mathf.Pi / 2); // Set velocity forward direction to point along the direction of the wall
            TargetRotationAngle = Velocity.Angle() + Mathf.Pi;  // Set target rotation to the Velocity angle + rotate sprite 180 degrees
            Animation.FlipH = false;                            // Flip the animation so the legs move according to the direction            
        }
        SnapVector = collision.Normal.Rotated((Mathf.Pi));      // Get the Snap vector - The direction pointing down towards the wall
    }
    private void RotateTowardsTargetVector(float delta)
    {
        // Linearly interpolate the rotation angle towards the target rotation angle, using the LerpAngle() method
        Rotation = Mathf.LerpAngle(Rotation, TargetRotationAngle, TurnSpeed * delta);
    }
    public override void _PhysicsProcess(float delta)
    {
        var collision = MoveAndCollide(Velocity * Speed * delta);   // Move the wall crawler   
        RotateTowardsTargetVector(delta);                           // Rotate the crawler towards the target vector        
        if (collision != null)
        {
            SetVelocitySnapAndRotation(collision);
        }
        else
        {
            // If the crawler is not on the floor
            if (!IsOnFloor())
            {
                Velocity += SnapVector.Normalized();                // Pull the crawler towards the wall
            }
        }
        // If the cloud explosion animation is finised
        if (CloudExplosionAnim.AnimationFinished)
        {
            this.QueueFree();   // Free the wall crawler from memory
        }
    }
    private void OnHurtTimerTimeout()
    {
        ShaderMaterial.SetShaderParam("active", false); // Remove the white hit effect
    }
    public void TakeDamage(float damage, string damagedBy, Vector2? entityPositon)
    {
        // If the crawler already is dead or is damaged by something other than the hero or an Arrow
        if (Health <= 0 || (damagedBy != "Hero" && !damagedBy.Contains("Arrow")))
        {
            return; // Return out of the method
        }
        Health -= damage;                               // Decrease the wallcrawlers health

        ShaderMaterial.SetShaderParam("active", true);  // Display the sprite as white
        HurtTimer.Start();                              // Start the hurt timer
        SwordImpactSfx.Play();                          // Play the sword impact sound effect

        // If the wall crawlers health is less than 0
        if (Health <= 0)
        {
            this.Animation.Visible = false;             // Hide the crawler
            CrawlerDieSfx.Play();                       // Play the crawler die sound effect

            // Play the cloud explosion animation
            CloudExplosionAnim.PlayCloudExplodeAnimation(new Vector2(0.25f, 0.25f), this.Position);
        }
        else
        {
            CrawlerTakeDamageSfx.Play();                // Play the taking damage sound effect
        }
    }
}
