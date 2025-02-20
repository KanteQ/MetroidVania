using Godot;
using System;

public class HeroStateBowDraw : IHeroState
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
        return "StateBowDraw";
    }

    public IHeroState DoState(HeroStateMachine hero, float delta)
    {
        InitState(hero);
        return BowDraw(delta);
    }
    public IHeroState BowDraw(float delta)
    {
        Hero.HeroMoveLogic.OnlyChangeDirectionEnabled = true;   // Block hero movement
        Hero.HeroAnimations.Play("HeroBowDraw");                // Play the hero bow draw animation
        Hero.HeroEquipment.Bow.PlayDrawBowSound();              // Play the draw bow sound

        // If the bow draw animation has reached it's final frame (frame 4)
        if (Hero.HeroAnimations.Frame.Equals(4))
        {
            Hero.HeroEquipment.Bow.ResetAimAngle(); // Reset the aim angle to 0
            return Hero.StateBowAim;
        }
        // If no state change was made, continue in the draw bow state
        return Hero.StateBowDraw;
    }
}
