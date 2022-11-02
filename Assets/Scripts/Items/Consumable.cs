using UnityEngine;

public abstract class Consumable : Item
{
    /// Returns whether the usage was sucessful or not
    public abstract bool Use(Entity entity);
}
