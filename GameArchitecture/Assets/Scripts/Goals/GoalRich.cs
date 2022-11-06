using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalRich : Goal
{
    public GoalRich()
    {
        requisites = new WorldStateList(
            new WorldState<object>("money", 2)
        );
    }

    public override float CalculatePriority(ActionPlanner agent)
    {
        float happinessPercent = (int)agent.WorldState.FindState("happiness").State / 3f;
        float richPercent = (int)agent.WorldState.FindState("money").State / 2f;

        return (1 - richPercent) + 0.04f * happinessPercent;
    }
}
