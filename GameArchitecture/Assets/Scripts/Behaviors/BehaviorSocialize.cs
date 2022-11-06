using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorSocialize : Behavior
{
    GameObject agentLocs;
    GameObject nearestAgent;
    bool finishedSocialize;

    public BehaviorSocialize()
    {
        agentLocs = GameObject.Find("Agents");
        nearestAgent = null;
        finishedSocialize = false;
    }

    public override void Run(ActionPlanner agent)
    {
        isRunning = true;

        if (nearestAgent == null)
        {
            nearestAgent = FindNearestFromChildren(agent, agentLocs.transform);
        }

        timer += Time.deltaTime;

        if (finishedSocialize)
        {
            MoveFrom(agent, nearestAgent.transform.position, 2);
        }

        if (!finishedSocialize && timer >= timerLength)
        {
            finishedSocialize = true;
            timer = 0;
            timerLength = 1;
        }

        if (finishedSocialize && timer >= timerLength)
        {
            Complete();
        }
    }

    protected override void Complete()
    {
        base.Complete();
        nearestAgent = null;
        finishedSocialize = false;
    }
}
