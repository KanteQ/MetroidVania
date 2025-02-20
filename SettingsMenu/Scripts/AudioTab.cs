using Godot;
using System;

public class AudioTab : Tabs
{
    private VolumeBar SoundEffectsVolumeBar;    // The sound effects volume bar
    private VolumeBar MusicVolumeBar;           // The music volume bar
    private VolumeBar MasterVolumeBar;          // The master volume bar
    private Settings Settings;                  // The Settings singleton    

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Settings = GetNode<Settings>("/root/Settings"); // Get the settings singleton
        if (Settings == null)
        {
            GD.Print("Settinsg is null!");
        }
        GetVolumes();                                   // Get the volumes        
    }
    private void GetVolumes()
    {
        // Get the volume bars
        SoundEffectsVolumeBar = FindNode("SoundVolumeBar", recursive: true) as VolumeBar;
        MusicVolumeBar = FindNode("MusicVolumeBar", recursive: true) as VolumeBar;
        MasterVolumeBar = FindNode("MasterVolumeBar", recursive: true) as VolumeBar;

        // Set the values for the volume bars 
        SoundEffectsVolumeBar.Value = Settings.GetBusVolume(Settings.SoundEffectBusIndex);
        MusicVolumeBar.Value = Settings.GetBusVolume(Settings.MusicBusIndex);
        MasterVolumeBar.Value = Settings.GetBusVolume(Settings.MasterBusIndex);
    }
    private void UpdateVolumes()
    {
        if (SoundEffectsVolumeBar.VolumeUpdated)
        {
            SoundEffectsVolumeBar.VolumeUpdated = false;
            Settings.SoundVolume = (float)SoundEffectsVolumeBar.Value;
            Settings.SetBusVolume(Settings.SoundEffectBusIndex, (float)SoundEffectsVolumeBar.Value);
            GetParent<SettingsTabContainer>().PlayMenuSoundLevelTestSound();
        }
        if (MusicVolumeBar.VolumeUpdated)
        {
            MusicVolumeBar.VolumeUpdated = false;
            Settings.MusicVolume = (float)MusicVolumeBar.Value;
            Settings.SetBusVolume(Settings.MusicBusIndex, (float)MusicVolumeBar.Value);
        }
        if (MasterVolumeBar.VolumeUpdated)
        {
            MasterVolumeBar.VolumeUpdated = false;
            Settings.MasterVolume = (float)MasterVolumeBar.Value;
            Settings.SetBusVolume(Settings.MasterBusIndex, (float)MasterVolumeBar.Value);
        }
    }
    public void SetFocusOnCloseButton()
    {
        var button = FindNode("CloseButton") as Button;
        button.GrabFocus();
    }
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        UpdateVolumes();
    }
}
