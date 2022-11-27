using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardianAI : AI
{
    // TODO
    public override List<(Action, Vector3Int)> GenerateNewBestDecision(Entity entity, Room room, Entity targetEntity)
    {

        // TODO
        // Ideally we need to run through every possibility and see how the game state changes from each sequence of actions
        // We need to evaluate the game state at after each possible sequence of actions and choice the one with the highest heuristic
        // The guardian should focus on reducing core damage as much as possible

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
            // FIXME
            // List<List<(Action, Vector3Int)>> sequences = new List<List<(Action, Vector3Int)>>();
            // GenerateSequences(permutation, entity, room, 0, new List<(Action, Vector3Int)>(), sequences);
            // Debug.Log(sequences.Count);

            // // REMOVE LATER
            // continue;

            // Generate pairs from first action (HARD CODED)
            var actionPairs = GenerateActionPairs(permutation[0], entity.location, room);

            // Now go through each pair
            foreach (var pair in actionPairs)
            {
                // Perform that action
                // TODO?

                // If pair is a No-Action, then feed location back into entity
                var start = pair.Item2 != Vector3Int.back ? pair.Item2 : entity.location;

                // Now generate new pairs from that new location
                var actionPairs2 = GenerateActionPairs(permutation[1], start, room);

                // Now go through each pair and find result
                foreach (var pair2 in actionPairs2)
                {
                    // If pair2 is a No-Action, then feed location back into entity
                    var start2 = pair2.Item2 != Vector3Int.back ? pair2.Item2 : start;

                    // Evaluate state
                    float value = GetStateValue();

                    // float value = GetDamageHeuristic(entity.location, start, targetEntity.location);
                    // value += GetDamageHeuristic(start, start2, targetEntity.location);

                    // Add distance value
                    // value += GetDistanceHeuristic(start2, targetEntity.location);

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
        // Debug.Log("Best Value: " + bestValue);

        // foreach (var choice in bestChoiceSquence)
        // {
        //     Debug.Log(choice.Item1.name + " at " + choice.Item2 + "~~~~~~~~~~~~~~~~~~~~~~~");
        // }

        // Return best result
        return bestChoiceSquence;
    }

    public float GetStateValue()
    {
        return 0f;
    }
}
