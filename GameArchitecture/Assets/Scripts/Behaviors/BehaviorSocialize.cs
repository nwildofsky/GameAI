using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorSocialize : Behavior
{
    GameObject agentLocs;
    GameObject nearestAgent;

    public BehaviorSocialize()
    {
        agentLocs = GameObject.Find("Agents");
    }

    public override void Run(ActionPlanner agent)
    {
        isRunning = true;

        nearestAgent = FindNearestFromChildren(agent, agentLocs.transform);
        
        timer += Time.deltaTime;

        if (timer >= timerLength)
        {
            if (MoveFrom(agent, nearestAgent.transform.position, 2))
            {
                Complete();
            }
        }
    }
}
