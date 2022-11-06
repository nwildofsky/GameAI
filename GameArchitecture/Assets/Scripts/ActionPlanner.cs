using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Transform))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(SphereCollider))]
public class ActionPlanner : MonoBehaviour
{
    // Agent's accesible properties
    [NonSerialized]
    public Transform transform;

    // Variables that influence each state
    private bool bumped = false;
    private bool insideLocation = false;
    private int bumpMax = 2;
    private int collisionsToBump;

    // Variables for goap action planning
    WorldStateList worldState;
    List<Action> actions;
    List<Goal> goals;
    List<Action> plan;
    Action runningAction;
    [SerializeField]
    string runningActionDebugString = "";
    Goal currentGoal;

    public bool Bumped
    { get => bumped; }
    public WorldStateList WorldState
    { get => worldState; }

    private void Awake()
    {
        collisionsToBump = bumpMax;
    }

    void Start()
    {
        transform = GetComponent<Transform>();

        Init();
    }

    // Setup goap variables and lists
    private void Init()
    {
        worldState = new WorldStateList(
            new WorldState<object>("happiness", UnityEngine.Random.Range(0, 4)),
            new WorldState<object>("hunger", UnityEngine.Random.Range(0, 3)),
            new WorldState<object>("money", UnityEngine.Random.Range(0, 3)),
            new WorldState<object>("bumpedInto", bumped)
        );

        actions = new List<Action>();
        runningAction = null;
        goals = new List<Goal>();
        currentGoal = null;
        plan = new List<Action>();

        // Eat
        actions.Add(new ActionEat());
        actions.Add(new ActionEatRich());
        // Drink
        actions.Add(new ActionDrink());
        // Bump Person
        actions.Add(new ActionBump());
        // Avoid
        actions.Add(new ActionAvoidSad());
        actions.Add(new ActionAvoidAngry());
        // Socialize
        actions.Add(new ActionSocializeHappy());
        actions.Add(new ActionSocializeHungry());
        // Time Alone
        actions.Add(new ActionAlone());
        // Shop
        actions.Add(new ActionShopRich());
        actions.Add(new ActionShop());
        // Work
        actions.Add(new ActionWorkSad());
        actions.Add(new ActionWorkEfficient());
        actions.Add(new ActionWorkHappy());


        goals.Add(new GoalHappy());
        goals.Add(new GoalRich());
        goals.Add(new GoalNotHungry());
        goals.Add(new GoalSpendMoney());
        goals.Add(new GoalResolveBump());

        RecalculatePlan();
    }

    // Run each action one at a time
    // On completion apply its effects to the world state, recalculate plan,
    // and then pop the next action from the plan and start the process over
    private void Update()
    {
        // While there are more actions left in the plan
        if (plan.Count > 0)
        {
            if (runningAction == null)
            {
                runningAction = plan[0];
                plan.RemoveAt(0);

                runningAction.Behavior.Run(this);
            }
            else
            {
                if (runningAction.Behavior.IsCompleted)
                {
                    worldState.ApplyStateList(runningAction.Effects);
                    RecalculatePlan();
                    runningAction = plan[0];
                    plan.RemoveAt(0);

                    runningAction.Behavior.Run(this);
                }
                else
                {
                    runningAction.Behavior.Run(this);
                }
            }
        }
        // If this is the last action in the plan
        else if (runningAction != null)
        {
            if (runningAction.Behavior.IsCompleted)
            {
                worldState.ApplyStateList(runningAction.Effects);
                runningAction = null;
                RecalculatePlan();
            }
            else
            {
                runningAction.Behavior.Run(this);
            }
        }
        // If the entire plan has been completed
        else
        {
            currentGoal = null;
            RecalculatePlan();
        }

        // Update the editor with the name of the currently running action
        if (runningAction != null)
        {
            runningActionDebugString = runningAction.GetType().Name;
        }
    }

    private void RecalculatePlan()
    {
        // Set the current goal to the goal in the list with the highest priority
        foreach (Goal goal in goals)
        {
            if (currentGoal == null)
            {
                currentGoal = goal;
            }
            else if (goal.CalculatePriority(this) > currentGoal.CalculatePriority(this))
            {
                currentGoal = goal;
            }
        }

        // Reset all completed actions
        foreach (Action action in actions)
        {
            action.Behavior.IsRunning = false;
            action.Behavior.IsCompleted = false;
        }
        plan = AStar.PlanAction(worldState, currentGoal, actions);
        runningAction = null;

        // If the A* search could not find a path, there is no path with this current world state
        // As a quick fix, randomize the world state until a path is found
        if (plan == null)
        {
            worldState = new WorldStateList(
                new WorldState<object>("happiness", UnityEngine.Random.Range(0, 4)),
                new WorldState<object>("hunger", UnityEngine.Random.Range(0, 3)),
                new WorldState<object>("money", UnityEngine.Random.Range(0, 3)),
                new WorldState<object>("bumpedInto", bumped)
            );

            Debug.Log("No plan could be calculated");

            RecalculatePlan();
        }
    }    

    private void OnTriggerEnter(Collider other)
    {
        // Collisions with other agents
        if (other.GetType() == typeof(SphereCollider))
        {
            ActionPlanner otherAgent = other.gameObject.GetComponent<ActionPlanner>();
            // Only initiate bumps while moving and outside of a trigger location
            if (otherAgent && !bumped && !insideLocation && runningAction != null && runningAction.Behavior.IsMoving)
            {
                // Only initiate bumps every bumpMax amount of times
                if (collisionsToBump == 0)
                {
                    collisionsToBump = bumpMax;
                    bumped = true;
                    if (worldState != null)
                    {
                        worldState.ApplyState(new WorldState<object>("bumpedInto", true));
                        RecalculatePlan();
                    }
                }
                else
                {
                    collisionsToBump--;
                }
            }
        }

        // Collisions with location volumes
        if (other.GetType() == typeof(BoxCollider) && other.isTrigger)
        {
            insideLocation = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Collisions with other agents
        if (other.GetType() == typeof(SphereCollider))
        {
            ActionPlanner otherAgent = other.gameObject.GetComponent<ActionPlanner>();
            if (otherAgent && bumped)
            {
                bumped = false;
                if (worldState != null)
                {
                    worldState.ApplyState(new WorldState<object>("bumpedInto", false));
                }
            }
        }

        // Collisions with location volumes
        if (other.GetType() == typeof(BoxCollider) && other.isTrigger)
        {
            insideLocation = false;
        }
    }
}
