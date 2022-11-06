using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionShop : Action
{
    public ActionShop()
    {
        preconditions = new WorldStateList(
            new WorldState<object>("money", 1)
        );

        effects = new WorldStateList(
            new WorldState<object>("money", 0),
            new WorldState<object>("happiness", 2)
        );

        behavior = new BehaviorShop();
    }

    protected override int CalculateHeuristic()
    {
        return base.CalculateHeuristic();
    }
}
