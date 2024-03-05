using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Collider coll;
    [HideInInspector] public GameObject target;
    [HideInInspector] public Vector3 startPos;
    float speed;
    float TimeToLive;
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
        linePoints = 175;
        timeIntervalPoints = 0.01f;
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
        coll.isTrigger = true;
        rb.mass = 0.1f;
        rb.angularDrag = 0;
        speed = 700;
        TimeToLive = 10;
        rb.useGravity = false; //ensures rigidbodies gravity is not used
        rb.interpolation = RigidbodyInterpolation.None;
        timeIntervalPoints = 0.01f;

        StartCoroutine(DestroyProjectile());
    }

    private void Update()
    {
        if(lineRenderer != null)
        {
            drawBulletTrajectory();
            lineRenderer.enabled = true;
        }
    }

    IEnumerator DestroyProjectile()
    {
        yield return new WaitForSeconds(TimeToLive);
        Destroy(gameObject);
    }

    // this algorithm applies the 
    // 
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
