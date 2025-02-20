using Godot;
using System;

public class SettingsMenu : CanvasLayer
{

    private TabContainer TabContainer;      // The Tab container
    private const int ControlsTAB = 0;      // Controls tab mapping to int

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        TabContainer = FindNode("TabContainer", recursive: true) as TabContainer;// Get the tab container             
    }

    public void SetMenuVisible(bool visible)
    {
        this.Visible = visible;                                                  // Set the settings menu visible
        TabContainer.CurrentTab = ControlsTAB;                                   // Set controlstab as visible
        TabContainer.GetNode<ControlsTab>("Controls").SetFocusOnCloseButton();   // Set focus on the Close button
    }
}
