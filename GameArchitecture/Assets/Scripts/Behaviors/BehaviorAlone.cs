using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorAlone : Behavior
{
    GameObject center;

    public BehaviorAlone()
    {
        center = GameObject.Find("Center");
    }

    public override void Run(ActionPlanner agent)
    {
        isRunning = true;

        if (MoveFrom(agent, center.transform.position, 2))
        {
            timer += Time.deltaTime;
        }

        if (timer >= timerLength)
        {
            Complete();
        }
    }
}
