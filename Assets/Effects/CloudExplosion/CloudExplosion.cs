using Godot;
using System;

public class CloudExplosion : Node2D
{
    public bool AnimationFinished = false;  // Flag to keep track of if the animation is finished

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.SetAsToplevel(true);   // Set the effect as top level, so it won't inherity any properties from its parent        
    }
    public void PlayCloudExplodeAnimation(Vector2 scale, Vector2 position)
    {
        AnimationFinished = false;                                      // Flag that the animation isn't finished
        this.Position = position;                                       // Set the positon
        var CloudAnimation = GetNode<AnimatedSprite>("AnimatedSprite"); // Get the animated sprite node

        // If the scale passed in larger than 0
        if (scale > Vector2.Zero)
        {
            this.Scale = scale;         // Apply the new scale
        }
        // If 0 or a negative number was passed in
        else
        {
            this.Scale = Vector2.One;   // Set scale to 1.0
        }
        CloudAnimation.Frame = 0;       // Reset frame to 0
        this.Visible = true;            // Set the cloud explosion node visible
        CloudAnimation.Play("Explode"); // Play the cloud explode animation
    }

    private void OnAnimatedSpriteAnimationFinished()
    {
        this.Visible = false;       // Hide the cloud animation node
        AnimationFinished = true;   // Flag that the animation is finished
    }
}
