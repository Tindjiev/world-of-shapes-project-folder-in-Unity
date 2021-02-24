using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStateMachine
{
    void Initialize();

    void LogicalUpdate();
    void LogicalFixedUpdate();
    void LogicalLateUpdate();

    void ChangeState(IState newState);
    void ClearState();

}

public class StateMachine : IStateMachine
{
    protected IState _currentState, _startingState;

    public void SetStartingState(IState startingState)
    {
        _startingState = startingState;
    }

    public void Initialize()
    {
        (_currentState = _startingState).OnStateEnter();
    }

    public void LogicalUpdate()
    {
        _currentState.LogicalUpdate();
    }
    public void LogicalFixedUpdate()
    {
        _currentState.LogicalFixedUpdate();
    }
    public void LogicalLateUpdate()
    {
        _currentState.LogicalLateUpdate();
    }

    public void ChangeState(IState newState)
    {
        if (newState == _currentState) return;
        _currentState.OnStateExit();
        (_currentState = newState).OnStateEnter();
    }

    public void ClearState()
    {
        _currentState.OnStateExit();
        _currentState = null;
    }

}



public interface IState
{

    void OnStateEnter();
    
    void LogicalUpdate();
    void LogicalFixedUpdate();
    void LogicalLateUpdate();

    void OnStateExit();

}
