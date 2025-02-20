using Godot;
using System;

public interface ITakeDamage
{
    void TakeDamage(float damage, string damagedBy = "", Vector2? entityPosition = null);
}