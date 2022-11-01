using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;


public abstract class Behavior_Base
{
    protected bool isRunning;
    protected bool isCompleted;

    protected Behavior_Base()
    {
        isRunning = false;
        isCompleted = false;
    }

    public bool IsRunning
    { get => isRunning; set => isRunning = value; }
    public bool IsCompleted
    { get => isCompleted; set => isCompleted = value; }

    public abstract void Run(ActionPlanner agent);
}
