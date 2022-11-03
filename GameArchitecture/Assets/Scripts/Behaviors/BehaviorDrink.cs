using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorDrink : Behavior_Base
{
    GameObject pubLocs;
    GameObject nearestPub;

    public BehaviorDrink()
    {
        pubLocs = GameObject.Find("Pubs");
        nearestPub = null;
    }

    public override void Run(ActionPlanner agent)
    {
        nearestPub = FindNearestFromChildren(agent, pubLocs.transform);

        isRunning = true;

        if (MoveTo(agent, nearestPub, 2))
        {
            timer += Time.deltaTime;
        }

        if (timer >= timerLength)
        {
            Complete();
        }
    }
}
