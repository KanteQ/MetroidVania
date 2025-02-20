using Godot;
using System;

public class CustomSignals : Node
{
    [Signal]
    public delegate void PauseGameSignal(bool paused, string pauseName);

    [Signal]
    public delegate void SettingsMenuClosed();

    [Signal]
    public delegate void ItemPickupSphereSignal(string item);

    [Signal]
    public delegate void InteractSignal(string interaction);
    [Signal]
    public delegate void ShopSellItemToPlayerSignal(string itemResourceName, int amount);
    [Signal]
    public delegate void PlayerSellItemToShopSignal(string itemResourceName, int amount);
    [Signal]
    public delegate void SetInventoryFocusSignal(InventoryModeE inventory);
    [Signal]
    public delegate void SetSelectedItemSlot(int slot);       
    [Signal]
    public delegate void UpdateMoneyGuiSignal();    
    [Signal]
    public delegate void ItemPickupSignal(string itemResourceName, int amount);    
    [Signal]
    public delegate void EquipItem(string itemResourceName, EquipmentTypeE equipType);
}
