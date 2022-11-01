using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorMove : Behavior_Base
{
    Vector3 moveTo;
    float moveSpeed;
    float posCheckRange = 0.1f;

    public BehaviorMove(Vector3 moveTo, float moveSpeed)   : base()
    {
        this.moveTo = moveTo;
        this.moveSpeed = moveSpeed;
    }

    public override void Run(ActionPlanner agent)
    {
        isRunning = true;

        Vector3 curPos = agent.transform.position;
        Vector3 moveDir = Vector3.Normalize(moveTo - curPos);

        curPos += moveDir * moveSpeed * Time.deltaTime;
        agent.transform.position = curPos;

        if (curPos.x != moveTo.x && curPos.x < moveTo.x + posCheckRange && curPos.x > moveTo.x - posCheckRange)
        {
            agent.transform.position = new Vector3(moveTo.x, curPos.y, curPos.z);
        }
        if (curPos.y != moveTo.y && curPos.y < moveTo.y + posCheckRange && curPos.y > moveTo.y - posCheckRange)
        {
            agent.transform.position = new Vector3(curPos.x, moveTo.y, curPos.z);
        }
        if (curPos.z != moveTo.z && curPos.z < moveTo.z + posCheckRange && curPos.z > moveTo.z - posCheckRange)
        {
            agent.transform.position = new Vector3(curPos.x, curPos.y, moveTo.z);
        }

        if (curPos == moveTo)
        {
            isCompleted = true;
        }
    }
}
