using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateMachine : MonoBehaviour
{

    //current state
    BaseState currentState; // variable to hold base state variable
    //list of the states in game
    public IdleState idleState = new IdleState(); // idle state variable
    public SearchState searchState = new SearchState(); // search state variable
    public CombatState combatState = new CombatState(); // combat state variable
    public PatrolState patrolState = new PatrolState(); // patrol state variable

    //navmesh details
    public NavMeshAgent agent; // navmesh agent - the game object script is attached to
    public float TravelRange; // range at which the object can travel
    public Vector3 CenterPoint; //center point used in calculations of random point

    public Vector3 PatrolPoint1; //stores point to patrol to
    public Vector3 PatrolPoint2; //stores point to patrol to

    public bool UseRandomPoints; //set to true if random patroling is desired

    public Vector3 PlayerLastKnow; //used to store last player positiojn for search state

    public GameObject thisObject; //the current game object
    public GameObject Projectile; //bullet game object

    public AStar a;

    void Start()
    {
        currentState = idleState; //idle set as the default current state
        
        currentState.EnterState(this); //call enter state

        //sets up navmesh variables
        agent = GetComponent<NavMeshAgent>();
        CenterPoint = new Vector3(0,0,0);
        TravelRange = 50.0f;
        agent.stoppingDistance = 0.0f;

        thisObject = gameObject;
        a = GetComponent<AStar>();
    }

    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState(this); //updates the currentState
    }

    public void SwitchState(BaseState state) // used to switch between state and call the states EnterState method
    {
        currentState = state;
        state.EnterState(this);
    }
}
