using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class Behavior
{
    // Behvaior states inherited by all behavior children
    protected bool isRunning;
    protected bool isCompleted;
    protected bool isMoving;
    // A small value for position checking
    protected float epsilon = 0.1f;
    // Every behavior has access to a timer
    protected float timer = 0;
    protected float timerLength = 2.5f;

    protected Behavior()
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

    // Logic behind each behavior that must be implemented by each
    public abstract void Run(ActionPlanner agent);

    // Signifies the completion of this behavior to the action planner
    protected virtual void Complete()
    {
        isRunning = false;
        isCompleted = true;
        timer = 0;
    }

    // Find a random position in a box collider given a GameObject
    protected Vector3 FindLocInBox(GameObject box)
    {
        if (box.GetComponent<BoxCollider>())
        {
            float xOffset = box.transform.right.x * (box.transform.localScale.x / 2f);
            float zOffset = box.transform.forward.z * (box.transform.localScale.z / 2f);
            float xMin = box.transform.position.x - xOffset;
            float xMax = box.transform.position.x + xOffset;
            float zMin = box.transform.position.z - zOffset;
            float zMax = box.transform.position.z + zOffset;

            return new Vector3(Random.Range(xMin, xMax), 1, Random.Range(zMin, zMax));
        }

        return Vector3.zero;
    }

    // Move directly to a position specified and lock into place when arrived
    // Returns true if the agent position has moved into place, false if not
    protected bool MoveTo(ActionPlanner agent, Vector3 dest, float speed)
    {
        isMoving = true;

        Vector3 curPos = agent.transform.position;
        dest.y = curPos.y;
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

    // Move directly away from a specified position
    // Returns true when the agent is bumping another agent
    // Returns false and moves when the agent is not bumping
    protected bool MoveFrom(ActionPlanner agent, Vector3 targetPos, float speed)
    {
        isMoving = true;

        if (agent.Bumped)
        {
            Vector3 curPos = agent.transform.position;
            targetPos.y = curPos.y;
            Vector3 moveDir = Vector3.Normalize(curPos - targetPos);

            curPos += moveDir * speed * Time.deltaTime;
            agent.transform.position = curPos;

            return false;
        }

        isMoving = false;
        return true;
    }

    // Find the closest positioned object from a list of transforms given by the children of a parent transform
    // Returns a Vector3 position
    protected Vector3 FindNearestFromChildren(ActionPlanner agent, Transform parent, Vector3 nearest)
    {
        if (nearest == Vector3.zero)
        {
            nearest = Vector3.positiveInfinity;
            foreach (Transform loc in parent.transform)
            {
                if (Vector3.Magnitude(loc.position - agent.transform.position) < Vector3.Magnitude(nearest - agent.transform.position))
                {
                    if (loc.position != agent.transform.position)
                    {
                        nearest = loc.position;
                    }
                }
            }

            return nearest;
        }

        return nearest;
    }

    // Find the closest positioned object from a list of transforms given by the children of a parent transform
    // Returns the GameObject found
    protected GameObject FindNearestFromChildren(ActionPlanner agent, Transform parent)
    {
        Vector3 nearest = Vector3.positiveInfinity;
        GameObject nearestObj = null;

        foreach (Transform loc in parent.transform)
        {
            if (Vector3.Magnitude(loc.position - agent.transform.position) < Vector3.Magnitude(nearest - agent.transform.position))
            {
                if (loc.position != agent.transform.position)
                {
                    nearest = loc.position;
                    nearestObj = loc.gameObject;
                }
            }
        }


        return nearestObj;
    }
}
