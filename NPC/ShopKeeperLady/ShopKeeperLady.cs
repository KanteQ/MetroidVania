using Godot;
using System;

public class ShopKeeperLady : Node2D
{
    private MappedInput MappedInput;
    private CustomSignals CustomSignals;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        CustomSignals = GetNode<CustomSignals>("/root/CustomSignals");
        MappedInput = GetNode<MappedInput>("MappedInput");
        MappedInput.Visible = false;
    }

    private void HandleOpenShop()
    {
        if (MappedInput.Visible && Input.IsActionJustReleased("InteractWithObject"))
        {
            CustomSignals.EmitSignal(nameof(CustomSignals.InteractSignal), "OpenShop");
        }
    }


    private void OnArea2DAreaEntered(Area2D area)
    {
        if (area.Name.Equals("HeroPickupArea"))
        {
            MappedInput.Visible = true;
        }
    }
    private void OnArea2DAreaExited(Area2D area)
    {
        if (area.Name.Equals("HeroPickupArea"))
        {
            MappedInput.Visible = false;
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        HandleOpenShop();
    }
}
