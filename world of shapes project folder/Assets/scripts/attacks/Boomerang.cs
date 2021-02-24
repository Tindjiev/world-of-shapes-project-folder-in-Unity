using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomerang : Attack, Projectile
{
    private SpriteRenderer _rend;
    private Collider2D _coll;

    private bool _outOfReach;
    private float _maxTime;
    private Vector3 _tempDirection;
    private Vector3 _v;
    private float _directionMagnitude;
    private const float _SPIN_RATIO = 4f;

    //stats
    [SerializeField]
    public float _damage = 7f;
    public float Size = 2f;
    public float Drift = 1f;

    public override float Damage => _damage;

    private float _wDrift => Drift * MyMathlib.TAU;

    protected new void Awake()
    {
        base.Awake();
        (_coll = GetComponent<Collider2D>()).enabled = false;
        (_rend = GetComponent<SpriteRenderer>()).enabled = false;

        _ASM.InitializeWithStates(new InactiveEnabledAttackState(_ASM, this), new BoomerangThrown(_ASM, this));
    }

    protected new void Start()
    {
        base.Start();
    }

    public override bool InputFunction(KeyCode key) => Input.GetKeyDown(key);

    void FixedUpdate()
    {
        _ASM.LogicalFixedUpdate();
    }

    public void Blocked() => _ASM.ChangeToInactive();

    public Attack GetAttack() => this;

    void OnTriggerEnter2D(Collider2D collision)
    {
        CollisionInfo collparameters = collision.GetComponent<CollisionInfo>();
        ApplyDamage(collparameters, _damage);
        this.CheckToBlockAttack(collparameters);
    }

    private float GetDistance()
    {
        float f = _directionMagnitude;
        if (f > 1.5f)
        {
            if (f < Reach)
            {
                _outOfReach = false;
                return f;
            }
            else
            {
                _outOfReach = true;
                return Reach;
            }
        }
        else
        {
            return 1.5f;
        }
    }

    protected override void DisableAttack()
    {
        _coll.enabled = _rend.enabled = false;
    }


    protected override void InitiateAttack()
    {
        _tempDirection = Holder.TargetPosition - MoveComponent.Position;
        _directionMagnitude = _tempDirection.magnitude;
        _tempDirection /= _directionMagnitude;
        Drift = -Drift;

        _v = GetDistance() / 2f * _wDrift * MyMathlib.Rotate90(-_tempDirection);
        transform.position = MoveComponent.Position;
        _coll.enabled = _rend.enabled = true;
        _maxTime = 3f;
        ResetCoolDown();
    }

    public void UpdatePhysics()
    {
        UpdateVelocity();
        transform.position += _v * Time.fixedDeltaTime;
        transform.Rotate(0f, 0f, Drift * 360f * Time.fixedDeltaTime * _SPIN_RATIO);
    }

    private void UpdateVelocity()
    {
        //v = v * (float)System.Math.Sqrt(1 - spin * spin * Time.fixedDeltaTime * Time.fixedDeltaTime) + spin * mathlib.rotate90(v) * Time.fixedDeltaTime;
        //v += wspin * mathlib.rotate90(v) * Time.fixedDeltaTime;
        //v = (v + wspin * Time.fixedDeltaTime * mathlib.rotate90(v)) / (1 + wspin * wspin * Time.fixedDeltaTime * Time.fixedDeltaTime);

        if (_outOfReach)
        {
            _v += _wDrift * Time.fixedDeltaTime * MyMathlib.Rotate90(_v);
        }
        else
        {
            float w2T2div4 = _wDrift * _wDrift * Time.fixedDeltaTime * Time.fixedDeltaTime / 4f;
            _v = ((1f - w2T2div4) * _v + _wDrift * Time.fixedDeltaTime * MyMathlib.Rotate90(_v)) / (1f + w2T2div4);
        }
    }

    public override void SetUpAI()
    {
        throw new System.NotImplementedException();
    }

    protected abstract class BoomerangAttackState : AttackState
    {
        protected Boomerang _boomerang;

        protected BoomerangAttackState(AttackStateMachine ASM,Boomerang boomerang) : base(ASM)
        {
            _boomerang = boomerang;
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

    protected class BoomerangThrown : BoomerangAttackState, InputAttackState
    {
        private Coroutine _secondsOnAir;

        public BoomerangThrown(AttackStateMachine ASM, Boomerang boomerang) : base(ASM, boomerang)
        {
        }

        public override void OnStateEnter()
        {
            _boomerang.InitiateAttack();
            _secondsOnAir = _boomerang.DoActionInTime(_ASM.ChangeToInactive, _boomerang._maxTime);
        }

        public override void LogicalFixedUpdate()
        {
            _boomerang.UpdatePhysics();
        }

        public override void OnStateExit()
        {
            _boomerang.DisableAttack();
            _boomerang.StopCoroutine(_secondsOnAir);
        }

        public bool ActivateAttack(bool input)
        {
            return input && _boomerang.CooldownFinished();
        }
    }

}
