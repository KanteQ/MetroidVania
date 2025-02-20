using Godot;
using System;


/*
    Default Godot xbox controller buttons mapping for windows / linux,
    as seen in the dropdown list inside the Input Map tab when assigning
    actions to a gamepad buttons
*/
public enum DefaultGodot3XboxInputButtons
{
    Button0_A = 0,
    Button1_B = 1,
    Button2_X = 2,
    Button3_Y = 3,
    Button4_LB = 4,
    Button5_RB = 5,
    Button6_LT = 6,
    Button7_RT = 7,
    Button8_LeftStick = 8,
    Button9_RightStick = 9,
    Button10_Back = 10,
    Button11_Start = 11,
    Button12_DpadUp = 12,
    Button13_DpadDown = 13,
    Button14_DpadLeft = 14,
    Button15_DpadRight = 15,
    Button16_Guide = 16,    // Xbox Home button
}

/*
    Default Godot xbox controller axes mapping for windows / linux,
    as seen in the dropdown list inside the Input Map tab when assigning
    actions to a gamepad axis

    You can find more in the Godot.JoystickList enum, 
    however it shares both axises and buttons, which is a bit messy.
    I like to keep them separated, so I therefore created two different enums
    to keep things nice and tidy.
*/
public enum DefaultGodot3XboxInputAxis
{
    LeftStickHorizontalAxis = 0,    // Left stick horizontal movement default Axis = 0
    LeftStickVerticalAxis = 1,      // Left stick vertical movement default Axis = 1
    RightStickHorizontalAxis = 2,   // Right stick horizontal movement default Axis = 2
    RightStickVerticalAxis = 3,     // Left stick vertical movement default Axis = 3
    LT_Axis = 6,                    // Default LT axis = 6
    RT_Axis = 7,                    // Default RT axis = 7
}

