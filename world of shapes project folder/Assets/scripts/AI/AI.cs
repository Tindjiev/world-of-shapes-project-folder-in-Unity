using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : BaseCharacterControl, IStateMachine
{

    [SerializeField, ReadOnlyOnInspector] private AIModeClass _currentMode = null;
    [SerializeField, ReadOnlyOnInspector] private ChillModeClass _chillMode = null;
    [SerializeField, ReadOnlyOnInspector] private AttackModeClass _attackMode = null;
    [SerializeField, ReadOnlyOnInspector] private FleeModeClass _fleeMode = null;
    [SerializeField, ReadOnlyOnInspector] private ObjectiveModeClass _objectiveMode = null;

    public void Initialize()
    {
        SetModeChill();
    }

    public void LogicalUpdate()
    {
        CheckForErrors();
        CheckToChangeMode();
        _currentMode.SetDirectionOfCharacter();
        _currentMode.LogicalUpdate();
    }

    public void LogicalFixedUpdate()
    {
        throw new NotImplementedException();
    }

    public void LogicalLateUpdate()
    {
        throw new NotImplementedException();
    }

    public void ChangeState(IState newState)
    {
        if(!(newState is AIModeClass newAIMode)) throw new Exception("State is not an AImodeClass");
        if (_currentMode == newAIMode) return;
        if (newAIMode == _chillMode)
        {
            SetModeChill();
        }
        else if(newAIMode == _attackMode)
        {
            SetModeAttack();
        }
        else if(newAIMode == _fleeMode)
        {
            SetModeFlee();
        }
        else if(newAIMode == _objectiveMode)
        {
            SetModeObjective();
        }
        throw new Exception("AImodeClass not found as part of the AI");
    }

    public void ClearState()
    {
        SetModeChill();
    }

    protected new void Awake()
    {
        base.Awake();
        _currentMode = null;
    }

    protected new void Start()
    {
        base.Start();
        SetAImodes();
        Initialize();
    }

    private void Update()
    {
        LogicalUpdate();
    }

    private void CheckToChangeMode()
    {
        if (CheckToChangeToObjectiveMode()) SetModeObjective();
        else if (CheckToChangeToFleeMode()) SetModeFlee();
        else if (CheckToChangeToAttackMode()) SetModeAttack();
        else if (_currentMode.CheckToChange() || CheckToChangeToChillMode()) SetModeChill();
    }

    private void CheckForErrors()
    {
        if (_currentMode == null) SetAImodes();
    }

    protected void OnDisable()
    {
        ClearState();
        MoveComponent.ClearPath();
    }

    private void SetAImodes()
    {
        _chillMode = this.SearchComponent<ChillModeClass>();
        _attackMode = this.SearchComponent<AttackModeClass>();
        _fleeMode = this.SearchComponent<FleeModeClass>();
        _objectiveMode = this.SearchComponent<ObjectiveModeClass>();
    }



    public Mode AssignModeScript<Mode>() where Mode : AIModeClass
    {
        Mode tempMode = gameObject.AddComponent<Mode>();
        SetAImodes();
        return tempMode;
    }

    public bool CheckModeChill() => _currentMode == _chillMode;
    public bool CheckModeAttack() => _currentMode == _attackMode;
    public bool CheckModeFlee() => _currentMode == _fleeMode;
    public bool CheckModeObjective() => _currentMode == _objectiveMode;

    public bool CheckModeChillNot() => !CheckModeChill();
    public bool CheckModeAttackNot() => !CheckModeAttack();
    public bool CheckModeFleeNot() => !CheckModeFlee();
    public bool CheckModeObjectiveNot() => !CheckModeObjective();



    public void SetModeChill()
    {
        if (CheckModeChill()) return;
        if (_currentMode != null) _currentMode.OnStateExit();
        //Debug.Log("c", this);
        (_currentMode = _chillMode).OnStateEnter();
    }
    public void SetModeAttack()
    {
        if (CheckModeAttack()) return;
        if (_currentMode != null) _currentMode.OnStateExit();
        //Debug.Log("a", this);
        (_currentMode = _attackMode).OnStateEnter();
    }
    public void SetModeFlee()
    {
        if (CheckModeFlee()) return;
        if (_currentMode != null) _currentMode.OnStateExit();
        //Debug.Log("f",this);
        (_currentMode = _fleeMode).OnStateEnter();
    }
    public void SetModeObjective()
    {
        if (CheckModeObjective()) return;
        if (_currentMode != null) _currentMode.OnStateExit();
        //Debug.Log("o",this);
        (_currentMode = _objectiveMode).OnStateEnter();
    }

    public ChillModeClass GetChillMode()
    {
        return _chillMode;
    }
    public AttackModeClass GetAttackMode()
    {
        return _attackMode;
    }
    public FleeModeClass GetFleeMode()
    {
        return _fleeMode;
    }
    public ObjectiveModeClass GetObjectiveMode()
    {
        return _objectiveMode;
    }

    public Mode GetMode<Mode>() where Mode : AIModeClass
    {
        if (typeof(Mode).IsExactTypeOrSubClassOf<ChillModeClass>())
        {
            return _chillMode as Mode;
        }
        else if (typeof(Mode).IsExactTypeOrSubClassOf<AttackModeClass>())
        {
            return _attackMode as Mode;
        }
        else if (typeof(Mode).IsExactTypeOrSubClassOf<FleeModeClass>())
        {
            return _fleeMode as Mode;
        }
        else if (typeof(Mode).IsExactTypeOrSubClassOf<ObjectiveModeClass>())
        {
            return _objectiveMode as Mode;
        }
        return null;
    }

    public bool CheckToChangeToChillMode() => _chillMode != null && _chillMode != _currentMode ? _chillMode.CheckToActivate() : false;
    public bool CheckToChangeToAttackMode() => _attackMode != null && _attackMode != _currentMode ? _attackMode.CheckToActivate() : false;
    public bool CheckToChangeToFleeMode() => _fleeMode != null && _fleeMode != _currentMode ? _fleeMode.CheckToActivate() : false;
    public bool CheckToChangeToObjectiveMode() => _objectiveMode != null && _objectiveMode != _currentMode ? _objectiveMode.CheckToActivate() : false;

    public Vector3 PredictDirection(Vector3 targetPosition, float projectileSpeed, Vector3 targetVelocity)
    {
        return PredictDirection(Position, targetPosition, projectileSpeed, targetVelocity);
    }

    public static Vector3 PredictDirection(Vector3 shooterPosition, Vector3 targetPosition, float projectileSpeed, Vector3 targetVelocity)
    {
        float speedRatioSq = projectileSpeed.Sq() / targetVelocity.sqrMagnitude; //vr^2 = vw^2 / vb^2

        targetVelocity.Normalize();     //from now on targetVelocity will be nomralized (magnitude == 1)
        Vector3 adjustedPosition = MyMathlib.MultiplyComplexConjugate(shooterPosition - targetPosition, targetVelocity); //dw = (xw,yw)

        if (speedRatioSq == 1f) // Equal velocities, its not a quadratic equation in this case
        {
            return adjustedPosition.x <= 0f ? targetVelocity :
                (targetPosition + adjustedPosition.sqrMagnitude / (2f * adjustedPosition.x) * targetVelocity - shooterPosition).normalized;
        }

        float discriminant = speedRatioSq * adjustedPosition.sqrMagnitude - adjustedPosition.y.Sq();  //D = b^2-4ac simplified and divided by 4

        if (speedRatioSq > 1f || (discriminant > 0f && adjustedPosition.x > 0f))
        {
            float xs = (adjustedPosition.x - Mathf.Sqrt(discriminant)) / (1 - speedRatioSq);
            return (targetPosition + xs * targetVelocity - shooterPosition).normalized;
        }
        else
        {   //in this case it can't hit the target (or barely can), so it shoots with the best aim
            float xs = Mathf.Abs(adjustedPosition.x) / (1 - speedRatioSq);
            return (targetPosition + xs * targetVelocity - shooterPosition).normalized;
        }
    }
}