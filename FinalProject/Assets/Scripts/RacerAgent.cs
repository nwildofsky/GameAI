using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RacerAgent : MonoBehaviour
{
    public float driveForce;
    public float turnSpeed;
    public float maxSpeed;

    Rigidbody rigidbody;
    Vector3 moveDirection;
    float timeStep;

    int horizontalInput;
    int verticalInput;

    AgentChromosomeData chromosomeData;
    List<AgentChromosomeData> chromosomeList;
    IEnumerator chromosome;
    //int chromosomeSize;
    float fitness;
    bool insideBounds;
    float insideBoundsTime;
    float outOfBoundsTime;
    float avgSpeed;
    float maxSpeedInSimulation;
    List<GameObject> visitedTiles;
    bool completedActions;
    bool exitedStart;
    bool completedTrack;

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

    //public int ChromosomeSize
    //{
    //    get => chromosomeSize;
    //    set => chromosomeSize = value;
    //}

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

    public bool CompletedActions
    {
        get => completedActions;
        set => completedActions = value;
    }

    public bool CompletedTrack
    {
        get => completedTrack;
    }

    // Start is called before the first frame update
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        //chromosomeSize = 0;
        fitness = 0;
        insideBounds = false;
        insideBoundsTime = 0.0f;
        outOfBoundsTime = 0.0f;
        avgSpeed = 0.0f;
        maxSpeedInSimulation = 0.0f;
        visitedTiles = new List<GameObject>();
        completedActions = false;
        exitedStart = false;
        completedTrack = false;

        //timeStep = 1 / 60f;

        //chromosomeData = new AgentChromosomeData(new InputData(2, 1, 0),
        //    new AgentChromosomeData(new InputData(0.5f, 0, 1),
        //        new AgentChromosomeData(new InputData(1, 0, 1), null, null),
        //        new AgentChromosomeData(new InputData(0.5f, 0, -1),
        //            new AgentChromosomeData(new InputData(1, 0, 0), null, null),
        //            null),
        //        chromosomeData),
        //    new AgentChromosomeData(new InputData(0.5f, 1, 0), null, null, chromosomeData));

        //chromosome = BuildChromosome(chromosomeData);

        //StartCoroutine(chromosome);

        //StartCoroutine(DriveAndTurnFor(2, 1, 0, null));
    }

    // Update is called once per frame
    void Update()
    {
        

        //horizontalInput = 0;
        //verticalInput = 0;

        //if (Input.GetKey(KeyCode.W))
        //{
        //    verticalInput = 1;
        //}

        //if (Input.GetKey(KeyCode.A))
        //{
        //    horizontalInput--;
        //}
        //if (Input.GetKey(KeyCode.D))
        //{
        //    horizontalInput++;
        //}

        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    Time.timeScale *= 2;
        //}
        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    Time.timeScale /= 2;
        //}
    }

    private void FixedUpdate()
    {
        //DriveAndTurn(verticalInput, horizontalInput);

        Vector3 rawVelocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
        if (rawVelocity.magnitude > maxSpeed)
        {
            Vector3 clampedVelocity = rawVelocity.normalized * maxSpeed;
            rigidbody.velocity = new Vector3(clampedVelocity.x, rigidbody.velocity.y, clampedVelocity.z);
        }

        if (!completedActions)
        {
            if (insideBounds)
            {
                insideBoundsTime += Time.fixedDeltaTime;
            }
            else
            {
                outOfBoundsTime += Time.fixedDeltaTime;
            }

            // A relative average speed
            // Faster agents will still end up with higher speeds but to avoid
            // making this float too high we will multiply it by the deltaTime
            avgSpeed += rigidbody.velocity.magnitude * Time.fixedDeltaTime;
            maxSpeedInSimulation += maxSpeed * Time.fixedDeltaTime;
        }

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
                insideBounds = true;

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
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Layer 8 is InsideTrack
        if (other.gameObject.layer == 8 && insideBounds)
        {
            insideBounds = false;
        }

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
        completedActions = false;
        exitedStart = false;
        completedTrack = false;
    }

    // Take the tree based structure AgentChromosomeData and condense it into a linked list of Coroutine actions by preorder
    IEnumerator BuildChromosome(AgentChromosomeData root /*, AgentChromosomeData prevNode = null*/)
    {
        chromosomeList = new List<AgentChromosomeData>();
        MakeChromosomeList(root, chromosomeList);
        return MakeChromosomeFromList(chromosomeList);

        //if (node == null)
        //{
        //    return null;
        //}

        //if (node.Left != null)
        //{
        //    return DriveAndTurnFor(node.data.duration, node.data.movement, node.data.turnDirection, BuildChromosome(node.Left, node));
        //}
        //else if (node.Right != null)
        //{
        //    return DriveAndTurnFor(node.data.duration, node.data.movement, node.data.turnDirection, BuildChromosome(node.Right, node));
        //}
        //else if (prevNode != null)
        //{
        //    //if (prevNode.right != node && prevNode.right != null)
        //    //{
        //    //    return DriveAndTurnFor(node.data.duration, node.data.movement, node.data.turnDirection, BuildChromosome(prevNode.right, prevNode));
        //    //}
        //    //else if (prevNode.parent != null)
        //    //{
        //    //    return DriveAndTurnFor(node.data.duration, node.data.movement, node.data.turnDirection, BuildChromosome(prevNode.parent.right, prevNode.parent));
        //    //}

        //    AgentChromosomeData x = prevNode;
        //    AgentChromosomeData y = node;
        //    while ((x.Right == y || x.Right == null) && x.Parent != null)
        //    {
        //        x = x.Parent;
        //        y = y.Parent;
        //    }
        //    if (x.Right != y && x.Right != null)
        //    {
        //        return DriveAndTurnFor(node.data.duration, node.data.movement, node.data.turnDirection, BuildChromosome(x.Right, x));
        //    }
        //}

        //return DriveAndTurnFor(node.data.duration, node.data.movement, node.data.turnDirection, null);
    }

    void MakeChromosomeList(AgentChromosomeData root, List<AgentChromosomeData> list)
    {
        if (root != null)
        {
            list.Add(root);

            MakeChromosomeList(root.Left, list);
            MakeChromosomeList(root.Right, list);
        }
    }

    IEnumerator MakeChromosomeFromList(List<AgentChromosomeData> list, int index = 0)
    {
        if (index == list.Count - 1)
        {
            return DriveAndTurnFor(list[index].data.duration, list[index].data.movement, list[index].data.turnDirection, null);
        }

        return DriveAndTurnFor(list[index].data.duration, list[index].data.movement, list[index].data.turnDirection, MakeChromosomeFromList(list, index + 1));
    }

    public void Run()
    {
        chromosome = BuildChromosome(chromosomeData);
        StartCoroutine(chromosome);
    }

    void Drive(int inputMovement)
    {
        rigidbody.AddForce(transform.forward * driveForce * Mathf.Sign(inputMovement) * Time.deltaTime, ForceMode.Force);
    }

    void Turn(int inputDirection)
    {
        Vector3 newOrientation = Quaternion.Euler(0, inputDirection * turnSpeed * Time.deltaTime, 0) * transform.forward;
        transform.rotation = Quaternion.LookRotation(newOrientation, Vector3.up);
    }

    void DriveAndTurn(int moveInput, int turnDirection)
    {
        Turn(turnDirection);
        Drive(moveInput);
    }

    public IEnumerator DriveAndTurnFor(float duration, int inputMovement, int inputDirection, IEnumerator next)
    {
        for (float time = 0f; time < duration; time += Time.fixedDeltaTime)
        {
            DriveAndTurn(inputMovement, inputDirection);
            yield return new WaitForFixedUpdate();
        }

        if (next != null)
        {
            StartCoroutine(next);
        }
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
