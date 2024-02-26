using System.Collections;
using UnityEngine;

public abstract class Consumable : Item
{
    public string unusableMessage;

    public abstract bool CanUse(Entity entity);
    public abstract IEnumerator Use(Entity entity);
}
