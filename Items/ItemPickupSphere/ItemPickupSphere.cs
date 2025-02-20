using Godot;
using System;

public class ItemPickupSphere : Node2D, ITakeDamage
{

    // Create a drop-down list of which item to show inside the item pickup sphere
    [Export(PropertyHint.Enum, "None,Glider")]
    public string SelectedItem = "None";            // The default selected item to show inside the sphere
    private float Health = 4;                       // The item pickup sphere health
    private bool Opened = false;                    // If the sphere has been opened or not
    private AnimationPlayer AnimPlayer;             // The item pickup sphere animation player
    private Sprite GliderItem;                      // The glider item
    private float animationLength = 0;              // The length of the current playing animation
    private AudioStreamPlayer TakingDamageSfx;      // The sound effect for the sphere taking damage
    private AudioStreamPlayer RemoveShieldSfx;      // The remove shield sound effect  
    private AudioStreamPlayer ItemPickupSfx;        // The item pickup sound effect      
    private CustomSignals CustomSignals;            // The custom signals singleton
    private HeroSingletonVariables HeroVariables;   // The hero singleton variables   
    private Area2D ItemArea2D;                      // The Area2D for the item inside the sphere     
    private bool DoQueueFree = false;               // Boolean to keep track of if the sphere should be removed from memory        

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        AnimPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        GliderItem = GetNode<Sprite>("Item/ItemInside/Glider");
        ItemArea2D = GetNode<Area2D>("Item/ItemInside/ItemArea2D");
        SetItemInside();
        // Load the pickup shpere sound effect nodes
        TakingDamageSfx = GetNode<AudioStreamPlayer>("SoundEffects/TakingDamageSfx");
        RemoveShieldSfx = GetNode<AudioStreamPlayer>("SoundEffects/RemoveShieldSfx");
        ItemPickupSfx = GetNode<AudioStreamPlayer>("SoundEffects/ItemPickupSfx");

        HeroVariables = GetNode<HeroSingletonVariables>("/root/HeroSingletonVariables");    // Get the hero singleton variables
        CustomSignals = GetNode<CustomSignals>("/root/CustomSignals");                      // Get the custom signals singleton
    }

    private void SetItemInside()
    {
        if (SelectedItem.Contains("Glider"))
        {
            GliderItem.Visible = true;
        }
    }
    public void TakeDamage(float damage, string damagedBy, Vector2? entityPositon)
    {
        if (Health == 4)
        {
            Health--;
            AnimPlayer.Play("ShieldRemove");
            RemoveShieldSfx.Play();
            TakingDamageSfx.Play();
            animationLength = AnimPlayer.CurrentAnimationLength;
        }
        // If the item sphere hasn't been opened and the shield remove animation is finished playing
        else if (!Opened && AnimPlayer.CurrentAnimationPosition >= animationLength)
        {
            animationLength = 0;                    // reset the length to 0, so the next animation will play
            AnimPlayer.Play("TakingDamage");        // Play the Taking damage animation
            TakingDamageSfx.Play();                 // Play the take damage sound effect            
            Health -= damage;                       // Decrease the health with the given damage

            // If the health of the item pickup sphere is below zero
            if (Health <= 0)
            {
                AnimPlayer.Play("ItemPickup");      // Play the item pickup animation
            }
        }
    }
    private void OnAnimationPlayerAnimationFinished(string animationName)
    {
        if (animationName.Equals("ItemPickup"))
        {
            Opened = true;                      // Flag that the sphere has been opened       
            ItemArea2D.Monitoring = true;       // Enable monitoring on the ItemArea2D                 
        }
    }

    private void TriggerItemPickup()
    {
        // Play the item pickup sound effect
        ItemPickupSfx.Play();

        // Emit the signal for the item pickup, and what item that is beeing picked up
        CustomSignals.EmitSignal(nameof(CustomSignals.ItemPickupSphereSignal), SelectedItem);

        // Enable the item
        HeroVariables.EnableSpecialItem(SelectedItem);

        // Hide the pickup sphere
        this.Visible = false;

        // Flag that the sphere should be freed from memory
        DoQueueFree = true;
    }

    private void HandleItemPickup()
    {
        if (ItemArea2D.Monitoring && !DoQueueFree)
        {
            var overlappingAreas = ItemArea2D.GetOverlappingAreas();

            foreach (Area2D a in overlappingAreas)
            {
                if (a.Name.Equals("HeroPickupArea"))
                {
                    TriggerItemPickup();
                }
            }
        }
    }
    private void CheckRemoveFromMemory()
    {
        if (!ItemPickupSfx.Playing && DoQueueFree)
        {
            this.QueueFree();
        }
    }

    public override void _Process(float delta)
    {
        HandleItemPickup();
        CheckRemoveFromMemory();
    }
}
