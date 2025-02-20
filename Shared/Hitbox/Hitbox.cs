using Godot;
using System;

public class Hitbox : Area2D
{
    [Export]
    public float Damage = 1.0f;

    [Export]
    public float ResetTime = 1.5f;  // The time it takes for the hitbox to deliver damage again
    Timer ResetTimer;               // The reset timer 

    public override void _Ready()
    {
        var layersAndMasks = (LayersAndMasks)GetNode("/root/LayersAndMasks");
        CollisionLayer = layersAndMasks.GetCollisionLayerByName("Hitbox");
        CollisionMask = 0;
        AddResetTimer();
    }
    private void AddResetTimer()
    {
        ResetTimer = new Timer();           // Create a new timer
        ResetTimer.WaitTime = ResetTime;    // Set the wait time
        ResetTimer.OneShot = true;          // Make it a one shot timer

        // Connect the timeout signal to the hitbox, and run the OnResetTimerTimeOut() method whenever the timer times out
        ResetTimer.Connect("timeout", this, nameof(OnResetTimerTimeOut));

        // Add the timer as a child to the hitbox
        this.AddChild(ResetTimer);
    }
    private void OnResetTimerTimeOut()
    {
        CallDeferred(nameof(SetHitBoxMonitoring), true);    // Enable monitoring of the Area2D again
    }
    private void SetHitBoxMonitoring(bool enabled)
    {
        this.Monitorable = enabled;         // Set monitorable to the passed in value
        this.Monitoring = enabled;          // Set Monitoring to the passed in value
    }
    public void ResetHitboxMonitoring()
    {
        CallDeferred(nameof(SetHitBoxMonitoring), false);   // Disable monitoring of the Area2D
        ResetTimer.Start();                                 // Start the hitbox rest timer
    }
}
