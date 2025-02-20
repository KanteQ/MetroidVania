using Godot;
using System;
using System.Linq;

public class VideoTab : Tabs
{
    private Settings Settings;                                      // Settings singleton
    private OptionButton ResolutionOptions;                         // The drop-down with the resolutions
    private Vector2 CurrentResolution = new Vector2(1280, 720);     // The current resolution
    private CheckBox CheckBoxFullscreen;                            // The fullscreen checkbox
    private CheckBox CheckBoxVsync;                                 // The c-sync checkbox
    private CheckBox CheckBoxBorderLessWindow;                      // The borderless window checkbox

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Get the checkbox nodes
        CheckBoxFullscreen = FindNode("CheckBoxFullscreen", recursive: true) as CheckBox;
        CheckBoxVsync = FindNode("CheckBoxVsync", recursive: true) as CheckBox;
        CheckBoxBorderLessWindow = FindNode("CheckBoxBorderlessWindow", recursive: true) as CheckBox;
        Settings = GetNode<Settings>("/root/Settings"); // Get the settings singleton    

        AddVideoResolutions();
        SetLoadedSettings();
    }

    private void SetLoadedSettings()
    {
        CurrentResolution = Settings.GetCurrentResolution();            // Get the current resolution
        CheckBoxFullscreen.Pressed = Settings.Fullscreen;               // Get the fullscreen settings
        CheckBoxVsync.Pressed = Settings.Vsync;                         // Get the vsync setting
        CheckBoxBorderLessWindow.Pressed = Settings.BorderlessWindow;   // Get the borderless window setting
        SetSelectedResolutionInDropdown();                              // Set the selected resolution in the dropdown
    }
    private void SetSelectedResolutionInDropdown()
    {
        for (int i = 0; i < ResolutionOptions.GetItemCount(); ++i)
        {
            // If the name matches with the set resolution from the config file
            if (ResolutionOptions.GetItemText(i).Equals(Settings.Resolution))
            {
                ResolutionOptions.Selected = i; // Set the dropdown option as selected
                return;                         // Return out of the mthod
            }
        }
    }

    private void AddVideoResolutions()
    {
        var index = 0;
        ResolutionOptions = FindNode("OptionButton") as OptionButton;
        foreach (var resolution in Settings.Resolutions)
        {
            ResolutionOptions.AddItem(resolution.Key, index);
            index++;
        }
        ResolutionOptions.Selected = 2;
    }

    public void SetFocusOnCloseButton()
    {
        var button = FindNode("CloseButton") as Button;                 // Find the close button
        button.GrabFocus();                                             // Give the close button focus
    }
    private void OnCheckBoxFullscreenToggled(bool pressed)
    {
        OS.WindowFullscreen = pressed;                                  // Set the OS window fullscreen flag
        Settings.Fullscreen = pressed;                                  // Store the fullscreen flag in the settings singleton

        // If the screen is no longer fullscreen
        if (!pressed)
        {
            Settings.SetResolution(CurrentResolution);                  // Rescale the screen size to the current resolution
        }
        GetParent<SettingsTabContainer>().PlayMenuSelectSound();        // Play the menu select sound        
    }
    private void OnCheckBoxVsyncToggled(bool pressed)
    {
        OS.VsyncEnabled = pressed;                                      // Update OS vsync enabled
        Settings.Vsync = pressed;                                       // Store the vsync enabled option in the settings singleton
        GetParent<SettingsTabContainer>().PlayMenuSelectSound();        // Play the menu select sound        
    }
    private void OnCheckBoxBorderlessWindowToggled(bool pressed)
    {
        OS.WindowBorderless = pressed;                                  // Update OS window borderless 
        Settings.BorderlessWindow = pressed;                            // Store the borderless window settings in the settings singleton
        GetParent<SettingsTabContainer>().PlayMenuSelectSound();        // Play the menu select sound
    }

    private void OnOptionButtonItemSelected(int index)
    {
        var viewportSize = GetViewport().Size;
        Vector2 newResolution = Settings.Resolutions.ElementAt(index).Value;// Get the new resolution
        Settings.Resolution = Settings.Resolutions.ElementAt(index).Key;    // Update the resolution inside settings for saving
        // If no change took place
        if (viewportSize == newResolution)
        {
            return; // Return out of the method
        }
        Settings.SetResolution(newResolution);                              // Set the new resolution
        CurrentResolution = newResolution;                                  // Store the current solution
        GetParent<SettingsTabContainer>().PlayMenuSelectSound();            // Play the menu select sound        
    }
    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
