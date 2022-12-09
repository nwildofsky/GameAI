using System.Collections;
using System.Collections.Generic;
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
    IEnumerator chromosome;
    int chromosomeSize;

    public AgentChromosomeData ChromosomeData
    {
        get => chromosomeData;
        set
        {
            chromosomeData = value;
            chromosome = BuildChromosome(chromosomeData);
        }
    }

    public int ChromosomeSize
    {
        get => chromosomeSize;
        set => chromosomeSize = value;
    }

    // Start is called before the first frame update
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        chromosomeSize = 0;

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
        horizontalInput = 0;
        verticalInput = 0;

        if (Input.GetKey(KeyCode.W))
        {
            verticalInput = 1;
        }

        if (Input.GetKey(KeyCode.A))
        {
            horizontalInput--;
        }
        if (Input.GetKey(KeyCode.D))
        {
            horizontalInput++;
        }

        Vector3 rawVelocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
        if (rawVelocity.magnitude > maxSpeed)
        {
            Vector3 clampedVelocity = rawVelocity.normalized * maxSpeed;
            rigidbody.velocity = new Vector3(clampedVelocity.x, rigidbody.velocity.y, clampedVelocity.z);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            Time.timeScale *= 2;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale /= 2;
        }
    }

    private void FixedUpdate()
    {
        //DriveAndTurn(verticalInput, horizontalInput);
    }

    // Take the tree based structure AgentChromosomeData and condense it into a linked list of Coroutine actions by preorder
    IEnumerator BuildChromosome(AgentChromosomeData node, AgentChromosomeData prevNode = null)
    {
        if (node == null)
        {
            return null;
        }

        if (node.Left != null)
        {
            return DriveAndTurnFor(node.data.duration, node.data.movement, node.data.turnDirection, BuildChromosome(node.Left, node));
        }
        else if (node.Right != null)
        {
            return DriveAndTurnFor(node.data.duration, node.data.movement, node.data.turnDirection, BuildChromosome(node.Right, node));
        }
        else if (prevNode != null)
        {
            //if (prevNode.right != node && prevNode.right != null)
            //{
            //    return DriveAndTurnFor(node.data.duration, node.data.movement, node.data.turnDirection, BuildChromosome(prevNode.right, prevNode));
            //}
            //else if (prevNode.parent != null)
            //{
            //    return DriveAndTurnFor(node.data.duration, node.data.movement, node.data.turnDirection, BuildChromosome(prevNode.parent.right, prevNode.parent));
            //}

            AgentChromosomeData x = prevNode;
            AgentChromosomeData y = node;
            while ((x.Right == y || x.Right == null) && x.Parent != null)
            {
                x = x.Parent;
                y = y.Parent;
            }
            if (x.Right != y && x.Right != null)
            {
                return DriveAndTurnFor(node.data.duration, node.data.movement, node.data.turnDirection, BuildChromosome(x.Right, x));
            }
        }

        return DriveAndTurnFor(node.data.duration, node.data.movement, node.data.turnDirection, null);
    }

    public void Run()
    {
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
    }
}
