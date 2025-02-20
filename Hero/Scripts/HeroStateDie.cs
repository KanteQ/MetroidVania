using Godot;
using System;

public class HeroStateDie : IHeroState
{
    private bool Initialized = false;           // Flag to keep track of if the state has been initialized
    private HeroStateMachine Hero;              // The hero state machine reference
    private IHeroState PreviousState;           // The previous state of the hero
    private AudioStreamPlayer DieSfx;           // The Hero die sound effect
    private AudioStreamPlayer GroundImpactSfx;  // The Ground impact sound effect

    public string GetStateName()
    {
        return "StateDie";  // Return the name of the state
    }

    public void InitState(HeroStateMachine hero)
    {
        if (!Initialized)
        {
            Initialized = true;                                                                 // Flag that the state has been initialized
            Hero = hero;                                                                        // Set the hero reference
            DieSfx = Hero.GetNode<AudioStreamPlayer>("SoundEffects/DieSfx");                    // Get the Hero death sound effect
            GroundImpactSfx = Hero.GetNode<AudioStreamPlayer>("SoundEffects/GroundImpactSfx");  // Get the Hero ground impact sound effect
        }
    }

    private void PlayCloseCircleTransition()
    {
        // Play the circle transition effect
        Hero.HeroCircleTransition.PlayCloseCircle(Hero.GlobalPosition, Hero.HeroAnimations.FlipH, "res://Scenes/Main.tscn", showHero: true);
    }
    public void SetPreviousState(IHeroState previousState)
    {
        PreviousState = previousState;  // Set the previous state the hero was in.
    }
    public IHeroState DoState(HeroStateMachine hero, float deltatime)
    {
        return Die();
    }
    private IHeroState Die()
    {
        Hero.HeroMoveLogic.MovementDisabled = true;         // Disable movement
        Hero.HeroMoveLogic.IsMoving = false;                // Flag that the hero no longer is moving

        // If the hero isn't dead
        if (!Hero.HeroSingletonVariables.IsDead)
        {
            if (Hero.IsOnFloor())
            {
                Hero.HeroSingletonVariables.IsDead = true;      // Flag that the hero has died - prevents other events from triggering other states.
                DieSfx.Play();                                  // Play the die sound effect
                PlayCloseCircleTransition();                    // Play the close circle transition
                GroundImpactSfx.Play();                         // Play the ground impact sound effect
            }
        }

        // If the Hero is in a crouching position
        if (PreviousState == Hero.StateSlide || PreviousState == Hero.StateSlideStandUp)
        {
            Hero.HeroAnimations.Play("HeroDeathCrouching"); // Play the HeroDeathCrouching animation
        }
        // If the Hero is standing
        else
        {
            Hero.HeroAnimations.Play("HeroDeath");          // Play the normal hero death animation
        }

        // If the frame is 2 = when the body impacts the ground
        if (Hero.HeroAnimations.Frame == 2 && Hero.IsOnFloor())
        {
            // If the ground impact sound effect isn't playing
            if (!GroundImpactSfx.Playing)
            {
                GroundImpactSfx.Play();                     // Play the ground impact sound effect
            }
        }
        Hero.HeroMoveLogic.InterpolateHorizontalMovementTowardsStop();

        return Hero.StateDie;                               // Continue in the die state
    }
}
