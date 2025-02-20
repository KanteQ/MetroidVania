using Godot;
using System;

public enum InputDeviceE
{
    Mouse = 0,
    Gamepad = 1,
    Keyboard = 2,
}

public class SpecialItems
{
    public bool Glider = false;
}

public class HeroSingletonVariables : Node
{
    public float Health = 3;                    // The current health of the Hero
    public float MaxHealth = 3;                 // The current max health of the Hero
    public float IncomingDamage = 0;            // The curent incoming damage
    public bool IsDead = false;                 // If the player is dead    
    public Vector2 GlobalPosition;              // The hero player's global position
    public bool GameIsPaused = false;           // If the game is paused or not
    public InputDeviceE LastUsedInputDevice;    // The last input device the player used    
    public SpecialItems SpecialItems;           // The special items     
    public int Money = 100;                     // The amount of money the player has
    public int NumFreeInventorySlots = 25;      // The number of free inventory slots
    public bool IsBowEquipped = false;          // If a bow is equipped    

    public HeroSingletonVariables()
    {
        SpecialItems = new SpecialItems();  // Initialize the special items        
        PauseMode = PauseModeEnum.Process;  // Make sure that the hero singleton variables are processed during pause
    }
    public void Reset()
    {
        IsDead = false;
        GameIsPaused = false;
        ResetSpecialItems();
        Money = 100;
        NumFreeInventorySlots = 25;
        IsBowEquipped = false;
    }

    public void ResetSpecialItems()
    {
        SpecialItems = new SpecialItems();  // Reset the special items by creating a new instance of them
    }

    public void EnableSpecialItem(string itemName)
    {
        if (itemName.ToLower().Equals("glider"))
        {
            SpecialItems.Glider = true;
        }
    }

    private void SetLastUsedInputDevice(InputEvent @event)
    {
        if (@event is InputEventMouseButton || @event is InputEventMouseMotion)
        {
            LastUsedInputDevice = InputDeviceE.Mouse;
        }
        else if (@event is InputEventJoypadButton)
        {
            LastUsedInputDevice = InputDeviceE.Gamepad;
        }
        else if (@event is InputEventJoypadMotion motion)
        {
            // And the button is pressed above the 0.2 threshold
            if (Mathf.Abs(motion.AxisValue) > 0.2f)
            {
                LastUsedInputDevice = InputDeviceE.Gamepad;
            }
        }
        else if (@event is InputEventKey keyEvent && keyEvent.Pressed)
        {
            LastUsedInputDevice = InputDeviceE.Keyboard;
        }
    }
    public override void _Input(InputEvent @event)
    {
        SetLastUsedInputDevice(@event);
    }
}
