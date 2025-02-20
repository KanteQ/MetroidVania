using Godot;
using System;

public class HeroStateBowFire : Node2D, IHeroState
{
    private bool Initialized = false;           // Flag to keep track if this class has been initialized
    private HeroStateMachine Hero;              // The Hero state machine

    public void InitState(HeroStateMachine hero)
    {
        if (!Initialized)
        {
            Initialized = true;     // Flag that the class has been initialized
            Hero = hero;            // Set the hero reference

        }
    }
    public string GetStateName()
    {
        return "StateBowAim";
    }
    public IHeroState DoState(HeroStateMachine hero, float delta)
    {
        InitState(hero);
        return BowFire(delta);
    }

    public IHeroState BowFire(float delta)
    {
        Hero.HeroAnimations.Play("HeroBowFire");                            // Play the hero bow fire animation
        Hero.HeroEquipment.Bow.Visible = true;                              // Make the bow visible
        Hero.HeroEquipment.Bow.FlipHorizontal = Hero.HeroAnimations.FlipH;  // Update the bow direction

        // If the hero bow fire animation has finished playing
        if (Hero.HeroAnimations.Frame == 3)
        {
            Hero.HeroAnimations.Frame = 0;                                  // Reset the frame to 0
            Hero.HeroEquipment.Bow.Visible = false;                         // Remove the bow visibility
            return Hero.StateIdle;                                          // Return the idle state
        }
        return Hero.StateBowFire;
    }
}
