using Godot;
using System;

public class HeroMoveLogic
{
    private float Gravity = 1500;                                       // Gravity strength pulling the Hero towards the ground
    private float MovementAcceleration = 20;                            // The movement acceleration of the hero
    public float MaxMovementSpeed = 280;                               // The max movement speed of the hero
    private float Friction = 1.0f;                                      // Friction for horizontal movement on the ground
    public Vector2 SnapVector;                                          // Keeps the Hero attached to slopes. Set this to zero when jumping
    public Vector2 Velocity = Vector2.Zero;                             // The direction the Hero is moving in
    private HeroStateMachine Hero;                                      // The hero state machine
    public bool MovementDisabled = false;                               // Flag to control of movement should be disabled or not
    public bool HorizontalMovementDisabled = false;                     // Flat to control horizontal movement on/off
    public bool GravityDisabled = false;                                // Flag to control of gravity should be disabled or not
    public bool IsMoving;                                               // Boolean to keep track of if the hero is moving or not
    public bool OnlyChangeDirectionEnabled = false;                     // If only changing the hero direction is allowed

    public HeroMoveLogic(HeroStateMachine hero)
    {
        Hero = hero;
    }
    public void ApplyKnockBackForce()
    {
        // If the position of where the damage came from isn't known
        if (Hero.StateTakeDamage.DamagePosition == Vector2.Inf)
        {
            //And If the hero is facing left
            if (Hero.HeroAnimations.FlipH)
            {
                Velocity.x = 300f;  // Knock the hero to the right
            }
            // And the hero is facing right
            else
            {
                Velocity.x = -300f; // Knock the hero to the left
            }
        }       
        // If the direction of the damage is known
        else
        {
            // And the hero is to the left of where the damage came from
            if (Hero.GlobalPosition >= Hero.StateTakeDamage.DamagePosition)
            {
                // Knockback to the left
                Velocity.x = 300f;
            }
            // And the hero is to the right of where the damage came from
            else if (Hero.GlobalPosition < Hero.StateTakeDamage.DamagePosition)
            {
                // Knockback to the right
                Velocity.x = -300f;
            }
        }         
    }
    public void MoveHero(float delta)
    {
        if (!OnlyChangeDirectionEnabled)
        {
            // Move the Hero according to controller input
            Velocity = Hero.MoveAndSlideWithSnap(Velocity, SnapVector, Vector2.Up, stopOnSlope: true);
            // Update the global position of the Hero
            Hero.HeroSingletonVariables.GlobalPosition = Hero.GlobalPosition;            
        }

        // if the hero is not moving
        if (!IsMoving)
        {
            if (IsHeroOnSlope() || Hero.IsOnFloor())
            {
                // Linearly interpolate horizontal movement towards stop
                InterpolateHorizontalMovementTowardsStop();
            }
        }
        // If the hero is currently taking damage
        if (Hero.CurrentState == Hero.StateTakeDamage)
        {
            // Apply the knockback force
            ApplyKnockBackForce();
        }        
    }

