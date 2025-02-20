using Godot;
using System;

public class HeroCircleTransition : Node2D
{
    private AnimationPlayer Animations;     // The Animation player node
    private Node2D CircleTransition;        // The circle transition node
    private Sprite HeroDead;                // The sprite for showing the hero as dead
    private string SceneToLoad = "";        // Next scene to load
    public override void _Ready()
    {
        Visible = false;                                                                    // Hide the transition
        Animations = GetNode<AnimationPlayer>("CircleTransition/ColorRect/AnimationPlayer");// Get the animation player node
        CircleTransition = GetNode<Node2D>("CircleTransition");                             // Get the circle transition node
        HeroDead = GetNode<Sprite>("CircleTransition/HeroDead");                            // Get the hero dead sprite node
    }

    public void PlayCloseCircle(Vector2 position, bool flipHero, string sceneToLoad, bool showHero)
    {
        Visible = true;                                                 // Enable visibiliy
        HeroDead.FlipH = flipHero;                                      // Set the hero sprite direction
        CircleTransition.Position = position -= new Vector2(1280, 720); // Center the circle transition around the position that was passed in
        SceneToLoad = sceneToLoad;                                      // Set which scene to load

        // If the hero should not be shown
        if (!showHero)
        {
            Animations.Play("CloseCircleNoHero");                       // Play the close circle animation without the hero
        }
        // if the hero should be drawn
        else
        {
            Animations.Play("CloseCircle");                             // Play the close circle animation
        }
    }

    public void PlayOpenCircle(Vector2 position)
    {
        Visible = true;                                                 // Enable visibiliy        
        CircleTransition.Position = position -= new Vector2(1280, 720); // Center the circle transition around the position that was passed in
        Animations.Play("OpenCircle");                                  // Play the close circle animation
    }

    private void OnAnimationPlayerAnimationFinished(string animation)
    {
        // If the animation that finished playing contains CloseCircle
        if (animation.Contains("CloseCircle"))
        {
            // And there is a scene to transition to
            if (!string.IsNullOrEmpty(SceneToLoad))
            {
                GetTree().ChangeScene(SceneToLoad);     // Change scene to what the "SceneToLoad" is pointing towards
            }
        }
    }

}
