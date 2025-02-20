using Godot;
using System;

public enum CollisionLayersEnum
{
    TILES_LAYER = 0,
    HERO_LAYER = 1,
    PASS_THROUGH_PLATFORM_LAYER = 2,
}

public class HeroCollisionShapes
{
	private HeroStateMachine Hero;              // The hero state machine
    private CollisionShape2D Head;       		// The hero head collision shape
    private CollisionShape2D Body;              // The hero body collision shape
    private CollisionShape2D Slide;             // The hero slide collision shape
    private bool ShapesInitialized;           	// Flag for tracking if all collisions were initialized

    public HeroCollisionShapes(HeroStateMachine hero, ref bool initOk)
    {
        // Set the hero state machine reference
        Hero = hero;
        initOk = InitHeroCollisionShapes();
    }    
    private bool InitHeroCollisionShapes()
    {
        // Set positive outcome as the initial state for the initialization
        ShapesInitialized = true;
        // Set the collision shapes, return false if a collision shape was not initialized properly
        Head = InitHeroCollisionShape("./CollisionShapeHead");
        if(!ShapesInitialized) return false;
        Body = InitHeroCollisionShape("./CollisionShapeBody");
        if(!ShapesInitialized) return false;        
        Slide = InitHeroCollisionShape("./CollisionShapeSlide");
        if(!ShapesInitialized) return false;        
        return ShapesInitialized;
    }    
    private CollisionShape2D InitHeroCollisionShape(string shapeNodeName)
    {
        string collision2DNodeName = "./" + shapeNodeName;
        var collisionShape = Hero.GetNode<CollisionShape2D>(collision2DNodeName);
        if(collisionShape == null)
        {
            ShapesInitialized = false;
            GD.PrintErr("HeroCollisionShapes.cs - InitHeroCollisionShape() - Could not initialize collision shape, node:" + shapeNodeName + " was not found!");
        }
        return collisionShape;
    }
	public bool IsCollisionShape2DColliding(string collisionShapeName)
	{
		// loop through all the current collision shapes that are currently colliding
		for(int i=0; i < Hero.GetSlideCount(); ++i)
		{
			//Get the collision
			var collision = Hero.GetSlideCollision(i);
			// If the collision is a CollisionShape2D
			if(collision.LocalShape is CollisionShape2D)
			{
				// Get the shape by typecasting it to a CollisionShape2D
				var shape = (CollisionShape2D)collision.LocalShape;
				// If the names are equal, return true, the requested collision shape is colliding
				if(shape.Name.Equals(collisionShapeName))
					return true;
			}
		}
		return false;
	}
    public void ChangeCollisionShapesToSlide()
    {
        Head.Disabled = true;               // Disable the hero head collision shape        
        Body.Disabled = true;               // Disable the hero body collision shape
        Slide.Disabled = false;             // Enable the hero slide collision shape
    }

    public void ChangeCollisionShapesToStanding()
    {
        Head.Disabled = true;              // Deactivate head collision shape
        Body.Disabled = false;             // Enable the body collision shape
        Slide.Disabled = true;             // Disable the slide collision shape
    }
    public void ChangeCollisionShapesToSlideStandUp()
    {
        Head.Disabled = false;             // Set head collision shape to active        
        Body.Disabled = true;              // Disable the body collision shape
        Slide.Disabled = false;            // Enable the slide collision shape
    }    
    public void DisableAllCollisionShapes()
    {
        Head.Disabled = true;               // Disable the hero head collision shape        
        Body.Disabled = true;               // Disable the hero body collision shape
        Slide.Disabled = true;              // Disable the hero slide collision shapes        
    }    
	public void TurnOffPassThroughLayerCollision(int layer)
	{
		Hero.SetCollisionMaskBit(layer, false);
	}
	public void TurnOnPassThroughLayerCollision(int layer)
	{
		Hero.SetCollisionMaskBit(layer,true);
	}    
}
