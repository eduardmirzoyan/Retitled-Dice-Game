using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityEnchantment : ScriptableObject
{
    [Header("Static Data")]
    public new string name;
    [TextArea(5, 2)] public string description;

    [Header("Dynamic Data")]
    public Entity entity;

    public abstract void Initialize(Entity entity);
    public abstract void Uninitialize();

    public EntityEnchantment Copy()
    {
        return Instantiate(this);
    }
}
