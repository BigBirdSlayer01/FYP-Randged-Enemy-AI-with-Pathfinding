using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  Each ray will re store the play position in combat state ?
 *  Variable will need to be created in Base State ?
 *  Using AStar move to stored previous position ?
 *  look around - make it better
 *  if player seen go back to combat state ?
 *  else go back to patrol state ?
 */

public class SearchState : BaseState
{
    Node LastPosNode;
    bool started;
    bool seenPlayer;
    Coroutine followingPathCoroutine;
    Coroutine UpdatePath;

    public override void EnterState(StateMachine machine)
    {
        Debug.Log("Entered Search State");
        LastPosNode = machine.PlayerLastKnow;
        machine.a.FindPath(machine.gameObject.transform.position, Grid.instance.WorldPointFromNode(LastPosNode));
        machine.a.follow = true;
        started = false;
    }

    public override void UpdateState(StateMachine machine)
    {
        Casting(machine); //checks if the player can be seen
        ReachedPosition(machine);
        if (machine.a.pathFound)
        {
            if (!started)
            {
                machine.a.follow = true;
                machine.a.FollowPath();
                UpdatePath = machine.StartCoroutine(SearchPathAgain(machine));
                started = true;
            }
        }

    }

    void ReachedPosition(StateMachine machine) //method that moves the enemy
    {
        //if the enemy reaches the destination switch back to the idle state
        if ((int)machine.gameObject.transform.position.z == (int)Grid.instance.WorldPointFromNode(LastPosNode).z && (int)machine.gameObject.transform.position.x == (int)Grid.instance.WorldPointFromNode(LastPosNode).x)
        {
            if(!seenPlayer)
            {
                machine.a.follow = false;
                machine.StopCoroutine(UpdatePath);
                machine.SwitchState(machine.idleState);
            }
        }
    }

    IEnumerator SearchPathAgain(StateMachine machine)
    {
        yield return new WaitForSeconds(machine.TimeToUpdatePath);
        if (machine.a.follow && machine.currentState == machine.searchState)
        {
            machine.a.FindPath(machine.gameObject.transform.position, Grid.instance.WorldPointFromNode(LastPosNode));
            machine.StopCoroutine(UpdatePath);
            UpdatePath = machine.StartCoroutine(SearchPathAgain(machine));
        }
        else
        {
            yield break;
        }
    }

    public void Casting(StateMachine machine) //checks if player can be seen
    {
        //checks the angle that the player is away from the gameobject for line of sight
        Vector3 targetDirection = machine.a.target.transform.position - machine.gameObject.transform.position;
        float angleToTarget = Vector3.Angle(targetDirection, machine.transform.forward);

        //if the angle between the player and object is less than or equal to 50 fire a raycast
        if (angleToTarget < machine.ViewingAngle)
        {
            float distanceToPlayer = targetDirection.magnitude;
            Ray ray = new Ray(machine.gameObject.transform.position, targetDirection.normalized);
            RaycastHit hit;
            //if (Physics.Linecast(machine.gameObject.transform.position, GameManager.instance.thePlayer.transform.position) == false)
            if (Physics.Raycast(ray, out hit, distanceToPlayer))
            {
                if (hit.collider.gameObject == machine.a.target.gameObject)
                {
                    seenPlayer = true;
                    Debug.DrawLine(machine.gameObject.transform.position, machine.a.target.transform.position, Color.blue, 1);
                    machine.gameObject.transform.LookAt(machine.a.target.transform.gameObject.transform); //look at the player
                    machine.a.follow = false;
                    machine.StopCoroutine(UpdatePath);
                    machine.SwitchState(machine.combatState); //switch to combat state
                }
                else
                {
                    seenPlayer = false;
                }
            }
        }
    }
}
