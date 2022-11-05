using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorDrink : Behavior
{
    GameObject pubLocs;
    GameObject nearestPub;
    Vector3 nearestPubLoc;

    public BehaviorDrink()
    {
        pubLocs = GameObject.Find("Pubs");
        nearestPubLoc = Vector3.zero;
    }

    public override void Run(ActionPlanner agent)
    {
        isRunning = true;

        nearestPub = FindNearestFromChildren(agent, pubLocs.transform);

        if (nearestPubLoc == Vector3.zero)
        {
            nearestPubLoc = FindLocInBox(nearestPub);
        }

        if (MoveTo(agent, nearestPubLoc, 2))
        {
            timer += Time.deltaTime;
        }

        if (timer >= timerLength)
        {
            Complete();
        }
    }
}
