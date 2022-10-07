using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action : ScriptableObject
{
    public new string name;
    public string description;
    public Sprite icon;
    public Sprite background;
    public Color color;

    public Die die;
    public Weapon weapon;

    public abstract List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room);
    public abstract IEnumerator Perform(Entity entity, Vector3Int targetLocation, Room room);

    public Action Copy()
    {
        // Make a copy
        var copy = Instantiate(this);

        // Make a copy of its die as well
        copy.die = die.Copy();

        return copy;
    }
}
