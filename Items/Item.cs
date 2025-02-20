using Godot;
using System;

public enum ItemTypeE
{
    Common = 1,
    Equipable = 2,
    Consumable = 3,
}

// Used to define the type of equipment an item is
// and also to define which slots in the equipmnent scene 
// that can hold what type of equipment.
public enum EquipmentTypeE
{
    Passive = 0,        // Used for inventory and shop.
    Helmet = 1,
    Leftshoulder = 2,
    Rightshoulder = 3,
    Pants = 4,
    Chest = 5,
    Sword = 6,
    Bow = 7,
    Boots = 8
}

public class Item : Node2D
{
    [Export]
    public string ItemName { get; set; }                            // Name of the item
    [Export]
    public string ResourceName { get; set; }                        // Name of the scene on disk    
    [Export(PropertyHint.Enum, "Common, Equipable, Consumable")]    // Create a drop-down for choosing itemtype
    public ItemTypeE ItemType { get; set; }                         // The item type
    [Export(PropertyHint.Enum, "")]                                 // Create a drop-down for choosing itemtype
    public EquipmentTypeE EquipType { get; set; }                   // The equipment type    
    [Export]
    public int Price { get; set; }                                  // Price of the item
    [Export]
    public float RestoreHPAmount { get; set; }                      // How much HP the consumable item restores
    [Export]
    public bool HasGrayItem { get; set; }                           // If the item has a gray background (used for special items)
    [Export]
    public bool HideBorder { get; set; }                            // If the border should be hidden or not
    private Sprite GrayItem { get; set; }                           // The gray item sprite
    private Sprite ItemSprite { get; set; }                         // The item sprite    
    private Sprite Border { get; set; }                             // The border sprite
    private Label LabelPrice;                                       // The price label
    private Node2D ShopNode;                                        // The shop node    
    private CustomSignals CustomSignals;                            // The custom signals singleton    
    private AudioStreamPlayer UseItemSfx;                           // The use item sound effect    

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        UseItemSfx = GetNode<AudioStreamPlayer>("UseItemSfx");  // Get the use item sound effect        
        Border = GetNode<Sprite>("Border");                     // Get the border sprite node
        GrayItem = GetNode<Sprite>("GrayItem");                 // Get the grayitem sprite node
        ItemSprite = GetNode<Sprite>("Sprite");                 // Get the item sprite node        
        LabelPrice = GetNode<Label>("ShopNode/LabelPrice");     // Get the price label node
        LabelPrice.Text = Price.ToString();                 // Set the price label to the item price value
        ShopNode = GetNode<Node2D>("ShopNode");                 // Get the shop node
        ShopNode.Visible = true;                                // Set the shop node as visible     
        SetBorderColor();                                       // Set the border color   
        SetGrayItemVisibility();                                // Set the grayitem sprite visibility
        Border.Visible = HideBorder ? false : true;             // Set initial border visibility
        SetPriceLabel();                                        // Set price label for shop        

        // Get the custom signals singleton
        CustomSignals = GetNode<CustomSignals>("/root/CustomSignals");

        // Connect the item pickup signal
        CustomSignals.Connect(nameof(CustomSignals.ItemPickupSphereSignal), this, nameof(EnableItem));
    }

    public void SetShopNodeVisible(bool visible)
    {
        if (ShopNode == null)
        {
            ShopNode = GetNode<Node2D>("ShopNode"); // Get the shop node
        }

        ShopNode.Visible = visible; // Set the shop node visibily
    }
    public void PlayUseItemSoundEffect()
    {
        if (UseItemSfx != null)
        {
            UseItemSfx.Play();
        }
    }    
    private void EnableItem(string itemName)
    {
        if (HasGrayItem && itemName.Equals(this.ItemName))
        {
            ItemSprite.Visible = true;
        }
    }

    private void SetGrayItemVisibility()
    {
        if (HasGrayItem)
        {
            GrayItem.Visible = true;
        }
        else
        {
            GrayItem.Visible = false;
        }
    }
    public void SetBorderVisibility(bool visible)
    {
        if (visible)
        {
            Border.Visible = true;
        }
        else
        {
            Border.Visible = false;
        }
    }

    private void SetPriceLabel()
    {
        if (Price > 0)
        {
            LabelPrice.Text = Price.ToString();
        }
    }

    private void SetBorderColor()
    {
        if (ItemType == ItemTypeE.Common)
        {
            Border.SelfModulate = new Color(0.5f, 0.5f, 0.5f, 0.5f);  // Gray border for common items
        }
        if (ItemType == ItemTypeE.Consumable)
        {
            Border.SelfModulate = new Color(0, 1.0f, 0, 0.5f);        // Green border for consumable items
        }
        if (ItemType == ItemTypeE.Equipable)
        {
            Border.SelfModulate = new Color(1.0f, 1.0f, 0, 0.5f);     // Yellow border for equipable items
        }
    }
}
