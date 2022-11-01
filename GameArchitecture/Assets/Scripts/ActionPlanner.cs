using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Transform))]
public class ActionPlanner : MonoBehaviour
{
    [NonSerialized]
    public Transform transform;

    WorldStateList worldState;
    List<Action> actions;
    List<Goal> goals;
    List<Action> plan;
    Action runningAction;
    Goal currentGoal;

    private void Awake()
    {
        worldState = new WorldStateList(
            new WorldState<object>("enemyVisible", false),
            new WorldState<object>("armedWithGun", true),
            new WorldState<object>("weaponLoaded", false),
            new WorldState<object>("enemyLinedUp", false),
            new WorldState<object>("enemyAlive", true),
            new WorldState<object>("armedWithBomb", true),
            new WorldState<object>("nearEnemy", false),
            new WorldState<object>("alive", true)
        );

        actions = new List<Action>();
        goals = new List<Goal>();

        // Scout
        actions.Add(new Action(
            new WorldStateList(
                new WorldState<object>("armedWithGun", true)
            ),
            new WorldStateList(
                new WorldState<object>("enemyVisible", true)
            ),
            new BehaviorMove(new Vector3(0, 0.97f, 0), 2f)
        ));
        // Approach
        actions.Add(new Action(
            new WorldStateList(
                new WorldState<object>("enemyVisible", true)
            ),
            new WorldStateList(
                new WorldState<object>("nearEnemy", true)
            ),
            new BehaviorMove(new Vector3(0, 0, 0), 2f)
        ));
        // Aim
        actions.Add(new Action(
            new WorldStateList(
                new WorldState<object>("enemyVisible", true),
                new WorldState<object>("weaponLoaded", true)
            ),
            new WorldStateList(
                new WorldState<object>("enemyLinedUp", true)
            ),
            new BehaviorMove(new Vector3(5, 0, 5), 2f)
        ));
        // Shoot
        actions.Add(new Action(
            new WorldStateList(
                new WorldState<object>("enemyLinedUp", true)
            ),
            new WorldStateList(
                new WorldState<object>("enemyAlive", false)
            ),
            new BehaviorMove(new Vector3(5, 5, 5), 2f)
        ));
        // Load
        actions.Add(new Action(
            new WorldStateList(
                new WorldState<object>("armedWithGun", true)
            ),
            new WorldStateList(
                new WorldState<object>("weaponLoaded", true)
            ),
            new BehaviorMove(new Vector3(0, 0, 0), 2f)
        ));
        // Detonate Bomb
        actions.Add(new Action(
            new WorldStateList(
                new WorldState<object>("armedWithBomb", true),
                new WorldState<object>("nearEnemy", true)
            ),
            new WorldStateList(
                new WorldState<object>("alive", false),
                new WorldState<object>("enemyAlive", false)
            ),
            new BehaviorMove(new Vector3(0, 0, 0), 2f)
        ));
        // Flee
        actions.Add(new Action(
            new WorldStateList(
                new WorldState<object>("enemyVisible", true)
            ),
            new WorldStateList(
                new WorldState<object>("nearEnemy", false)
            ),
            new BehaviorMove(new Vector3(0, 0, 0), 2f)
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
