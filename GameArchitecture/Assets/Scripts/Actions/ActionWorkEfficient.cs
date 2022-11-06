using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionWorkEfficient : Action
{
    public ActionWorkEfficient()
    {
        preconditions = new WorldStateList(
            new WorldState<object>("happiness", 1)
        );

        effects = new WorldStateList(
            new WorldState<object>("money", 2),
            new WorldState<object>("happiness", 0),
            new WorldState<object>("hunger", 2)
        );

        behavior = new BehaviorWork();
    }

    protected override int CalculateHeuristic()
    {
        return base.CalculateHeuristic();
    }
}
