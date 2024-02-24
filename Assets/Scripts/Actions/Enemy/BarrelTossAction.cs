using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Enemy/Barrel Toss")]
public class BarrelTossAction : Action
{
    [SerializeField] private GameObject test;
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
                if (Room.ManhattanDistance(startLocation, location) <= range && room.IsInBounds(location))
                    if (!room.IsWall(location) && !room.IsChasam(location) && !room.HasEntity(location))
                        validLocations.Add(location);

            }
        }

        return validLocations;
    }

    public override List<Vector3Int> GetThreatenedLocations(Entity entity, Vector3Int targetLocation)
    {
        return new List<Vector3Int> { targetLocation };
    }

    public override IEnumerator Perform(Entity entity, Vector3Int targetLocation, List<Vector3Int> threatenedLocations, Room room)
    {
        Debug.Log($"Spawned barrel at {targetLocation}");

        yield return new WaitForSeconds(1f);

        room.SpawnEntity(barrel.Copy(), targetLocation);
    }
}
