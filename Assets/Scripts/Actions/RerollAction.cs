using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Reroll")]
public class RerollAction : Action
{
    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room)
    {
        // Can only be used on self
        return new List<Vector3Int>() { startLocation };
    }

    public override List<Vector3Int> GetThreatenedLocations(Entity entity, Vector3Int targetLocation)
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerator Perform(Entity entity, Vector3Int targetLocation, List<Vector3Int> threatenedLocations, Room room)
    {
        // Reroll all other action's die
        foreach (var action in entity.GetActions())
        {
            if (action != this && !action.die.isExhausted)
            {
                // Reroll all die
                action.die.Roll();
                // Trigger event
                GameEvents.instance.TriggerOnDieRoll(action.die);
            }
        }

        // Wait for rolling
        yield return new WaitForSeconds(DieUI.rollTime);
    }
}
