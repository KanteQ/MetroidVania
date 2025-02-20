using Godot;
using System;

public class HeroParticleEffects : Node2D
{
    private HeroStateMachine Hero;              // The hero state machine
    private bool HeroParticlesInitialized;      // Flag to keep track of if the particle effects have been properly initialized
    private PackedScene DustParticles;          // Packed scene for the dust particles effect
    private PackedScene DustParticlesLanding;   // Packed scene for the dust particles landing effect

    public HeroParticleEffects()
    {
        // Empty constructor to remove the error message:
        // "Cannot construct temporary MonoObject because the class does not define a parameterless constructor"
    }

    public HeroParticleEffects(HeroStateMachine hero, ref bool initOk)
    {
        Hero = hero;
        initOk = InitParticleEffects();
    }

    private bool InitParticleEffects()
    {
        // Set positive outcome
        HeroParticlesInitialized = true;
        // Load the dust particle effect as a packed scene
        DustParticles = LoadParticleEffectPackedScene("res://Scenes/Particles/Dust/DustParticles.tscn");
        // If the initialization failed, return false
        if(!HeroParticlesInitialized) return false;
        // Load the dust particle landing effect as a packed scene
        DustParticlesLanding = LoadParticleEffectPackedScene("res://Scenes/Particles/Dust/DustParticlesLanding.tscn");
        // If the initialization failed, return false
        if(!HeroParticlesInitialized) return false;

        return true;    // Return true, we could find all the partile effect nodes
    }
    private PackedScene LoadParticleEffectPackedScene(string pathToScene)
    {
        var PackedScene = GD.Load<PackedScene>(pathToScene);    // Try to load the packed scene
        // If the scene wasn't found
        if(PackedScene == null)
        {
            HeroParticlesInitialized = false;                   // Flag that initialization failed
        }
        // return the packed scene (even if it's null)
        return PackedScene;
    }

    public void InstanceDustParticles(bool followParentPosition = false, float scale = 1.0f, Vector2? offset = null)
    {
        // If the particle effects isn't initialzed, return out of the method
        if(!HeroParticlesInitialized) return;
        var dustParticles = DustParticles.Instance() as Particles2D;    // Instance the dust particles effect
        Hero.AddChild(dustParticles);                                   // Add the effect to the hero node
        if(!followParentPosition)                                       // If the particles should not follow the hero position
        {
            dustParticles.SetAsToplevel(true);                          // Set them as top level, so they won't inherit transforms
        }                                                               // from the parent node
        // Get the position for where to place the particles
        dustParticles.GlobalPosition = Hero.GetNode<Position2D>("ParticleEffects/DustParticlesPosition").GlobalPosition;
        dustParticles.Scale = new Vector2(scale, scale);                // Set the scale of the dust particles
        if(Hero.HeroAnimations.FlipH)                                   // If the hero is facing left
        {
            if(Hero.HeroMoveLogic.IsMoving)                             // And is moving
            {
                dustParticles.GlobalPosition += new Vector2(15,0);      // Offset the dust particles with 15 pixels to the right
            }
            if(offset != null)                                          // If an offset is passed in
            {
                dustParticles.GlobalPosition -= (Vector2)offset;        // move the position of the particles with the passed in offset
            }
        }
        else if(!Hero.HeroAnimations.FlipH)                             // if the hero is facing right
        {
            if(Hero.HeroMoveLogic.IsMoving)                             // And is moving
            {
                dustParticles.GlobalPosition -= new Vector2(15,0);      // Offset the dust particles with 15 pixels to the left
            }
            if(offset != null)                                          // If an offset is passed in
            {
                dustParticles.GlobalPosition += (Vector2)offset;        // move the position of the particles with the passed in offset
            }
        }
        dustParticles.Emitting = true;                                  // Flag that the dust particles are emitting
    }

    public void InstanceDustLandingParticles()
    {
        // If the particle effects isn't initialzed, return out of the method
        if(!HeroParticlesInitialized)
        {
            return;
        }
        // Instance the dust particles landing effect
        var dustParticlesLanding = DustParticlesLanding.Instance() as Particles2D;
        // Add the effect to the hero node
        Hero.AddChild(dustParticlesLanding);
        // Set them as top level, so they won't inherit transforms from the hero node
        dustParticlesLanding.SetAsToplevel(true);
        // Get the position for where to place the particles
        dustParticlesLanding.GlobalPosition = Hero.GetNode<Position2D>("ParticleEffects/DustLandingParticlesPosition").GlobalPosition;
        // Emitt the particles
        dustParticlesLanding.Emitting = true;
    }
}
