using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorAvoid : Behavior
{
    GameObject agentLocs;
    GameObject nearestAgent;

    public BehaviorAvoid()
    {
        agentLocs = GameObject.Find("Agents");
        nearestAgent = null;
        timerLength = 1f;
    }

    public override void Run(ActionPlanner agent)
    {
        isRunning = true;

        if (nearestAgent == null)
        {
            nearestAgent = FindNearestFromChildren(agent, agentLocs.transform);
        }

        if (MoveFrom(agent, nearestAgent.transform.position, 2))
        {
            timer += Time.deltaTime;
        }

        if (timer >= timerLength)
        {
            Complete();
        }
    }

    protected override void Complete()
    {
        base.Complete();
        nearestAgent = null;
    }
}
