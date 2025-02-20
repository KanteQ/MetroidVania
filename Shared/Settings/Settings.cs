using Godot;
using System;
using System.Collections.Generic;

public class Settings : Node
{
    private ConfigFile ControlsConfigFile = new ConfigFile();                   // The Configuration file for the Controls    
    private ConfigFile AudioVideoConfigFile = new ConfigFile();                 // The Configuration file for the Controls    
    private string ControlConfigFileName = "ControlSettings.cfg";               // The control settings configuration filename       
    private string AudioVideoSettingsConfigFileName = "AudioVideoSettings.cfg"; // The Audio & Video configurations config filename    
    public Dictionary<string, Vector2> Resolutions = new Dictionary<string, Vector2> {
        {"720x480",new Vector2(720,480)},
        {"960x540",new Vector2(960,540)},
        {"1280x720",new Vector2(1280,720)},
        {"1366x768",new Vector2(1366,768)},
        {"1600x900",new Vector2(1600,900)},
        {"1920x1080",new Vector2(1920,1080)},
        {"2560x1440",new Vector2(2560,1440)},
        {"3840x2160",new Vector2(3840,2160)}
    };

    // Default Audio & Video settings
    public float MusicVolume = 1.0f;                                            // The current music volume 0.0 -> 1.0
    public float SoundVolume = 1.0f;                                            // The current sound volume 0.0 -> 1.0
    public float MasterVolume = 1.0f;                                           // The current master volume 0.0 -> 1.0
    public bool Fullscreen = false;                                             // If the game should run int fullscreen
    public bool Vsync = true;                                                   // If the game should run with vsync
    public bool BorderlessWindow = false;                                       // If the game should run borderless    
    public string Resolution = "1280x720";                                      // The default screen resolution    

    public int SoundEffectBusIndex = 0;         // The bus index for sound effects volume
    public int MusicBusIndex = 0;               // The bus index for music volume
    public int MasterBusIndex = 0;              // The bus index for the master volume    

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Get the bus index for sound, music and the master volumes
        SoundEffectBusIndex = AudioServer.GetBusIndex("SoundEffects");
        MusicBusIndex = AudioServer.GetBusIndex("Music");
        MasterBusIndex = AudioServer.GetBusIndex("Master");