    public void InterpolateHorizontalMovementTowardsStop()
    {
        // Linearly interpolate horizontal movement towards stop
        Velocity.x = Mathf.Lerp(Velocity.x, 0, Friction);
    }
    public void UpdateMovement(float delta)
    {
        // Get the left and right direction action strength
        float leftDirectionStrength = Input.GetActionStrength("MoveLeft");
        float rightDirectionStrength = Input.GetActionStrength("MoveRight");

        // Only apply movement if it's not disabled
        if (!MovementDisabled)
        {
            // Update velocity
            UpdateVelocity(leftDirectionStrength, rightDirectionStrength);

            // If the horizontal movement is not disabled
            if (!HorizontalMovementDisabled)
            {
                // Update left & right movement
                UpdateRightMovement(leftDirectionStrength, rightDirectionStrength);
                UpdateLeftMovement(leftDirectionStrength, rightDirectionStrength);
            }

            // Update the isMoving state
            UpdateIsMoving(leftDirectionStrength, rightDirectionStrength);
        }
    }
    private void UpdateVelocity(float leftDirectionStrength, float rightDirectionStrength)
    {
        if (HorizontalMovementDisabled)
        {
            return;
        }

        // Pressing both direction keys (left + right) will have the hero continue in the direction he is facing
        if (leftDirectionStrength > 0 && rightDirectionStrength > 0)
        {
            // If the hero is facing left
            if (Hero.HeroAnimations.FlipH)
            {
                Velocity.x -= leftDirectionStrength * MovementAcceleration;
            }
            // Or the hero is facing right
            else
            {
                Velocity.x += rightDirectionStrength * MovementAcceleration;
            }
        }
        else
        {
            // update the velocity accordingly
            Velocity.x += (rightDirectionStrength - leftDirectionStrength) * MovementAcceleration;
        }
        // Clamp the velocity to the maximum movement speed
        Velocity.x = Mathf.Clamp(Velocity.x, -MaxMovementSpeed, MaxMovementSpeed);
    }
    private void UpdateRightMovement(float leftDirectionStrength, float rightDirectionStrength)
    {
        // If the hero is moving to the right
        if (leftDirectionStrength < rightDirectionStrength)
        {
            if (IsHeroOnSlope())
            {
                // If friction is max
                if (Friction == 1.0f)
                {
                    // Run at full speed at all times
                    Velocity.x = MaxMovementSpeed;
                }
            }
            // Draw the hero animations unflipped
            Hero.HeroAnimations.FlipH = false;

            //reset the ledge-grab ray casts
            Hero.HeroRaycasts.LedgeGrabRayCastTileAbove.RotationDegrees = 0;
            Hero.HeroRaycasts.LedgeGrabRayTileHead.RotationDegrees = 0;

            // Disable the rope swing aread2D behind the hero
            Hero.HeroArea2Ds.UpdateArea2DDirections();
        }
    }
    private void UpdateLeftMovement(float leftDirectionStrength, float rightDirectionStrength)
    {
        // If the hero is moving to the left
        if (rightDirectionStrength < leftDirectionStrength)
        {
            // If the hero is on a slope
            if (IsHeroOnSlope())
            {
                // If friction is at max
                if (Friction == 1.0f)
                {
                    // Run at full speed at all times
                    Velocity.x = -MaxMovementSpeed;
                }
            }
            // Draw the hero animations flipped
            Hero.HeroAnimations.FlipH = true;

            // Flip the ledge-grab ray casts
            Hero.HeroRaycasts.LedgeGrabRayCastTileAbove.RotationDegrees = -180;
            Hero.HeroRaycasts.LedgeGrabRayTileHead.RotationDegrees = -180;

            // Disable the rope swing aread2D behind the hero
            Hero.HeroArea2Ds.UpdateArea2DDirections();
        }
    }
    private void UpdateIsMoving(float leftDirectionStrength, float rightDirectionStrength)
    {
        // If there is no movement in the left or the right directions
        if (leftDirectionStrength is 0 && rightDirectionStrength is 0)
        {
            // Set is moving to false
            IsMoving = false;
        }
        // If there is movement
        else
        {
            // Set is moving to true
            IsMoving = true;
        }
    }
    private bool IsHeroOnSlope()
    {
        // If the floor normal x variable is not zero (Horizontal)
        if (Hero.GetFloorNormal().x != 0)
            // The hero is standing on a slope, so return true
            return true;
        // If not, return false
        return false;
    }
    public void ApplyGravity(float delta)
    {
        if (!GravityDisabled)
        {
            // If the current state is the glide state
            if (Hero.CurrentState == Hero.StateGlide)
            {
                Velocity.y += Hero.StateGlide.GliderGravity * delta;    // Apply the glider gravity
                return;                                                 // return out of the method
            }
            // Apply gravity to the hero
            Velocity.y += Gravity * delta;
        }
    }

    public void EnableSnap()
    {
        SnapVector = new Vector2(0, 15);
    }
    public void DisableSnap()
    {
        SnapVector = Vector2.Zero;
    }

    public void ResetMovementToDefault()
    {
        MovementDisabled = false;
        HorizontalMovementDisabled = false;
        GravityDisabled = false;
    }
}
