using Godot;
using System;

public class HeroStateLedgeClimb : Timer, IHeroState
{
    private bool Initialized = false;                   // Boolean to keep track of if the class has been initialized
    private bool LedgeClimbTimerHasTimedOut = false;    // If the timer has timed out (climb is finished)
    private HeroStateMachine Hero;                      // The hero state machine
    private bool LedgeClimbInitiated = false;           // If the hero has started to perform the ledge-climb
    private AudioStreamPlayer ClimbLedgeSfx;            // The ledge climb sound effect    

    public IHeroState DoState(HeroStateMachine hero, float deltatime)
    {
        InitState(hero);
        return LedgeClimb(deltatime);
    }
    public void InitState(HeroStateMachine hero)
    {
        if (!Initialized)
        {
            Hero = hero;                    // Set the hero reference
            ConnectLedgeClimbTimerSignal(); // Connect the timer signal
            Initialized = true;             // Set the class as initialized
        }
        // Get the GrabLedge sound effect
        ClimbLedgeSfx = Hero.GetNode<AudioStreamPlayer>("SoundEffects/ClimbLedgeSfx");
    }
    public string GetStateName()
    {
        return "StateLedgeClimb";
    }
    private void ConnectLedgeClimbTimerSignal()
    {
        // Whenever the slide timer times out, run the OnSlideTimerTimeout() method
        Hero.HeroTimers.LedgeClimbTimer.Connect("timeout", this, nameof(OnLedgeClimbTimerTimeout));
    }
    private void OnLedgeClimbTimerTimeout()
    {
        Hero.HeroTimers.LedgeClimbTimer.Stop();     // Stop the ledge-climb timer
        LedgeClimbTimerHasTimedOut = true;          // Flag that the ledge climb timer has timed out
    }
    public IHeroState LedgeClimb(float delta)
    {
        CheckLedgeClimbStateInitiated();            // check if the ledge climb state should be initiated
        Hero.HeroAnimations.Play("HeroLedgeClimb"); // Play the ledge climb animation
        PerformClimb(delta);                        // Perform the climb

        // If the ledge-climb timer has timed out
        if (LedgeClimbTimerHasTimedOut)
        {
            LedgeClimbInitiated = false;                                    // Reset that ledge climb is no longer initiated
            Hero.HeroCollisionShapes.ChangeCollisionShapesToStanding();     // Set the collision shapes to standing
            Hero.HeroMoveLogic.GravityDisabled = false;                     // Enable gravity again
            Hero.HeroMoveLogic.MovementDisabled = false;                    // Enable movement again
            return Hero.StateIdle;                                          // return the idle state
        }
        return Hero.StateLedgeClimb;        // If no other state was triggered, continue in the ledge climb state
    }
    private void CheckLedgeClimbStateInitiated()
    {
        // If the slide state is not initiated
        if (!LedgeClimbInitiated)
        {
            LedgeClimbTimerHasTimedOut = false;                     // Reset ledge climb timer timeout state
            Hero.HeroCollisionShapes.DisableAllCollisionShapes();   // Disable all collision shapes while climbing so the hero can pass through tiles
            Hero.HeroTimers.LedgeClimbTimer.Start();                // Start the ledge climb timer
            LedgeClimbInitiated = true;                             // Set ledge climb as initiated
            ClimbLedgeSfx.Play();                                   // Play the climb ledge sound effect            
        }
    }
    private void PerformClimb(float delta)
    {
        // If the hero is facing left (Animations are flipped)       
        if (Hero.HeroAnimations.FlipH)
        {
            Hero.HeroMoveLogic.Velocity.x = -25;    // Push the hero left onto the platform
        }
        // If the hero is facing right (Animations are not flipped)
        else if (!Hero.HeroAnimations.FlipH)
        {
            Hero.HeroMoveLogic.Velocity.x = 25;     // Push the hero right onto the platform
        }
        // Push the hero upwards
        Hero.HeroMoveLogic.Velocity.y = -4300 * delta;
    }
}
