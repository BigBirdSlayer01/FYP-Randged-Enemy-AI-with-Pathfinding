using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class StateMachine : MonoBehaviour
{
    //current state
    public BaseState currentState; // variable to hold base state variable
    //list of the states in game
    public IdleState idleState = new IdleState(); // idle state variable
    public SearchState searchState = new SearchState(); // search state variable
    public CombatState combatState = new CombatState(); // combat state variable
    public PatrolState patrolState = new PatrolState(); // patrol state variable

    [HideInInspector] public Vector3 CenterPoint; //center point used in calculations of random point

    public Vector3 PatrolPoint1; //stores point to patrol to
    public Vector3 PatrolPoint2; //stores point to patrol to

    public bool UseRandomPoints; //set to true if random patroling is desired

    public int ViewingAngle = 100; //angle that the field of view is at

    [HideInInspector] public Node PlayerLastKnow; //used to store last player positiojn for search state

    [HideInInspector] public GameObject thisObject; //the current game object
    public GameObject Projectile; //bullet game object

    [HideInInspector] public AStar a;

    public int TimeToUpdatePath = 5; //wait time before checking path again, higher the number the higher the performance

    void Start()
    {
        currentState = idleState; //idle set as the default current state
        
        currentState.EnterState(this); //call enter state

        //sets up navmesh variables
        CenterPoint = new Vector3(0,0,0);

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
