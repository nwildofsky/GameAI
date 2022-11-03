using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorEat : Behavior_Base
{
    GameObject diningLocs;
    GameObject nearestDining;

    public BehaviorEat()
    {
        diningLocs = GameObject.Find("Diners");
        nearestDining = null;
    }

    public override void Run(ActionPlanner agent)
    {
        nearestDining = FindNearestFromChildren(agent, diningLocs.transform);

        isRunning = true;

        if (MoveTo(agent, nearestDining, 2))
        {
            timer += Time.deltaTime;
        }

        if (timer >= timerLength)
        {
            Complete();
        }
    }
}
