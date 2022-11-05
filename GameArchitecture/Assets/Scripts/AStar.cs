using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AStar
{   
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

    public static List<Action> PlanAction(WorldStateList worldState, Goal goal, List<Action> actions)
    {
        List<Action> astarQueue = new List<Action>();
        List<Action> visitedActions = new List<Action>();

        foreach (Action action in actions)
        {
            if (worldState.MeetsRequirements(action.Preconditions))
            {
                astarQueue.Add(action);
            }
        }

        while(astarQueue.Count != 0)
        {
            astarQueue.Sort((a, b) => a.Cost.CompareTo(b.Cost));
            Action q = astarQueue[0];
            astarQueue.RemoveAt(0);

            WorldStateList pathWorldState = worldState.DeepCopy();
            pathWorldState = ApplyPathEffects(q, pathWorldState);

            if (pathWorldState.MeetsRequirements(goal.Requisites))
                return TracePath(q);

            for(int i = 0; i < actions.Count; i++)
            {
                Action action = actions[i].DeepCopy();
                if (action.Equals(q) /*|| visitedActions.Contains(action)*/)
                {
                    continue;
                }
                else if (pathWorldState.MeetsRequirements(action.Preconditions))
                {
                    action.Previous = q;
                    action.Depth = q.Depth + 1;
                    action.Cost = action.Depth + action.Heuristic;

                    int queueIdx = astarQueue.IndexOf(action);
                    if (queueIdx == -1 || (queueIdx != -1 && action.Cost < astarQueue[queueIdx].Cost))
                    {
                        astarQueue.Add(action);
                    }
                }
            }

            //visitedActions.Add(q);
        }

        return null;
    }
}
