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
        //this.size = size;

        this.Left = left;
        //if (this.left != null)
        //    this.left.parent = this;

        this.Right = right;
        //if (this.right != null)
        //    this.right.parent = this;
    }

    public InputData data;
    int size;
    private AgentChromosomeData left;
    private AgentChromosomeData right;
    private AgentChromosomeData parent;

    public int Size
    {
        get => size;
        set
        {
            this.size = value;

            if (this.parent != null)
            {
                this.parent.Size = 1 + this.size;
            }
        }
    }

    public AgentChromosomeData Left
    {
        get => this.left;
        set
        {
            this.left = value;
            this.size = 1;

            if (this.left != null)
            {
                this.left.parent = this;
                this.size += this.left.size;
            }

            if (this.right != null)
            {
                this.size += this.right.size;
            }

            this.Size = this.size;
        }
    }

    public AgentChromosomeData Right
    {
        get => this.right;
        set
        {
            this.right = value;
            this.size = 1;

            if (this.right != null)
            {
                this.right.parent = this;
                this.size += this.right.size;
            }

            if (this.left != null)
            {
                this.size += this.left.size;
            }

            this.Size = this.size;
        }
    }

    public AgentChromosomeData Parent
    {
        get => this.parent;
        set => this.parent = value;
    }

    public AgentChromosomeData DeepCopy()
    {
        AgentChromosomeData root = new AgentChromosomeData(data);

        root.parent = parent;
        root.Left = left != null ? left.DeepCopy() : null;
        root.Right = right != null ? right.DeepCopy() : null;

        return root;
    }
}
