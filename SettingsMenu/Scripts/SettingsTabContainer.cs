using Godot;
using System;

public class SettingsTabContainer : TabContainer
{
    private AudioStreamPlayer MenuMoveSfx;
    private AudioStreamPlayer MenuSelectSfx;
    private AudioStreamPlayer MenuSoundLevelSfx;
    private Settings Settings;              // The Settings singleton
    private CustomSignals CustomSignals;    // The Custom Signals singleton    

    private const int ControlsTAB = 0;      // Controls tab mapping to int
    private const int AudioTAB = 1;         // Audio tab mapping to int
    private const int VideoTAB = 2;         // Video tab mapping to int    

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        MenuMoveSfx = FindNode("MenuMoveSfx") as AudioStreamPlayer;
        MenuSelectSfx = FindNode("MenuSelectSfx") as AudioStreamPlayer;
        MenuSoundLevelSfx = FindNode("MenuSoundLevelSfx") as AudioStreamPlayer;
        Settings = GetNode<Settings>("/root/Settings");                         // The Settings singleton        
        CustomSignals = GetNode<CustomSignals>("/root/CustomSignals");          // Get the custom signals singleton        
    }
    private void OnTabContainerTabSelected(int tab)
    {
        if(tab == ControlsTAB)
        {
            GetNode<ControlsTab>("Controls").SetFocusOnCloseButton();   // Set focus on the Close button 
        }
        if(tab == AudioTAB)
        {
            GetNode<AudioTab>("Audio").SetFocusOnCloseButton();         // Set focus on the Close button         
        }
        if(tab == VideoTAB)
        {
            GetNode<VideoTab>("Video").SetFocusOnCloseButton();         // Set focus on the Close button            
        }
        PlayMenuSelectSound();                                          // Play the menu select sound                     
    }
    public void PlayMenuSoundLevelTestSound()
    {
        if (!MenuSoundLevelSfx.Playing)
        {
            MenuSoundLevelSfx.Play();
        }
    }
    public void PlayMenuMoveSound()
    {
        if (MenuMoveSfx != null)
        {
            MenuMoveSfx.Play();
        }
    }
    public void PlayMenuSelectSound()
    {
        if (MenuSelectSfx != null)
        {
            MenuSelectSfx.Play();
        }
    }
    private void OnToAudioButtonPressed()
    {
        this.CurrentTab = AudioTAB;                             // Set the Audio tab as the current tab
        GetNode<AudioTab>("Audio").SetFocusOnCloseButton();     // Set focus on the Close button
        PlayMenuSelectSound();                                  // Play the menu select sound                        
    }
    private void OnToVideoButtonPressed()
    {
        this.CurrentTab = VideoTAB;                             // Set the current tab to the Video tab        
        GetNode<VideoTab>("Video").SetFocusOnCloseButton();     // Set focus on the Close button
        PlayMenuSelectSound();                                  // Play the menu select sound                        
    }

    private void OnCloseButtonPressed()
    {
        GetOwner<CanvasLayer>().Visible = false;                            // When the close button is pressed, hide the settings menu        
        PlayMenuSelectSound();                                              // Play the menu select sound                     
        CustomSignals.EmitSignal(nameof(CustomSignals.SettingsMenuClosed)); // Emit the settings menu closed signal      
        Settings.SaveConfigFiles();                                         // Save the configuration files          
    }

    private void OnToControlsButtonPressed()
    {
        this.CurrentTab = ControlsTAB;                              // Set the current tab to the Controls tab        
        GetNode<ControlsTab>("Controls").SetFocusOnCloseButton();   // Set focus on the Close button
        PlayMenuSelectSound();                                      // Play the menu select sound   
    }
    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
