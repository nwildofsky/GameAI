using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class Action
{
    WorldStateList preconditions;
    WorldStateList effects;
    int heuristic;
    Behavior_Base behavior;

    public Action()
    {
        preconditions = new WorldStateList();
        effects = new WorldStateList();
        heuristic = 0;
    }
    public Action(WorldStateList preconditions, WorldStateList effects, Behavior_Base behavior, int heuristic = 0)
    {
        this.preconditions = preconditions;
        this.effects = effects;
        this.behavior = behavior;
        this.heuristic = heuristic;
    }
    public Action DeepCopy()
    {
        Action copy = new Action();
        copy.preconditions = this.preconditions.DeepCopy();
        copy.effects = this.effects.DeepCopy();
        copy.behavior = this.behavior;
        copy.Previous = this.Previous;
        copy.Depth = this.Depth;
        copy.Heuristic = this.Heuristic;
        copy.Cost = this.Cost;

        return copy;
    }

    public Action Previous
    { get; set; }
    public WorldStateList Preconditions
    { get => preconditions; }
    public WorldStateList Effects
    { get => effects; }
    public Behavior_Base Behavior
    { get => behavior; }
    public int Depth
    { get; set; }
    public int Heuristic
    {
        get
        {
            if (heuristic == 0)
                return CalculateHeuristic();

            return heuristic;
        }
        set => heuristic = value;
    }
    public int Cost
    { get; set; }

    protected virtual int CalculateHeuristic()
    {
        return 0;
    }

    public void ApplyEffects(ref WorldStateList worldState)
    {
        foreach(WorldState<object> effect in effects.states)
        {
            WorldState<object> cond = worldState.FindState(effect.Tag);

            if (cond != null)
                cond.State = effect.State;
        }
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;
        Action objAsAction = obj as Action;
        if (objAsAction == null)
            return false;

        return Equals(objAsAction);
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    public bool Equals(Action other)
    {
        if (other == null)
            return false;

        return preconditions.Equals(other.Preconditions) && effects.Equals(other.Effects);
    }
}
