using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Transform))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(SphereCollider))]
public class ActionPlanner : MonoBehaviour
{
    // Agent's accesible properties
    [NonSerialized]
    public Transform transform;

    // Variables that influence each state
    private bool bumped = false;

    // Variables for goap action planning
    WorldStateList worldState;
    List<Action> actions;
    List<Goal> goals;
    List<Action> plan;
    Action runningAction;
    Goal currentGoal;

    public bool Bumped
    { get => bumped; }
    public WorldStateList WorldState
    { get => worldState; }

    void Start()
    {
        transform = GetComponent<Transform>();

        Init();
    }

    private void Init()
    {
        worldState = new WorldStateList(
            new WorldState<object>("happiness", UnityEngine.Random.Range(0, 4)),
            new WorldState<object>("hunger", UnityEngine.Random.Range(0, 3)),
            new WorldState<object>("money", UnityEngine.Random.Range(0, 3)),
            new WorldState<object>("bumpedInto", false)
        );

        actions = new List<Action>();
        runningAction = null;
        goals = new List<Goal>();
        currentGoal = null;
        plan = new List<Action>();

        // Eat
        actions.Add(new Action(
            new WorldStateList(
                new WorldState<object>("bumpedInto", false),
                new WorldState<object>("hunger", 2),
                new WorldState<object>("money", 1)
            ),
            new WorldStateList(
                new WorldState<object>("hunger", 0),
                new WorldState<object>("money", 0)
            ),
            new BehaviorEat()
        ));
        actions.Add(new Action(
            new WorldStateList(
                new WorldState<object>("bumpedInto", false),
                new WorldState<object>("hunger", 2),
                new WorldState<object>("money", 2)
            ),
            new WorldStateList(
                new WorldState<object>("hunger", 0),
                new WorldState<object>("money", 1)
            ),
            new BehaviorEat()
        ));
        // Drink
        actions.Add(new Action(
            new WorldStateList(
                new WorldState<object>("bumpedInto", false),
                new WorldState<object>("hunger", 1),
                new WorldState<object>("happiness", 0)
            ),
            new WorldStateList(
                new WorldState<object>("hunger", 0)
            ),
            new BehaviorDrink()
        ));
        // Bump Person
        actions.Add(new Action(
            new WorldStateList(
                new WorldState<object>("bumpedInto", false)
            ),
            new WorldStateList(
                new WorldState<object>("bumpedInto", true)
            ),
            new BehaviorBump()
        ));
        // Avoid
        actions.Add(new Action(
            new WorldStateList(
                new WorldState<object>("bumpedInto", true),
                new WorldState<object>("happiness", 0)
            ),
            new WorldStateList(
                new WorldState<object>("bumpedInto", false)
            ),
            new BehaviorAvoid()
        ));
        // Avoid 
        actions.Add(new Action(
            new WorldStateList(
                new WorldState<object>("bumpedInto", true),
                new WorldState<object>("happiness", 1)
            ),
            new WorldStateList(
                new WorldState<object>("bumpedInto", false),
                new WorldState<object>("happiness", 0)
            ),
            new BehaviorAvoid()
        ));
        // Socialize
        actions.Add(new Action(
            new WorldStateList(
                new WorldState<object>("bumpedInto", true),
                new WorldState<object>("happiness", 2)
            ),
            new WorldStateList(
                new WorldState<object>("bumpedInto", false),
                new WorldState<object>("happiness", 3),
                new WorldState<object>("hunger", 1)
            ),
            new BehaviorSocialize()
        ));
        // Socialize
        actions.Add(new Action(
            new WorldStateList(
                new WorldState<object>("bumpedInto", true),
                new WorldState<object>("happiness", 1)
            ),
            new WorldStateList(
                new WorldState<object>("bumpedInto", false),
                new WorldState<object>("happiness", 0)
            ),
            new BehaviorSocialize()
        ));
        // Time Alone
        actions.Add(new Action(
            new WorldStateList(
                new WorldState<object>("bumpedInto", false),
                new WorldState<object>("happiness", 0)
            ),
            new WorldStateList(
                new WorldState<object>("happiness", 2),
                new WorldState<object>("hunger", 1)
            ),
            new BehaviorAlone()
        ));
        // Shop
        actions.Add(new Action(
            new WorldStateList(
                new WorldState<object>("bumpedInto", false),
                new WorldState<object>("money", 2)
            ),
            new WorldStateList(
                new WorldState<object>("money", 0),
                new WorldState<object>("happiness", 3),
                new WorldState<object>("hunger", 2)
            ),
            new BehaviorShop()
        ));
        actions.Add(new Action(
            new WorldStateList(
            new WorldState<object>("bumpedInto", false),
            new WorldState<object>("money", 1)
        ),
            new WorldStateList(
            new WorldState<object>("money", 0),
            new WorldState<object>("happiness", 2)
        ),
            new BehaviorShop()
        ));
        // Work
        actions.Add(new Action(
            new WorldStateList(
                new WorldState<object>("bumpedInto", false),
                new WorldState<object>("happiness", 0)
            ),
            new WorldStateList(
                new WorldState<object>("money", 1),
                new WorldState<object>("hunger", 2)
            ),
            new BehaviorWork()
        ));
        // Work
        actions.Add(new Action(
            new WorldStateList(
                new WorldState<object>("bumpedInto", false),
                new WorldState<object>("happiness", 1)
            ),
            new WorldStateList(
                new WorldState<object>("money", 2),
                new WorldState<object>("happiness", 0),
                new WorldState<object>("hunger", 1)
            ),
            new BehaviorWork()
        ));
        // Work
        actions.Add(new Action(
            new WorldStateList(
                new WorldState<object>("bumpedInto", false),
                new WorldState<object>("happiness", 2)
            ),
            new WorldStateList(
                new WorldState<object>("money", 1),
                new WorldState<object>("hunger", 2),
                new WorldState<object>("happiness", 1)
            ),
            new BehaviorWork()
        ));


        goals.Add(new GoalHappy());
        goals.Add(new GoalRich());
        goals.Add(new GoalNotHungry());
        goals.Add(new GoalSpendMoney());

        RecalculatePlan();
    }

