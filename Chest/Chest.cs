using Godot;
using System;

public class Chest : Node2D
{
    [Export]
    public string ItemResourceNameInside = "";
    [Export]
    public int Amount = 1;
    private MappedInput MappedInput;
    private AnimationPlayer ChestAnimation;
    private bool HeroInRange = false;
    private bool ChestOpened = false;
    private AudioStreamPlayer ChestOpenSfx;
    private AudioStreamPlayer ChestEmptySfx;
    private AudioStreamPlayer ItemAquiredSfx;
    private Node2D ItemNode;
    private bool ChestEmpty = false;
    private CustomSignals CustomSignals;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        CustomSignals = GetNode<CustomSignals>("/root/CustomSignals");  // Get the custom signals singleton    
        MappedInput = GetNode<MappedInput>("MappedInput");
        ChestAnimation = GetNode<AnimationPlayer>("AnimationPlayer");
        ChestOpenSfx = GetNode<AudioStreamPlayer>("ChestOpenSfx");
        ItemAquiredSfx = GetNode<AudioStreamPlayer>("ItemAquiredSfx");
        ChestEmptySfx = GetNode<AudioStreamPlayer>("ChestEmptySfx");
        ChestAnimation.Play("HideMappedInput");
        ItemNode = GetNode<Node2D>("Item");
        ItemNode.Visible = false;
        HeroInRange = false;                

        LoadItem();
    }

    private void LoadItem()
    {
        if (ItemResourceNameInside.Length > 0)
        {
            var itemToLoad = $"res://Scenes/Items/{ItemResourceNameInside}/{ItemResourceNameInside}.tscn";
            PackedScene itemScene = GD.Load<PackedScene>(itemToLoad);         
            var item = itemScene.Instance() as Item;
            item.HideBorder = true;
            item.Scale = new Vector2(0.35f, 0.35f);
            this.GetNode<Node2D>("Item").AddChild(item);               
        }
        else
        {
            ChestEmpty = true;
        }
    }    

    private void OnArea2DAreaEntered(Area2D area)
    {
        if (ChestOpened)
        {
            return;
        }
        if (area.Name.Equals("HeroPickupArea"))
        {
            MappedInput.Visible = true;
            HeroInRange = true;
        }        
    }

    private void OnArea2DAreaExited(Area2D area)
    {
        if (ChestOpened)
        {
            return;
        }
        if (area.Name.Equals("HeroPickupArea"))
        {
            MappedInput.Visible = false;
            HeroInRange = false;
        }        
    }


    private void OnAnimationPlayerAnimationFinished(string animation)
    {
        if (!ChestOpened && animation.Equals("OpenChest"))
        {
            ChestOpened = true;
        }        
    }
    private void HandleOpenChest()
    {
        if (!HeroInRange || ChestOpened)
        {
            return;
        }
        if (Input.IsActionJustPressed("InteractWithObject"))
        {
            ChestOpenSfx.Play();
            ChestAnimation.Play("OpenChest");
            MappedInput.Visible = false;
            // If the chest contained an item
            if (!string.IsNullOrEmpty(ItemResourceNameInside))
            {
                // Emit the item pickup signal so the inventory can add it.
                CustomSignals.EmitSignal(nameof(CustomSignals.ItemPickupSignal), ItemResourceNameInside, Amount);
            }
        }        
    }


    public override void _Process(float delta)
    {
        HandleOpenChest();
    }
}