public class MappedAction : PanelContainer
{
    [Export]
    public string ActionName = "";              // Acion name in the input map 
    private Label ActionNameLabel;              // Label to show the action name
    private XboxInputGfx XboxInputGfx;          // The xbox controller buttons/axis graphics    
    private KeyboardInputKey KeyboardInputKey;  // The keyboard key graphics
    private bool ChangeKeyboardAction = false;  // If the assigned keyboard action should be changed
    private bool ChangeGamepadAction = false;   // If the assigned gamepad action should be changed    
    private const float JoyAxisThreshold = 0.2f;// The threshold before an axis is considered triggered     
    private Control CurrentFocusedNode;         // The current focused node    
    private PopUpMessage PopUpMessage;          // The PopUpMessage dialogue    
    private Timer RestoreFocusTimer;            // The restore focus timer    

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        XboxInputGfx = FindNode("XBoxInputGfx", recursive: true) as XboxInputGfx;               // Get the xbox graphics node
        KeyboardInputKey = FindNode("KeyboardInputKey", recursive: true) as KeyboardInputKey;   // Get keyboard key graphics node        
        ActionNameLabel = FindNode("ActionNameLabel", recursive: true) as Label;                // Get the ActionName label node
        PopUpMessage = GetNode<PopUpMessage>("/root/PopUpMessage");                             // Get the PopUpMessage singleton         
        RestoreFocusTimer = GetNode<Timer>("RestoreFocusTimer");                                // Get the restore focus timer        
        GetAssignedInputMappings();
    }

    private void GetAssignedInputMappings()
    {
        var actions = InputMap.GetActionList(ActionName);   // Get all the existing actions mapped to the action name
        ActionNameLabel.Text = ActionName;                  // Update the ActionName Label text to the ActionName

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

    private void UpdateGamepadAction(InputEvent @event)
    {
        // If the flag for changing a gampad action is not yet triggered
        if (!ChangeGamepadAction)
        {
            return; // return out of the method
        }

        if (@event is InputEventJoypadButton eventButton)
        {
            RemoveGamepadActionEvent();                             // Remove all gamepad action events
            CurrentFocusedNode = GetFocusOwner();                   // Get the current focused node
            ActionNameLabel.GrabFocus();                            // Grab focus, to prevent A button from triggering assigning button again
            InputMap.ActionAddEvent(ActionName, eventButton);       // Update the assigned action with the gamepad button
            GetAssignedInputMappings();                             // Get the assigned input mappings
            ChangeGamepadAction = false;                            // Reset ChangeKeyboardAction to false
            PopUpMessage.HidePopUp();                               // Hide the popup message      
            RestoreFocusTimer.Start();                              // Start the restore focus timer
        }
        if (@event is InputEventJoypadMotion eventMotion)
        {
            if (Mathf.Abs(eventMotion.AxisValue) > JoyAxisThreshold)
            {
                RemoveGamepadActionEvent();                         // Remove all gamepad action events
                InputMap.ActionAddEvent(ActionName, eventMotion);   // Update the assigned action with the gamepad motion
                GetAssignedInputMappings();                         // Get the assigned input mappings
                ChangeGamepadAction = false;                        // Reset ChangeKeyboardAction to false
                PopUpMessage.HidePopUp();                           // Hide the popup message       
            }
        }
    }

    private void RemoveGamepadActionEvent()
    {
        // Initialise the remove button and remove motion variables
        InputEventJoypadButton removeButton = new InputEventJoypadButton();
        InputEventJoypadMotion removeMotion = new InputEventJoypadMotion();
        // Initialize the variables used to flag if a button or motion was found
        bool buttonFound = false;
        bool motionFound = false;

        // Get the list of the current assigned actions
        var list = InputMap.GetActionList(ActionName);

        // Find the previous assigned gamepad action in the list
        foreach (var i in list)
        {
            // If it's a gamepad button
            if (i is InputEventJoypadButton)
            {
                removeButton = i as InputEventJoypadButton; // Get the previous assigned button
                buttonFound = true;                         // Flag that the button was found
                break;                                      // break out of the foreach loop
            }
            // If it's a gamepad motion
            if (i is InputEventJoypadMotion)
            {
                removeMotion = i as InputEventJoypadMotion; // Get the previous assigned motion
                motionFound = true;                         // Flag that the motion was found
                break;                                      // break out of the foreach loop                
            }
        }
        // If the previous assigned action was a button
        if (buttonFound)
        {
            InputMap.ActionEraseEvent(ActionName, removeButton);   // Remove the previous assigned gamepad button
        }
        // If the previous assigned action was a motion
        if (motionFound)
        {
            InputMap.ActionEraseEvent(ActionName, removeMotion);   // Remove the previous assigned gamepad motion            
        }
    }

    private void UpdateKeyboardAction(InputEvent @event)
    {
        // If the flag for changing a keyboard action is not yet triggered
        if (!ChangeKeyboardAction)
        {
            return; // Return out of the method
        }
        if (@event is InputEventKey eventKey)
        {
            CurrentFocusedNode = GetFocusOwner();           // Get the current focused node            
            RemoveKeyboardActionEvent();                    // Remove the old assigned key
            ActionNameLabel.GrabFocus();                    // Grab focus, to prevent spacebar from triggering assigning button again            
            InputMap.ActionAddEvent(ActionName, eventKey);  // Update the assigned key
            GetAssignedInputMappings();                     // Get the assigned input mappings
            ChangeKeyboardAction = false;                   // Reset ChangeKeyboardAction to false
            PopUpMessage.HidePopUp();                       // Hide the popup message
            RestoreFocusTimer.Start();                      // Start the restore focus timer            
        }
    }
    public override void _Input(InputEvent @event)
    {
        // Update the keyboard and gamepad actions
        UpdateKeyboardAction(@event);
        UpdateGamepadAction(@event);
    }


    private void RemoveKeyboardActionEvent()
    {
        InputEventKey removeKey = new InputEventKey();
        bool keyFound = false;
        var list = InputMap.GetActionList(ActionName);
        // Find the previous assigned keyboard key in the list
        foreach (var i in list)
        {
            if (i is InputEventKey)
            {
                removeKey = i as InputEventKey;                 // Assign the input event key
                keyFound = true;                                // Flag that a key was found
                break;                                          // break out of the foreach loop
            }
        }
        // If a key was found
        if (keyFound)
        {
            InputMap.ActionEraseEvent(ActionName, removeKey);   // Remove the previous assigned key
        }
    }

    private void OnKeyboardActionButtonPressed()
    {
        ChangeKeyboardAction = true;    // Flag that it's time to change the assigned keyboard action
        // Show the pop-up message
        PopUpMessage.ShowPopUp("Update key for: " + ActionName, "Press the desired key");
        PopUpMessage.Visible = true;    // Make the pop-ip message visible        
    }
    private void OnGamepadActionButtonPressed()
    {
        ChangeGamepadAction = true;     // Flag that it's time to change the assigned gamepad action
        // Show the pop-up message
        PopUpMessage.ShowPopUp("Update gamepad for:  " + ActionName, "Push/move the desired button/axis");
        PopUpMessage.Visible = true;    // Make the pop-ip message visible
    }

    private void OnRestoreFocusTimerTimeout()
    {
        CurrentFocusedNode.GrabFocus();
    }
    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