    private void Update()
    {
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
        else if (runningAction != null)
        {
            if (runningAction.Behavior.IsCompleted)
            {
                worldState.ApplyStateList(runningAction.Effects);
                runningAction = null;
            }
            else
            {
                runningAction.Behavior.Run(this);
            }
        }
        else
        {
            currentGoal = null;
        }

        RecalculatePlan();
    }

    private void RecalculatePlan()
    {
        foreach (Goal goal in goals)
        {
            if (currentGoal == null)
            {
                currentGoal = goal;
                foreach (Action action in actions)
                {
                    action.Behavior.IsRunning = false;
                    action.Behavior.IsCompleted = false;
                }
                plan = AStar.PlanAction(worldState, currentGoal, actions);
                runningAction = null;
                break;
            }
            else if (goal.CalculatePriority(this) > currentGoal.CalculatePriority(this))
            {
                currentGoal = goal;
                foreach (Action action in actions)
                {
                    action.Behavior.IsRunning = false;
                    action.Behavior.IsCompleted = false;
                }
                plan = AStar.PlanAction(worldState, currentGoal, actions);
                runningAction = null;
                break;
            }
        }

        if (plan == null)
        {
            worldState = new WorldStateList(
                new WorldState<object>("happiness", UnityEngine.Random.Range(0, 4)),
                new WorldState<object>("hunger", UnityEngine.Random.Range(0, 3)),
                new WorldState<object>("money", UnityEngine.Random.Range(0, 3)),
                new WorldState<object>("bumpedInto", false)
            );

            RecalculatePlan();
        }
    }    

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetType() == typeof(SphereCollider))
        {
            ActionPlanner otherAgent = other.gameObject.GetComponent<ActionPlanner>();
            if (otherAgent)
            {
                bumped = true;
                otherAgent.bumped = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetType() == typeof(SphereCollider))
        {
            ActionPlanner otherAgent = other.gameObject.GetComponent<ActionPlanner>();
            if (otherAgent)
            {
                bumped = false;
                otherAgent.bumped = false;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetType() == typeof(SphereCollider))
        {
            ActionPlanner otherAgent = other.gameObject.GetComponent<ActionPlanner>();
            if (otherAgent && !bumped)
            {
                bumped = true;
                otherAgent.bumped = true;
            }
        }
    }
}
