using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorWork : Behavior
{
    GameObject workLocs;
    GameObject nearestWork;
    Vector3 nearestWorkLoc;
    GameObject center;
    bool finishedWork;

    public BehaviorWork()
    {
        workLocs = GameObject.Find("Jobs");
        center = GameObject.Find("Center");
        nearestWorkLoc = Vector3.zero;
        finishedWork = false;
    }

    public override void Run(ActionPlanner agent)
    {
        isRunning = true;

        nearestWork = FindNearestFromChildren(agent, workLocs.transform);

        if (nearestWorkLoc == Vector3.zero)
        {
            nearestWorkLoc = FindLocInBox(nearestWork);
        }

        if (!finishedWork && MoveTo(agent, nearestWorkLoc, 2))
        {
            agent.GetComponent<MeshRenderer>().enabled = false;
            agent.GetComponent<SphereCollider>().enabled = false;
            timer += Time.deltaTime;
        }

        if (!finishedWork && timer >= timerLength)
        {
            agent.GetComponent<MeshRenderer>().enabled = true;
            finishedWork = true;
            timerLength = Random.Range(1f, 2.3f);
            timer = 0;
        }

        if (finishedWork)
        {
            MoveTo(agent, center.transform.position, 2);
            timer += Time.deltaTime;
            if (timer >= timerLength)
            {
                agent.GetComponent<SphereCollider>().enabled = true;
                Complete();
            }
        }
    }

    protected override void Complete()
    {
        base.Complete();

        timerLength = 2.5f;
        finishedWork = false;
    }
}
