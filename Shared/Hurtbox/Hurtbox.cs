using Godot;
using System;
using System.Collections.Generic;

public class Hurtbox : Area2D
{
    [Export]
    public List<string> IgnoredObjects = new List<string>();    // List of objects to ignore impact from    
    public override void _Ready()
    {
        var layersAndMasks = (LayersAndMasks)GetNode("/root/LayersAndMasks");
        CollisionLayer = 0;
        CollisionMask = layersAndMasks.GetCollisionLayerByName("Hitbox");
        Connect("area_entered", this, nameof(OnAreaEntered));
    }

    private bool IsObjectIgnored(Hitbox hitbox)
    {
        foreach (var objectName in IgnoredObjects)
        {
            GD.Print("ignored object:" + objectName);
            if (hitbox.Owner.Name.Contains(objectName))
            {
                return true;
            }
        }
        return false;
    }    
    private void OnAreaEntered(Hitbox hitbox)
    {
        // If the hitbox is attached to an object that should be ignored
        if (IsObjectIgnored(hitbox))
        {
            return; // Return out of the method
        }        
        GD.Print(GetType().ToString());
        GD.Print(Owner.Name, "hit by:", hitbox.Owner);

        // If the hitbox is invalid, or the hurbox and hitbox have the same owner
        if (hitbox == null || hitbox.Owner == this.Owner)
        {
            return; // Return out of the method
        }

        ITakeDamage ownerTakeDamage = (ITakeDamage)Owner;                   // Typecast the hurtbox owner to the ITakeDamage
        // If the hurtbox owner has the ITakeDamage interface
        if (ownerTakeDamage != null)
        {
            ownerTakeDamage.TakeDamage(hitbox.Damage, hitbox.Owner.Name, hitbox.GlobalPosition);    // Run the TakeDamage() method
            hitbox.ResetHitboxMonitoring();                                                         // Stop the hitbox monitoring 
        }
    }
}
