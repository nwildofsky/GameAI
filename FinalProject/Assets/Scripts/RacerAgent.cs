using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RacerAgent : MonoBehaviour
{
    // Public variables that determine how the driving and turning actions are performed
    public float driveForce;
    public float turnSpeed;
    public float maxSpeed;

    Rigidbody rigidbody;

    // Variables representing this agent's chromosome in different ways
    AgentChromosomeData chromosomeData;
    List<AgentChromosomeData> chromosomeList;
    IEnumerator chromosome;

    // Variables that store data while each generation of the simulation is running
    // for fitness evaluation and generation sequencing
    float fitness;
    bool insideBounds;
    float insideBoundsTime;
    float outOfBoundsTime;
    float avgSpeed;
    float maxSpeedInSimulation;
    List<GameObject> visitedTiles;
    Vector3 lastTileLocation;

    bool completedActions;
    bool exitedStart;
    bool completedTrack;

    // Public getters and setters for this agent's data
    public AgentChromosomeData ChromosomeData
    {
        get => chromosomeData;
        set
        {
            chromosomeData = value;
            chromosome = BuildChromosome(chromosomeData);
        }
    }

    public List<AgentChromosomeData> ChromosomeList
    {
        get => chromosomeList;
    }

    public float Fitness
    {
        get => fitness;
        set => fitness = value;
    }

    public float InsideBoundsTime
    {
        get => insideBoundsTime;
    }

    public float OutOfBoundsTime
    {
        get => outOfBoundsTime;
    }

    public float AvgSpeed
    {
        get => avgSpeed;
    }

    public float MaxSpeedInSimulation
    {
        get => maxSpeedInSimulation;
    }

    public int VisitedTilesCount
    {
        get => visitedTiles.Count;
    }

    public float DistanceFromLastTile
    {
        get
        {
            return (insideBounds ? 0 : 1) * Vector3.Distance(transform.position, lastTileLocation);
        }
    }

    public bool CompletedActions
    {
        get => completedActions;
        set => completedActions = value;
    }

    public bool CompletedTrack
    {
        get => completedTrack;
    }

    // Awake is used so this is called before the simulation Start
    void Awake()
    {
        // Set all data values to their defaults
        rigidbody = GetComponent<Rigidbody>();
        fitness = 0;
        insideBounds = false;
        insideBoundsTime = 0.0f;
        outOfBoundsTime = 0.0f;
        avgSpeed = 0.0f;
        maxSpeedInSimulation = 0.0f;
        visitedTiles = new List<GameObject>();
        lastTileLocation = Vector3.zero;
        completedActions = false;
        exitedStart = false;
        completedTrack = false;
    }

    // Fixed update is used to carry out this agent's actions so that the time scale can be manipulated during the simulation
    private void FixedUpdate()
    {
        // Clamp this agent's speed to its max speed
        Vector3 rawVelocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
        if (rawVelocity.magnitude > maxSpeed)
        {
            Vector3 clampedVelocity = rawVelocity.normalized * maxSpeed;
            rigidbody.velocity = new Vector3(clampedVelocity.x, rigidbody.velocity.y, clampedVelocity.z);
        }

        // While this agent's chromosome is still running
        if (!completedActions)
        {
            // Calculate how much time this agent is inside the track bounds and how much it is outside track bounds
            if (insideBounds)
            {
                insideBoundsTime += Time.fixedUnscaledDeltaTime;
            }
            else
            {
                outOfBoundsTime += Time.fixedUnscaledDeltaTime;
            }

            // Calculate a relative average speed
            // Faster agents will still end up with higher speeds but to avoid
            // making this float too high we will multiply it by the deltaTime
            avgSpeed += rigidbody.velocity.magnitude * Time.fixedUnscaledDeltaTime;
            maxSpeedInSimulation += maxSpeed * Time.fixedUnscaledDeltaTime;
        }

        // Once this agent has passed the finished line, stop all of its actions
        if (completedTrack)
        {
            StopAllCoroutines();
            completedActions = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Layer 8 is InsideTrack
        if (other.gameObject.layer == 8)
        {
            if (!insideBounds)
            {
                insideBounds = true;
                lastTileLocation = other.transform.position;
            }

            // Add this tile to the list if this agent has not collided with it this generation
            if (!visitedTiles.Contains(other.gameObject) && !completedTrack)
            {
                visitedTiles.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Layer 8 is InsideTrack
        if (other.gameObject.layer == 8 && !insideBounds)
        {
            insideBounds = true;
            lastTileLocation = other.transform.position;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Layer 8 is InsideTrack
        if (other.gameObject.layer == 8 && insideBounds)
        {
            insideBounds = false;
            lastTileLocation = other.transform.position;
        }

        // Each agent starts inside of the finish line collider, so it must first leave the collider,
        // then when it exits the same collider again, that means that it has passed the finish line
        if (other.tag == "FinishLine")
        {
            if (!exitedStart)
            {
                exitedStart = true;
            }
            else
            {
                completedTrack = true;
            }
        }
    }

    public void ResetVariables()
    {
        rigidbody.velocity = Vector3.zero;

        fitness = 0;
        insideBounds = false;
        insideBoundsTime = 0.0f;
        outOfBoundsTime = 0.0f;
        avgSpeed = 0.0f;
        maxSpeedInSimulation = 0.0f;
        visitedTiles = new List<GameObject>();
        lastTileLocation = Vector3.zero;
        completedActions = false;
        exitedStart = false;
        completedTrack = false;
    }

    // Take the tree based structure AgentChromosomeData and condense it into a linked list of Coroutine actions by preorder
    IEnumerator BuildChromosome(AgentChromosomeData root)
    {
        chromosomeList = new List<AgentChromosomeData>();
        MakeChromosomeList(root, chromosomeList);
        return MakeChromosomeFromList(chromosomeList);
    }

    // Traverse the passed in tree structure by preorder and create a list out of its values
    void MakeChromosomeList(AgentChromosomeData root, List<AgentChromosomeData> list)
    {
        if (root != null)
        {
            list.Add(root);

            MakeChromosomeList(root.Left, list);
            MakeChromosomeList(root.Right, list);
        }
    }

    // Turn a list of Chromosome Data into an actual usable chromosome
    // Coroutines are used because they are functions that can be stored as a value and called continuously for a certain amount of time
    // Each coroutine action has a reference to the next so they are all called one after another
    IEnumerator MakeChromosomeFromList(List<AgentChromosomeData> list, int index = 0)
    {
        if (index == list.Count - 1)
        {
            return DriveAndTurnFor(list[index].data.duration, list[index].data.movement, list[index].data.turnDirection, null);
        }

        return DriveAndTurnFor(list[index].data.duration, list[index].data.movement, list[index].data.turnDirection, MakeChromosomeFromList(list, index + 1));
    }

    // Start the coroutine list that carries out the actions defined by its chromosome
    public void Run()
    {
        chromosome = BuildChromosome(chromosomeData);
        StartCoroutine(chromosome);
    }

    // Simple force based movement in the forward direction based on this agent's transform
    void Drive(int inputMovement)
    {
        rigidbody.AddForce(transform.forward * driveForce * inputMovement * Time.deltaTime, ForceMode.Force);
    }

    // Rotate this agent either clockwise or counterclockwise depending on the input
    void Turn(int inputDirection)
    {
        Vector3 newOrientation = Quaternion.Euler(0, inputDirection * turnSpeed * Time.deltaTime, 0) * transform.forward;
        transform.rotation = Quaternion.LookRotation(newOrientation, Vector3.up);
    }

    // By taking in both inputs required, this function calls both Turn and then Drive
    void DriveAndTurn(int moveInput, int turnDirection)
    {
        Turn(turnDirection);
        Drive(moveInput);
    }

    // The coroutine that makes up this agent's chromosome
    public IEnumerator DriveAndTurnFor(float duration, int inputMovement, int inputDirection, IEnumerator next)
    {
        // For the entire duration specified, call DriveAndTurn each FixedUpdate()
        for (float time = 0f; time < duration; time += Time.fixedDeltaTime)
        {
            DriveAndTurn(inputMovement, inputDirection);
            yield return new WaitForFixedUpdate();
        }

        // When the duration is over, call the next action in the chromosome
        if (next != null)
        {
            StartCoroutine(next);
        }
        // If there is no next action, then this agent has completed its chromosome
        else
        {
            completedActions = true;

            // A relative average speed
            // Faster agents will still end up with higher speeds but to
            // avoid counting the number of frames where a speed value was recorded
            // we will simply divide elasped time of this agent's actions
            avgSpeed = avgSpeed / (insideBoundsTime + outOfBoundsTime);
            maxSpeedInSimulation = maxSpeedInSimulation / (insideBoundsTime + outOfBoundsTime);
        }
    }
}
