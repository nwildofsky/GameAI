using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalSpendMoney : Goal
{
    public GoalSpendMoney()
    {
        requisites = new WorldStateList(
            new WorldState<object>("money", 0)
        );
    }

    public override float CalculatePriority(ActionPlanner agent)
    {
        float happinessPercent = (int)agent.WorldState.FindState("happiness").State / 3f;
        float richPercent = (int)agent.WorldState.FindState("money").State / 2f;
        float hungerPercent = (int)agent.WorldState.FindState("hunger").State / 2f;

        return 0.8f * richPercent + 0.205f * happinessPercent;
    }
}
