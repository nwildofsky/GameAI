using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    public GameObject agent;
    public GameObject agentParent;
    public int agentsPerGeneration;
    [System.NonSerialized]
    public int generation;

    List<RacerAgent> racerAgents;
    float crossoverChance;
    float mutationChance;

    // Start is called before the first frame update
    void Start()
    {
        racerAgents = new List<RacerAgent>();
        generation = 0;
        Random.InitState((int)Time.time);

        GameObject start = GameObject.FindGameObjectWithTag("TrackStart");
        for (int i = 0; i < agentsPerGeneration; i++)
        {
            RacerAgent newAgent = Instantiate(agent, start.transform.position, start.transform.rotation, agentParent.transform).transform.GetChild(0).GetComponent<RacerAgent>();

            int chromosomeSize = Random.Range(2, 10);
            newAgent.ChromosomeData = CreateRandomChromosome(chromosomeSize);
            newAgent.ChromosomeSize = chromosomeSize;
            newAgent.Run();

            racerAgents.Add(newAgent);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // https://stackoverflow.com/questions/56873764/how-to-randomly-generate-a-binary-tree-given-the-node-number
    AgentChromosomeData CreateRandomChromosome(int treeSize)
    {
        if (treeSize == 0)
            return null;

        AgentChromosomeData root = new AgentChromosomeData(new InputData(GetRandomDuration(), GetRandomMovementInput(), GetRandomTurnDirection()));

        int sizeLeft = Random.Range(0, treeSize);

        root.Left = CreateRandomChromosome(sizeLeft);
        root.Right = CreateRandomChromosome(treeSize - sizeLeft - 1);

        return root;
    }

    float EvaluateFitness(RacerAgent agent)
    {

    }

    void CreateNewGeneration()
    {

    }

    void PerformCrossover(RacerAgent parent1, RacerAgent parent2)
    {
        int maxCrossoverPoint = Mathf.Max(parent1.ChromosomeSize, parent2.ChromosomeSize);
        int location = Random.Range(0, maxCrossoverPoint);

        AgentChromosomeData section1 = GetTreeAt(parent1.ChromosomeData, location);
        AgentChromosomeData section2 = GetTreeAt(parent2.ChromosomeData, location);

        AgentChromosomeData tempSection1 = section1.DeepCopy();
        if (section1.Parent != null)
        {
            section1.Parent = section2.Parent;
            if (section2.Parent.Left == section2)
            {
                section2.Parent.Left = section1;
            }
            else
            {
                section2.Parent.Right = section1;
            }
        }
        else
        {
            parent2.ChromosomeData = section1;
        }

        if (section2.Parent != null)
        {
            section2.Parent = tempSection1.Parent;
            if (tempSection1.Parent.Left == tempSection1)
            {
                tempSection1.Parent.Left = section2;
            }
            else
            {
                tempSection1.Parent.Right = section2;
            }

            parent1.ChromosomeData = GetRootFromNode(section2);
        }
        else
        {
            parent1.ChromosomeData = section2;
        }
    }

    void PerformMutation(RacerAgent agent)
    {
        int numMutations = Random.Range(0, 4);
        for (int i = 0; i < numMutations; i++)
        {
            AgentChromosomeData randomNode = GetTreeAt(agent.ChromosomeData, Random.Range(0, agent.ChromosomeSize));
            randomNode.data = new InputData(GetRandomDuration(), GetRandomMovementInput(), GetRandomTurnDirection());
        }
    }

    AgentChromosomeData GetTreeAt(AgentChromosomeData root, int traverseIdx)
    {
        if (root == null)
            return null;

        if (traverseIdx == 0)
            return root;

        if (root.Left != null)
        {
            return GetTreeAt(root.Left, traverseIdx - 1);
        }
        else if (root.Right != null)
        {
            return GetTreeAt(root.Right, traverseIdx - 1);
        }
        else if (root.Parent != null)
        {
            AgentChromosomeData x = root.Parent;
            AgentChromosomeData y = root;
            while ((x.Right == y || x.Right == null) && x.Parent != null)
            {
                x = x.Parent;
                y = y.Parent;
            }
            if (x.Right != y && x.Right != null)
            {
                return GetTreeAt(x.Right, traverseIdx - 1);
            }
        }

        return root;
    }

    AgentChromosomeData GetRootFromNode(AgentChromosomeData node)
    {
        while (node.Parent != null)
        {
            node = node.Parent;
        }

        return node;
    }

    float GetRandomDuration()
    {
        return Random.Range(0.0f, 3.0f);
    }

    int GetRandomMovementInput()
    {
        return Random.Range(0, 3);
    }

    int GetRandomTurnDirection()
    {
        int direction = Random.Range(-2, 3);
        if (direction == -2)
        {
            direction = -1;
        }
        else if (direction == 3)
        {
            direction = 1;
        }
        else
        {
            direction = 0;
        }

        return direction;
    }
}
