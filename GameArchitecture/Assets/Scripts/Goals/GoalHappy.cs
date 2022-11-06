using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalHappy : Goal
{
    public GoalHappy()
    {
        requisites = new WorldStateList(
            new WorldState<object>("happiness", 3)
        );
    }

    public override float CalculatePriority(ActionPlanner agent)
    {
        float happinessPercent = (int)agent.WorldState.FindState("happiness").State / 3f;
        float richPercent = (int)agent.WorldState.FindState("money").State / 2f;

        return (1 - happinessPercent) + 0.4f * (1 - richPercent);
    }
}
