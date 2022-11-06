using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalResolveBump : Goal
{
    public GoalResolveBump()
    {
        requisites = new WorldStateList(
            new WorldState<object>("bumpedInto", false)
        );
    }

    public override float CalculatePriority(ActionPlanner agent)
    {
        if (agent.Bumped)
            return 5;

        return 0;
    }
}
