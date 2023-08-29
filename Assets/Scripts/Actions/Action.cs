using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionSpeed { Instant, Reactive, Delayed }
public enum ActionType { Movement, Attack, Utility }

public abstract class Action : ScriptableObject
{
    [Header("Static Data")]
    public new string name;
    public string description;
    public Sprite icon;
    public Sprite background;
    public Color color;
    public ActionSpeed actionSpeed;
    public ActionType actionType;

    [Header("Dynamic Data")]
    public Die die;
    public Weapon weapon;

    public abstract List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room);

    public abstract List<Vector3Int> GetThreatenedLocations(Entity entity, Vector3Int targetLocation);

    public abstract IEnumerator Perform(Entity entity, Vector3Int targetLocation, List<Vector3Int> threatenedLocations, Room room);

    public Action Copy()
    {
        // Make a copy
        var copy = Instantiate(this);

        // Make a copy of its die as well
        copy.die = die.Copy();

        return copy;
    }
}
