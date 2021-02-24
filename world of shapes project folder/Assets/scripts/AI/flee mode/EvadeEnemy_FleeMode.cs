using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvadeEnemy_FleeMode : FleeModeClass
{
    [field: SerializeField, ReadOnlyOnInspector]
    public BaseCharacterControl TargetToFleeFrom { get; protected set; }
    [field: SerializeField]
    public float SafeDistanceFromFleeTarget { get; protected set; } = 30f;

    protected new void Awake()
    {
        base.Awake();

        _SM = new StateMachine();
        WayTooCloseState s1 = new WayTooCloseState(_SM, this);
        FleeingState s2 = new FleeingState(_SM, this);
        InSafeDistanceState s3 = new InSafeDistanceState(_SM, this);
        s1.SetTargetStates(s2);
        s2.SetTargetStates(s1, s3);
        s3.SetTargetStates(s2);
        _SM.SetStartingState(s1);
    }

    public override bool CheckToActivate()
    {
        var temp = GetClosestOfSeenTargets();
        if (temp != null)
        {
            TargetToFleeFrom = temp;
            return base.CheckToActivate();
        }
        return false;
    }

    public override bool CheckToChange()
    {
        return base.CheckToChange() || TargetToFleeFrom.IsDead();
    }

    public override void LogicalUpdate()
    {
        base.LogicalUpdate();
    }

    public override void SetDirectionOfCharacter() => SetDirectionToTarget(TargetToFleeFrom);



    protected abstract class AIEvadeModeMoverAroundState : AIModeState
    {
        protected readonly EvadeEnemy_FleeMode _evadeMode;

        protected AIEvadeModeMoverAroundState(StateMachine AISM, EvadeEnemy_FleeMode evadeMode) : base(AISM)
        {
            _evadeMode = evadeMode;
        }
    }

    protected class WayTooCloseState : AIEvadeModeMoverAroundState
    {
        protected FleeingState _fleeingState;

        public WayTooCloseState(StateMachine AISM, EvadeEnemy_FleeMode evadeMode) : base(AISM, evadeMode)
        {
        }


        public void SetTargetStates(FleeingState fleeingState)
        {
            _fleeingState = fleeingState;
        }

        public override void OnStateEnter()
        {
            _evadeMode.AICharacter.MoveComponent.ClearPath();
        }

        public sealed override void LogicalUpdate()
        {
            MoveComponent move = _evadeMode.AICharacter.MoveComponent;
            Vector3 pos_sub_targetNormed = (move.Position - _evadeMode.TargetToFleeFrom.Position).normalized;
            _evadeMode._currentattack.Activate(true);
            if (!move.HasPath)
            {
                _evadeMode.Safeplace = _evadeMode.AICharacter.Position + pos_sub_targetNormed;
                move.StartFromScratchNewEndpos(_evadeMode.Safeplace);
                move.SetRatioToBaseSpeed(2f);
            }
            if ((_evadeMode.TargetToFleeFrom.Position - move.Position).sqrMagnitude > _evadeMode.SafeDistanceFromFleeTarget.Sq() * (0.25f * 0.25f))
            {
                _SM.ChangeState(_fleeingState);
            }
        }

        public override void OnStateExit()
        {
        }
    }

    protected class FleeingState : AIEvadeModeMoverAroundState
    {

        protected WayTooCloseState _wayTooCloseState;
        protected InSafeDistanceState _inSafeDistanceState;

        public FleeingState(StateMachine AISM, EvadeEnemy_FleeMode evadeMode) : base(AISM, evadeMode)
        {
        }

        public void SetTargetStates(WayTooCloseState wayTooCloseState, InSafeDistanceState inSafeDistanceState)
        {
            _wayTooCloseState = wayTooCloseState;
            _inSafeDistanceState = inSafeDistanceState;
        }


        public override void OnStateEnter()
        {
            _evadeMode.AICharacter.MoveComponent.ClearPath();
        }

        public sealed override void LogicalUpdate()
        {
            MoveComponent move = _evadeMode.AICharacter.MoveComponent;
            Vector3 pos_sub_targetNormed = (move.Position - _evadeMode.TargetToFleeFrom.Position).normalized;

            if (!move.HasPath)
            {
                _evadeMode.Safeplace = _evadeMode.TargetToFleeFrom.Position + _evadeMode.SafeDistanceFromFleeTarget * pos_sub_targetNormed;
                move.StartFromScratchNewEndpos(_evadeMode.Safeplace);
                move.SetBaseSpeed();
            }
            if ((_evadeMode.TargetToFleeFrom.Position - move.Position).sqrMagnitude < _evadeMode.SafeDistanceFromFleeTarget.Sq() * (0.2f * 0.2f))
            {
                _SM.ChangeState(_wayTooCloseState);
            }
            else if ((_evadeMode.TargetToFleeFrom.Position - move.Position).sqrMagnitude > _evadeMode.SafeDistanceFromFleeTarget.Sq())
            {
                _SM.ChangeState(_inSafeDistanceState);
            }
        }

        public override void OnStateExit()
        {
        }
    }

    protected class InSafeDistanceState : AIEvadeModeMoverAroundState
    {

        private FleeingState _fleeingState;

        public InSafeDistanceState(StateMachine AISM, EvadeEnemy_FleeMode evadeMode) : base(AISM, evadeMode)
        {
        }

        public void SetTargetStates(FleeingState fleeingState)
        {
            _fleeingState = fleeingState;
        }


        public override void OnStateEnter()
        {
            _evadeMode.AICharacter.MoveComponent.ClearPath();
        }

        public override void LogicalUpdate()
        {
            MoveComponent move = _evadeMode.AICharacter.MoveComponent;
            Vector3 pos_sub_targetNormed = (move.Position - _evadeMode.TargetToFleeFrom.Position).normalized;

            if (!move.HasPath)
            {
                _evadeMode.Safeplace = _evadeMode.TargetToFleeFrom.Position - _evadeMode.SafeDistanceFromFleeTarget * 0.5f * pos_sub_targetNormed;
                move.StartFromScratchNewEndpos(MoveAround_ChillMode.FollowAround(_evadeMode.Safeplace, move.Position, _evadeMode.SafeDistanceFromFleeTarget / 10f));
            }
            if ((_evadeMode.TargetToFleeFrom.Position - move.Position).sqrMagnitude < _evadeMode.SafeDistanceFromFleeTarget.Sq() * (0.9f * 0.9f))
            {
                _SM.ChangeState(_fleeingState);
            }
        }

        public override void OnStateExit()
        {
        }
    }

}
