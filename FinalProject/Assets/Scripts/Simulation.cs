using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Simulation : MonoBehaviour
{
    public GameObject agent;
    public GameObject agentParent;
    public Material activeMaterial;
    public Material inactiveMaterial;
    public int agentsPerGeneration;
    public float fitnessRequirement;
    public float crossoverChance;
    public float mutationChance;
    [System.NonSerialized]
    public int generation;

    List<RacerAgent> racerAgents;
    RacerAgent finalAgent;
    float sumFitnessInCurGeneration;

    TextMeshProUGUI generationText;
    TextMeshProUGUI timeScaleText;
    int numTilesInTrack;

    // Start is called before the first frame update
    void Start()
    {
        racerAgents = new List<RacerAgent>();
        finalAgent = null;
        generation = 0;
        Random.InitState(System.DateTime.Now.Millisecond);

        numTilesInTrack = 0;
        Transform track = GameObject.Find("Racetrack").transform;
        for (int i = 0; i < track.childCount; i++)
        {
            if (track.GetChild(i).gameObject.layer == 8)
                numTilesInTrack++;
        }


        GameObject start = GameObject.FindGameObjectWithTag("TrackStart");
        GameObject finishLineCollider = GameObject.FindGameObjectWithTag("FinishLine");
        finishLineCollider.transform.position = start.transform.position;
        finishLineCollider.transform.rotation = start.transform.rotation;

        generationText = GameObject.FindGameObjectWithTag("GenerationText").GetComponent<TextMeshProUGUI>();
        timeScaleText = GameObject.FindGameObjectWithTag("TimeScaleText").GetComponent<TextMeshProUGUI>();

        if (agentsPerGeneration % 2 != 0)
        {
            agentsPerGeneration++;
        }

        for (int i = 0; i < agentsPerGeneration; i++)
        {
            RacerAgent newAgent = Instantiate(agent, start.transform.position, start.transform.rotation, agentParent.transform).transform.GetChild(0).GetComponent<RacerAgent>();
            racerAgents.Add(newAgent);
        }

        CreateNewGeneration();
    }

    // Update is called once per frame
    void Update()
    {
        if (finalAgent != null)
        {
            if (finalAgent.CompletedActions)
            {
                ResetAgentVariables(finalAgent);
                finalAgent.Run();
            }
        }
        else
        {
            foreach (RacerAgent agent in racerAgents)
            {
                if (!agent.CompletedActions)
                    return;

                if (agent.Fitness == 0.0f)
                {
                    agent.Fitness = EvaluateFitness(agent);
                    agent.transform.GetComponentInChildren<MeshRenderer>().material = inactiveMaterial;
                }
            }

            generation++;
            CreateNewGeneration();
        }
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

    void CreateNewGeneration()
    {
        if (generation == 0)
        {
            sumFitnessInCurGeneration = -1.0f;

            foreach (RacerAgent agent in racerAgents)
            {
                agent.ChromosomeData = CreateRandomChromosome(Random.Range(2, 10));
            }
        }
        else
        {
            sumFitnessInCurGeneration = -1.0f;

            List<RacerAgent> newGeneration = new List<RacerAgent>();
            List<AgentChromosomeData> originalChromosomes = new List<AgentChromosomeData>();
            foreach (RacerAgent agent in racerAgents)
            {
                originalChromosomes.Add(agent.ChromosomeData.DeepCopy());

                if (agent.Fitness > fitnessRequirement)
                {
                    finalAgent = agent;
                    ResetAgentVariables(finalAgent);
                    DestroyNonFinalAgents();
                    finalAgent.Run();
                    return;
                }
            }

            int agentIdx = 0;
            while (newGeneration.Count < agentsPerGeneration)
            {
                RacerAgent[] parents =
                {
                    racerAgents[agentIdx++],
                    racerAgents[agentIdx++] 
                };

                AgentChromosomeData[] selectedData = PerformSelection(racerAgents, originalChromosomes);
                parents[0].ChromosomeData = selectedData[0];
                parents[1].ChromosomeData = selectedData[1];

                float crossoverProbability = Random.Range(0.0f, 1.0f);
                if (crossoverProbability < crossoverChance)
                {
                    PerformCrossover(parents[0], parents[1]);
                }

                foreach (RacerAgent agent in parents)
                {
                    float mutationProbability = Random.Range(0.0f, 1.0f);
                    if (mutationProbability < mutationChance)
                    {
                        PerformMutation(agent);
                    }
                }

                newGeneration.Add(parents[0]);
                newGeneration.Add(parents[1]);
            }

            if (newGeneration.Count > agentsPerGeneration)
            {
                agentsPerGeneration = newGeneration.Count;
            }

            racerAgents = newGeneration;
        }

        foreach (RacerAgent agent in racerAgents)
        {
            ResetAgentVariables(agent);
            agent.Run();
        }

        generationText.text = "Generation: " + generation;
    }

    float EvaluateFitness(RacerAgent agent)
    {
        //float speedPercent = agent.AvgSpeed / agent.MaxSpeedInSimulation / 2.0f;
        float insideBoundsPercent = agent.InsideBoundsTime / (agent.InsideBoundsTime + agent.OutOfBoundsTime);
        float visitedTrackPercent = agent.VisitedTilesCount / (float)numTilesInTrack;
        float completionPercent = agent.CompletedTrack ? 0.5f : 0.0f;

        float fitness = (insideBoundsPercent + visitedTrackPercent * 1.5f + completionPercent) / 3.0f;

        return fitness;
    }

    // Roulette Wheel Selection
    AgentChromosomeData[] PerformSelection(List<RacerAgent> agents, List<AgentChromosomeData> originalData)
    {
        // Sort the agent list so that agents with a higher fitness are placed in the beginning of the list
        //agents.Sort((a, b) => b.Fitness.CompareTo(a.Fitness));

        if (sumFitnessInCurGeneration == -1.0f)
        {
            sumFitnessInCurGeneration = 0.0f;
            foreach (RacerAgent agent in agents)
            {
                sumFitnessInCurGeneration += agent.Fitness;
            }
        }

        AgentChromosomeData[] selected = new AgentChromosomeData[2];
        int firstSelectedIndex = -1;
        for (int i = 0; i < 2; i++)
        {
            float spin = Random.Range(0.0f, sumFitnessInCurGeneration);
            float partialSum = 0.0f;

            for (int j = 0; j < agents.Count; j++)
            {
                if (j == firstSelectedIndex)
                    continue;

                if (spin >= partialSum && spin < agents[j].Fitness + partialSum)
                {
                    selected[i] = originalData[j].DeepCopy();

                    if (i == 0)
                        firstSelectedIndex = j;
                    
                    break;
                }

                partialSum += agents[j].Fitness;
            }

            if (i == 0)
                sumFitnessInCurGeneration -= agents[firstSelectedIndex].Fitness;
        }

        sumFitnessInCurGeneration += agents[firstSelectedIndex].Fitness;

        return selected;
    }

    void PerformCrossover(RacerAgent parent1, RacerAgent parent2)
    {
        //int maxCrossoverPoint = Mathf.Min(parent1.ChromosomeData.Size, parent2.ChromosomeData.Size);
        //int location = Random.Range(0, maxCrossoverPoint);
        int location1 = Random.Range(0, parent1.ChromosomeData.Size);
        int location2 = Random.Range(0, parent2.ChromosomeData.Size);

        AgentChromosomeData section1 = parent1.ChromosomeList[location1];
        AgentChromosomeData section2 = parent2.ChromosomeList[location2];
        AgentChromosomeData tempSection1 = section1.DeepCopy();

        // Place section1 at the location of section2 in parent2
        if (section2.Parent != null)
        {
            //section1.Parent = section2.Parent;
            
            if (section2.Parent.Left == section2)
            {
                section2.Parent.Left = section1;
            }
            else
            {
                section2.Parent.Right = section1;
            }

            parent2.ChromosomeData = GetRootFromNode(section1);
        }
        else
        {
            parent2.ChromosomeData = section1;
        }

        // Place section2 at the location of section1 in parent1
        if (tempSection1.Parent != null)
        {
            //section2.Parent = tempSection1.Parent;
            
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
        int numMutations = Random.Range(1, 4);
        for (int i = 0; i < numMutations; i++)
        {
            AgentChromosomeData randomNode = agent.ChromosomeList[Random.Range(0, agent.ChromosomeList.Count)];
            randomNode.data = new InputData(GetRandomDuration(), GetRandomMovementInput(), GetRandomTurnDirection());
        }
    }

    /*
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
    */

    /*
    int GetTreeSize(AgentChromosomeData root)
    {
        if (root == null)
            return 0;

        if (root.Left != null)
        {
            return 1 + GetTreeSize(root.Left);
        }
        else if (root.Right != null)
        {
            return 1 + GetTreeSize(root.Right);
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
                return 1 + GetTreeSize(x.Right);
            }
        }

        return 1;
    }
    */

    AgentChromosomeData GetRootFromNode(AgentChromosomeData node)
    {
        while (node.Parent != null)
        {
            node = node.Parent;
        }

        return node;
    }

    void ResetAgentVariables(RacerAgent agent)
    {
        GameObject start = GameObject.FindGameObjectWithTag("TrackStart");

        agent.gameObject.transform.position = start.transform.position;
        agent.gameObject.transform.rotation = start.transform.rotation;

        agent.transform.GetComponentInChildren<MeshRenderer>().material = activeMaterial;

        agent.ResetVariables();
    }

    void DestroyNonFinalAgents()
    {
        foreach (RacerAgent agent in racerAgents)
        {
            if (agent != finalAgent)
            {
                Destroy(agent.gameObject);
            }
        }
    }

    float GetRandomDuration()
    {
        return Random.Range(0.0f, 0.5f);
    }

    int GetRandomMovementInput()
    {
        return Random.Range(0, 2);
    }

    int GetRandomTurnDirection()
    {
        //int direction = Random.Range(-2, 3);
        //if (direction == -2)
        //{
        //    direction = -1;
        //}
        //else if (direction == 2)
        //{
        //    direction = 1;
        //}
        //else
        //{
        //    direction = 0;
        //}

        //return direction;

        return Random.Range(-1, 2);
    }

    public void SlowDownSimulation()
    {
        Time.timeScale /= 2;
        timeScaleText.text = "Simulation speed: " + Time.timeScale + "x";
    }

    public void SpeedUpSimulation()
    {
        Time.timeScale *= 2;
        timeScaleText.text = "Simulation speed: " + Time.timeScale + "x";
    }
}
