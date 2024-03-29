using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Fireball")]
public class FireballAction : Action
{
    [SerializeField] private bool useCrossPattern;

    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room)
    {
        List<Vector3Int> targets = new List<Vector3Int>();

        // Check in each direction
        foreach (var direction in cardinalDirections)
        {
            var location = startLocation + direction * die.value;

            if (room.IsObsacle(location))
                continue;

            targets.Add(location);
        }

        return targets;
    }

    public override List<Vector3Int> GetThreatenedLocations(Entity entity, Vector3Int targetLocation, Room room)
    {
        List<Vector3Int> targets = new List<Vector3Int>() { targetLocation };

        if (!useCrossPattern)
        {
            // Check + pattern
            foreach (var direction in cardinalDirections)
            {
                Vector3Int newLocation = targetLocation + direction;

                if (room.IsObsacle(newLocation))
                    continue;

                targets.Add(newLocation);
            }
        }
        else
        {
            // Check X pattern
            foreach (var direction in new Vector3Int[] { new Vector3Int(1, 1), new Vector3Int(-1, 1), new Vector3Int(1, -1), new Vector3Int(-1, -1) })
            {
                Vector3Int newLocation = targetLocation + direction;

                if (room.IsObsacle(newLocation))
                    continue;

                targets.Add(newLocation);
            }
        }

        return targets;
    }

    public override IEnumerator Perform(Entity entity, Vector3Int targetLocation, List<Vector3Int> threatenedLocations, Room room)
    {
        Vector3Int direction = targetLocation - entity.location;
        direction.Clamp(-Vector3Int.one, Vector3Int.one);

        entity.model.FaceDirection(direction);
        yield return weapon.model.Draw(direction);

        foreach (var location in threatenedLocations)
            entity.AttackLocation(location, weapon, GetTotalDamage());

        yield return weapon.model.Attack();
        yield return weapon.model.Sheathe();
    }
}
