using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal
{
    protected WorldStateList requisites;

    public Goal()
    {
        requisites = new WorldStateList();
    }

    public Goal(WorldStateList requisites)
    {
        this.requisites = requisites;
    }

    public WorldStateList Requisites
    { get => requisites; }

    public virtual float CalculatePriority(ActionPlanner agent)
    {
        return 0;
    }
}
