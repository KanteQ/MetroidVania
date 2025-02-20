using Godot;
using System;

public class HeroStateFall : Timer, IHeroState
{
    private bool Initialized = false;                   // If the class has been initialized
    private bool CoyoteTimeTimerHasTimedOut = false;   // If the coyote-time timer has timed out
    public bool CanCoyoteTimeJump = false;             // If the hero can perform a coyote-time jump
    private HeroStateMachine Hero;                      // the hero state machine
    private AudioStreamPlayer JumpLandSfx;              // The sound effect for landing on the ground after a jump
    private AudioStreamPlayer GrabLedgeSfx;             // The sound effect for grabbing a ledge    
    public IHeroState DoState(HeroStateMachine hero, float delta)
    {
        InitState(hero);        // Initialize the state
        return Fall(delta);     // run the fall method
    }
    public void InitState(HeroStateMachine hero)
    {
        if (!Initialized)
        {
            Initialized = true;     // Flag that the class has been initialized
            Hero = hero;            // Set the hero reference

            // Whenever the coyote-time timer times out, run the OnCoyoteTimeTimerTimeout() method
            Hero.HeroTimers.CoyoteTimeTimer.Connect("timeout", this, nameof(OnCoyoteTimeTimerTimeout));
        }
        // Get the JumpLand sound effect
        JumpLandSfx = Hero.GetNode<AudioStreamPlayer>("SoundEffects/JumpLandSfx");

        // Get the GrabLedge sound effect
        GrabLedgeSfx = Hero.GetNode<AudioStreamPlayer>("SoundEffects/GrabLedgeSfx");
    }
    public string GetStateName()
    {
        return "StateFall";
    }
    private void OnCoyoteTimeTimerTimeout()
    {
        CanCoyoteTimeJump = false;  // Reset coyote-time jump flag
    }
    private IHeroState Fall(float delta)
    {
        // Reset movement to default
        Hero.HeroMoveLogic.ResetMovementToDefault();

        // Enable snapping so the Hero will be able to walk on slopes
        Hero.HeroMoveLogic.EnableSnap();

        // Reset the collisionshapes to standing
        Hero.HeroCollisionShapes.ChangeCollisionShapesToStanding();

        // Play the hero falling animation
        Hero.HeroAnimations.Play("HeroFall");

        // If the attack action is just pressed
        if (Input.IsActionJustPressed("Attack"))
        {
            return Hero.StateAttack;
        }

        // If the hero is falling next to an ledge
        if (Hero.StateLedgeGrab.CanHeroLedgeGrab())
        {
            GrabLedgeSfx.Play();            // Play the grab ledge sound effect
            return Hero.StateLedgeGrab;     // Return the ledge grab states
        }

        // If the glide action is pressed
        if (Hero.HeroSingletonVariables.SpecialItems.Glider && Input.IsActionPressed("Glide"))
        {
            Hero.HeroEquipment.Glider.OpenGlider();     // Open the glider
            return Hero.StateGlide;                     // Return the glide state
        }

        // If the Hero is landing on the ground/floor
        if (Hero.IsOnFloor())
        {
            // Reset the jump counter so the hero can perform multiple jumps in the air again
            Hero.StateJump.ResetJumpCounter();

            // Play the landing sound effect
            JumpLandSfx.Play();

            // Draw the landing particle effect
            Hero.HeroParticleEffects.InstanceDustLandingParticles();

            // If the Hero is moving
            if (Hero.HeroMoveLogic.IsMoving)
            {
                return Hero.StateRun;
            }
            // if not, return the idle state
            return Hero.StateIdle;
        }

        // If the jump action is just pressed
        if (Input.IsActionJustPressed("Jump"))
        {
            if (CanCoyoteTimeJump                            // If the hero can perform a coyote-time jump
            || Hero.StateJump.CanHeroJumpBuferJump()        // or the hero can perform a jump-buffer jump
            || Hero.StateJump.CanWallJump()                 // or the hero can perform a wall jump
            || Hero.StateJump.CanJumpAgainInAir())          // of if the hero can jump in the air again
            {
                CanCoyoteTimeJump = false;                  // reset coyote-time flag
                return Hero.StateInitJump;                  // Return the init jump state
            }
        }

        // If no other state was triggered, continue the falling state
        return Hero.StateFall;
    }
    public void HeroPassedOverAnEdgeStartCoyoteTimeTimer()
    {
        Hero.StateFall.CanCoyoteTimeJump = true;        // Set can coyote-time jump to true
        Hero.HeroTimers.CoyoteTimeTimer.Start();        // Start the coyote-time timer
    }
}
