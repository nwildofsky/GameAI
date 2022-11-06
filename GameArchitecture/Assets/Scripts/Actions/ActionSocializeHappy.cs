using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSocializeHappy : Action
{
    public ActionSocializeHappy()
    {
        preconditions = new WorldStateList(
            new WorldState<object>("bumpedInto", true),
            new WorldState<object>("happiness", 2)
        );

        effects = new WorldStateList(
            new WorldState<object>("bumpedInto", false),
            new WorldState<object>("happiness", 3),
            new WorldState<object>("hunger", 1)
        );

        behavior = new BehaviorSocialize();
    }

    protected override int CalculateHeuristic()
    {
        return base.CalculateHeuristic();
    }
}
