using Godot;
using System;

public class HeroStateBowAim : IHeroState
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
        return BowAim(delta);
    }
    public IHeroState BowAim(float delta)
    {
        Hero.HeroAnimations.Play("HeroBowAim");                             // Play the hero bow aim animation
        Hero.HeroEquipment.Bow.Visible = true;                              // Make the bow visible
        Hero.HeroEquipment.Bow.FlipHorizontal = Hero.HeroAnimations.FlipH;  // Update the bow direction

        if (Input.IsActionJustReleased("AttackWithBow"))
        {
            Hero.HeroMoveLogic.OnlyChangeDirectionEnabled = false;          // Unlock Hero movement
            Hero.HeroEquipment.Bow.FireArrow(Hero);                         // Fire the Arrow
            return Hero.StateBowFire;                                       // Return the bow fire state
        }
        return Hero.StateBowAim;
    }
}
