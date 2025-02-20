using Godot;
using System;

public class LayersAndMasks : Node
{
    public uint GetCollisionLayerByName(string layerName)
    {
        // Loop through all the 32 collision layers
        for(uint i = 1; i <= 32; ++i)
        {
            // Get the layer name setting
            var layer = ProjectSettings.GetSetting("layer_names/2d_physics/layer_" + i).ToString();
            // If the layer name matches the passed in layer name
            if(layer.Equals(layerName))
            {
                return i;   // return the index, we've found the layer
            }
        }
        GD.PrintErr("Could not find the " + layerName + " collision layer.");
        GD.PrintErr("Make sure to set the name: " + layerName + " under 'project settings -> layer names' for the collision layer");
        return 0;
    }
}
