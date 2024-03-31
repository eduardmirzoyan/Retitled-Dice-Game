using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Enemy/Barrel Toss")]
public class BarrelTossAction : Action
{
    [SerializeField] private GameObject barrelTossPrefab;
    [SerializeField] private Entity barrel;

    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room)
    {
        // Range based on die value
        int range = die.value;

        var validLocations = new List<Vector3Int>();

        for (int i = -range; i <= range; i++)
        {
            for (int j = -range; j <= range; j++)
            {
                Vector3Int location = startLocation + new Vector3Int(i, j);

                // Skip if blocked
                if (room.IsObsacle(location) || room.HasEntity(location))
                    continue;

                // Throw within distance
                if (MathUtil.ManhattanDistance(startLocation, location) <= range)
                    validLocations.Add(location);
            }
        }

        return validLocations;
    }

    public override List<Vector3Int> GetThreatenedLocations(Entity entity, Vector3Int targetLocation, Room room)
    {
        return new List<Vector3Int> { targetLocation };
    }

    public override IEnumerator Perform(Entity entity, Vector3Int targetLocation, List<Vector3Int> threatenedLocations, Room room)
    {
        // Handle visuals
        var barrelToss = Instantiate(barrelTossPrefab).GetComponent<BarrelTossModel>();
        yield return barrelToss.Toss(entity.location, targetLocation, barrel.modelSprite);
        Destroy(barrelToss.gameObject);

        // Handle logic after visuals are done
        room.SpawnEntity(barrel.Copy(), targetLocation);
    }
}
