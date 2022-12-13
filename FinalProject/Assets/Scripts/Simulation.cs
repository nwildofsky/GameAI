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
    public float driveChance;
    public float turnChance;
    [System.NonSerialized]
    public int generation;

    List<RacerAgent> racerAgents;
    RacerAgent finalAgent;
    float sumFitnessInCurGeneration;

    TextMeshProUGUI generationText;
    TextMeshProUGUI populationText;
    TextMeshProUGUI timeScaleText;
    int numTilesInTrack;

    // Start is called before the first frame update
    void Start()
    {
        racerAgents = new List<RacerAgent>();
        finalAgent = null;
        generation = 0;
        // Seed the random number generator
        Random.InitState(System.DateTime.Now.Millisecond);

        // Count the number of tiles in the track that contain a track (not the green tile)
        numTilesInTrack = 0;
        Transform track = GameObject.Find("Racetrack").transform;
        for (int i = 0; i < track.childCount; i++)
        {
            if (track.GetChild(i).gameObject.layer == 8)
                numTilesInTrack++;
        }

        // Grab the location and rotation of the start and end piece on the track
        GameObject start = GameObject.FindGameObjectWithTag("TrackStart");
        // Set the finish line collider to the transform of the start
        GameObject finishLineCollider = GameObject.FindGameObjectWithTag("FinishLine");
        finishLineCollider.transform.position = start.transform.position;
        finishLineCollider.transform.rotation = start.transform.rotation;

        // Grab the Text UI that the simulation will need to update
        generationText = GameObject.FindGameObjectWithTag("GenerationText").GetComponent<TextMeshProUGUI>();
        populationText = GameObject.FindGameObjectWithTag("PopulationText").GetComponent<TextMeshProUGUI>();
        timeScaleText = GameObject.FindGameObjectWithTag("TimeScaleText").GetComponent<TextMeshProUGUI>();

        // Make sure there is an even number of agents in the simulation
        if (agentsPerGeneration % 2 != 0)
        {
            agentsPerGeneration++;
        }
        populationText.text = "Population Size: " + agentsPerGeneration;

        // Instantiate each agent for the initial generation
        for (int i = 0; i < agentsPerGeneration; i++)
        {
            RacerAgent newAgent = Instantiate(agent, start.transform.position, start.transform.rotation, agentParent.transform).transform.GetChild(0).GetComponent<RacerAgent>();
            racerAgents.Add(newAgent);
        }

        // Start the simulation
        CreateNewGeneration();
    }

    // Update is called once per frame
    void Update()
    {
        // When the final agent is found that passes a certain fitness threshold, continuously run that agent's actions
        if (finalAgent != null)
        {
            if (finalAgent.CompletedActions)
            {
                ResetAgentVariables(finalAgent);
                finalAgent.Run();
            }
        }
        // While no final agent has been found, continue running through generations
        else
        {
            // Calculate fitness and change the material of every agent that has finished their actions
            bool generationDone = true;
            foreach (RacerAgent agent in racerAgents)
            {
                if (!agent.CompletedActions)
                    generationDone = false;

                if (agent.Fitness == 0.0f && agent.CompletedActions)
                {
                    agent.Fitness = EvaluateFitness(agent);
                    agent.transform.GetComponentInChildren<MeshRenderer>().material = inactiveMaterial;
                }
            }

            // Once every agent has finished their actions, start the next generation
            if (generationDone)
            {
                generation++;
                CreateNewGeneration();
            }
        }
    }

    // This function creates the initial generation's chromosomes randomly
    // Since the chromosome data is a binary tree, this function creates a random binary tree of a given size
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

    // Prepares each generation of agents for the next generation, or stops the simulation if a sufficiently fit agent has been found
    void CreateNewGeneration()
    {
        // Run the initial generation
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
            // Copy each agent's chromosome so the originals can still be referenced while the current agents change their chromosomes
            List<AgentChromosomeData> originalChromosomes = new List<AgentChromosomeData>();
            foreach (RacerAgent agent in racerAgents)
            {
                originalChromosomes.Add(agent.ChromosomeData.DeepCopy());

                // Check if each agent is fit enough to end the simulation
                if (agent.Fitness > fitnessRequirement)
                {
                    finalAgent = agent;
                    ResetAgentVariables(finalAgent);
                    DestroyNonFinalAgents();
                    finalAgent.Run();
                    return;
                }
            }

            // Create the new generation of chromosome for the pre-existing agents
            int agentIdx = 0;
            while (newGeneration.Count < agentsPerGeneration)
            {
                RacerAgent[] parents =
                {
                    racerAgents[agentIdx++],
                    racerAgents[agentIdx++] 
                };

                // Select two agents from the current generation based on fitness
                AgentChromosomeData[] selectedData = PerformSelection(racerAgents, originalChromosomes);
                parents[0].ChromosomeData = selectedData[0];
                parents[1].ChromosomeData = selectedData[1];

                // Crossover the chromosome of the selected agents
                float crossoverProbability = Random.Range(0.0f, 1.0f);
                if (crossoverProbability < crossoverChance)
                {
                    PerformCrossover(parents[0], parents[1]);
                }

                // Mutate the chromosome of the selected agents
                foreach (RacerAgent agent in parents)
                {
                    float mutationProbability = Random.Range(0.0f, 1.0f);
                    if (mutationProbability < mutationChance)
                    {
                        PerformMutation(agent);
                    }
                }

                // Add the changed agents to the new generation
                newGeneration.Add(parents[0]);
                newGeneration.Add(parents[1]);
            }

            // Set the new generation to be the current generation
            racerAgents = newGeneration;
        }

        // Reset all generation-specific variables in the agents
        foreach (RacerAgent agent in racerAgents)
        {
            ResetAgentVariables(agent);
            agent.Run();
        }

        // Update UI
        generationText.text = "Generation: " + generation;
    }

    // Formula to evaluate the fitness of each agent based on data collected during the simulation
    float EvaluateFitness(RacerAgent agent)
    {
        float speedPercent = agent.AvgSpeed / agent.MaxSpeedInSimulation;
        float insideBoundsPercent = agent.InsideBoundsTime / (agent.InsideBoundsTime + agent.OutOfBoundsTime);
        float visitedTrackPercent = agent.VisitedTilesCount / (float)numTilesInTrack;
        float distancePercent = 1.0f - (agent.DistanceFromLastTile / 200.0f);
        float completionPercent = agent.CompletedTrack ? 1.0f : 0.8f;

        // A fit agent is one that stays within the bounds of the track, drives through every drivable tile, and passes the finish line
        float fitness = (insideBoundsPercent * 1.3f) * (distancePercent * 0.8f) * (visitedTrackPercent * 1.1f) * (completionPercent * 0.8f);

        // Agent's fitness cannot be zero or less
        if (fitness < 0.1f)
            fitness = 0.1f;

        return fitness;
    }

    // Roulette Wheel Selection, chances are proportionate to each agent's fitness
    AgentChromosomeData[] PerformSelection(List<RacerAgent> agents, List<AgentChromosomeData> originalData)
    {
        // Calculate the sum of each agent's fitness for this generation if it hasn't been calculated yet
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

    // Picks a random spot in the trees of each parent and switches the node in each spot
    void PerformCrossover(RacerAgent parent1, RacerAgent parent2)
    {
        int location1 = Random.Range(0, parent1.ChromosomeData.Size);
        int location2 = Random.Range(0, parent2.ChromosomeData.Size);

        AgentChromosomeData section1 = parent1.ChromosomeList[location1];
        AgentChromosomeData section2 = parent2.ChromosomeList[location2];
        AgentChromosomeData tempSection1 = section1.DeepCopy();

        // Place section1 at the location of section2 in parent2
        if (section2.Parent != null)
        {
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

    // Changes one or two random nodes in an agent's chromosome tree to random values
    void PerformMutation(RacerAgent agent)
    {
        int numMutations = Random.Range(1, 3);
        for (int i = 0; i < numMutations; i++)
        {
            AgentChromosomeData randomNode = agent.ChromosomeList[Random.Range(0, agent.ChromosomeList.Count)];
            randomNode.data = new InputData(GetRandomDuration(), GetRandomMovementInput(), GetRandomTurnDirection());
        }
    }

    // Get a reference to the root of the tree this node is in
    AgentChromosomeData GetRootFromNode(AgentChromosomeData node)
    {
        while (node.Parent != null)
        {
            node = node.Parent;
        }

        return node;
    }

    // Reset all generation specific variables
    void ResetAgentVariables(RacerAgent agent)
    {
        GameObject start = GameObject.FindGameObjectWithTag("TrackStart");

        agent.gameObject.transform.position = start.transform.position;
        agent.gameObject.transform.rotation = start.transform.rotation;

        agent.transform.GetComponentInChildren<MeshRenderer>().material = activeMaterial;

        agent.ResetVariables();
    }

    // When a final agent is found, remove all other agents by destroying them
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

    // Based on a probability set in the editor
    int GetRandomMovementInput()
    {
        float rand = Random.Range(0.0f, 1.0f);
        if (rand < driveChance)
        {
            return 1;
        }

        return 0;
    }

    // Based on a probability set in the editor
    // Turn direction is locked at 50-50 for right and left
    int GetRandomTurnDirection()
    {
        float turnRand = Random.Range(0.0f, 1.0f);
        if (turnRand < turnChance)
        {
            float directionRand = Random.Range(0.0f, 1.0f);
            if (directionRand < 0.5f)
            {
                return -1;
            }

            return 1;
        }

        return 0;
    }

    // Half the time scale of the simulation
    public void SlowDownSimulation()
    {
        Time.timeScale /= 2;
        timeScaleText.text = "Simulation Speed: " + Time.timeScale + "x";
    }

    // Double the time scale of the simulation
    public void SpeedUpSimulation()
    {
        Time.timeScale *= 2;
        timeScaleText.text = "Simulation Speed: " + Time.timeScale + "x";
    }
}
