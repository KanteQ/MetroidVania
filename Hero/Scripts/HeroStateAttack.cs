using Godot;
using System;

public class HeroStateAttack : Timer, IHeroState
{
    private bool Initialized;                           // If the class has been initialized
    private bool AttackInProgress = false;              // If an attack is in progress
    private bool AttackTimerHasTimedOut = false;        // If the timer has timed out (attack finished)
    private HeroStateMachine Hero;                      // The hero state machine
    private AudioStreamPlayer SwordSwingSfx;            // The sword swing sound effect
    public IHeroState DoState(HeroStateMachine hero, float delta)
    {
        InitState(hero);        // Initialize the attack state
        return Attack(delta);   // Run the attack method, and return the state outcome
    }
    public void InitState(HeroStateMachine hero)
    {
        // If the class is not yet initialized
        if (!Initialized)
        {
            Hero = hero;                    // Set the hero reference
            ConnectAttackTimerSignal();     // Connect the timer signal
            Initialized = true;             // Set the class as initialized
        }
        // Get the sword swing sound effect node
        SwordSwingSfx = Hero.GetNode<AudioStreamPlayer>("SoundEffects/SwordSwingSfx");
    }
    public string GetStateName()
    {
        return "StateAttack";
    }
    private void ConnectAttackTimerSignal()
    {
        // Whenever the attack timer times out, run the OnAttackTimerTimeout() method
        Hero.HeroTimers.AttackTimer.Connect("timeout", this, nameof(OnAttackTimerTimeout));
    }

    private void OnAttackTimerTimeout()
    {
        Hero.HeroTimers.AttackTimer.Stop();     // Stop the attack timer
        AttackTimerHasTimedOut = true;          // Flag that the attack timer has timed out
    }
    private IHeroState Attack(float delta)
    {
        // If the hero is on the ground
        if (Hero.IsOnFloor())
        {
            PlayAttackAnimation("HeroAttack");    // Play the normal attack animation
        }
        // If the hero is in the air
        else if (!Hero.IsOnFloor())
        {
            PlayAttackAnimation("HeroAttackInAir");   // Play the in-air attack animation
        }
        // If the attack timer has timed out
        if (AttackTimerHasTimedOut)
        {
            AttackInProgress = false;   // Reset the attack in progress flag

            // Disable the Area2DSwordHitbox
            Hero.HeroArea2Ds.DisableArea2DCollision(Hero.HeroArea2Ds.Area2DSwordHitbox);

            // If the hero is on the floor
            if (Hero.IsOnFloor())
            {
                return Hero.StateIdle;  // return the idle state
            }
            // If the hero is in the air
            else if (!Hero.IsOnFloor())
            {
                return Hero.StateFall;  // return the falling state
            }
        }
        // If no other state was triggered, continue in the attack state
        return Hero.StateAttack;
    }
    private void PlayAttackAnimation(string attackAnimation)
    {
        // If an attack is not in progress
        if (!AttackInProgress)
        {
            Hero.HeroAnimations.Frame = 0;              // Reset the animation frame to 0
            AttackTimerHasTimedOut = false;             // Reset attack timer timeout state
            Hero.HeroTimers.AttackTimer.Start();        // Start the attack timer
            AttackInProgress = true;                    // Flag that an attack is in progress
            Hero.HeroAnimations.Play(attackAnimation);  // Play the provided attack animation
            SwordSwingSfx.Play();                       // Play the sword swing sound effect
        }
        else
        {
            // If the hero animation is on the 2nd frame (the large sword swing frame)
            if (Hero.HeroAnimations.Frame == 1)
            {
                // Enable the Area2DSwordHitbox
                Hero.HeroArea2Ds.EnableArea2DCollision(Hero.HeroArea2Ds.Area2DSwordHitbox);
            }
        }
    }
}
