using Godot;
using System;

public class Hero2DRaycasts
{
    private HeroStateMachine Hero;                      // The hero state machine
    private bool RaycastsInitialized;                   // Flag to keep track of if the raycasts have been properly initialized
    public RayCast2D LedgeGrabRayCastTileAbove;         // The raycast ray check if a tile is above the hero
    public RayCast2D LedgeGrabRayTileHead;              // The raycast to check if a tile is next to the head
    public RayCast2D LeftWallRay;                       // The raycast to check if there is a wall to the left
    public RayCast2D RightWallRay;                      // The raycast to check if there is a wall to the right
    public RayCast2D JumpBufferRayCast;                 // The raycast to check if the player can perform a jump-buffered jump
    public RayCast2D CornerCorrLeftRayCast;             // The left corner correction raycast
    public RayCast2D CornerCorrMiddleRayCast;           // The middle corner correction raycast
    public RayCast2D CornerCorrRightRayCast;            // The right corner correction raycast
    public Hero2DRaycasts(HeroStateMachine hero, ref bool initOk)
    {
        Hero = hero;                    // Set the hero reference
        initOk = InitHeroRaycasts();    // Initialize the hero raycasts
    }
    private bool InitHeroRaycasts()
    {
        RaycastsInitialized = true;                                                 // Set positive outcome

        LedgeGrabRayCastTileAbove = GetRaycastNode("LedgeGrabRayCastTileAbove");    // Initialize the ledge grab ray above the head
        if(!RaycastsInitialized) return false;                                      // If initialization failed, return false

        LedgeGrabRayTileHead = GetRaycastNode("LedgeGrabRayTileHead");              // Initialize the ledge grab ray next to the head
        if(!RaycastsInitialized) return false;                                      // If initialization failed, return false

        LeftWallRay = GetRaycastNode("LeftWallRayCast");                            // Initialize the left wall raycast
        if(!RaycastsInitialized) return false;                                      // If initialization failed, return false

        RightWallRay = GetRaycastNode("RightWallRayCast");                          // Initialize the right wall raycast
        if(!RaycastsInitialized) return false;                                      // If initialization failed, return false

        JumpBufferRayCast = GetRaycastNode("JumpBufferRayCast");                    // Initialize the jump buffer raycast
        if(!RaycastsInitialized) return false;                                      // If initialization failed, return false

        CornerCorrLeftRayCast = GetRaycastNode("CornerCorrLeftRayCast");            // Initialize the left corner correction raycast
        if(!RaycastsInitialized) return false;                                      // If initialization failed, return false

        CornerCorrMiddleRayCast = GetRaycastNode("CornerCorrMiddleRayCast");        // Initialize the middle corner correction raycast
        if(!RaycastsInitialized) return false;                                      // If initialization failed, return false

        CornerCorrRightRayCast = GetRaycastNode("CornerCorrRightRayCast");          // Initialize the right corner correction raycast
        if(!RaycastsInitialized) return false;                                      // If initialization failed, return false

        return true;                                                                // If all went well, return true
    }
    private RayCast2D GetRaycastNode(string rayCastNodeName)
    {
        string raycastPath = "./RayCasts/" + rayCastNodeName;       // Set the path to the raycast
        var raycast = Hero.GetNode<RayCast2D>(raycastPath);         // Try to get the raycast node

        // If the raycast node was not found
        if(raycast == null)
        {
            // Set initiated to false
            RaycastsInitialized = false;
            // Print a detailed message of where things went wrong
            GD.PrintErr("Error in Hero2DRaycasts.cs - GetRaycastNode() - Could not initialize raycast, node: '" + rayCastNodeName + "' was not found!");
        }
        // return the raycast node (even if it's null)
        return raycast;
    }
}
