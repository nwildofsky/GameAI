using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionAlone : Action
{
    public ActionAlone()
    {
        preconditions = new WorldStateList(
            new WorldState<object>("happiness", 0)
        );

        effects = new WorldStateList(
            new WorldState<object>("happiness", 2),
            new WorldState<object>("hunger", 1)
        );

        behavior = new BehaviorAlone();
    }

    protected override int CalculateHeuristic()
    {
        return base.CalculateHeuristic();
    }
}
