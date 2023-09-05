using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Player : Entity
{
    protected override void Interact()
    {
        room.InteractWithLocation(this, location);
    }
}
