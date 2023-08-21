using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Enemy/Melee")]
public class MeleeEnemyAction : EnemyAction
{
    public List<Vector3Int> tilesToWatch;

    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room)
    {
        return room.GetNeighbors(startLocation);
    }

    public override IEnumerator Perform(Entity entity, Vector3Int targetLocation, Room room)
    {
        // Watch the tiles around the enemy
        tilesToWatch = room.GetNeighbors(entity.location);

        // Trigger event
        // TODO

        yield return new WaitForSeconds(0.5f);

        // Sub to event
        // TODO
    }

    private void WatchTiles()
    {

    }
}
