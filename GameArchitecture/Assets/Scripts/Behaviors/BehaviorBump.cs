using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorBump : Behavior
{
    GameObject agentLocs;
    GameObject nearestAgent;

    public BehaviorBump()
    {
        agentLocs = GameObject.Find("Agents");
        timerLength = 0.5f;
    }

    public override void Run(ActionPlanner agent)
    {
        isRunning = true;

        nearestAgent = FindNearestFromChildren(agent, agentLocs.transform);

        if (nearestAgent && nearestAgent.GetComponent<SphereCollider>().enabled)
        {
            MoveTo(agent, nearestAgent.transform.position, 2);
        }

        if (agent.Bumped)
        {
            Complete();
        }
    }
}
