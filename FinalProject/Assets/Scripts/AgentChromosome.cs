using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public struct InputData
{
    public InputData(float duration, int movement, int turnDirection)
    {
        this.duration = duration;
        this.movement = movement;
        this.turnDirection = turnDirection;
    }

    public float duration;
    public int movement;
    public int turnDirection;
}

public class AgentChromosomeData
{
    public AgentChromosomeData(InputData data, AgentChromosomeData left = null, AgentChromosomeData right = null, AgentChromosomeData parent = null)
    {
        this.data = data;
        
        this.left = left;
        if (this.left != null)
            this.left.parent = this;

        this.right = right;
        if (this.right != null)
            this.right.parent = this;
    }

    public InputData data;
    private AgentChromosomeData left;
    private AgentChromosomeData right;
    private AgentChromosomeData parent;

    public AgentChromosomeData Left
    {
        get => this.left;
        set
        {
            this.left = value;
            if (this.left != null)
                this.left.parent = this;
        }
    }

    public AgentChromosomeData Right
    {
        get => this.right;
        set
        {
            this.right = value;
            if (this.right != null)
                this.right.parent = this;
        }
    }

    public AgentChromosomeData Parent
    {
        get => this.parent;
        set => this.parent = value;
    }

    public AgentChromosomeData DeepCopy()
    {
        return new AgentChromosomeData(data, left.DeepCopy(), right.DeepCopy(), parent.DeepCopy());
    }
}
