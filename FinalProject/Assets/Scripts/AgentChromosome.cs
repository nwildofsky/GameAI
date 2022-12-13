using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// A struct that represents all the data required for a single Coroutine action of the agent
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

// A data structure that represents the data required to make a chromosome for the agent
// This is a binary tree that is traversed using preorder
public class AgentChromosomeData
{
    public AgentChromosomeData(InputData data, AgentChromosomeData left = null, AgentChromosomeData right = null, AgentChromosomeData parent = null)
    {
        this.data = data;

        // Call the advanced set functions that recursively set up every relevant part of the tree
        this.Left = left;
        this.Right = right;
    }

    public InputData data;
    int size;
    private AgentChromosomeData left;
    private AgentChromosomeData right;
    private AgentChromosomeData parent;

    public int Size
    {
        get => size;
        // Set this node's size and change the size of all of its parents accordingly
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
        // Set this node's left leaf and set that leaf's parent to this node
        // Also replace the size of this node and all parents with the new size
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
        // Set this node's right leaf and set that leaf's parent to this node
        // Also replace the size of this node and all parents with the new size
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

    // This is a way get this node by value instead of by reference
    public AgentChromosomeData DeepCopy()
    {
        AgentChromosomeData root = new AgentChromosomeData(data);

        root.parent = parent;
        // Make sure to also copy by value everything from each leaf node as well
        root.Left = left != null ? left.DeepCopy() : null;
        root.Right = right != null ? right.DeepCopy() : null;

        return root;
    }
}
