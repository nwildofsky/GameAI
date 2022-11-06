using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionDrink : Action
{
    public ActionDrink()
    {
        preconditions = new WorldStateList(
            new WorldState<object>("hunger", 1)
        );

        effects = new WorldStateList(
            new WorldState<object>("hunger", 0)
        );

        behavior = new BehaviorDrink();
    }

    protected override int CalculateHeuristic()
    {
        return base.CalculateHeuristic();
    }
}
