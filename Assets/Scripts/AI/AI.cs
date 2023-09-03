using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Alignment { Allied, Neutral, Hostile }

public abstract class AI : ScriptableObject
{
    public Alignment alignment;

    public abstract List<(Action, Vector3Int)> GenerateSequenceOfActions(Entity entity, Room room, Entity targetEntity);

    protected Vector3Int GetClosestLocationToTarget(Vector3Int currentLocation, Vector3Int targetLocation, List<Vector3Int> locations)
    {
        float closest = Vector3Int.Distance(currentLocation, targetLocation);
        Vector3Int result = Vector3Int.back;

        foreach (var location in locations)
        {
            var distance = Vector3Int.Distance(location, targetLocation);
            if (distance < closest)
            {
                closest = distance;
                result = location;
            }
        }

        return result;
    }

    protected Vector3Int GetFarthestLocationToTarget(Vector3Int currentLocation, Vector3Int targetLocation, List<Vector3Int> locations)
    {
        float farthest = Vector3Int.Distance(currentLocation, targetLocation);
        Vector3Int result = Vector3Int.back;

        foreach (var location in locations)
        {
            var distance = Vector3Int.Distance(location, targetLocation);
            if (distance > farthest)
            {
                farthest = distance;
                result = location;
            }
        }

        return result;
    }

    protected bool HasLineOfSight(Vector3Int start, Vector3Int end, Room room)
    {
        Vector3Int direction = end - start;
        direction.Clamp(-Vector3Int.one, Vector3Int.one);

        // If not a straight line
        if (direction.magnitude > 1)
            return false;

        start += direction;
        while (start != end)
        {
            // Check if blocked by wall at any point
            if (room.IsWall(start))
                return false;

            start += direction;
        }

        return true;
    }
}
