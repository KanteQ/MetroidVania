using Godot;
using System;

public class ControlsTab : Tabs
{
    public void SetFocusOnCloseButton()
    {
        var button = FindNode("CloseButton") as Button;
        button.GrabFocus();
    }
}
