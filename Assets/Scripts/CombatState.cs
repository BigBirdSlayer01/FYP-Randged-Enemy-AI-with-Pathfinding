using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CombatState : BaseState
{
    public override void EnterState(StateMachine machine)
    {
        GameManager.instance.StartCoroutine(ShootCountDown(machine));
    }

    public override void UpdateState(StateMachine machine)
    {
        canSee(machine);
    }

    void fire(StateMachine machine) //for shooting at the player
    { 
        RaycastHit hit; //stores the object that the ray hits
        Ray ray = new Ray(machine.thisObject.transform.position, machine.thisObject.transform.forward); //creates a ray
        Vector3 pos = new Vector3();
        if(Physics.Raycast(ray, out hit)) //if the ray hits
        {
            if(hit.transform.gameObject != GameManager.instance.thePlayer) //if not the player this may be bad re look at it
            {
                Debug.Log("blocked");
                GameManager.instance.StopAllCoroutines();
                machine.SwitchState(machine.searchState);
            }
            else if(hit.transform.gameObject == GameManager.instance.thePlayer) //else if it is the player
            {
                Transform hitObject = hit.transform;
                Debug.Log(hit.transform.name);
                Debug.DrawRay(machine.thisObject.transform.position, machine.thisObject.transform.forward, Color.red);
                Debug.DrawLine(machine.thisObject.transform.position, GameManager.instance.thePlayer.transform.position, Color.red, 1);
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
        machine.thisObject.transform.LookAt(GameManager.instance.thePlayer.transform);
        GameManager.instance.StartCoroutine(ShootCountDown(machine));
    }

    void canSee(StateMachine machine)
    {
        if (!Physics.Linecast(machine.thisObject.transform.position, GameManager.instance.thePlayer.transform.position))
        {
            Debug.Log("blocked");
            GameManager.instance.StopAllCoroutines();
            machine.SwitchState(machine.idleState);
        }
    }
}
