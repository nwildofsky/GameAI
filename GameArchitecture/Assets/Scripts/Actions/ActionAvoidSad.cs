using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionAvoidSad : Action
{
    public ActionAvoidSad()
    {
        preconditions = new WorldStateList(
            new WorldState<object>("bumpedInto", true),
            new WorldState<object>("happiness", 0)
        );

        effects = new WorldStateList(
            new WorldState<object>("bumpedInto", false)
        );

        behavior = new BehaviorAvoid();
    }

    protected override int CalculateHeuristic()
    {
        return base.CalculateHeuristic();
    }
}
