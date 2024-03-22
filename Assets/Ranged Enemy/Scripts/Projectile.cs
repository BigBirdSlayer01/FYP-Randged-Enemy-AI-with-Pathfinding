using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public Rigidbody rb; //stores the rigidbody of the bullet
    [HideInInspector] public Collider coll; //stores the collider of the bullet
    [HideInInspector] public GameObject target; //stores the target of the bullet
    [HideInInspector] public Vector3 startPos; //stores the start position of the bullet
    [Header("Bullet Speed")]
    public float speed = 700; //stores the speed of the bullet
    float TimeToLive; //stores the time the bullet will live for
    [Header("Used for Bullet Drop Off")]
    public float drag; // stores the required drag of the bullet
    public float airDensity; // stores the air density

    //for debugging;
    LineRenderer lineRenderer; 
    int linePoints;
    float timeIntervalPoints;

    void Start()
    {
        SetupVariables();
        StartCoroutine(SimulatePhysics()); //starts the bullet physics simulation
    }

    void SetupVariables()
    {
        linePoints = 175; //sets the amount of points for the line renderer
        timeIntervalPoints = 0.01f; //sets the time interval for the line renderer
        rb = GetComponent<Rigidbody>(); //gets the rigidbody of the bullet
        coll = GetComponent<Collider>(); //gets the collider of the bullet
        coll.isTrigger = true; //sets the collider to a trigger
        rb.mass = 0.1f;     //sets the mass of the bullet
        rb.angularDrag = 0; //sets the angular drag of the bullet
        TimeToLive = 10; //sets the time to live of the bullet
        rb.useGravity = false; //ensures rigidbodies gravity is not used
        rb.interpolation = RigidbodyInterpolation.None; //sets the interpolation of the rigidbody
        timeIntervalPoints = 0.01f; //sets the time interval for the line renderer

        StartCoroutine(DestroyProjectile()); //starts the destroy projectile coroutine
    }

    private void Update()
    {
        if(lineRenderer != null)
        {
            drawBulletTrajectory(); //draws the bullet trajectory
            lineRenderer.enabled = true; //enables the line renderer
        }
    }

    IEnumerator DestroyProjectile()
    {
        yield return new WaitForSeconds(TimeToLive); //waits for the time to live
        Destroy(gameObject); //destroys the bullet
    }

    IEnumerator SimulatePhysics() 
    {
        float time = 0; //initaites the time variable
        Vector3 position = transform.position; //stores the position of the object
        Vector3 velocity = speed * (target.transform.position - transform.position).normalized; // stores the velocity of the object

        while(time < TimeToLive) //loops until the bullet object needs to be destroyed
        {
            Vector3 newVel = FindNewVelocity(velocity); // gets new velocity
            position = position + newVel * timeIntervalPoints; //gets new position

            transform.position = position; // sets new position
            velocity = newVel; //sets new velocity

            yield return null;
            time += timeIntervalPoints; // increases time variable
        }
        Destroy(gameObject); //destroys object
    }

    Vector3 FindNewVelocity(Vector3 vel) //generates the new velocity based on drag, air density and gravity 
    {
        if(drag == 0)
        {
            drag = 0.1f; //low amount of drag, this is only used if the user doesnt set any drag
        }
        if(airDensity == 0)
        {
            airDensity = 1.255f; //this is set as it is the air desnity at sea level, should be adjusted to serve needs
        }

        Vector3 gravity = Physics.gravity; // stores gravity
        Vector3 resistance = -drag * airDensity * vel.magnitude * vel.normalized; // calculates the resistance force 

        Vector3 airAccel = resistance / rb.mass; // takes the resistance and divides it by the mass of the object
        Vector3 Accel = airAccel + gravity; // adds the acceleration to the gravity

        Vector3 returnVal = vel + Accel * timeIntervalPoints; //adds the velocity and acceleration over time

        return returnVal; //returns new velocity
    }


    void drawBulletTrajectory()
    {
        Vector3 startVeclocity = speed * (target.transform.position - transform.position);
        lineRenderer.positionCount = linePoints;
        float time = 0;
        for(int i = 0; i < linePoints; i++)
        {
            float x = (startVeclocity.x * time) + (Physics.gravity.x / 2 * time * time);
            float y = (startVeclocity.y * time) + (Physics.gravity.y / 2 * time * time);
            Vector3 p = new Vector3(x, y, 0);
            lineRenderer.SetPosition(i, startPos + p);
            time += timeIntervalPoints;
        }
    }
}
