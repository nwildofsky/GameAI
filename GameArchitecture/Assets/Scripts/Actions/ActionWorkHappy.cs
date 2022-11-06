using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionWorkHappy : Action
{
    public ActionWorkHappy()
    {
        preconditions = new WorldStateList(
            new WorldState<object>("happiness", 2)
        );

        effects = new WorldStateList(
            new WorldState<object>("money", 1),
            new WorldState<object>("happiness", 1),
            new WorldState<object>("hunger", 2)
        );

        behavior = new BehaviorWork();
    }

    protected override int CalculateHeuristic()
    {
        return base.CalculateHeuristic();
    }
}
