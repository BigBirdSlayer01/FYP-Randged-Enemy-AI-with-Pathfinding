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
    Vector3 LastPosNode;
    bool ReachedLastPos;
    bool started;
    Coroutine followingPathCoroutine;

    public override void EnterState(StateMachine machine)
    {
        Debug.Log("Entered Search State");
        LastPosNode = machine.PlayerLastKnow;
        machine.a.FindPath(machine.gameObject.transform.position, LastPosNode);
        ReachedLastPos = false;
        machine.a.follow = true;
        started = false;
    }

    public override void UpdateState(StateMachine machine)
    {
        Casting(machine); //checks if the player can be seen
        ReachedPosition(machine);
        //if the enemy has yet to reach the players last known position
        if(!ReachedLastPos)
        {
            //machine.a.FindPath(machine.gameObject.transform.position, LastPosNode);
            //machine.a.MoveAlongPath(); // move along the A* path
        }
        if (machine.a.pathFound)
        {
            if (!started)
            {
                Debug.Log("Starting Coroutine");
                machine.a.follow = true;
                followingPathCoroutine = machine.a.StartCoroutine(machine.a.followingPath());
                started = true;
            }
        }

    }

    void ReachedPosition(StateMachine machine) //method that moves the enemy
    {
        //if the enemy reaches the destination switch back to the idle state
        if ((int)machine.gameObject.transform.position.z == (int)LastPosNode.z && (int)machine.gameObject.transform.position.x == (int)LastPosNode.x)
        {
            ReachedLastPos=true;
            machine.a.follow = false;
            machine.SwitchState(machine.idleState);
        }
    }

    public void Casting(StateMachine machine) //checks if player can be seen
    {
        //checks the angle that the player is away from the gameobject for line of sight
        Vector3 targetDirection = GameManager.instance.thePlayer.transform.position - machine.gameObject.transform.position;
        float angleToTarget = Vector3.Angle(targetDirection, machine.transform.forward);

        //if the angle between the player and object is less than or equal to 50 fire a raycast
        if (angleToTarget < 100)
        {
            float distanceToPlayer = targetDirection.magnitude;
            Ray ray = new Ray(machine.gameObject.transform.position, targetDirection.normalized);
            RaycastHit hit;
            //if (Physics.Linecast(machine.gameObject.transform.position, GameManager.instance.thePlayer.transform.position) == false)
            if (Physics.Raycast(ray, out hit, distanceToPlayer))
            {
                if (hit.collider.gameObject == GameManager.instance.thePlayer.gameObject)
                {

                    Debug.DrawLine(machine.gameObject.transform.position, GameManager.instance.thePlayer.transform.position, Color.blue, 1);
                    machine.gameObject.transform.LookAt(GameManager.instance.thePlayer.transform.gameObject.transform); //look at the player
                    machine.a.follow = false;
                    machine.SwitchState(machine.combatState); //switch to combat state
                }
            }

            //var ray = new Ray(machine.thisObject.transform.position, machine.thisObject.transform.forward); //creates the ray
            //RaycastHit hit; //will store what object is hit
            //if (Physics.Raycast(ray, out hit, 100)) //if ray hits something
            //{
            //    if (hit.transform.gameObject == GameManager.instance.thePlayer) //if the player is seen
            //    {
            //        //machine.agent.isStopped = true; //stops agent moving
            //        machine.gameObject.transform.LookAt(hit.transform.gameObject.transform); //look at the player
            //        Debug.Log("Hit");
            //        machine.SwitchState(machine.combatState); //switch to combat state
            //    }
            //}
        }
    }
}
