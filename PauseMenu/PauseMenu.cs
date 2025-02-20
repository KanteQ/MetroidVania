using Godot;
using System;

public class PauseMenu : Control
{
    private CustomSignals CustomSignals;                    // The custom signals singleton
    private HeroCircleTransition HeroCircleTransition;      // The hero circle transition singleton    
    private HeroSingletonVariables HeroSingletonVariables;  // The hero singleton variables   
    private SettingsMenu SettingsMenu;                      // The settings menu singleton     
    private AudioStreamPlayer MenuMoveSfx;                  // The menu move sound effect
    private AudioStreamPlayer MenuSelectSfx;                // The menu select sound effect          

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Get the custom signals singleton
        CustomSignals = GetNode<CustomSignals>("/root/CustomSignals");
        // Get the hero circle transition singleton
        HeroCircleTransition = GetNode<HeroCircleTransition>("/root/HeroCircleTransition");
        // Get the hero singleton variables reference        
        HeroSingletonVariables = GetNode<HeroSingletonVariables>("/root/HeroSingletonVariables");
        // Get the settings menu singleton
        SettingsMenu = GetNode<SettingsMenu>("/root/SettingsMenu");
        // Connect to the SettingsMenuClosed signal
        CustomSignals.Connect(nameof(CustomSignals.SettingsMenuClosed), this, nameof(ResumePauseMenuFromSettings));
        // Get the menu move and menu select sound effect nodes
        MenuMoveSfx = GetNode<AudioStreamPlayer>("MenuMoveSfx");
        MenuSelectSfx = GetNode<AudioStreamPlayer>("MenuSelectSfx");

    }
    private void ResumePauseMenuFromSettings()
    {
        GetNode<Button>("MarginContainer/VBoxContainer/SettingsButton").GrabFocus();  // Set focus on the settings button             
    }
    public void SetResumeButtonFocus()
    {
        GetNode<Button>("MarginContainer/VBoxContainer/ResumeButton").GrabFocus();  // Set focus on the resume button
    }
    private void OnResumeButtonPressed()
    {
        this.Visible = false;
        CustomSignals.EmitSignal(nameof(CustomSignals.PauseGameSignal), false, "PauseMenu");    // Emit the "un-pause" game signal    
        MenuSelectSfx.Play();                                                                   // Play the menu select sound effect             
    }
    private void OnSettingsButtonPressed()
    {
        SettingsMenu.SetMenuVisible(visible: true); // Show the settings menu
        MenuSelectSfx.Play();                       // Play the menu select sound effect              
    }
    private void OnExitGameButtonPressed()
    {
        this.Visible = false;
        CustomSignals.EmitSignal(nameof(CustomSignals.PauseGameSignal), false, "PauseMenu");    // Emit the "un-pause" game signal        
        HeroSingletonVariables.Reset();                                                         // Reset the Hero variables to default
        MenuSelectSfx.Play();                                                                   // Play the menu select sound effect        

        // Play the circle transition effect
        HeroCircleTransition.PlayCloseCircle(HeroSingletonVariables.GlobalPosition,             // Close the circle on the Hero's position
                                            flipHero: false,                                    // Can be anything, since we hide the hero
                                            "res://Scenes/Main.tscn",                           // Transition to the Main scene
                                            showHero: false);                                   // Don't show the hero        
    }
    private void PlayMenuSoundOnUpAndDownInput()
    {
        if (Input.IsActionJustPressed("ui_down") || Input.IsActionJustPressed("ui_up"))
        {
            MenuMoveSfx.Play();
        }
    }
    public override void _Process(float delta)
    {
        if (Visible)
        {
            PlayMenuSoundOnUpAndDownInput();
        }
    }
}
