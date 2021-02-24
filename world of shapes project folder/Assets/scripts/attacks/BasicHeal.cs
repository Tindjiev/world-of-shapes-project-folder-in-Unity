using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicHeal : Heal, Projectile
{
    private SpriteRenderer _rend;
    private Collider2D _coll;

    private MoveComponent _targetMove;
    private Transform _targetTransform;

    protected Vector3 _targetPos => _targetMove != null ? _targetMove.Position : _targetTransform.position;

    private float v;
    private bool _dampingPhase;
    private float _b => 2.5f / TimeReach;

    //stats
    public float Heal = 2f;
    public float TimeReach = 0.5f;
    public float Vterminal = 20f;
    private float _distance
    {
        get => Reach;
        set => Reach = value;
    }

    public override float Damage => -Heal;

    protected new void Awake()
    {
        base.Awake();
        _rend = GetComponent<SpriteRenderer>();
        _coll = GetComponent<Collider2D>();
        DisableAttack();
        _distance = 7f;
        CooldownTimer = 2f;

        _ASM.InitializeWithStates(new InactiveEnabledDisableAttackState(_ASM, this), new HealingState(_ASM, this));
    }

    public override bool InputFunction(KeyCode key)
    {
        return Input.GetKeyDown(key);
    }

    protected new void Start()
    {
        base.Start();
    }

    protected void FixedUpdate()
    {
        _ASM.LogicalFixedUpdate();
    }

    private void MoveTowardsTarget()
    {
        if (_dampingPhase)
        {
            transform.position += v * (_targetPos - transform.position).normalized * Time.fixedDeltaTime;
            v -= _b * v * Time.fixedDeltaTime;
            if (v < Vterminal) _dampingPhase = false;
        }
        else
        {
            transform.position += Vterminal * (_targetPos - transform.position).normalized * Time.fixedDeltaTime;
        }
    }

    protected override void DisableAttack()
    {
        _rend.enabled = false;
        _coll.enabled = false;
    }

    protected override void InitiateAttack()
    {
        _targetMove = _targetTransform.SearchComponent<MoveComponent>();
        if (_targetMove != null)
        {
            _targetTransform = _targetMove.transform;
        }
        transform.position = MoveComponent.Position;
        v = _distance * _b;
        _dampingPhase = true;
        _rend.enabled = true;
        _coll.enabled = true;
        ResetCoolDown();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform == _targetTransform)
        {
            _ASM.ChangeToInactive();
            _targetTransform.SearchComponent<LifeComponent>().Heal(this, Heal);
        }
    }

    public void Blocked()
    {
        _ASM.ChangeToInactive();
    }

    public Attack GetAttack()
    {
        return this;
    }

    public override void SetUpAI()
    {
        DestroyAIMode<FleeModeClass>();
        GetCheckNullAndSetMode<Healer_AttackMode>();
    }

    protected abstract class HealWepAttackState : AttackState
    {
        protected BasicHeal _healWep;
        protected HealWepAttackState(AttackStateMachine ASM, BasicHeal healWep) : base(ASM)
        {
            _healWep = healWep;
        }

        public override void LogicalUpdate()
        {
            throw new System.NotImplementedException();
        }

        public override void LogicalLateUpdate()
        {
            throw new System.NotImplementedException();
        }
    }

    protected class HealingState : HealWepAttackState, InputAttackState
    {
        private Coroutine _maxSecondsOnAir;
        public HealingState(AttackStateMachine ASM, BasicHeal healWep) : base(ASM, healWep)
        {
        }

        public override void OnStateEnter()
        {
            _healWep.InitiateAttack();
            _maxSecondsOnAir = _healWep.DoActionInTime(_ASM.ChangeToInactive, _healWep.TimeReach);
        }

        public override void LogicalFixedUpdate()
        {
            if (_healWep._targetTransform == null || !_healWep._targetTransform.gameObject.activeInHierarchy)
            {
                _ASM.ChangeToInactive();
            }
            else
            {
                _healWep.MoveTowardsTarget();
            }
        }

        public override void OnStateExit()
        {
            _healWep.StopCoroutine(_maxSecondsOnAir);
        }

        public bool ActivateAttack(bool input)
        {
            return input && _healWep.CooldownFinished() && (_healWep._targetTransform = _healWep.Holder.TargetTransform) != null;
        }
    }
}
