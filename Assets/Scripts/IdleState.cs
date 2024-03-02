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

    public void Casting(StateMachine machine)
    {
        var ray = new Ray(machine.thisObject.transform.position,machine.thisObject.transform.forward);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 100))
        {
            if (hit.transform.gameObject == GameManager.instance.thePlayer)
            {
                Debug.Log("Hit");
                machine.SwitchState(machine.combatState);
            }
        }
    }
}

