using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionShopRich : Action
{
    public ActionShopRich()
    {
        preconditions = new WorldStateList(
            new WorldState<object>("money", 2)
        );

        effects = new WorldStateList(
            new WorldState<object>("money", 0),
            new WorldState<object>("happiness", 3),
            new WorldState<object>("hunger", 2)
        );

        behavior = new BehaviorShop();
    }

    protected override int CalculateHeuristic()
    {
        return base.CalculateHeuristic();
    }
}
