using Godot;
using System;

public class HeroArea2Ds : Node
{
    private HeroStateMachine Hero;                              // The hero state machine
    public Area2D Area2DGrabRopeRight;                          // Area2D for detecting rope-grabbing to the right
    public Area2D Area2DGrabRopeLeft;                           // Area2D for detecting rope-grabbing to the left
    public Area2D Area2DSwordHitbox;                            // Area2D for the player's sword hitbox
    private bool Area2DsInitialized;                            // Flag to keep track of if the area2Ds have been properly initialized
    private Vector2 SwordHitBoxLeft = new Vector2(-13,3.5f);    // Sword Hibox position when facing left
    private Vector2 SwordHitBoxRight = new Vector2(13,3.5f);    // Sword Hibox position when facing right
    public HeroArea2Ds(HeroStateMachine hero, ref bool initOk)
    {
        Hero = hero;
        initOk = InitHeroArea2Ds();
    }
    private bool InitHeroArea2Ds()
    {
        Area2DsInitialized = true;                                  // Set positive outcome

        Area2DGrabRopeRight = GetArea2DNode("Area2DGrabRopeRight"); // Get the Area for grabbing the rope to the right
        if(!Area2DsInitialized) return false;                       // If initialization failed, return false

        Area2DGrabRopeLeft = GetArea2DNode("Area2DGrabRopeLeft");   // Get the Area for grabbing the rope to the left
        if(!Area2DsInitialized) return false;                       // If initialization failed, return false

        Area2DSwordHitbox = GetArea2DNode("Area2DSwordHitbox");     // Get the Area2D for the sword hitbox
        if(!Area2DsInitialized) return false;                       // If initialization failed, return false
        DisableArea2DCollision(Area2DSwordHitbox);                  // Disable the sword hitbox

        return true;                                                // If all went well, return true
    }

    public void DisableArea2DCollision(Area2D area2D)
    {
        area2D.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;
    }
    public void EnableArea2DCollision(Area2D area2D)
    {
        area2D.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
    }
    private Area2D GetArea2DNode(string area2DNode)
    {
        string area2DsPath = "./Area2Ds/" + area2DNode;     // Set the path to the Area2D
        var area2D = Hero.GetNode<Area2D>(area2DsPath);     // Try to get the Area2D node
        // If the Area2D node was not found
        if(area2D == null)
        {
            // Set initiated to false
            Area2DsInitialized = false;
            // Print a detailed message of where things went wrong
            GD.PrintErr("Error in HeroArea2Ds.cs - GetArea2DNode() - Could not initialize Area2D, node: '" + area2DNode + "' was not found!");
        }
        // return the Area2D (even if it's null)
        return area2D;
    }
    public void DisableRopeSwingArea2DBehindHero()
    {
        if(Hero.HeroAnimations.FlipH)
        {
            Hero.HeroArea2Ds.Area2DGrabRopeRight.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;
            Hero.HeroArea2Ds.Area2DGrabRopeLeft.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
        }
        if(!Hero.HeroAnimations.FlipH)
        {
            Hero.HeroArea2Ds.Area2DGrabRopeRight.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
            Hero.HeroArea2Ds.Area2DGrabRopeLeft.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;
        }
    }
    private void UpdateArea2DSwordHitboxDirection()
    {
        if(Hero.HeroAnimations.FlipH)
        {
            Area2DSwordHitbox.Position = SwordHitBoxLeft;
        }
        if(!Hero.HeroAnimations.FlipH)
        {
            Area2DSwordHitbox.Position = SwordHitBoxRight;
        }
    }
    public void UpdateArea2DDirections()
    {
        DisableRopeSwingArea2DBehindHero();
        UpdateArea2DSwordHitboxDirection();
    }
}
