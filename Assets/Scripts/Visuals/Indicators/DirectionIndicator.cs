using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionIndicator : MonoBehaviour
{
    private Entity entity;

    public void Initialize(Entity entity, Vector3 direction)
    {
        this.entity = entity;

        transform.position += direction / 2;

        if (direction.x > 0) // Right
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (direction.x < 0) // Left
        {
            transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        else if (direction.y > 0) // Up
        {
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else if (direction.y < 0) // Down
        {
            transform.rotation = Quaternion.Euler(0, 0, -90);
        }

        GameEvents.instance.onTurnStart += OnTurnStart;
    }

    private void OnDestroy()
    {
        GameEvents.instance.onTurnStart -= OnTurnStart;
    }

    private void OnTurnStart(Entity entity)
    {
        if (this.entity == entity)
        {
            Destroy(gameObject);
        }
    }
}
