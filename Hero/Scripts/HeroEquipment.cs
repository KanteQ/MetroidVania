using Godot;
using System;

public class HeroEquipment
{
    private HeroStateMachine Hero;                      // The hero state machine
    private bool HeroEquipmentInitialized;              // Flag to keep track of if the hero equipment have been properly initialized
    public Glider Glider;                               // The Hero glider
    public Bow Bow;                                     // The hero bow
    public HeroEquipment(HeroStateMachine hero, ref bool initOk)
    {
        Hero = hero;
        initOk = InitHeroEquipment();
    }
    private bool InitHeroEquipment()
    {
        HeroEquipmentInitialized = true;                    // Set a positive outcome

        InitGlider();                                       // Try to initialize the glider
        if (!HeroEquipmentInitialized) { return false; }    // If initialization failed, return false
        InitBow();                                          // Try to initialize the bow
        if (!HeroEquipmentInitialized) { return false; }     // If initialization failed, return false        
        return true;                                        // If all equipment was initialized ok, return true
    }
    private void InitGlider()
    {
        Glider = Hero.GetNode<Glider>("./Equipment/Glider");

        // If the glider node was not found
        if (Glider == null)
        {
            // Print a detailed message of where things went wrong
            GD.PrintErr("Error in HeroStateGlide.cs - GetGliderNode() - Glider node was not found!");
            // Set initialized to false
            HeroEquipmentInitialized = false;
            // return out of the method
            return;
        }
        // If all went ok, set initialized to true
        HeroEquipmentInitialized = true;
    }
    private void InitBow()
    {
        Bow = Hero.GetNode<Bow>("./Equipment/Bow");
        // If the bow node was not found
        if (Bow == null)
        {
            // Print a detailed message of where things went wrong
            GD.PrintErr("Error in HeroStateGlide.cs - GetGliderNode() - Bow node was not found!");
            // Set initialized to false
            HeroEquipmentInitialized = false;
            // return out of the method
            return;
        }
        // If all went ok, set initialized to true
        HeroEquipmentInitialized = true;
    }
}
