using Godot;
using System;

public class HeroStateInitJump : IHeroState
{
    private bool Initialized = false;           // Flag to keep track if this class has been initialized
    private HeroStateMachine Hero;              // The Hero state machine
    private float JumpForce = -700.0f;          // The strenght of the jump
    private bool ShowDustParticles = true;      // If the dust particles should show or not
    private AudioStreamPlayer JumpLiftOffSfx;   // The jump lift off sound effect
    public IHeroState DoState(HeroStateMachine hero, float delta)
    {
        return InitiateJump(hero, delta);
    }
    public void InitState(HeroStateMachine hero)
    {
        if (!Initialized)
        {
            Initialized = true;
            Hero = hero;
        }
        // Get the jump lift off sound effect node
        JumpLiftOffSfx = Hero.GetNode<AudioStreamPlayer>("SoundEffects/JumpLiftOffSfx");
    }
    public string GetStateName()
    {
        return "StateInitJump";
    }
    public void SetJumpForce(float jumpForce, bool showDustParticles, float xVelocity = 0)
    {
        Hero.HeroMoveLogic.Velocity.x = xVelocity;
        JumpForce = jumpForce;
        ShowDustParticles = showDustParticles;
    }
    private void ResetJumpForceToNormal()
    {
        JumpForce = -700.0f;
    }
    private IHeroState InitiateJump(HeroStateMachine hero, float delta)
    {
        // Disable snap so the hero can jump off from slopes
        hero.HeroMoveLogic.DisableSnap();

        // Apply the jump force to the hero velocity
        hero.HeroMoveLogic.Velocity.y = JumpForce;

        // Set animation to the initiate jump animation
        hero.HeroAnimations.Play("HeroInitJump");

        // If the dust particles should be shown
        if (ShowDustParticles)
        {
            // Instance the dust particles
            hero.HeroParticleEffects.InstanceDustParticles();
        }
        else
        {
            ShowDustParticles = true;
        }

        // Reset jump force to normal
        ResetJumpForceToNormal();

        // Play the jump lift off sound
        JumpLiftOffSfx.Play();

        // If no other state was triggered, continue the initiate jump state
        return hero.StateJump;
    }
}