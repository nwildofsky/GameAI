using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WorldState<T>
{
    string tag;
    T state;

    public WorldState(string tag, T state)
    {
        this.tag = tag;
        this.state = state;
    }

    public string Tag
    { get => tag; set => tag = value; }
    public T State
    { get => state; set => state = value; }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;
        WorldState<T> objAsState = obj as WorldState<T>;
        if (objAsState == null)
            return false;

        return Equals(objAsState);
    }
    public override int GetHashCode()
    {
        return tag.GetHashCode();
    }
    public bool Equals(WorldState<T> other)
    {
        if (other == null)
            return false;

        return tag == other.tag && state.Equals(other.state);
    }
}

public class WorldStateList
{
    public List<WorldState<object>> states;

    public WorldStateList(params WorldState<object>[] states)
    {
        this.states = new List<WorldState<object>>();

        foreach (WorldState<object> state in states)
        {
            this.states.Add(state);
        }
    }
    public WorldStateList DeepCopy()
    {
        WorldStateList copy = new WorldStateList();

        foreach(WorldState<object> state in states)
        {
            copy.states.Add(new WorldState<object>(state.Tag, state.State));
        }

        return copy;
    }

    // Check if all states in this world state match the inputted list
    public bool MeetsRequirements(WorldStateList requirements)
    {
        foreach (WorldState<object> condition in requirements.states)
        {
            WorldState<object> state = FindState(condition.Tag);

            if (state == null)
                return false;

            if (!state.Equals(condition))
                return false;
        }

        return true;
    }

    // Apply all the states of the inputted list to this world state
    public void ApplyStateList(WorldStateList list)
    {
        foreach (WorldState<object> state in list.states)
        {
            ApplyState(state);
        }
    }

    // Apply a single state to this world state
    public void ApplyState(WorldState<object> state)
    {
        WorldState<object> cond = FindState(state.Tag);

        if (cond != null)
            cond.State = state.State;
    }

    // Find a state within this world state given the state's tag
    public WorldState<object> FindState(string tag)
    {
        foreach(WorldState<object> state in states)
        {
            if (state.Tag == tag)
            {
                return state;
            }
        }

        return null;
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;
        WorldStateList objAsList = obj as WorldStateList;
        if (objAsList == null)
            return false;

        return Equals(objAsList);
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    public bool Equals(WorldStateList other)
    {
        if (other == null)
            return false;

        if (states.Count != other.states.Count)
            return false;
        
        for(int i = 0; i < states.Count; i++)
        {
            if (states[i].Tag != other.states[i].Tag || states[i].State != other.states[i].State)
                return false;
        }

        return true;
    }
}
