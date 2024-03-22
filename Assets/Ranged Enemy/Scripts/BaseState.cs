using UnityEngine;

public abstract class BaseState
{
    //state methods
    public abstract void EnterState(StateMachine machine);

    public abstract void UpdateState(StateMachine machine);
}
