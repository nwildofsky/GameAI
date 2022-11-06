using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEatRich : Action
{
    public ActionEatRich()
    {
        preconditions = new WorldStateList(
            new WorldState<object>("hunger", 2),
            new WorldState<object>("money", 2)
        );

        effects = new WorldStateList(
            new WorldState<object>("hunger", 0),
            new WorldState<object>("money", 1)
        );

        behavior = new BehaviorEat();
    }

    protected override int CalculateHeuristic()
    {
        return base.CalculateHeuristic();
    }
}
