using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPlanner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        WorldStateList worldState = new WorldStateList(
            new WorldState<object>("enemyVisible", false),
            new WorldState<object>("armedWithGun", true),
            new WorldState<object>("weaponLoaded", false),
            new WorldState<object>("enemyLinedUp", false),
            new WorldState<object>("enemyAlive", true),
            new WorldState<object>("armedWithBomb", true),
            new WorldState<object>("nearEnemy", false),
            new WorldState<object>("alive", true)
        );

        List<Action> actions = new List<Action>();

        // Scout
        actions.Add(new Action(
            new WorldStateList(
                new WorldState<object>("armedWithGun", true)
            ),
            new WorldStateList(
                new WorldState<object>("enemyVisible", true)
            )
        ));
        // Approach
        actions.Add(new Action(
            new WorldStateList(
                new WorldState<object>("enemyVisible", true)
            ),
            new WorldStateList(
                new WorldState<object>("nearEnemy", true)
            )
        ));
        // Aim
        actions.Add(new Action(
            new WorldStateList(
                new WorldState<object>("enemyVisible", true),
                new WorldState<object>("weaponLoaded", true)
            ),
            new WorldStateList(
                new WorldState<object>("enemyLinedUp", true)
            )
        ));
        // Shoot
        actions.Add(new Action(
            new WorldStateList(
                new WorldState<object>("enemyLinedUp", true)
            ),
            new WorldStateList(
                new WorldState<object>("enemyAlive", false)
            )
        ));
        // Load
        actions.Add(new Action(
            new WorldStateList(
                new WorldState<object>("armedWithGun", true)
            ),
            new WorldStateList(
                new WorldState<object>("weaponLoaded", true)
            )
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
            )
        ));
        // Flee
        actions.Add(new Action(
            new WorldStateList(
                new WorldState<object>("enemyVisible", true)
            ),
            new WorldStateList(
                new WorldState<object>("nearEnemy", false)
            )
        ));

        Goal goal = new Goal(
            new WorldStateList(
                new WorldState<object>("enemyAlive", false),
                new WorldState<object>("alive", true)
            )
        );

        List<Action> plan = AStar.PlanAction(worldState, goal, actions);
    }
}
