using Godot;
using System;

public class XboxInputGfx : Node2D
{
    public void SetXboxActionVisible(string nodeName, bool visible)
    {
        HideAllButtonsAndAxis();                // Hide all the buttons and axis
        var node = GetNode<Sprite>(nodeName);   // Try to get the node for the action
        // If the node doesn't exist
        if (node == null)
        {
            GD.PrintErr("Error in SetXboxActionVisible : ", "node:" + nodeName + " not found!");
            return; // return out of the method
        }
        node.Visible = visible;                 // Set the node visibilty to the passed in visible value
    }
    private void HideAllButtonsAndAxis()
    {
        var list = GetChildren();       // Get the list of all children
        // Loop through all the children (All children are of the type Sprite)
        foreach (Sprite child in list)
        {
            child.Visible = false;      // Set the child's visibility to false
        }
    }
}
