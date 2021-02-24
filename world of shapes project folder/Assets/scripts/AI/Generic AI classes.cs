using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class AIModeClass : MonoBehaviour, IState
{
    public AI AICharacter { get; protected set; }

    protected StateMachine _SM;

    protected void Awake()
    {
        AICharacter = this.SearchComponent<AI>();
    }

    public abstract bool CheckToActivate();
    public abstract bool CheckToChange();
    public virtual void SetDirectionOfCharacter()
    {
        AICharacter.DirectionVector = AICharacter.MoveComponent.Velocity.normalized;
    }

    public BaseCharacterControl GetClosestOfSeenTargets() => BaseCharacterControl.GetClosestOfSeenTargets(AICharacter);
    public BaseCharacterControl GetClosestOfSeenAllies() => BaseCharacterControl.GetClosestOfSeenAllies(AICharacter);

    public void SetDirectionToTarget(BaseCharacterControl target)
    {
        AICharacter.DirectionVector = ((AICharacter.TargetPosition = target.Position) - AICharacter.Position).normalized;
    }
    public void SetDirectionToTarget(MoveComponent target)
    {
        AICharacter.DirectionVector = ((AICharacter.TargetPosition = target.Position) - AICharacter.Position).normalized;
    }
    public void SetDirectionToTarget(Transform target)
    {
        AICharacter.DirectionVector = ((AICharacter.TargetPosition = target.position) - AICharacter.Position).normalized;
    }
    public void SetDirectionToTarget(in Vector3 targetPosition)
    {
        AICharacter.DirectionVector = ((AICharacter.TargetPosition = targetPosition) - AICharacter.Position).normalized;
    }

    public virtual void OnStateEnter() => _SM.Initialize();
    public virtual void LogicalUpdate() => _SM.LogicalUpdate();
    public virtual void OnStateExit() => _SM.ClearState();

    public void LogicalFixedUpdate()
    {
        throw new System.NotImplementedException();
    }

    public void LogicalLateUpdate()
    {
        throw new System.NotImplementedException();
    }

    protected abstract class AIModeState : IState
    {
        protected StateMachine _SM;

        protected AIModeState(StateMachine SM)
        {
            _SM = SM;
        }
        public abstract void OnStateEnter();
        public abstract void LogicalUpdate();
        public void LogicalFixedUpdate()
        {
            throw new System.NotImplementedException();
        }
        public void LogicalLateUpdate()
        {
            throw new System.NotImplementedException();
        }
        public abstract void OnStateExit();
    }

}

public abstract class AttackModeClass : AIModeClass
{
    [field: SerializeField]
    public Attack Currentattack { get; protected set; }

    protected new void Awake()
    {
        base.Awake();
    }

    public override bool CheckToChange() => Currentattack == null;
}


public abstract class AttackModeSingleTarget : AttackModeClass
{
    [field: SerializeField]
    public BaseCharacterControl Target { get; protected set; }
    public float DistanceOfTargetToAbandon = 50f;
    public bool AbandonTargetIfFar = false;

    public override bool CheckToActivate()
    {
        if (Target != null) return true;
        if ((Target = GetClosestOfSeenTargets()) != null) return true;
        return false;
    }

    public void SetTarget(BaseCharacterControl newTarget)
    {
        if (AICharacter.CanTarget(newTarget)) Target = newTarget;
    }

    protected new void Awake()
    {
        base.Awake();
    }

    public override void SetDirectionOfCharacter()
    {
        SetDirectionToTarget(Target);
    }

    public override bool CheckToChange() => base.CheckToChange() || Target.IsDead() ||
        (AbandonTargetIfFar && (Target.Position - AICharacter.Position).sqrMagnitude > DistanceOfTargetToAbandon.Sq());

    public override void OnStateExit()
    {
        base.OnStateExit();
        Target = null;
    }
}


public abstract class ChillModeClass : AIModeClass
{
    protected new void Awake()
    {
        base.Awake();
    }

    public override sealed bool CheckToChange() => false;
    public override bool CheckToActivate() => false;
}



public abstract class ObjectiveModeClass : AIModeClass
{
    protected new void Awake()
    {
        base.Awake();
    }
}


public abstract class FleeModeClass : AIModeClass
{
    public System.Func<bool> CheckActivateFunction { protected get; set; }

    [field: SerializeField, ReadOnlyOnInspector]
    public Vector3 Safeplace { protected get; set; }

    protected Attack _currentattack;

    protected new void Awake()
    {
        base.Awake();
        _currentattack = this.SearchComponent<Attack>();
    }

    public override bool CheckToActivate()
    {
        if (CheckActivateFunction == null) return false;
        return CheckActivateFunction();
    }

    public override bool CheckToChange() => !CheckToActivate();
}



