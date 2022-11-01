using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal
{
    WorldStateList requisites;

    public Goal(WorldStateList requisites)
    {
        this.requisites = requisites;
    }

    public WorldStateList Requisites
    { get => requisites; }

    public float CalculatePriority(ActionPlanner agent)
    {
        return 0;
    }
}
