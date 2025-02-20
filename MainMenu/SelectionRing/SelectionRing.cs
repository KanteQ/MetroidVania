using Godot;
using System;

public class SelectionRing : Node2D
{
    [Export]
    public string SelectedMenuOption = "start";             // The current selected menu option
    private Sprite StartActive;                             // The start active sprite
    private Sprite SettingsActive;                          // The settings active sprite
    private Sprite ExitActive;                              // The exit active sprite
    private AudioStreamPlayer MenuSelectSfx;                // The menu select sound effect
    private bool exitGame = false;                          // Boolean to keep track of if the user pressed the exit game button    
    private AnimationPlayer RingAnimations;                 // Animation player for the ring animations
    private HeroCircleTransition HeroCircleTransition;      // The hero circle transition singleton
    private HeroSingletonVariables HeroSingletonVariables;  // The hero singleton variables    
    private Timer SelectTimer;                              // Timer to control when it's OK to rotate the selection wheel    
    private bool OkToRotate = true;                         // Boolean to stop user from rotating the selection wheel too fast    
    private CustomSignals CustomSignals;                    // The Custom Signals singleton    
    private SettingsMenu SettingsMenu;                      // The settings menu singleton 
    private bool RingActive = true;                         // If the selection ring is active or not
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        StartActive = GetNode<Sprite>("StartActive");                   // Get sprite node for showing start button as active
        SettingsActive = GetNode<Sprite>("SettingsActive");             // Get sprite node for showing setting button as active
        ExitActive = GetNode<Sprite>("ExitActive");                     // Get sprite node for showing exit button as active
        MenuSelectSfx = GetNode<AudioStreamPlayer>("MenuSelectSfx");    // Get the node for the menu select sound
        GetNode<TextureButton>("StartButton").GrabFocus();              // Make the start button grab focus        
        RingAnimations = GetNode<AnimationPlayer>("RingAnimations");    // Get the ring animations node
        SelectTimer = GetNode<Timer>("SelectionTimer");                 // Get the select timer                

        // Get the hero singleton variables reference        
        HeroSingletonVariables = GetNode<HeroSingletonVariables>("/root/HeroSingletonVariables");
        HeroSingletonVariables.Reset();                                 // Reset the variables to default
        // Get the hero circle transition singleton
        HeroCircleTransition = GetNode<HeroCircleTransition>("/root/HeroCircleTransition");
        HeroCircleTransition.PlayOpenCircle(new Vector2(640, 360));     // Play the open circle transition animation          

        CustomSignals = GetNode<CustomSignals>("/root/CustomSignals");  // Get the custom signals singleton
        SettingsMenu = GetNode<SettingsMenu>("/root/SettingsMenu");     // Get the settings menu           

