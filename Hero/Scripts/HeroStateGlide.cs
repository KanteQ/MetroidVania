using Godot;
using System;

public class HeroStateGlide :  Node2D, IHeroState
{
    public float GliderGravity = 100;                   // Glider gravity
    public IHeroState DoState(HeroStateMachine hero, float deltatime)
    {
         return Glide(hero);     // run the Glide() method, and return the state outcome
    }
    public void InitState(HeroStateMachine hero)
    {
        // Empty, not needed here
    }
    public string GetStateName()
    {
        return "StateGlide";
    }
    private IHeroState Glide(HeroStateMachine hero)
    {
        StopUpwardGliding(hero);    // Stop upward gliding
        CatchTheWind(hero);         // Catch the wind

        // Flip the glider in the same direction as the hero
        hero.HeroEquipment.Glider.FlipHorizontal = hero.HeroAnimations.FlipH;

        if(Input.IsActionJustReleased("Glide"))
        {
            hero.HeroAnimations.Play("HeroFall");       // Set hero animation to fall
            hero.HeroEquipment.Glider.CloseGlider();    // Close the glider
            return hero.StateFall;                      // Set state to falling
        }
        // If the Hero is on the ground/floor
        if(hero.IsOnFloor())
        {
            hero.HeroEquipment.Glider.CloseGlider();    // Close the glider
            return hero.StateIdle;                      // Return the idle state
        }

        // If the hero is next to a ledge
        if(hero.StateLedgeGrab.CanHeroLedgeGrab())
        {
            hero.HeroEquipment.Glider.CloseGlider();    // Close the glider
            return hero.StateLedgeGrab;                 // Return the ledge grab state
        }

        // If no other state was triggered, continue in the glide state
        return hero.StateGlide;
    }
    private void StopUpwardGliding(HeroStateMachine hero)
    {
        // Make sure that the hero cannot glide upwards
        if(hero.HeroMoveLogic.Velocity.y <= 40)
        {
            // Liniary interpolate down towards zero
            hero.HeroMoveLogic.Velocity.y = Mathf.Lerp(hero.HeroMoveLogic.Velocity.y, 40, 0.1f);
        }
    }
    private void CatchTheWind(HeroStateMachine hero)
    {
        // If the current gravity is greater than the glider gravity
        if(hero.HeroMoveLogic.Velocity.y > GliderGravity)
        {
            // Liniary interpolate the falling velocity to match the hero glider gravity
            hero.HeroMoveLogic.Velocity.y = Mathf.Lerp(hero.HeroMoveLogic.Velocity.y, GliderGravity, 0.15f);
        }
    }

}
