using Godot;
using System;

public class KeyboardInputKey : Node2D
{
    private Label KeyLabel;     // The key label
    private Sprite LargeKey;    // The large key sprite
    private Sprite SmallKey;    // The small key sprite

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        KeyLabel = GetNode<Label>("KeyLabel");  // Get the key label node
        LargeKey = GetNode<Sprite>("LargeKey"); // Get the large key node
        SmallKey = GetNode<Sprite>("SmallKey"); // Get the small key node
        LargeKey.Visible = false;               // Hide the large key sprite
        SmallKey.Visible = false;               // Hide the small key sprite
        KeyLabel.Text = string.Empty;           // Empty the key label string        
    }

    public void SetKey(string keyName)
    {
        if (keyName.Length == 0)
        {
            return;
        }
        // If the name of the key has more than one character
        if (keyName.Length > 1)
        {
            LargeKey.Visible = true;
            SmallKey.Visible = false;
        }
        else
        {
            SmallKey.Visible = true;
            LargeKey.Visible = false;
        }
        KeyLabel.Text = keyName;
    }

}
