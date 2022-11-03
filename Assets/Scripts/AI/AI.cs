using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AI : ScriptableObject
{
    public bool isHostile = true;

    public virtual void DisplayIntent(Entity entity, Room room)
    {
        // Does nothing
    }
    
    // Hard coded so far, but action 0 should be move, action 1 is attack
    public virtual (Action, Vector3Int) GenerateBestDecision(Entity entity, Room room)
    {
        // Only consider the first action of the entity
        var moveAction = entity.GetActions()[0];
        var meleeAction = entity.GetActions()[1]; 

        // ~~~~ First check to see if player can be attacked ~~~

        // Loop through valid locations
        foreach (var location in meleeAction.GetValidLocations(entity.location, room))
        {
            // If there is a location crosses the player
            if (IsBetween(entity.location, location, room.player.location))
            {
                // Debug
                // Debug.Log("Yielded true with the vectors, A: " + entity.location + " B: " + location + " C: " + dungeon.player.location);
                
                // Finish
                return (meleeAction, location);
            }
        }


        // ~~~ Else go to location that is closest to the player ~~~
        
        // Store variables
        float closestDistance = Vector3.Distance(room.player.location, entity.location);
        Vector3Int bestLocation = Vector3Int.back;
        
        // Loop through valid locations
        foreach (var location in moveAction.GetValidLocations(entity.location, room))
        {
            // Calculate distance from player
            float distanceFromPlayer = Vector3.Distance(room.player.location, location);
            if (distanceFromPlayer < closestDistance)
            {
                closestDistance = distanceFromPlayer;
                bestLocation = location;
            }
        }

        // Returns Vector.z = -1 if entity won't perform any action
        // Return the choice pair
        return (moveAction, bestLocation);
    }

    /// Returns whever current point is between point1 and point2
    private bool IsBetween(Vector3Int point1, Vector3Int point2, Vector3Int currPoint) {
        int dxc = currPoint.x - point1.x;
        int dyc = currPoint.y - point1.y;

        int dxl = point2.x - point1.x;
        int dyl = point2.y - point1.y;

        int cross = dxc * dyl - dyc * dxl;

        if (cross != 0)
            return false;

        if (Mathf.Abs(dxl) >= Mathf.Abs(dyl))
            return dxl > 0 ?
              point1.x <= currPoint.x && currPoint.x <= point2.x :
              point2.x <= currPoint.x && currPoint.x <= point1.x;
        else
            return dyl > 0 ?
              point1.y <= currPoint.y && currPoint.y <= point2.y :
              point2.y <= currPoint.y && currPoint.y <= point1.y;
    }

    public List<(Action, Vector3Int)> GenerateNewBestDecision(Entity entity, Room room, Entity targetEntity)
    {
        // Store best action w/ heuristic
        List<(Action, Vector3Int)> bestChoiceSquence = new List<(Action, Vector3Int)>();
        float bestValue = 0f;

        // Get all actions
        var actions = entity.GetActions();

        // Get a list of all possible permuations
        var permutations = GeneratePermutations(actions);

        // For each permutation
        foreach (var permutation in permutations)
        {
            // Debug
            // string result = "Output: ";
            // foreach (var item in permutation)
            // {
            //     result += item.name + ", ";
            // }
            // Debug.Log(result);

            // Store starting value at 0
            // float value = 0f;
        
            // Generate pairs from first action (HARD CODED)
            var actionPairs = GenerateActionPairs(permutation[0], entity.location, room);

            // Now go through each pair
            foreach (var pair in actionPairs)
            {
                // Perform that action
                // TODO?
                
                // If pair is a No-Action, then feed location back into entity
                var start = pair.Item2 != Vector3Int.back ? pair.Item2 : entity.location;

                // Add damage value
                // float value = GetDamageHeuristic(entity.location, start, targetEntity.location);

                // Now generate new pairs from that new location
                var actionPairs2 = GenerateActionPairs(permutation[1], start, room);
                
                // Now go through each pair and find result
                foreach (var pair2 in actionPairs2)
                {
                    // If pair2 is a No-Action, then feed location back into entity
                    var start2 = pair2.Item2 != Vector3Int.back ? pair2.Item2 : start;

                    // Add damage value
                    float value = GetDamageHeuristic(entity.location, start, targetEntity.location);
                    value += GetDamageHeuristic(start, start2, targetEntity.location);

                    // Add distance value
                    value += GetDistanceHeuristic(start2, targetEntity.location);

                    // Now save best value
                    if (value > bestValue)
                    {
                        // Update best value
                        bestValue = value;

                        // Update sequence
                        bestChoiceSquence.Clear();
                        bestChoiceSquence.Add(pair);
                        bestChoiceSquence.Add(pair2);
                    }
                }
            }
        }

        // Debug
        Debug.Log("Best Value: " + bestValue);

        foreach (var choice in bestChoiceSquence)
        {
            Debug.Log(choice.Item1.name + " at " + choice.Item2 + "~~~~~~~~~~~~~~~~~~~~~~~");
        }

        // Return best result
        return bestChoiceSquence;
    }

    private float GetDistanceHeuristic(Vector3Int endLocation, Vector3Int targetLocation)
    {
        return 1 / Vector3Int.Distance(endLocation, targetLocation);
    }

    private float GetDamageHeuristic(Vector3Int startLocation, Vector3Int endLocation, Vector3Int targetLocation)
    {
        // If you attacked the player, return 1
        if (IsBetween(startLocation, endLocation, targetLocation)) {
            Debug.Log("YES!");
            return 10f;
        }

        return 0f;
    }

    private List<(Action, Vector3Int)> GenerateActionPairs(Action action, Vector3Int start, Room room) 
    {
        List <(Action, Vector3Int)> result = new List<(Action, Vector3Int)>();

        // Manually add possiblity of No-action
        result.Add((action, Vector3Int.back));

        // Pair each action with all of it's valid locations
        foreach (var location in action.GetValidLocations(start, room))
        {
            // Add
            result.Add((action, location));
        }

        return result;
    }

    private List<List<Action>> GeneratePermutations(List<Action> nums)
    {
        var list = new List<List<Action>>();
        return DoPermute(nums, 0, nums.Count - 1, list);
    }

    private List<List<Action>> DoPermute(List<Action> nums, int start, int end, List<List<Action>> list)
    {
        if (start == end)
        {
            // We have one of our possible n! solutions,
            // add it to the list.
            list.Add(new List<Action>(nums));
        }
        else
        {
            for (var i = start; i <= end; i++)
            {
                // Swap actions
                Action action1 = nums[start];
                Action action2 = nums[i];
                Swap(ref action1, ref action2);
                nums[start] = action1;
                nums[i] = action2;
                
                // Recusively permute
                DoPermute(nums, start + 1, end, list);

                // Swap again
                action1 = nums[start];
                action2 = nums[i];
                Swap(ref action1, ref action2);
                nums[start] = action1;
                nums[i] = action2;
            }
        }

        return list;
    }

    private void Swap(ref Action a, ref Action b)
    {
        var temp = a;
        a = b;
        b = temp;
    }
}
