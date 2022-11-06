using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AStar
{   
    // Recursive connect each planned action to create a action path
    private static List<Action> TracePath(Action final)
    {
        if (final.Previous == null)
        {
            List<Action> pathStart = new List<Action>();
            pathStart.Add(final);
            return pathStart;
        }

        List<Action> path = TracePath(final.Previous);
        path.Add(final);
        return path;
    }

    // Recursively apply each action's effects to the inputted world state
    private static WorldStateList ApplyPathEffects(Action currentAction, WorldStateList currentState)
    {
        if (currentAction.Previous == null)
        {
            currentState.ApplyStateList(currentAction.Effects);
            return currentState;
        }

        WorldStateList pathState = ApplyPathEffects(currentAction.Previous, currentState);
        pathState.ApplyStateList(currentAction.Effects);
        return pathState;
    }

    // Use an A* search to find a path from the beginning world state to the desired world state of the goal
    public static List<Action> PlanAction(WorldStateList worldState, Goal goal, List<Action> actions)
    {
        // A sorted queue of all the actions yet to be processed in the algorithm
        List<Action> astarQueue = new List<Action>();
        // A list of all actions already visited, which won't be placed back into the sorted queue
        List<Action> visitedActions = new List<Action>();

        // Find all actions whose preconditions meet the starting world state
        foreach (Action action in actions)
        {
            if (worldState.MeetsRequirements(action.Preconditions))
            {
                astarQueue.Add(action);
            }
        }

        while(astarQueue.Count != 0)
        {
            // Sort and remove the lowest cost action from the queue
            astarQueue.Sort((a, b) => a.Cost.CompareTo(b.Cost));
            Action q = astarQueue[0];
            astarQueue.RemoveAt(0);

            // Apply that actions effects to the world state
            WorldStateList pathWorldState = worldState.DeepCopy();
            pathWorldState = ApplyPathEffects(q, pathWorldState);

            // If the world state now meets the goal, we are done
            if (pathWorldState.MeetsRequirements(goal.Requisites))
                return TracePath(q);

            // Loop through all the actions
            for(int i = 0; i < actions.Count; i++)
            {
                Action action = actions[i].DeepCopy();
                // Skip this action and all actions already visited
                if (action.Equals(q) || visitedActions.Contains(action))
                {
                    continue;
                }
                // Foreach action that can take place after this action
                else if (pathWorldState.MeetsRequirements(action.Preconditions))
                {
                    // Assign a connection to this action
                    action.Previous = q;
                    // Compute depth and cost
                    // Depth - the index of this action once it would be in the path
                    action.Depth = q.Depth + 1;
                    // Cost - value used to sort queue
                    action.Cost = action.Depth + action.Heuristic;

                    // If this action is not in the queue, add it
                    // If this action is in the queue but its cost is less then the one in the queue, add it
                    int queueIdx = astarQueue.IndexOf(action);
                    if (queueIdx == -1 || (queueIdx != -1 && action.Cost < astarQueue[queueIdx].Cost))
                    {
                        astarQueue.Add(action);
                    }
                }
            }

            // We've looked at all possible actions after this action
            visitedActions.Add(q);
        }

        // return null if not path can be formed from this world state
        return null;
    }
}
