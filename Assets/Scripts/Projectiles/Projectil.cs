using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectil : MonoBehaviour
{
    public float speed = 0.5f;

    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    

    [Header("Data")]
    [SerializeField] private GameObject deathParticles;
    [SerializeField] private Vector3Int location; // Location of this entity on the floor
    [SerializeField] private Room room;

    public void Initialize(Vector3Int location)
    {
        this.location = location;

        // Sub to events
    }

    public void Uninitialize() 
    {
        // Unsub

        // Spawn particles
        // Instantiate(deathParticles, transform.position, Quaternion.identity);
    }

    public IEnumerator Travel(Vector3Int destination)
    {
        // Calculate direction
        Vector3Int direction = destination - location;

        // Keep looping til we reach destination
        while (location != destination)
        {
            // Check if there is something to collide with
            if (AttackLocation(location))
            {
                // Finish
                break;
            }

            // Increment location
            location += direction;

            // Trigger event
            GameEvents.instance.TriggerOnProjectileMove(this);

            // Wait
            yield return new WaitForSeconds(speed);
        }


        // ?
    }

    private bool AttackLocation(Vector3 location)
    {
        var targets = new List<Entity>();

        // Consider player, enemies and barrels
        targets.Add(room.player);
        targets.AddRange(room.enemies);
        targets.AddRange(room.barrels);

        // Check if any entities are on the same tile, if so damage them
        foreach (var target in targets)
        {
            // If the target is not itself
            if (target.location == location && target != this)
            {
                // Currently deal 1 damage, but this might change?
                target.TakeDamage(1);

                return true;
            }
        }

        return false;
    }
}
