using Godot;
using System;

public class HeroStateJump : IHeroState
{
    private const float CutJumpThreshold = -200.0f;                         // The threshold for cutting a jump short
    private const float JumpForceAfterJumpCutShort = -320.0f;               // The jump force after a jump has been cut short
    private int MaxJumps = 1;                                               // Maximum number of jumps the hero can perform in-air
    private int JumpCount = 1;                                              // The number of jumps that has been performed in-air
    private bool Initialized = false;                                       // If this class has been initialized    
    private HeroStateMachine Hero;                                          // The Hero state machine    
    public IHeroState DoState(HeroStateMachine hero, float delta)
    {
        InitState(hero);
        return Jump(delta);
    }
    public string GetStateName()
    {
        return "StateJump";
    }
    public void InitState(HeroStateMachine hero)
    {
        if (!Initialized)
        {
            Initialized = true;
            Hero = hero;
        }
    }
    private IHeroState Jump(float delta)
    {
        // Disable snap so the hero can jump off from slopes
        Hero.HeroMoveLogic.DisableSnap();
        // Set animation to the jump animation
        Hero.HeroAnimations.Play("HeroJump");

        // Cut jump short if the jump button is released
        if (Input.IsActionJustReleased("Jump") && Hero.HeroMoveLogic.Velocity.y < CutJumpThreshold)
        {
            // Set the velocity to the new jump force for when a jump has been cut short
            Hero.HeroMoveLogic.Velocity.y = JumpForceAfterJumpCutShort;
            // Return the falling state
            return Hero.StateFall;
        }
        // If the glide action is pressed
        if (Hero.HeroSingletonVariables.SpecialItems.Glider && Input.IsActionPressed("Glide"))
        {
            Hero.HeroAnimations.Play("HeroFall");       // Set animation to the fall animation, so it looks like the hero is holding the glider
            Hero.HeroEquipment.Glider.OpenGlider();     // Open the glider
            return Hero.StateGlide;                     // Return the glide state
        }

        // If the attack action is just pressed
        if (Input.IsActionJustPressed("Attack"))
        {
            return Hero.StateAttack;
        }

        // If the hero is not on the ground/floor
        if (!Hero.IsOnFloor())
        {
            // If the hero velocity is less than 0 (going upwards)
            if (Hero.HeroMoveLogic.Velocity.y < 0)
            {
                // Corner correct the jump if needed
                CornerCorrectTheJump(delta);

                // Return the jump state
                return Hero.StateJump;
            }
            // If the hero velocity is greater than 0 (falling)
            if (Hero.HeroMoveLogic.Velocity.y > 0)
            {
                // Return the falling state
                return Hero.StateFall;
            }
        }
        else if (Hero.IsOnFloor())
        {
            ResetJumpCounter();         // Reset the jump counter
            return Hero.StateIdle;
        }
        // This line should never be reached, but is here to keep the compiler happy
        return Hero.StateJump;
    }
    public void SetMaxJumps(int numJumps)
    {
        this.MaxJumps = numJumps;       // Set the new number of max jumps
    }
    public void ResetJumpCounter()
    {
        this.JumpCount = 0;     // Reset the jump counter to zero
    }
    public bool CanJumpAgainInAir()
    {
        JumpCount++;    // Increase the jump counter

        // If the current number of jumps is less than the maximum jumps that can be performed in-air
        if (JumpCount < MaxJumps)
        {
            return true;        // Return true
        }
        // if not, return false
        return false;
    }
    public bool CanWallJump()
    {
        // if there is a wall left to the player and the hero is facing the right (non-flipped sprite)
        if (Hero.HeroRaycasts.LeftWallRay.IsColliding() && !Hero.HeroAnimations.FlipH)
        {
            return true;        // return true, the hero can walljump
        }
        // if there is a wall right to the player and the hero is facing the left (flipped sprite)
        if (Hero.HeroRaycasts.RightWallRay.IsColliding() && Hero.HeroAnimations.FlipH)
        {
            return true;        // return true, the hero can walljump
        }
        return false;           // Return false, there is no wall next to the player, so he can't walljump
    }
    public bool CanHeroJumpBuferJump()
    {
        if (!Hero.IsOnFloor()                                    // If the hero is not on the floor
        && Hero.HeroRaycasts.JumpBufferRayCast.IsColliding()    // and the jump buffer raycast is colliding
        && Hero.HeroMoveLogic.Velocity.y > 0)                   // and the hero is falling
        {
            return true;    // return true, the hero can jump-buffer jump
        }
        return false;       // return false, the hero can't jump-buffer jump
    }

    private void CornerCorrectTheJump(float delta)
    {
        // If the left raycast is colliding, but not the middle one
        if (Hero.HeroRaycasts.CornerCorrLeftRayCast.IsColliding()
        && !Hero.HeroRaycasts.CornerCorrMiddleRayCast.IsColliding())
        {
            // If the hero is not next to the wall - checked by making sure that the head ray is not colliding
            if (!Hero.HeroRaycasts.LedgeGrabRayTileHead.IsColliding())
            {
                Hero.Translate(new Vector2(400 * delta, 0));    // Translate the positon to the right
            }
        }
        // If the right raycast is colliding, but not the middle one
        if (Hero.HeroRaycasts.CornerCorrRightRayCast.IsColliding()
        && !Hero.HeroRaycasts.CornerCorrMiddleRayCast.IsColliding())
        {
            // If the hero is not next to the wall - checked by making sure that the head ray is not colliding
            if (!Hero.HeroRaycasts.LedgeGrabRayTileHead.IsColliding())
            {
                Hero.Translate(new Vector2(-400 * delta, 0));    // Translate the positon to the left
            }
        }
    }
}
