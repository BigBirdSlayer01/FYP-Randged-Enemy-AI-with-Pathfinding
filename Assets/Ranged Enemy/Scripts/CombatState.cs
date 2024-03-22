using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CombatState : BaseState
{
    Coroutine shooting;
    public override void EnterState(StateMachine machine)
    {
        Debug.Log("Entered Combat State"); //debug message
        shooting = machine.StartCoroutine(ShootCountDown(machine)); //starts the shooting coroutine
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
                machine.StopCoroutine(shooting); //stops the shooting coroutine
                machine.SwitchState(machine.searchState); //switches to the search state
            }
            else if(hit.transform.gameObject == machine.a.target) //else if it is the player
            {
                Transform hitObject = hit.transform; //stores the object that the ray hits
                Debug.DrawRay(machine.thisObject.transform.position, machine.thisObject.transform.forward, Color.red);
                Debug.DrawLine(machine.thisObject.transform.position, machine.a.target.transform.position, Color.red, 1);
                GameObject projectile = GameObject.Instantiate(machine.Projectile, machine.transform.position, Quaternion.identity); //creates a projectile
                projectile.GetComponent<Projectile>().startPos = machine.transform.position;    //sets the start position of the projectile
                projectile.gameObject.GetComponent<Projectile>().target = hit.transform.gameObject; //sets the target of the projectile
                pos.x = (int)hit.transform.position.x; //sets the x position of the player
                pos.y = (int)hit.transform.position.y; //sets the y position of the player
                pos.z = (int)hit.transform.position.z; //sets the z position of the player
                machine.PlayerLastKnow = Grid.instance.NodeFromWorldPoint(pos); //sets the players last known position
            }
        }
    }

    IEnumerator ShootCountDown(StateMachine machine) //coroutine to shoot at the player
    {
        fire(machine); //fires at the player
        yield return new WaitForSeconds(2); //waits for 2 seconds
        if(machine.currentState == machine.combatState) //if the current state is the combat state
        {
            machine.thisObject.transform.LookAt(machine.a.target.transform); //look at the player
            shooting = machine.StartCoroutine(ShootCountDown(machine)); //starts the shooting coroutine
        }
        else
        {
            yield break; //breaks the coroutine
        }
        
    }
}
