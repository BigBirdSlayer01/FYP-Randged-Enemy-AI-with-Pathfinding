using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class IdleState : BaseState
{
    float WaitTime; //time to wait in idle state
    float minWait = 7.5f; //min wait time
    float maxWait = 12.5f; //max wait time

    public override void EnterState(StateMachine machine)
    {
        Debug.Log("Entered the idle State");
        WaitTime = Random.Range(minWait, maxWait); //sets random wait time for idle
    }

    public override void UpdateState(StateMachine machine)
    {
        //will need to add check if player seen -> go to Combat State, WaitTime check should be the else as is less important than seeing player
        WaitTime -= Time.deltaTime;
        if(WaitTime <= 0) // if wait time has run out switch to the patrol state
        {
            machine.SwitchState(machine.patrolState); //switch to the patrol state
        }
        Casting(machine); //checks if the player can be seen
    }

    public void Casting(StateMachine machine) //checks if player can be seen
    {
        //checks the angle that the player is away from the gameobject for line of sight
        Vector3 targetDirection = machine.a.target.transform.position - machine.gameObject.transform.position; //stores the direction to the player
        float angleToTarget = Vector3.Angle(targetDirection, machine.transform.forward); //stores the angle between the player and the object

        //if the angle between the player and object is less than or equal to 50 fire a raycast
        if (angleToTarget < machine.ViewingAngle)
        {
            float distanceToPlayer = targetDirection.magnitude; //stores the distance to the player
            Ray ray = new Ray(machine.gameObject.transform.position, targetDirection.normalized); //creates a ray
            RaycastHit hit; //stores the object that the ray hits
            if (Physics.Raycast(ray, out hit, distanceToPlayer))
            {
                if (hit.collider.gameObject == machine.a.target.gameObject)
                {
                    Debug.DrawLine(machine.gameObject.transform.position, machine.a.target.transform.position, Color.blue, 1);
                    machine.gameObject.transform.LookAt(machine.a.target.transform.gameObject.transform); //look at the player
                    machine.a.follow = false;
                    machine.SwitchState(machine.combatState); //switch to combat state
                }
            }
        }
    }
}

