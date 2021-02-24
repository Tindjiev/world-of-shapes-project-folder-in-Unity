using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallChargeAttack : Attack, Projectile
{
    private SpriteRenderer _rend;
    private Collider2D _coll;
    private Rigidbody2D _rb;

    private Vector3 _direction;
    private float _tempDamage;

    public float CurrentSize => transform.localScale.x;
    public bool Thrown { get; private set; } = false;

    //stats
    [SerializeField]
    private float _damage = 3f;
    public float MaxSize = 3f;
    public float Shrinkrate = 1f;
    public float Speed = 20f;
    public bool CanPassThroughBodies = true;

    public override float Damage => _damage;

    protected new void Awake()
    {
        base.Awake();
        (_rend = GetComponent<SpriteRenderer>()).enabled = false;
        (_coll = GetComponent<Collider2D>()).enabled = false;
        _rb = GetComponent<Rigidbody2D>();

        ChargingState s1 = new ChargingState(_ASM, this);
        BallThrownState s2 = new BallThrownState(_ASM, this);
        s1.SetTargetStates(s2);
        _ASM.InitializeWithStates(new InactiveEnabledAttackState(_ASM, this), s1);
    }

    protected new void Start()
    {
        base.Start();
    }

    public override bool InputFunction(KeyCode key) => Input.GetKey(key);

    private void LateUpdate()
    {
        _ASM.LogicalLateUpdate();
    }

    private void FixedUpdate()
    {
        _ASM.LogicalFixedUpdate();
    }

    public void Blocked() => _ASM.ChangeToInactive();
    public Attack GetAttack() => this;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        CollisionInfo collparameters = collision.GetComponent<CollisionInfo>();
        this.CheckToBlockAttack(collparameters);
        if (ApplyDamage(collparameters, _tempDamage * transform.localScale.magnitude) && !CanPassThroughBodies)
        {
            Blocked();
        }
    }

    protected override void InitiateAttack()
    {
        transform.localScale = new Vector3(1f, 1f);
        _rend.enabled = true;
        _tempDamage = _damage;
    }

    protected override void DisableAttack()
    {
    }

    public override void SetUpAI()
    {
        DestroyAIMode<FleeModeClass>();
        GetCheckNullAndSetMode<BallCharge_AttackMode>();
    }

    protected abstract class BallChargeState : AttackState
    {
        protected BallChargeAttack _ballCharge;

        public BallChargeState(AttackStateMachine ASM, BallChargeAttack ballCharge) : base(ASM)
        {
            _ballCharge = ballCharge;
        }

        public override void LogicalUpdate()
        {
            throw new System.NotImplementedException();
        }

    }


    protected class ChargingState : BallChargeState, InputAttackState
    {
        private bool _gotInput = false;
        protected BallThrownState _ballThrownState;

        public ChargingState(AttackStateMachine ASM, BallChargeAttack ballCharge) : base(ASM, ballCharge)
        {

        }

        public void SetTargetStates(BallThrownState ballThrownState)
        {
            _ballThrownState = ballThrownState;
        }

        public override void OnStateEnter()
        {
            _ballCharge.InitiateAttack();
        }

        public override void LogicalFixedUpdate()
        {
            if (_ballCharge.CurrentSize < _ballCharge.MaxSize)
            {
                _ballCharge.transform.localScale += new Vector3(_ballCharge.Shrinkrate * Time.fixedDeltaTime, _ballCharge.Shrinkrate * Time.fixedDeltaTime);
            }
        }

        public override void LogicalLateUpdate()
        {
            _ballCharge._direction = _ballCharge.Holder.DirectionVector;
            _ballCharge.transform.position = _ballCharge.MoveComponent.Position + _ballCharge.MoveComponent.transform.localScale.magnitude * 0.2f * _ballCharge._direction;


            if (!_gotInput)
            {
                _ASM.ChangeState(_ballThrownState);
            }
            else
            {
                _gotInput = false;
            }
        }

        public override void OnStateExit()
        {
        }

        public bool ActivateAttack(bool input)
        {
            return _gotInput = input && _ballCharge.CooldownFinished();
        }
    }


    protected class BallThrownState : BallChargeState
    {

        private Coroutine _secondsOnAir;

        public BallThrownState(AttackStateMachine ASM, BallChargeAttack ballCharge) : base(ASM, ballCharge)
        {

        }

        public override void OnStateEnter()
        {
            _ballCharge._coll.enabled = true;
            _ballCharge.Thrown = true;
            _ballCharge._rb.velocity = _ballCharge.Speed * _ballCharge._direction;
            _ballCharge.ResetCoolDown();
            _secondsOnAir = _ballCharge.DoActionInTime(_ASM.ChangeToInactive, _ballCharge.Reach / _ballCharge.Speed);
        }

        public override void LogicalFixedUpdate()
        {
        }

        public override void LogicalLateUpdate()
        {

        }

        public override void OnStateExit()
        {
            _ballCharge.StopCoroutine(_secondsOnAir);
            _ballCharge._coll.enabled = false;
            _ballCharge._rend.enabled = false;
            _ballCharge.Thrown = false;
        }
    }

}