        LoadControlsSettings();                 // Load the controller settings        
        LoadAudioVideoSettingsConfigFile();     // Load the audio and video settings
    }
    public float GetBusVolume(int busIndex)
    {
        var dbVolume = AudioServer.GetBusVolumeDb(busIndex);
        return GD.Db2Linear(dbVolume);
    }
    public void SetBusVolume(int busIndex, float level)
    {
        AudioServer.SetBusVolumeDb(busIndex, GD.Linear2Db(level));
    }
    public Vector2 GetCurrentResolution()
    {
        return Resolutions[Resolution]; // Return the Resolution Vector2 value
    }
    public void SetResolution(Vector2 resolution)
    {
        Vector2 nativeResolution = new Vector2(1280, 720);
        var scale = resolution / nativeResolution;
        OS.WindowSize = resolution;
        GetViewport().Size = resolution;
        GetTree().SetScreenStretch(SceneTree.StretchMode.Mode2d, SceneTree.StretchAspect.Keep, resolution, scale.x);
    }
    private void SetFullscreen(bool fullscreen, Vector2 currentResolution)
    {
        OS.WindowFullscreen = fullscreen;       // Set the fullscreen value

        // If the screen is no longer fullscreen
        if (!fullscreen)
        {
            SetResolution(currentResolution);   // Rescale the screen size to the current resolution
        }
    }
    public void ApplySettings()
    {
        // Set the bus volume for music, sound effects and master
        SetBusVolume(MusicBusIndex, MusicVolume);
        SetBusVolume(SoundEffectBusIndex, SoundVolume);
        SetBusVolume(MasterBusIndex, MasterVolume);

        Vector2 resolution = Resolutions[Resolution];                                       // Get the Resolution Vector2 value
        OS.WindowBorderless = BorderlessWindow;                                             // Set the borderless value        
        OS.VsyncEnabled = Vsync;                                                            // Set if vsync should be enabled or not
        SetFullscreen(Fullscreen, resolution);                                              // Set fullscreen mode        
        SetResolution(resolution);                                                          // Set the screen resolution
    }

    private void LoadAudioVideoSettingsConfigFile()
    {
        // Try to load the Audio & Video configuration file
        Error error = AudioVideoConfigFile.Load("user://" + AudioVideoSettingsConfigFileName);

        // If the file isn't found, return out of the method - there's nothing to load
        if (error == Error.FileNotFound)
        {
            return;
        }
        // If there was an error
        else if (error != Error.Ok)
        {
            // Log the error
            GD.PrintErr("Error: Could not load:" + OS.GetUserDataDir() + "/" + AudioVideoSettingsConfigFileName);
            return; // Return out of the method
        }

        MusicVolume = (float)AudioVideoConfigFile.GetValue("Audio", "MusicVolume");         // Read the music volume value 
        SoundVolume = (float)AudioVideoConfigFile.GetValue("Audio", "SoundVolume");         // Read the sound volume value 
        MasterVolume = (float)AudioVideoConfigFile.GetValue("Audio", "MasterVolume");       // Read the master volume value 

        Resolution = (string)AudioVideoConfigFile.GetValue("Video", "Resolution");          // Read the resolution value
        Fullscreen = (bool)AudioVideoConfigFile.GetValue("Video", "Fullscreen");            // Read the fullscreen value
        Vsync = (bool)AudioVideoConfigFile.GetValue("Video", "Vsync");                      // Read the Vsync value
        BorderlessWindow = (bool)AudioVideoConfigFile.GetValue("Video", "BorderlessWindow");// Read the borderless window value      

        ApplySettings();                                                                    // Apply the loaded settings          
    }

    private void SaveAudioVideoSettingsConfigFile()
    {
        AudioVideoConfigFile.SetValue("Audio", "MusicVolume", MusicVolume);
        AudioVideoConfigFile.SetValue("Audio", "SoundVolume", SoundVolume);
        AudioVideoConfigFile.SetValue("Audio", "MasterVolume", MasterVolume);
        AudioVideoConfigFile.SetValue("Video", "Resolutions", "720x480,960x540,1280x720,1366x768,1600x900,1920x1080,2560x1440,3840x2160");
        AudioVideoConfigFile.SetValue("Video", "Resolution", Resolution);
        AudioVideoConfigFile.SetValue("Video", "Fullscreen", Fullscreen);
        AudioVideoConfigFile.SetValue("Video", "Vsync", Vsync);
        AudioVideoConfigFile.SetValue("Video", "BorderlessWindow", BorderlessWindow);
        // Save the config file to disk
        Error error = AudioVideoConfigFile.Save("user://" + AudioVideoSettingsConfigFileName);

        // If there was an error saving the file
        if (error != Error.Ok)
        {
            // Log the error to disk
            GD.PrintErr("File save Failed!", "Could not save: " + AudioVideoSettingsConfigFileName);
        }
    }
    public void LoadControlsSettings()
    {
        // Try to load the controls config file
        Error error = ControlsConfigFile.Load("user://" + ControlConfigFileName);

        // If the file wasn't found, return out of the method - there's nothing to load
        if (error == Error.FileNotFound)
        {
            return;
        }
        // If there was an error
        else if (error != Error.Ok)
        {
            // Log the error
            GD.PrintErr("Error: Could not load:" + OS.GetUserDataDir() + "/" + "ControlSettings.cfg");
            // Return out of the method
            return;
        }

        // Create an InputEventJoypadMotion event to use for loading gamepad axis actions with
        InputEventJoypadMotion gamepadMotion = new InputEventJoypadMotion();

        // Loop through all sections (in this case, the action names) in the config file
        foreach (var actionName in ControlsConfigFile.GetSections())
        {
            // Get the section keys
            var keys = ControlsConfigFile.GetSectionKeys(actionName);

            // Loop through all the keys in the section
            foreach (var key in keys)
            {
                // If it's a keyboard key
                if (key.Equals("Keyboard"))
                {
                    InputEventKey assignedKey = new InputEventKey();                        // Create an InputEventKey event
                    // Typecasting directly to uint crashes, so therefore typecast to an int first
                    int value = (int)ControlsConfigFile.GetValue(actionName, key);          // Read the value 
                    assignedKey.Scancode = (uint)value;                                     // Get the Scancode value by typecasting to uint
                    InputMap.ActionAddEvent(actionName, assignedKey);                       // Update the InputMap assigned key for the action
                }
                else if (key.Equals("GamepadButton"))
                {
                    InputEventJoypadButton button = new InputEventJoypadButton();             // Create an InputEventJoypadButton event
                    button.ButtonIndex = (int)ControlsConfigFile.GetValue(actionName, key);   // Read the gamepad button value 

                    // Update the assigned gamepad button for the action
                    InputMap.ActionAddEvent(actionName, button);
                }
                else if (key.Equals("GamepadAxis"))
                {
                    // Store the axis value in the gamepadMotion InputEvent
                    gamepadMotion.Axis = (int)ControlsConfigFile.GetValue(actionName, key);
                }
                else if (key.Equals("GamepadAxisValue"))
                {
                    // The GamepadAxisValue always comes after the GamepadAxis in the config file.
                    // We can therefore add the assigned gamepad motion to the InputMap here
                    gamepadMotion.AxisValue = (float)ControlsConfigFile.GetValue(actionName, key);// Get the axis value
                    InputMap.ActionAddEvent(actionName, gamepadMotion);  // Update the assigned gamepad axis for the action
                }
            }
        }
    }
    private void RemoveGamepadButtonFromAction(string actionName)
    {
        // Skip all ui mappings
        if (actionName.StartsWith("ui"))
        {
            return;
        }
        // If the conntrols config section has the "GampeadButton" key
        if (ControlsConfigFile.HasSectionKey(actionName, "GamepadButton"))
        {
            ControlsConfigFile.EraseSectionKey(actionName, "GamepadButton");    // Erase it from the config file
        }
    }
    private void RemoveGamepadMotionFromAction(string actionName)
    {
        // Skip all ui mappings
        if (actionName.StartsWith("ui"))
        {
            return;
        }
        // If the conntrols config section has the "GamepadAxis" key
        if (ControlsConfigFile.HasSectionKey(actionName, "GamepadAxis"))
        {
            ControlsConfigFile.EraseSectionKey(actionName, "GamepadAxis");      // Erase it from the config file
        }
        // If the conntrols config section has the "GamepadAxisValue" key
        if (ControlsConfigFile.HasSectionKey(actionName, "GamepadAxisValue"))
        {
            ControlsConfigFile.EraseSectionKey(actionName, "GamepadAxisValue"); // Erase it from the config file
        }
    }

    private void SaveControlsConfigFile()
    {
        var actions = InputMap.GetActions();                        // Get all the existing input map actions

        // Loop through all the actions
        foreach (var a in actions)
        {
            string actionName = a.ToString();                       // Get the action name
            var actionList = InputMap.GetActionList(actionName);    // Get all the existing actions mapped to the action name    

            // Loop through all the assigned actions in the action list
            foreach (var action in actionList)
            {
                // If a keyboard key is assigned to the action
                if (action as InputEventKey != null)
                {
                    // Typecast the action to an InputEventKey
                    var key = action as InputEventKey;
                    // Write the key scancode value
                    ControlsConfigFile.SetValue(actionName, "Keyboard", key.GetScancodeWithModifiers());
                }
                // If a gamepad button is assigned to the action
                if (action as InputEventJoypadButton != null)
                {
                    var button = action as InputEventJoypadButton;  // Typecast the action to an InputEventJoypadButton
                    RemoveGamepadMotionFromAction(actionName);      // Remove the assigned gamepad motion from the config file                                        
                    // Write the gamepad button index value
                    ControlsConfigFile.SetValue(actionName, "GamepadButton", button.ButtonIndex);
                }
                // Check which gamepad motion is assigned to the action
                if (action as InputEventJoypadMotion != null)
                {
                    var motion = action as InputEventJoypadMotion;  // Typecast the action to an InputEventJoypadMotion
                    RemoveGamepadButtonFromAction(actionName);      // Remove the assigned gamepad button from the config file                          
                    // Write the gamepad axis and axis values
                    ControlsConfigFile.SetValue(actionName, "GamepadAxis", motion.Axis);
                    ControlsConfigFile.SetValue(actionName, "GamepadAxisValue", motion.AxisValue);
                }
            }
        }
        // Save the config file to disk
        Error error = ControlsConfigFile.Save("user://" + ControlConfigFileName);

        // If there was an error saving the file
        if (error != Error.Ok)
        {
            // Show the error message a bit longer to alert the user about it
            GD.PrintErr("File save Failed!", "Could not save: " + ControlsConfigFile);
        }
    }
    public void SaveConfigFiles()
    {
        SaveAudioVideoSettingsConfigFile();
        SaveControlsConfigFile();
    }

}
