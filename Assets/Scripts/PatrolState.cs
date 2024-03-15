using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.AI;

public class PatrolState : BaseState //State for patrolling enemy
{
    Vector3 GoalPosition;
    Node GoalNode;
    bool started;
    Coroutine followingPathCoroutine;
    Coroutine UpdatePath;
    bool lastVisited1;

    public override void EnterState(StateMachine machine) //start method
    {
        Debug.Log("Entered Patrol State");
        if(machine.UseRandomPoints) //called if using the random patrol points system
        {
            GoalNode = Grid.instance.GetRandomWalkableNode();
            machine.a.FindPath(machine.gameObject.transform.position, GoalNode.worldPos);
        }
        else //called if using static patrol points
        {
            if(!lastVisited1) //if not at patrol point 1 go to patrol point 1
            {
                GoalNode = Grid.instance.NodeFromWorldPoint(machine.PatrolPoint1);
                lastVisited1 = true;
            }
            else if(lastVisited1) //else if not at patrol point 2 go to patrol point 2
            {
                GoalNode = Grid.instance.NodeFromWorldPoint(machine.PatrolPoint2);
                lastVisited1 = false;
            }
            else //else go to a random point (should not be called unless there is an issue with the patrol points
            {
                GoalNode = Grid.instance.GetRandomWalkableNode();
            }
            machine.a.FindPath(machine.gameObject.transform.position, GoalNode.worldPos);
        }
        machine.a.follow = true;
        started = false;
    }

    public override void UpdateState(StateMachine machine) //update method
    {
        Casting(machine); //checks if the player can be seen

        //ReachedPosition(machine); //checks if the enemy has reached the goal position

        if(machine.a.pathFound) //checks if the path has been found
        {
            if(!started) //if has not started
            {
                machine.a.follow = true; //sets the follow to true
                machine.a.FollowPath(); //starts the coroutine to follow the path
                UpdatePath = machine.StartCoroutine(SearchPathAgain(machine));
                started = true; //sets the started bool to true
            }
            
            ReachedPosition(machine); //checks if the enemy has reached the goal position
        }
    }

    void ReachedPosition(StateMachine machine) //method that moves the enemy
    {
        //if the enemy reaches the destination switch back to the idle state
        if((int)machine.gameObject.transform.position.z == (int)GoalNode.worldPos.z && (int)machine.gameObject.transform.position.x == (int)GoalNode.worldPos.x)
        {
            //followingPathCoroutine = machine.a.StartCoroutine(machine.a.followingPath());
            machine.SwitchState(machine.idleState);
        }
    }

    IEnumerator SearchPathAgain(StateMachine machine)
    {

        yield return new WaitForSeconds(machine.TimeToUpdatePath);
        if(machine.a.follow && machine.currentState == machine.patrolState)
        {
            machine.a.FindPath(machine.gameObject.transform.position, GoalNode.worldPos);
            UpdatePath = machine.StartCoroutine(SearchPathAgain(machine));
        }   
    }

    public void Casting(StateMachine machine) //checks if player can be seen
    {
        //checks the angle that the player is away from the gameobject for line of sight
        Vector3 targetDirection = machine.a.target.transform.position - machine.gameObject.transform.position;
        float angleToTarget = Vector3.Angle(targetDirection, machine.transform.forward);
        
        //if the angle between the player and object is less than or equal to 50 fire a raycast
        if(angleToTarget < machine.ViewingAngle)
        {
            float distanceToPlayer = targetDirection.magnitude;
            Ray ray = new Ray(machine.gameObject.transform.position, targetDirection.normalized);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, distanceToPlayer))
            {
                if(hit.collider.gameObject == machine.a.target.gameObject)
                {
                    Debug.DrawLine(machine.gameObject.transform.position, machine.a.target.transform.position, Color.blue, 1);
                    machine.gameObject.transform.LookAt(machine.a.target.transform.gameObject.transform); //look at the player
                    machine.a.follow = false;
                    machine.StopCoroutine(UpdatePath);
                    machine.SwitchState(machine.combatState); //switch to combat state
                }
            }
        }
    }
}
