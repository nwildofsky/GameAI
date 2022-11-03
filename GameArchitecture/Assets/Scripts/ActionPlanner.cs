using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Transform))]
public class ActionPlanner : MonoBehaviour
{
    // Agent's accesible properties
    [NonSerialized]
    public Transform transform;

    // Variables that determine each state
    [Range(0, 3)]
    public int startHappiness;
    [Range(0, 2)]
    public int startHunger;
    [Range(0, 2)]
    public int startMoney;

    // Variables for goap action planning
    WorldStateList worldState;
    List<Action> actions;
    List<Goal> goals;
    List<Action> plan;
    Action runningAction;
    Goal currentGoal;

    private void Awake()
    {
        worldState = new WorldStateList(
            new WorldState<object>("happiness", startHappiness),
            new WorldState<object>("hunger", startHunger),
            new WorldState<object>("money", startMoney),
            new WorldState<object>("bumpedInto", false),
            new WorldState<object>("social", false)
        );

        actions = new List<Action>();
        goals = new List<Goal>();

        // Eat
        actions.Add(new Action(
            new WorldStateList(
                new WorldState<object>("hunger", 2),
                new WorldState<object>("money", 1)
            ),
            new WorldStateList(
                new WorldState<object>("hunger", 0),
                new WorldState<object>("money", 0)
            ),
            new BehaviorEat()
        ));
        // Drink
        actions.Add(new Action(
            new WorldStateList(
                new WorldState<object>("hunger", 1),
                new WorldState<object>("money", 1),
                new WorldState<object>("happiness", 0),
                new WorldState<object>("social", false)
            ),
            new WorldStateList(
                new WorldState<object>("money", 0),
                new WorldState<object>("happiness", 2)
            ),
            new Behavior_Base()
        ));
        // Find Person
        actions.Add(new Action(
            new WorldStateList(
                new WorldState<object>("social", false)
            ),
            new WorldStateList(
                new WorldState<object>("bumpedInto", true)
            ),
            new Behavior_Base()
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
            new Behavior_Base()
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
            new Behavior_Base()
        ));
        // Socialize
        actions.Add(new Action(
            new WorldStateList(
                new WorldState<object>("bumpedInto", true),
                new WorldState<object>("happiness", 2)
            ),
            new WorldStateList(
                new WorldState<object>("social", true),
                new WorldState<object>("happiness", 3),
                new WorldState<object>("hunger", 1)
            ),
            new Behavior_Base()
        ));
        // Socialize
        actions.Add(new Action(
            new WorldStateList(
                new WorldState<object>("bumpedInto", true),
                new WorldState<object>("happiness", 1)
            ),
            new WorldStateList(
                new WorldState<object>("social", false),
                new WorldState<object>("happiness", 0)
            ),
            new Behavior_Base()
        ));
        // Time Alone
        actions.Add(new Action(
            new WorldStateList(
                new WorldState<object>("happiness", 0)
            ),
            new WorldStateList(
                new WorldState<object>("happiness", 1),
                new WorldState<object>("hunger", 1),
                new WorldState<object>("social", false)
            ),
            new Behavior_Base()
        ));
        // Shop
        actions.Add(new Action(
            new WorldStateList(
                new WorldState<object>("money", 2),
                new WorldState<object>("social", true)
            ),
            new WorldStateList(
                new WorldState<object>("money", 0),
                new WorldState<object>("happiness", 1)
            ),
            new Behavior_Base()
        ));
        // Work
        actions.Add(new Action(
            new WorldStateList(
                new WorldState<object>("money", 0),
                new WorldState<object>("happiness", 0)
            ),
            new WorldStateList(
                new WorldState<object>("money", 1),
                new WorldState<object>("hunger", 2),
                new WorldState<object>("social", false)
            ),
            new Behavior_Base()
        ));
        // Work
        actions.Add(new Action(
            new WorldStateList(
                new WorldState<object>("money", 0),
                new WorldState<object>("happiness", 1)
            ),
            new WorldStateList(
                new WorldState<object>("money", 2),
                new WorldState<object>("happiness", 0),
                new WorldState<object>("hunger", 1)
            ),
            new Behavior_Base()
        ));
        // Work
        actions.Add(new Action(
            new WorldStateList(
                new WorldState<object>("money", 0),
                new WorldState<object>("happiness", 2)
            ),
            new WorldStateList(
                new WorldState<object>("money", 1),
                new WorldState<object>("hunger", 2),
                new WorldState<object>("social", true)
            ),
            new Behavior_Base()
        ));


        goals.Add(new Goal(
            new WorldStateList(
                new WorldState<object>("enemyAlive", false),
                new WorldState<object>("alive", true)
            )
        ));
        currentGoal = goals[0];

        plan = AStar.PlanAction(worldState, goals[0], actions);
    }

    void Start()
    {
        transform = GetComponent<Transform>();
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
                    runningAction = plan[0];
                    plan.RemoveAt(0);
                    runningAction.Behavior.Run(this);
                }
                else
                {
                    runningAction.Behavior.Run(this);

                    RecalculatePlan();
                }
            }
        }
        else if (runningAction != null)
        {
            if (runningAction.Behavior.IsCompleted)
            {
                runningAction = null;
            }
            else
            {
                runningAction.Behavior.Run(this);
                
                RecalculatePlan();
            }
        }
        else
        {
            currentGoal = null;
            RecalculatePlan();
        }
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
                break;
            }
            else if (goal.CalculatePriority(this) < currentGoal.CalculatePriority(this))
            {
                currentGoal = goal;
                foreach (Action action in actions)
                {
                    action.Behavior.IsRunning = false;
                    action.Behavior.IsCompleted = false;
                }
                plan = AStar.PlanAction(worldState, currentGoal, actions);
                break;
            }
        }
    }
}
