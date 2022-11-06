using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionWorkSad : Action
{
    public ActionWorkSad()
    {
        preconditions = new WorldStateList(
            new WorldState<object>("happiness", 0)
        );

        effects = new WorldStateList(
            new WorldState<object>("money", 1),
            new WorldState<object>("hunger", 2)
        );

        behavior = new BehaviorWork();
    }

    protected override int CalculateHeuristic()
    {
        return base.CalculateHeuristic();
    }
}
