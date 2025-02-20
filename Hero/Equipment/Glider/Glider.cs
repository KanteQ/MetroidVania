using Godot;
using System;

public class Glider : Node2D
{
    private AnimatedSprite GliderAnimation;             // The glider animation node
    public string LastPlayedAnimation = string.Empty;   // Name of the last glider animation that finished playing
    public bool FlipHorizontal = false;                 // If the glider should be drawed flipped horizontally

    private AudioStreamPlayer GliderInAirSfx;           // The in air glider sound effect
    private AudioStreamPlayer GliderOpenSfx;            // The open glider sound effect
    private AudioStreamPlayer GliderCloseSfx;           // The close glider sound effect
    public override void _Ready()
    {
        InitGlider();       // Initialize the glider
    }
    private void InitGlider()
    {
        // Get the glider animated sprite node
        GliderAnimation = GetNode<AnimatedSprite>("./AnimatedGlider");

        // If the node was not found
        if (GliderAnimation == null)
        {
            // Print what went wrong to the console
            GD.PrintErr("Error in Glider.cs - InitGlider(): Could not find the 'AnimatedGlider' node!");
            return;
        }
        // Listen for the animation_finished signal, and run the AnimationDone method when signal triggers
        GliderAnimation.Connect("animation_finished", this, nameof(AnimationDone));

        // Get the glider sound effect nodes
        GliderInAirSfx = GetNode<AudioStreamPlayer>("SoundEffects/GliderInAirSfx");
        GliderCloseSfx = GetNode<AudioStreamPlayer>("SoundEffects/GliderCloseSfx");
        GliderOpenSfx = GetNode<AudioStreamPlayer>("SoundEffects/GliderOpenSfx");
    }
    private void AnimationDone()
    {
        // Update the last played animation
        LastPlayedAnimation = GliderAnimation.Animation;
    }
    public override void _Process(float delta)
    {
        GliderAnimation.FlipH = FlipHorizontal;         // Update if the glider should be drawed flipped

        // If the close animation just finished playing
        if (LastPlayedAnimation.Equals("GliderClose"))
        {
            Visible = false;                            // Hide the glider
            LastPlayedAnimation = string.Empty;         // Reset last played animation to an empty string
        }
        // If the open animation just finished playing
        if (LastPlayedAnimation.Equals("GliderOpen"))
        {
            GliderAnimation.Play("Glide");              // Play the glide animation
            LastPlayedAnimation = string.Empty;         // Reset last played animation to an empty string   
            GliderInAirSfx.Play();                      // Play the glider in air sound effect                   
        }
    }
    public void OpenGlider()
    {
        Visible = true;                             // Make the glider animation visible
        GliderAnimation.Play("GliderOpen");         // Play the open animation
        GliderOpenSfx.Play();                       // Play the open glider sound effect
    }
    public void CloseGlider(bool playSoundEffect = true)
    {
        GliderInAirSfx.Stop();                      // Stop the in air glider sound effect
        GliderAnimation.Play("GliderClose");        // Play the close animation
        // If the sound should be played
        if (playSoundEffect)
        {
            GliderCloseSfx.Play();                  // Play the close glider sound effect
        }
    }
}
