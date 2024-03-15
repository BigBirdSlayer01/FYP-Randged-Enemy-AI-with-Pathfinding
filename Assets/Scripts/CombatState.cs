using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CombatState : BaseState
{
    Coroutine shooting;
    public override void EnterState(StateMachine machine)
    {
        Debug.Log("Entered Combat State");
        shooting = machine.StartCoroutine(ShootCountDown(machine));
    }

    public override void UpdateState(StateMachine machine)
    {

    }

    void fire(StateMachine machine) //for shooting at the player
    { 
        RaycastHit hit; //stores the object that the ray hits
        Ray ray = new Ray(machine.thisObject.transform.position, machine.thisObject.transform.forward); //creates a ray
        Vector3 pos = new Vector3();
        if(Physics.Raycast(ray, out hit)) //if the ray hits
        {
            if(hit.transform.gameObject != machine.a.target) //if not the player this may be bad re look at it
            {
                machine.StopCoroutine(shooting);
                machine.SwitchState(machine.searchState);
            }
            else if(hit.transform.gameObject == machine.a.target) //else if it is the player
            {
                Transform hitObject = hit.transform;
                Debug.Log(hit.transform.name);
                Debug.DrawRay(machine.thisObject.transform.position, machine.thisObject.transform.forward, Color.red);
                Debug.DrawLine(machine.thisObject.transform.position, machine.a.target.transform.position, Color.red, 1);
                GameObject projectile = GameObject.Instantiate(machine.Projectile, machine.transform.position, Quaternion.identity);
                projectile.GetComponent<Projectile>().startPos = machine.transform.position;
                projectile.gameObject.GetComponent<Projectile>().target = hit.transform.gameObject;
                pos.x = (int)hit.transform.position.x;
                pos.y = (int)hit.transform.position.y;
                pos.z = (int)hit.transform.position.z;
                machine.PlayerLastKnow = Grid.instance.NodeFromWorldPoint(pos);
            }
        }
    }

    IEnumerator ShootCountDown(StateMachine machine)
    {
        fire(machine);
        yield return new WaitForSeconds(2);
        if(machine.currentState == machine.combatState)
        {
            machine.thisObject.transform.LookAt(machine.a.target.transform);
            shooting = machine.StartCoroutine(ShootCountDown(machine));
        }
        else
        {
            yield break;
        }
        
    }
}
