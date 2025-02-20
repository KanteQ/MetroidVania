using Godot;
using System;

public class VolumeBar : ProgressBar
{
    private ColorRect FocusColorRect;
    public bool VolumeUpdated = false;
    private AudioStreamPlayer MenuSelectSfx;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        FocusColorRect = GetNode<ColorRect>("ColorRectFocus");              // Get the color rect
        FocusColorRect.SetSize(this.RectSize);                              // Set the color rect to the same size as the progress bar

        // This will make sure the color rect will not "spill" over to the edges of the progress bar in the y-axis
        FocusColorRect.RectSize -= new Vector2(0, 4);                       // Make the color rect a little bit less thick in the y-axis
        FocusColorRect.RectPosition += new Vector2(2, 2);                   // Move the color rect position with 2 pixels down
        MenuSelectSfx = GetNode<AudioStreamPlayer>("MenuSelectSfx");        // Get the menu select sound effect        
    }
    private void HandleMouseWheelVolumeChange()
    {
        // Mouse wheel only triggers on IsActionJustReleased
        if (Input.IsActionJustReleased("DecreaseVolume"))
        {
            // Decrease the value in the progress bar
            if (this.Value > 0)
            {
                this.Value -= 0.02;
                VolumeUpdated = true;
            }
        }
        if (Input.IsActionJustReleased("IncreaseVolume"))
        {
            // Increase the value in the progress bar
            if (this.Value < 1)
            {
                this.Value += 0.02;
                VolumeUpdated = true;
            }
        }
    }
    private void HandleVolumeChange()
    {
        HandleMouseWheelVolumeChange();
        if (Input.IsActionPressed("DecreaseVolume"))
        {
            // Decrease the value in the progress bar
            if (this.Value > 0)
            {
                this.Value -= 0.01;
                VolumeUpdated = true;
            }
        }
        if (Input.IsActionPressed("IncreaseVolume"))
        {
            // Increase the value in the progress bar
            if (this.Value < 1)
            {
                this.Value += 0.01;
                VolumeUpdated = true;
            }
        }
    }

    public override void _Process(float delta)
    {
        if (HasFocus())
        {
            HandleVolumeChange();
            FocusColorRect.Visible = true;
        }
        else
        {
            FocusColorRect.Visible = false;
        }
    }
    private void OnVolumeBarGuInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton && !mouseButton.Pressed)
        {
            if (mouseButton.ButtonIndex == (int)ButtonList.Left)
            {
                MenuSelectSfx.Play();
            }
        }
    }
}
