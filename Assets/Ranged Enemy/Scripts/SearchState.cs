using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchState : BaseState
{
    Node LastPosNode; //last position of the player
    bool started; //bool to check if the coroutine has started
    bool seenPlayer; //bool to check if the player has been seen
    Coroutine followingPathCoroutine; //coroutine to follow the path
    Coroutine UpdatePath; //coroutine to update the path

    public override void EnterState(StateMachine machine) //start method
    {
        Debug.Log("Entered Search State"); //debug message
        LastPosNode = machine.PlayerLastKnow; //sets the last position of the player
        machine.a.FindPath(machine.gameObject.transform.position, Grid.instance.WorldPointFromNode(LastPosNode)); //finds the path to the last known position of the player
        machine.a.follow = true; //sets the follow bool to true
        started = false; //sets the started bool to false
    }

    public override void UpdateState(StateMachine machine)
    {
        Casting(machine); //checks if the player can be seen 
        ReachedPosition(machine); //checks if the enemy has reached the goal position
        if (machine.a.pathFound) //checks if the path has been found
        {
            if (!started) //if has not started
            {
                machine.a.follow = true; //sets the follow to true
                machine.a.FollowPath(); //starts the coroutine to follow the path
                UpdatePath = machine.StartCoroutine(SearchPathAgain(machine)); //starts the coroutine to update the path
                started = true; //sets the started bool to true
            }
        }
    }

    void ReachedPosition(StateMachine machine) //method that moves the enemy
    {
        //if the enemy reaches the destination switch back to the idle state
        if ((int)machine.gameObject.transform.position.z == (int)Grid.instance.WorldPointFromNode(LastPosNode).z && (int)machine.gameObject.transform.position.x == (int)Grid.instance.WorldPointFromNode(LastPosNode).x)
        {
            if(!seenPlayer) //if the player has not been seen
            {
                machine.a.follow = false; //set the follow bool to false
                machine.StopCoroutine(UpdatePath); //stop the coroutine to update the path
                machine.SwitchState(machine.idleState); //switch to the idle state
            }
        }
    }

    IEnumerator SearchPathAgain(StateMachine machine) //coroutine to update the path
    {
        yield return new WaitForSeconds(machine.TimeToUpdatePath); //waits for the time to update the path
        if (machine.a.follow && machine.currentState == machine.searchState) //if the follow bool is true and the current state is the search state
        {
            machine.a.FindPath(machine.gameObject.transform.position, Grid.instance.WorldPointFromNode(LastPosNode)); //finds the path to the last known position of the player
            machine.StopCoroutine(UpdatePath); //stops the coroutine to update the path
            UpdatePath = machine.StartCoroutine(SearchPathAgain(machine)); //starts the coroutine to update the path
        }
        else //else
        {
            yield break; //yield break
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
