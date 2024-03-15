using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class IdleState : BaseState
{
    float WaitTime;
    float minWait = 7.5f;
    float maxWait = 12.5f;

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
            machine.SwitchState(machine.patrolState);
        }
        Casting(machine);
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

