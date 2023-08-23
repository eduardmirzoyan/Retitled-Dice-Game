using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionType { Instant, Reactive, Delayed }

public abstract class Action : ScriptableObject
{
    [Header("Static Data")]
    public new string name;
    public string description;
    public Sprite icon;
    public Sprite background;
    public Color color;
    public ActionType actionType;

    [Header("Dynamic Data")]
    public Die die;
    public Weapon weapon;

    public abstract List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room);

    public abstract List<Vector3Int> GetThreatenedLocations(Entity entity, Vector3Int targetLocation);

    public abstract IEnumerator Perform(Entity entity, List<Vector3Int> targetLocations, Room room); // Refactor to take in list

    public Action Copy()
    {
        // Make a copy
        var copy = Instantiate(this);

        // Make a copy of its die as well
        copy.die = die.Copy();

        return copy;
    }
}
