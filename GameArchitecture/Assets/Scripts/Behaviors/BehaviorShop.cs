using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorShop : Behavior
{
    GameObject shopLocs;
    GameObject nearestShop;
    Vector3 nearestShopLoc;

    public BehaviorShop()
    {
        shopLocs = GameObject.Find("Shops");
        nearestShopLoc = Vector3.zero;
    }

    public override void Run(ActionPlanner agent)
    {
        isRunning = true;

        nearestShop = FindNearestFromChildren(agent, shopLocs.transform);

        if (nearestShopLoc == Vector3.zero)
        {
            nearestShopLoc = FindLocInBox(nearestShop);
        }

        if (MoveTo(agent, nearestShopLoc, 2))
        {
            timer += Time.deltaTime;
        }

        if (timer >= timerLength)
        {
            Complete();
        }
    }
}
