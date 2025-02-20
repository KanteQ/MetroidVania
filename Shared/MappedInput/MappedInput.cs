using Godot;
using System;

public class MappedInput : Control
{
    [Export]
    public string ActionName;
    [Export]
    public bool HidePanel = false;
    [Export]
    public string Text = "";
    private HeroSingletonVariables HeroVariables;   // The hero singleton variables
    private KeyboardInputKey KeyboardInputKey;      // The keyboard input key scene
    private XboxInputGfx XboxInputGfx;              // The xbox input buttons/sticks gfx 
    private const float JoyAxisThreshold = 0.2f;    // The threshold before an axis is considered triggered     

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        HeroVariables = GetNode<HeroSingletonVariables>("/root/HeroSingletonVariables");    // Get the hero singleton variables
        KeyboardInputKey = FindNode("KeyboardInputKey") as KeyboardInputKey;                // Get the keyboard input key node
        XboxInputGfx = FindNode("XBoxInputGfx") as XboxInputGfx;                            // Get the xbox input graphics node
        InitPanel();                                                                        // Initialize panel
        InitText();                                                                         // Initialise the text above the button
        GetAssignedInputMappings();                                                         // Get the assigned key / gamepad button        
    }

    private void InitPanel()
    {
        if (HidePanel)
        {
            GetNode<Panel>("Panel").Visible = false;
        }
    }

    private void InitText()
    {
        if (!string.IsNullOrEmpty(Text))
        {
            GetNode<Label>("TextLabel").Visible = true;
            GetNode<Label>("TextLabel").Text = Text;
        }
        else
        {
            GetNode<Label>("TextLabel").Visible = false;
        }
    }

    public void SetMappedInputAction(string actionName)
    {
        ActionName = actionName;
        GetAssignedInputMappings();
    }

    private void GetAssignedInputMappings()
    {
        var actions = InputMap.GetActionList(ActionName);   // Get all the existing actions mapped to the action name

        // Loop through all the mapped actions
        foreach (var action in actions)
        {
            // If a keyboard key is assigned to the action
            if (action as InputEventKey != null)
            {
                var key = action as InputEventKey;                              // Typecast the action to an InputEventKey
                KeyboardInputKey.SetKey(OS.GetScancodeString(key.Scancode));    // Show the keyboard key          
            }
            // If a gamepad button is assigned to the action
            else if (action as InputEventJoypadButton != null)
            {
                var button = action as InputEventJoypadButton;              // Type cast the action to an InputEventJoypadButton
                // Get the enum name of the buttonIndex
                var buttonXboxNodeName = Enum.GetName(typeof(DefaultGodot3XboxInputButtons), button.ButtonIndex);
                // Show the xbox sprite for the active button
                XboxInputGfx.SetXboxActionVisible(buttonXboxNodeName, visible: true);
            }
            // Check which gamepad motion is assigned to the action
            else if (action as InputEventJoypadMotion != null)
            {
                var motion = action as InputEventJoypadMotion;  // Typecast the action to an InputEventJoypadMotion
                ShowLeftStickAction(motion);
                ShowRightStickAction(motion);
                ShowLtAndRtAxis(motion);
            }
        }
    }


    private void ShowLeftStickAction(InputEventJoypadMotion motion)
    {
        // If the left stick horiontal axis is assigned
        if (motion.Axis == (int)DefaultGodot3XboxInputAxis.LeftStickHorizontalAxis)
        {
            // And it's value is set to the left direction
            if (motion.AxisValue < 0)
            {
                // The "LeftStickAxisLeft" must match with the Node name in the XboxInputGfx scene
                XboxInputGfx.SetXboxActionVisible("LeftStickAxisLeft", visible: true);  // Show the xbox sprite for the active axis
            }
            // And it's value is set to the right direction
            if (motion.AxisValue > 0)
            {
                // The "LeftStickAxisRight" must match with the Node name in the XboxInputGfx scene
                XboxInputGfx.SetXboxActionVisible("LeftStickAxisRight", visible: true); // Show the xbox sprite for the active axis
            }
        }
        // If the left stick vertical axis is assigned
        else if (motion.Axis == (int)DefaultGodot3XboxInputAxis.LeftStickVerticalAxis)
        {
            // And it's value is set to the up direction
            if (motion.AxisValue < 0)
            {
                // The "LeftStickAxisUp" must match with the Node name in the XboxInputGfx scene
                XboxInputGfx.SetXboxActionVisible("LeftStickAxisUp", visible: true);    // Show the xbox sprite for the active axis
            }
            // And it's value is set to the down direction
            if (motion.AxisValue > 0)
            {
                // The "LeftStickAxisDown" must match with the Node name in the XboxInputGfx scene
                XboxInputGfx.SetXboxActionVisible("LeftStickAxisDown", visible: true);  // Show the xbox sprite for the active axis
            }
        }
    }

    private void ShowRightStickAction(InputEventJoypadMotion motion)
    {
        // If the right stick horiontal axis is assigned
        if (motion.Axis == (int)DefaultGodot3XboxInputAxis.RightStickHorizontalAxis)
        {
            // And it's value is set to the left direction
            if (motion.AxisValue < 0)
            {
                // The "RightStickAxisLeft" must match with the Node name in the XboxInputGfx scene
                XboxInputGfx.SetXboxActionVisible("RightStickAxisLeft", visible: true);  // Show the xbox sprite for the active axis
            }
            // And it's value is set to the right direction
            if (motion.AxisValue > 0)
            {
                // The "RightStickAxisRight" must match with the Node name in the XboxInputGfx scene
                XboxInputGfx.SetXboxActionVisible("RightStickAxisRight", visible: true); // Show the xbox sprite for the active axis
            }
        }
        // If the right stick vertical axis is assigned
        else if (motion.Axis == (int)DefaultGodot3XboxInputAxis.RightStickVerticalAxis)
        {
            // And it's value is set to the up direction
            if (motion.AxisValue < 0)
            {
                // The "RightStickAxisUp" must match with the Node name in the XboxInputGfx scene
                XboxInputGfx.SetXboxActionVisible("RightStickAxisUp", visible: true);   // Show the xbox sprite for the active axis
            }
            // And it's value is set to the down direction
            if (motion.AxisValue > 0)
            {
                // The "RightStickAxisDown" must match with the Node name in the XboxInputGfx scene
                XboxInputGfx.SetXboxActionVisible("RightStickAxisDown", visible: true); // Show the xbox sprite for the active axis
            }
        }
    }

    private void ShowLtAndRtAxis(InputEventJoypadMotion motion)
    {
        if (motion.Axis == (int)DefaultGodot3XboxInputAxis.LT_Axis)
        {
            // And the button is pressed above the 0.2 threshold
            if (motion.AxisValue > JoyAxisThreshold)
            {
                // The "LT_Axis" must match with the Node name in the XboxInputGfx scene
                XboxInputGfx.SetXboxActionVisible("LT_Axis", visible: true); // Show the xbox sprite for the active axis
            }
        }
        else if (motion.Axis == (int)DefaultGodot3XboxInputAxis.RT_Axis)
        {
            // And the button is pressed above the 0.2 threshold
            if (motion.AxisValue > JoyAxisThreshold)
            {
                // The "RT_Axis" must match with the Node name in the XboxInputGfx scene
                XboxInputGfx.SetXboxActionVisible("RT_Axis", visible: true); // Show the xbox sprite for the active axis
            }
        }
    }
    private void CheckLastUsedInput()
    {
        if (HeroVariables.LastUsedInputDevice == InputDeviceE.Keyboard)
        {
            KeyboardInputKey.Visible = true;    // Show the keyboard mapping
            XboxInputGfx.Visible = false;       // Show the xbox mapping
        }
        else if (HeroVariables.LastUsedInputDevice == InputDeviceE.Gamepad)
        {
            XboxInputGfx.Visible = true;        // Show the xbox mapping
            KeyboardInputKey.Visible = false;   // Hide the keyboard mapping
        }
    }       

     // Called every frame. 'delta' is the elapsed time since the previous frame.
     public override void _Process(float delta)
     {
        CheckLastUsedInput();
     }
}
