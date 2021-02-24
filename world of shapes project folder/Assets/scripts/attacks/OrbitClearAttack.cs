using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitClearAttack : Attack, Projectile
{
    private SpriteRenderer _rend;
    private Collider2D _coll;
    private MyAudioSource _audio;

    private float _side = 1;
    private float _currentRadius;
    private float _inputRadius;
    private Vector3 _rotateSpeed;
    private Vector3 _direction;
    private float _directionMagnitude;

    private const float _Time_TO_SET = 0.1f;

    //stats
    [SerializeField]
    private float _damage = 3.5f;
    public float Size = 2f;
    public float W = 2f * MyMathlib.TAU;
    public float MinReach = 1.5f;

    public override float Damage => _damage;

    protected new void Awake()
    {
        base.Awake();
        _audio = GetComponent<MyAudioSource>();
        (_coll = GetComponent<Collider2D>()).enabled = false;
        (_rend = GetComponent<SpriteRenderer>()).enabled = false;

        OrbitSpawnAttackState s1 = new OrbitSpawnAttackState(_ASM, this);
        OrbitClearingAttackState s2 = new OrbitClearingAttackState(_ASM, this);
        s1.SetTargetStates(s2);
        _ASM.InitializeWithStates(new InactiveEnabledDisableAttackState(_ASM, this), s1);
    }

    protected new void Start()
    {
        base.Start();
    }

    public override bool InputFunction(KeyCode key)
    {
        return Input.GetKeyDown(key);
    }

    void FixedUpdate()
    {
        _ASM.LogicalFixedUpdate();
    }

    private void LateUpdate()
    {
        _ASM.LogicalLateUpdate();
    }



    public void Blocked()
    {
        _ASM.ChangeToInactive();
    }

    public Attack GetAttack() => this;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        var collparameters = collision.GetComponent<CollisionInfo>();

        this.CheckToBlockAttack(collparameters);

        if (ApplyDamage(collparameters, _damage))
        {
            MoveComponent collMoveComponent = collision.SearchComponent<MoveComponent>();
            collMoveComponent.Push(10f, _side * MyMathlib.Rotate90(_direction));
            _audio.AddSoundToQueue(0, transform.position);
        }
    }

    private float GeRadius()
    {
        float radius = _directionMagnitude;
        if (radius > MinReach)
        {
            if (radius < Reach)
            {
                return radius;
            }
            else
            {
                return Reach;
            }
        }
        else
        {
            return MinReach;
        }
    }

    protected override void InitiateAttack()
    {
        _direction = Holder.TargetPosition - MoveComponent.Position;
        _directionMagnitude = _direction.magnitude;
        _direction /= _directionMagnitude;

        _inputRadius = GeRadius();

        transform.localScale = new Vector3(Size, Size, 1f);
        _coll.enabled = true;
        _rend.enabled = true;
        _rotateSpeed = MyMathlib.PolarVectorRad(W * _side * Time.fixedDeltaTime);
        ResetCoolDown();


        _side = -_side;
    }

    protected override void DisableAttack()
    {
        _coll.enabled = false;
        _rend.enabled = false;
    }

    public override void SetUpAI()
    {
        throw new System.NotImplementedException();
    }

    protected abstract class OrbitClearAttackState : AttackState
    {
        protected OrbitClearAttack _orbitClearAttack;
        protected OrbitClearAttackState(AttackStateMachine ASM, OrbitClearAttack orbitClearAttack) : base(ASM)
        {
            _orbitClearAttack = orbitClearAttack;
        }

        public override void LogicalUpdate()
        {
            throw new System.NotImplementedException();
        }

    }


    protected class OrbitSpawnAttackState : OrbitClearAttackState, InputAttackState
    {
        protected OrbitClearingAttackState _orbitClearingAttackState;
        public OrbitSpawnAttackState(AttackStateMachine ASM, OrbitClearAttack orbitClearAttack) : base(ASM, orbitClearAttack)
        {
        }

        public void SetTargetStates(OrbitClearingAttackState orbitClearingAttackState)
        {
            _orbitClearingAttackState = orbitClearingAttackState;
        }

        public override void OnStateEnter()
        {
            _orbitClearAttack.InitiateAttack();
            _orbitClearAttack._currentRadius = 0f;
        }

        public override void LogicalFixedUpdate()
        {
            if (_orbitClearAttack._currentRadius < _orbitClearAttack._inputRadius)
            {
                _orbitClearAttack._currentRadius += _orbitClearAttack._inputRadius / _Time_TO_SET * Time.fixedDeltaTime;
            }
            else
            {
                _ASM.ChangeState(_orbitClearingAttackState);
            }
        }

        public override void LogicalLateUpdate()
        {
            _orbitClearAttack.transform.position = _orbitClearAttack.MoveComponent.Position + _orbitClearAttack._currentRadius * _orbitClearAttack._direction;
        }

        public override void OnStateExit()
        {

        }

        public bool ActivateAttack(bool input)
        {
            return input && _orbitClearAttack.CooldownFinished();
        }

    }

    protected class OrbitClearingAttackState : OrbitClearAttackState
    {
        private Coroutine _secondsOnAir;

        public OrbitClearingAttackState(AttackStateMachine ASM, OrbitClearAttack orbitClearAttack) : base(ASM, orbitClearAttack)
        {
        }

        public override void OnStateEnter()
        {
            _secondsOnAir = _orbitClearAttack.DoActionInTime(_ASM.ChangeToInactive, (float)MyMathlib.TAU / _orbitClearAttack.W);
        }

        public override void LogicalFixedUpdate()
        {
            _orbitClearAttack._direction = MyMathlib.MultiplyComplex(_orbitClearAttack._direction, _orbitClearAttack._rotateSpeed);
        }

        public override void LogicalLateUpdate()
        {
            _orbitClearAttack.transform.position = _orbitClearAttack.MoveComponent.Position + _orbitClearAttack._inputRadius * _orbitClearAttack._direction;
        }

        public override void OnStateExit()
        {
            _orbitClearAttack.StopCoroutine(_secondsOnAir);
        }
    }

}
