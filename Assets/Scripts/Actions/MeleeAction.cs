using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Melee")]
public class MeleeAction : Action
{
    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room)
    {
        // ISSUE DOESN'T SCALE WITH DIE VALUE LOL

        return room.GetNeighbors(startLocation);
    }

    public override List<Vector3Int> GetThreatenedLocations(Entity entity, Vector3Int targetLocation)
    {
        // Calculate adjacent edges
        Vector3Int forward = targetLocation - entity.location;
        Vector3Int left = Vector3Int.RoundToInt(Vector3.Cross(forward, Vector3Int.forward));
        Vector3Int right = Vector3Int.RoundToInt(Vector3.Cross(forward, Vector3Int.back));

        return new List<Vector3Int>() { targetLocation, targetLocation + left, targetLocation + right };
    }

    public override IEnumerator Perform(Entity entity, Vector3Int targetLocation, Room room)
    {
        // Calculate direction
        Vector3Int direction = targetLocation - entity.location;
        direction.Clamp(-Vector3Int.one, Vector3Int.one);

        // Draw weapon
        GameEvents.instance.TriggerOnEntityDrawWeapon(entity, direction, weapon);

        // Wait for animation
        yield return new WaitForSeconds(EntityModel.moveSpeed);

        // Attack the location that you're at
        entity.MeleeAttackLocation(targetLocation, weapon);

        // Trigger event
        // GameEvents.instance.TriggerOnEntityMeleeAttack(entity, weapon);

        // Wait for animation
        yield return new WaitForSeconds(EntityModel.moveSpeed);

        // Sheathe weapon
        GameEvents.instance.TriggerOnEntitySheatheWeapon(entity, weapon);

        // Finnish!
    }
}
