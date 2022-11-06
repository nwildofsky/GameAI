using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBump : Action
{
    public ActionBump()
    {
        preconditions = new WorldStateList(
            new WorldState<object>("bumpedInto", false)
        );

        effects = new WorldStateList(
            new WorldState<object>("bumpedInto", true)
        );

        behavior = new BehaviorBump();
    }

    protected override int CalculateHeuristic()
    {
        return base.CalculateHeuristic();
    }
}
