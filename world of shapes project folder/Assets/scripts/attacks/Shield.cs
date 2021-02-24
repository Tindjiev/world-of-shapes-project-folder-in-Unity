using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Attack
{
    private SpriteRenderer _rend;
    private Collider2D _coll;
    private LifeComponent _lifeComponent;

    private Timer _closeTimer;

    //stats
    public float size = 1f;
    public float maxhealth = 20f;

    public override float Damage => 0f;

    private ShieldBreakState _breakState;
    protected new void Awake()
    {
        base.Awake();

        (_rend = GetComponent<SpriteRenderer>()).enabled = false;
        (_coll = GetComponent<Collider2D>()).enabled = false;
        if (TryGetComponent(out _lifeComponent))
        {
            _lifeComponent.Health = maxhealth;
            _lifeComponent.AddActionOnDeath(Break);
            _lifeComponent.AddActionDuringDeath(() => _ASM.ChangeState(_breakState));
        }
        Reach = 2f;
        _closeTimer = new Timer(CooldownTimer);

        _breakState = new ShieldBreakState(_ASM, this);
        _ASM.InitializeWithStates(new InactiveEnabledAttackState(_ASM, this), new ShieldActiveState(_ASM, this));

    }

    protected new void Start()
    {
        base.Start();

        var evadeMode = this.SearchComponent<EvadeEnemy_FleeMode>();
        if (evadeMode != null) evadeMode.CheckActivateFunction = IsOnCooldown;
    }

    public override bool InputFunction(KeyCode key) => Input.GetKey(key);

    private bool _gotInput;

    void LateUpdate()
    {
        _ASM.LogicalLateUpdate();
    }

    protected override void InitiateAttack()
    {
        _rend.enabled = true;
        _coll.enabled = true;
        if (_closeTimer.CheckIfTimePassed) _lifeComponent.Health = maxhealth;
    }

    protected override void DisableAttack()
    {
        _rend.enabled = false;
        _coll.enabled = false;
    }


    private void Break()
    {
        _ASM.ChangeToInactive();
        DisableAttack();
        _lifeComponent.Health = maxhealth;
    }

    public override void SetUpAI()
    {
        var attackFromDistanceMode = GetCheckNullAndSetMode<AttackFromDistance_AttackMode>();
        attackFromDistanceMode.ratioOutsq = 6.5f.Sq();
        attackFromDistanceMode.ratioStartAttacksq = 6f.Sq();
        attackFromDistanceMode.ratioTargetGettingFarSq = 2f.Sq();
        attackFromDistanceMode.ratioStopMovingsq = 1.5f.Sq();
        attackFromDistanceMode.ratioTooClosesq = 1.2f.Sq();
        GetCheckNullAndSetMode<EvadeEnemy_FleeMode>();
    }

    protected abstract class ShieldState : AttackState
    {

        protected Shield _shield;

        protected ShieldState(AttackStateMachine ASM, Shield shield) : base(ASM)
        {
            _shield = shield;
        }

        public override void LogicalUpdate()
        {
            throw new System.NotImplementedException();
        }

        public override void LogicalFixedUpdate()
        {
            throw new System.NotImplementedException();
        }

    }


    protected class ShieldActiveState : ShieldState, InputAttackState
    {

        public ShieldActiveState(AttackStateMachine ASM, Shield shield) : base(ASM, shield)
        {

        }

        public override void OnStateEnter()
        {
            _shield.InitiateAttack();
        }

        public override void LogicalLateUpdate()
        {
            _shield.transform.position = _shield.MoveComponent.Position + _shield.Reach * _shield.Holder.DirectionVector;
            _shield.transform.rotation = Quaternion.Euler(0f, 0f, _shield.Holder.DirectionVector.AnlgeDegrees());

            if (!_shield._gotInput)
            {
                _shield.DisableAttack();
                _shield._closeTimer.StartTimer();
                _ASM.ChangeToInactive();
            }
            else
            {
                _shield._gotInput = false;
            }
        }

        public override void OnStateExit()
        {

        }

        public bool ActivateAttack(bool input) => _shield._gotInput = input && _shield.CooldownFinished();
    }

    protected class ShieldBreakState : ShieldState
    {

        public ShieldBreakState(AttackStateMachine ASM, Shield shield) : base(ASM, shield)
        {

        }

        public override void OnStateEnter()
        {
            _shield._coll.enabled = false;
            _shield.ResetCoolDown();
        }

        public override void LogicalLateUpdate()
        {
            _shield.transform.position = _shield.MoveComponent.Position + _shield.Reach * _shield.Holder.DirectionVector;
            _shield.transform.rotation = Quaternion.Euler(0f, 0f, _shield.Holder.DirectionVector.AnlgeDegrees());
        }

        public override void OnStateExit()
        {
        }
    }

}