        // When the settings menu is closed, run the ShowSelectionRing() ring method
        CustomSignals.Connect(nameof(CustomSignals.SettingsMenuClosed), this, nameof(ShowSelectionRing));
    }


    private void ShowSelectionRing()
    {
        Visible = true;                 // Show the selection ring
        RingActive = true;              // Enable the selection ring again
        RingAnimations.Play("RESET");
    }

    private void PlayMenuSelectSfx()
    {
        // If the menu select sound effect is not playing
        if (!MenuSelectSfx.Playing)
        {
            MenuSelectSfx.Play();   // Play the menu select sound effect
        }
    }
    private void OnStartButtonButtonDown()
    {
        StartActive.Visible = false;    // Show the start active sprite
    }
    private void OnStartButtonButtonUp()
    {
        StartActive.Visible = true;    // Show the start active sprite
        PlayMenuSelectSfx();
    }
    private void OnStartButtonPressed()
    {
        // Start the game
        HeroCircleTransition.PlayCloseCircle(new Vector2(640, 360), flipHero: false, "res://Scenes/Game.tscn", showHero: false);
    }
    private void OnExitButtonButtonDown()
    {
        ExitActive.Visible = false; // Hide the exit active sprite        
    }
    private void OnExitButtonButtonUp()
    {
        SettingsActive.Visible = true;  // Show the settings active sprite     
        exitGame = true;
        PlayMenuSelectSfx();
    }
    private void OnSettingsButtonButtonDown()
    {
        SettingsActive.Visible = false; // Hide the settings active sprite        
    }
    private void OnSettingsButtonButtonUp()
    {
        SettingsActive.Visible = true;  // Show the settings active sprite      
        PlayMenuSelectSfx();    // Play the menu select sound effect          
    }
    private void OnSettingsButtonPressed()
    {
        SettingsMenu.SetMenuVisible(true);  // Open the settings menu
        Visible = false;                    // Hide the selection ring
        RingActive = false;                 // Disable the selection ring
        PlayMenuSelectSfx();                // Play the menu select sound
    }
    private void HandleSelectionWheelButtonPressed()
    {
        // If the player has pressed exit game, or the ring is not active
        if (exitGame || !RingActive)
        {
            return; // return out of the method, preventing any logic from triggering
        }
        // If the user pressed the ui accept action
        if (Input.IsActionJustPressed("ui_accept"))
        {
            // Run the current selected option method
            if (SelectedMenuOption.Equals("start")) { OnStartButtonPressed(); }
            if (SelectedMenuOption.Equals("settings")) { OnSettingsButtonPressed(); }
            if (SelectedMenuOption.Equals("exit")) { OnExitButtonButtonUp(); }
        }
    }
    private void HandleRotateSelectionWheelLeft()
    {
        // If the player has pressed exit game, or the ring is not active
        if (exitGame || !RingActive)
        {
            return; // return out of the method, preventing any logic from triggering
        }        
        // If the user is pressing the ui left action
        if (Input.IsActionJustPressed("ui_left") && OkToRotate)
        {
            OkToRotate = false;     // Flag that it's not OK to rotate the selection
            SelectTimer.Start();    // Start the select timer 

            // And the start option is selected
            if (SelectedMenuOption.Equals("start"))
            {
                // Rotate the selection ring from start to the exit option
                RingAnimations.Play("RotateStartToExit");
                // Make the exit button grab focus so it can be pressed with keyboard/gamepad
                GetNode<TextureButton>("ExitButtonNode/ExitButton").GrabFocus();
            }
            // And the exit option is selected
            if (SelectedMenuOption.Equals("exit"))
            {
                // Rotate the selection ring from exit to the settings option
                RingAnimations.Play("RotateExitToSettings");
                // Make the settings button grab focus so it can be pressed with keyboard/gamepad
                GetNode<TextureButton>("SettingsButtonNode/SettingsButton").GrabFocus();
            }
            // And the settings option is selected
            if (SelectedMenuOption.Equals("settings"))
            {
                // Rotate the selection ring from settings to the start option
                RingAnimations.Play("RotateSettingsToStart");
                // Make the start button grab focus so it can be pressed with keyboard/gamepad
                GetNode<TextureButton>("StartButton").GrabFocus();
            }
        }
    }
    private void HandleRotateSelectionWheelRight()
    {
        // If the player has pressed exit game, or the ring is not active
        if (exitGame || !RingActive)
        {
            return; // return out of the method, preventing any logic from triggering
        }        
        // If the user is pressing the ui right action
        if (Input.IsActionJustPressed("ui_right") && OkToRotate)
        {
            OkToRotate = false;     // Flag that it's not OK to rotate the selection
            SelectTimer.Start();    // Start the select timer 

            // And the start option is selected
            if (SelectedMenuOption.Equals("start"))
            {
                // Rotate the selection ring from start to the settings option
                RingAnimations.Play("RotateStartToSettings");
                // Make the settings button grab focus so it can be pressed with keyboard/gamepad
                GetNode<TextureButton>("SettingsButtonNode/SettingsButton").GrabFocus();
            }
            // And the start option is settings
            if (SelectedMenuOption.Equals("settings"))
            {
                // Rotate the selection ring from settings to the exit option
                RingAnimations.Play("RotateSettingsToExit");
                // Make the exit button grab focus so it can be pressed with keyboard/gamepad
                GetNode<TextureButton>("ExitButtonNode/ExitButton").GrabFocus();
            }
            // And the start exit is selected
            if (SelectedMenuOption.Equals("exit"))
            {
                // Rotate the selection ring from exit to the start option
                RingAnimations.Play("RotateExitToStart");
                // Make the start button grab focus so it can be pressed with keyboard/gamepad
                GetNode<TextureButton>("StartButton").GrabFocus();
            }
        }
    }

    private void HandleSelectionWheel()
    {
        // If the player has pressed exit game
        if (exitGame)
        {
            return; // return out of the method, preventing any logic from triggering
        }
        HandleSelectionWheelButtonPressed();    // Handle selection wheel button presses        
        HandleRotateSelectionWheelLeft();       // Handle rotating the wheel to the left
        HandleRotateSelectionWheelRight();      // Handle rotating the wheel to the right
    }

    private void ExitGame()
    {
        // If the game is about to exit
        if (exitGame)
        {
            // And the select sound effect has finished playing
            if (!MenuSelectSfx.Playing)
            {
                GetTree().Quit();   // Exit the game
            }
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        HandleSelectionWheel();
        ExitGame();
    }
    private void OnSelectionTimerTimeout()
    {
        OkToRotate = true;
    }
}
