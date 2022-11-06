using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionAvoidAngry : Action
{
    public ActionAvoidAngry()
    {
        preconditions = new WorldStateList(
            new WorldState<object>("bumpedInto", true),
            new WorldState<object>("happiness", 1)
        );

        effects = new WorldStateList(
            new WorldState<object>("bumpedInto", false),
            new WorldState<object>("happiness", 0)
        );

        behavior = new BehaviorAvoid();
    }

    protected override int CalculateHeuristic()
    {
        return base.CalculateHeuristic();
    }
}
