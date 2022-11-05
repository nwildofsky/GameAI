using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorEat : Behavior
{
    GameObject diningLocs;
    GameObject nearestDining;
    Vector3 nearestDiningLoc;

    public BehaviorEat()
    {
        diningLocs = GameObject.Find("Diners");
        nearestDiningLoc = Vector3.zero;
    }

    public override void Run(ActionPlanner agent)
    {
        isRunning = true;

        nearestDining = FindNearestFromChildren(agent, diningLocs.transform);

        if (nearestDiningLoc == Vector3.zero)
        {
            nearestDiningLoc = FindLocInBox(nearestDining);
        }

        if (MoveTo(agent, nearestDiningLoc, 2))
        {
            timer += Time.deltaTime;
        }

        if (timer >= timerLength)
        {
            Complete();
        }
    }
}
