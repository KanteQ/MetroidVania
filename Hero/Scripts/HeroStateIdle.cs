using Godot;
using System;

public class HeroStateIdle : Timer, IHeroState
{
    private bool Initialized = false;                   // If the class has been initialized
    private HeroStateMachine Hero;                      // The hero state machine

    public IHeroState DoState(HeroStateMachine hero, float delta)
    {
        InitState(hero);
        return Idle(delta);
    }
    public void InitState(HeroStateMachine hero)
    {
        if (!Initialized)
        {
            Initialized = true;     // Flag that the class has been initialized
            Hero = hero;            // Set the hero reference

            // Whenever the pass through platform timer times out, run the OnPassThroughPlatformTimerTimeout() method
            Hero.HeroTimers.PassThroughPlatformTimer.Connect("timeout", this, nameof(OnPassThroughPlatformTimerTimeout));
        }
    }
    public string GetStateName()
    {
        return "StateIdle";
    }

    private void OnPassThroughPlatformTimerTimeout()
    {
        // Restore hero collision with platforms
        Hero.HeroCollisionShapes.TurnOnPassThroughLayerCollision((int)CollisionLayersEnum.PASS_THROUGH_PLATFORM_LAYER);
    }
    private IHeroState Idle(float delta)
    {
        // If the hero is on the ground/floor
        if (Hero.IsOnFloor())
        {
            // Enable snap so the hero will snap to the slopes
            Hero.HeroMoveLogic.EnableSnap();

            // If the player is pressing jump and the down key at the same time
            if (Input.IsActionJustPressed("Jump") && Input.IsActionPressed("Down"))
            {
                // return the slide state
                return Hero.StateSlide;
            }
            if (Input.IsActionPressed("Down"))
            {
                // Pass through a platform if standing on one
                Hero.HeroCollisionShapes.TurnOffPassThroughLayerCollision((int)CollisionLayersEnum.PASS_THROUGH_PLATFORM_LAYER);
                Hero.HeroTimers.PassThroughPlatformTimer.Start();
            }
            // If the jump action is just pressed
            if (Input.IsActionJustPressed("Jump"))
            {
                return Hero.StateInitJump;
            }

            // If the attack action is just pressed
            if (Input.IsActionJustPressed("Attack"))
            {
                return Hero.StateAttack;
            }
            // If the action attack with bow is just pressed, and a bow is equipped
            if (Input.IsActionJustPressed("AttackWithBow") && Hero.HeroSingletonVariables.IsBowEquipped)
            {
                return Hero.StateBowDraw;
            }
            // Set animation to hero idle
            Hero.HeroAnimations.Play("HeroIdle");

            // If the hero is moving
            if (Hero.HeroMoveLogic.IsMoving)
            {
                // Return the hero run state
                return Hero.StateRun;
            }
            // If no other state was triggered, return the idle state
            return Hero.StateIdle;
        }
        // If the hero is not on the ground/floor
        else if (!Hero.IsOnFloor())
        {
            // Return the falling state
            return Hero.StateFall;
        }
        // This line should never be reached, but is here to keep the compiler happy
        return Hero.StateIdle;
    }
}
