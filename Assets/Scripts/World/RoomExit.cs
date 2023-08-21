using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomExit : ScriptableObject
{
    public Vector3Int location;
    public int keysRequired;
    public int destinationIndex;

    public void Initialize(Vector3Int location, int keysRequired, int destinationIndex)
    {
        this.location = location;
        this.keysRequired = keysRequired;
        this.destinationIndex = destinationIndex;
    }

    public void UseKey() {
        // Decrement keys required
        keysRequired = Mathf.Max(keysRequired - 1, 0);

        // Trigger event
        GameEvents.instance.TriggerOnUseKey(1);
    }

    public bool IsLocked()
    {
        return keysRequired > 0;
    }
}
