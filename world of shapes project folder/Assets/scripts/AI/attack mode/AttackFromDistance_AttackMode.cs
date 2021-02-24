using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackFromDistance_AttackMode : AttackModeSingleTarget
{

    private Vector3 _semiLastTargetPos;

    //ratios
    public float ratioOutsq = 1.25f.Sq();               //stops attack but still gets closer to target
    public float ratioStartAttacksq = 1f.Sq();          //starts attacking after being far and gets closer to target
    public float ratioTargetGettingFarSq = 0.9f.Sq();   //starts moving to get closer before target gets farther
    public float ratioStopMovingsq = 0.8f.Sq();         //is close enough to target to just stand still and keep attacking
    public float ratioTooClosesq = 0.3f.Sq();           //gets farther from target while attacking


    protected new void Awake()
    {
        base.Awake();
        Currentattack = this.SearchComponent<Attack>();

        _SM = new StateMachine();
        OutOfRangeState s1 = new OutOfRangeState(_SM, this);
        WithinRangeButMoveState s2 = new WithinRangeButMoveState(_SM, this);
        WithinRangeState s3 = new WithinRangeState(_SM, this);
        TooCloseState s4 = new TooCloseState(_SM, this);
        s1.SetTargetStates(s2);
        s2.SetTargetStates(s1,s3);
        s3.SetTargetStates(s4,s2);
        s4.SetTargetStates(s2);
        _SM.SetStartingState(s1);
    }


    protected abstract class AIAttackModeFromDistanceState : AIModeState
    {
        protected readonly AttackFromDistance_AttackMode _attackMode;

        protected AIAttackModeFromDistanceState(StateMachine SM, AttackFromDistance_AttackMode attackMode) : base(SM)
        {
            _attackMode = attackMode;
        }
    }

    protected class OutOfRangeState : AIAttackModeFromDistanceState
    {

        protected WithinRangeButMoveState _withinARMoveState;

        public OutOfRangeState(StateMachine SM, AttackFromDistance_AttackMode attackMode) : base(SM, attackMode)
        {
        }

        public void SetTargetStates(WithinRangeButMoveState withinARMoveState)
        {
            _withinARMoveState = withinARMoveState;
        }


        public override void OnStateEnter()
        {
            _attackMode.AICharacter.MoveComponent.StartFromScratchNewEndpos(_attackMode.Target.Position);
        }

        public override void LogicalUpdate()
        {
            MoveComponent move = _attackMode.AICharacter.MoveComponent;
            Vector3 targetPos = _attackMode.Target.Position;
            move.SetRatioToBaseSpeed(1.6f);
            if (!move.HasPath || (targetPos - _attackMode._semiLastTargetPos).sqrMagnitude > move.CurrentSpeed.Sq())
            {
                _attackMode._semiLastTargetPos = targetPos;
                move.StartFromScratchNewEndpos(targetPos);
            }
            if ((targetPos - move.Position).sqrMagnitude < _attackMode.Currentattack.Reach.Sq() * _attackMode.ratioStartAttacksq)
            {
                _SM.ChangeState(_withinARMoveState);
            }
        }

        public override void OnStateExit()
        {

        }
    }

    protected class WithinRangeButMoveState : AIAttackModeFromDistanceState
    {
        protected OutOfRangeState _outOfARState;
        protected WithinRangeState _withinARNoMoveState;

        public WithinRangeButMoveState(StateMachine SM, AttackFromDistance_AttackMode attackMode) : base(SM, attackMode)
        {
        }


        public void SetTargetStates(OutOfRangeState outOfARState, WithinRangeState withinARNoMoveState)
        {
            _outOfARState = outOfARState;
            _withinARNoMoveState = withinARNoMoveState;
        }

        public override void OnStateEnter()
        {
            _attackMode.AICharacter.MoveComponent.StartFromScratchNewEndpos(_attackMode.Target.Position);
        }

        public override void LogicalUpdate()
        {
            MoveComponent move = _attackMode.AICharacter.MoveComponent;
            Vector3 targetPos = _attackMode.Target.Position;
            _attackMode.Currentattack.Activate(true);
            float distSq = (targetPos - move.Position).sqrMagnitude;
            float ReachSq = _attackMode.Currentattack.Reach.Sq();
            if (distSq < ReachSq * _attackMode.ratioStartAttacksq / 4f)
            {
                move.SetBaseSpeed();
            }
            else
            {
                move.SetRatioToBaseSpeed(1.6f);
            }
            if (distSq > ReachSq * _attackMode.ratioOutsq)
            {
                _SM.ChangeState(_outOfARState);
            }
            else if (distSq > ReachSq * _attackMode.ratioStopMovingsq)
            {
                if (!move.HasPath || (targetPos - _attackMode._semiLastTargetPos).sqrMagnitude > move.CurrentSpeed.Sq())
                {
                    _attackMode._semiLastTargetPos = targetPos;
                    move.StartFromScratchNewEndpos(targetPos);
                }
            }
            else
            {
                _SM.ChangeState(_withinARNoMoveState);
            }
        }

        public override void OnStateExit()
        {
        }
    }

    protected class WithinRangeState : AIAttackModeFromDistanceState
    {

        protected TooCloseState _tooCloseState;
        protected WithinRangeButMoveState _withinARMoveState;

        public WithinRangeState(StateMachine SM, AttackFromDistance_AttackMode attackMode) : base(SM, attackMode)
        {
        }

        public void SetTargetStates(TooCloseState tooCloseState, WithinRangeButMoveState withinARMoveState)
        {
            _tooCloseState = tooCloseState;
            _withinARMoveState = withinARMoveState;
        }


        public override void OnStateEnter()
        {
            _attackMode.AICharacter.MoveComponent.ClearPath();
        }

        public sealed override void LogicalUpdate()
        {
            MoveComponent move = _attackMode.AICharacter.MoveComponent;
            Vector3 targetPos = _attackMode.Target.Position;
            _attackMode.Currentattack.Activate(true);
            move.ClearPath();
            float distsqr = (targetPos - move.Position).sqrMagnitude;
            if (distsqr > _attackMode.Currentattack.Reach.Sq() * _attackMode.ratioTargetGettingFarSq)
            {
                _SM.ChangeState(_withinARMoveState);
            }
            else if (distsqr < _attackMode.Currentattack.Reach.Sq() * _attackMode.ratioTooClosesq)
            {
                _SM.ChangeState(_tooCloseState);
            }
        }

        public override void OnStateExit()
        {
        }
    }

    protected class TooCloseState : AIAttackModeFromDistanceState
    {

        protected WithinRangeButMoveState _withinARMoveState;

        public TooCloseState(StateMachine SM, AttackFromDistance_AttackMode attackMode) : base(SM, attackMode)
        {
        }

        public void SetTargetStates(WithinRangeButMoveState withinARMoveState)
        {
            _withinARMoveState = withinARMoveState;
        }


        public override void OnStateEnter()
        {
            _attackMode.AICharacter.MoveComponent.ClearPath();
        }

        public override void LogicalUpdate()
        {
            MoveComponent move = _attackMode.AICharacter.MoveComponent;
            Vector3 targetPos = _attackMode.Target.Position;
            _attackMode.Currentattack.Activate(true);
            move.SetRatioToBaseSpeed(1.6f);
            if (!move.HasPath)
            {
                move.StartFromScratchNewEndpos(2f * (move.Position - targetPos) + targetPos);
            }
            if ((targetPos - move.Position).sqrMagnitude > _attackMode.Currentattack.Reach.Sq() * _attackMode.ratioTooClosesq)
            {
                _SM.ChangeState(_withinARMoveState);
            }
        }

        public override void OnStateExit()
        {
        }

    }

}
