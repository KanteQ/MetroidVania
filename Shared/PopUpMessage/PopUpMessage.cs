using Godot;
using System;

public class PopUpMessage : CanvasLayer
{
    private ColorRect DarkenBackground; // The darken background ColorRect
    private Timer DurationTimer;        // Timer for how long the Pop up message should be visible
    private WindowDialog PopUpWindow;   // The Pop Up Window

    public override void _Ready()
    {
        PopUpWindow = GetNode<WindowDialog>("Window");                  // Get the PopUp Window dialogue
        DarkenBackground = GetNode<ColorRect>("DarkenBackground");      // Get the DarkenBackground ColorRect
        DurationTimer = GetNode<Timer>("DurationTimer");                // Get the duration timer
        GetNode<LineEdit>("Window/LineEditMessage").Editable = false;   // Set the text to non-editable        
    }
    public void ShowPopUp(string title, string message, float closeWaitTime = 0, bool darkenBackground = true)
    {
        PopUpWindow.WindowTitle = title;                                // Set the window title
        PopUpWindow.Visible = true;
        GetNode<LineEdit>("Window/LineEditMessage").Text = message;     // Set the LineEdit text message

        // If the DarkenBackground background ColorRect should be hidden
        if (!darkenBackground)
        {
            DarkenBackground.Visible = false;   // Hide it
        }
        else
        {
            DarkenBackground.Visible = true;   // Hide it
        }
        // If the PopUp should automatically close after a while
        if (closeWaitTime > 0)
        {
            DurationTimer.WaitTime = closeWaitTime; // Update the duration timer
            DurationTimer.Start();                  // Start the duration timer
        }
    }
    public void HidePopUp()
    {
        PopUpWindow.Visible = false;        // Hide the Pop-up
        DarkenBackground.Visible = false;   // Hide the darken background
    }
    private void OnDurationTimerTimeout()
    {
        HidePopUp();
    }
}
