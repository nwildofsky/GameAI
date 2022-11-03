using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class Behavior_Base
{
    protected bool isRunning;
    protected bool isCompleted;
    protected bool isMoving;
    protected float epsilon = 0.1f;
    protected float timer = 0;
    protected float timerLength = 2.5f;

    protected Behavior_Base()
    {
        isRunning = false;
        isCompleted = false;
    }

    public bool IsRunning
    { get => isRunning; set => isRunning = value; }
    public bool IsCompleted
    { get => isCompleted; set => isCompleted = value; }
    public bool IsMoving
    { get => isMoving; }

    public abstract void Run(ActionPlanner agent);

    protected virtual void Complete()
    {
        isRunning = false;
        isCompleted = true;
        timer = 0;
    }

    protected bool MoveTo(ActionPlanner agent, GameObject dest, float speed)
    {
        Mesh destBox = dest.GetComponent<MeshFilter>().mesh;
        if (destBox != null)
        {
            float xMin = destBox.bounds.min.x;
            float xMax = destBox.bounds.max.x;
            float zMin = destBox.bounds.min.z;
            float zMax = destBox.bounds.max.z;

            Vector3 destPos = new Vector3(Random.Range(xMin, xMax), 1, Random.Range(zMin, zMax));
            return MoveTo(agent, destPos, speed);
        }

        return MoveTo(agent, dest.transform.position, speed);
    }

    protected bool MoveTo(ActionPlanner agent, Vector3 dest, float speed)
    {
        isMoving = true;

        Vector3 curPos = agent.transform.position;
        Vector3 moveDir = Vector3.Normalize(dest - curPos);

        curPos += moveDir * speed * Time.deltaTime;
        agent.transform.position = curPos;

        if (curPos.x != dest.x && curPos.x < dest.x + epsilon && curPos.x > dest.x - epsilon)
        {
            agent.transform.position = new Vector3(dest.x, curPos.y, curPos.z);
        }
        if (curPos.y != dest.y && curPos.y < dest.y + epsilon && curPos.y > dest.y - epsilon)
        {
            agent.transform.position = new Vector3(curPos.x, dest.y, curPos.z);
        }
        if (curPos.z != dest.z && curPos.z < dest.z + epsilon && curPos.z > dest.z - epsilon)
        {
            agent.transform.position = new Vector3(curPos.x, curPos.y, dest.z);
        }

        if (curPos == dest)
        {
            isMoving = false;
            return true;
        }

        return false;
    }

    protected Vector3 FindNearestFromChildren(ActionPlanner agent, Transform parent, Vector3 nearest)
    {
        if (nearest == Vector3.zero)
        {
            foreach (Transform loc in parent.transform)
            {
                if (Vector3.Magnitude(loc.position - agent.transform.position) < Vector3.Magnitude(nearest))
                    nearest = loc.position;
            }

            return nearest;
        }

        return nearest;
    }

    protected GameObject FindNearestFromChildren(ActionPlanner agent, Transform parent)
    {
        Vector3 nearest = Vector3.zero;
        GameObject nearestObj = null;

        foreach (Transform loc in parent.transform)
        {
            if (Vector3.Magnitude(loc.position - agent.transform.position) < Vector3.Magnitude(nearest))
            {
                nearest = loc.position;
                nearestObj = loc.gameObject;
            }
        }

            return nearestObj;
        }
    }
}
