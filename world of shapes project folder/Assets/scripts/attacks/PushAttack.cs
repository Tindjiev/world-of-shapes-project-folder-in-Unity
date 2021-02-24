using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushAttack : Attack
{
    private PolygonCollider2D _coll;
    private SpriteRenderer _rend;
    private MyAnimator _anim;
    private MyAudioSource _audio;

    private const float _TIME_OF_PUSHING = 0.1f;
    private readonly Vector2[] _points = new Vector2[6];
    private const float _ORIGINAL_HEIGHT = 10f;
    private const float _ORIGINAL_HALFWIDTH = 4f;

    private Vector3 _direction;

    //stats
    [SerializeField]
    private float _damage = 1f;
    public float halfwidth = _ORIGINAL_HALFWIDTH; //how wide it can push


    public override float Damage => _damage;

    private enum SoundIndexes
    {
        PushAttackSoundIndex,
        PushHitSoundIndex,
    }


    protected new void Awake()
    {
        base.Awake();
        _coll = GetComponent<PolygonCollider2D>();
        _rend = GetComponent<SpriteRenderer>();
        _anim = GetComponent<MyAnimator>();
        _audio = GetComponent<MyAudioSource>();

        _coll.enabled = false;
        _rend.enabled = false;
        enabled = false;

        PushingSmallState s1 = new PushingSmallState(_ASM,this);
        PushingBigState s2 = new PushingBigState(_ASM,this);

        s1.SetTargetStates(s2);

        _ASM.InitializeWithStates(new InactiveBaseAttackState(_ASM), s1);

    }
    protected new void Start()
    {
        base.Start();
    }

    public override bool InputFunction(KeyCode key) => Input.GetKeyDown(key);



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (ApplyDamage(collision.GetComponent<CollisionInfo>(), _damage))
        {
            var collMoveComponent = collision.SearchComponent<MoveComponent>();
            if (collMoveComponent != null)
            {
                _audio.AddSoundToQueue((int)SoundIndexes.PushHitSoundIndex, collMoveComponent.Position);
                collMoveComponent.Push(Reach, ((collision.transform.position - transform.position).normalized + 3f * _direction).normalized);
            }
        }
    }

    private void setpoints(Vector2[] points, float h, float w)
    {
        points[0] = new Vector2(h, -w / 3f);
        points[1] = new Vector2(h * 5f / 6f, -w);
        points[2] = new Vector2(0f, -0.5f);
        points[3] = new Vector2(0f, 0.5f);
        points[4] = new Vector2(h * 5f / 6f, w);
        points[5] = new Vector2(h, w / 3f);
    }

    protected override void InitiateAttack()
    {
        _direction = Holder.DirectionVector;
        ResetCoolDown();
    }

    protected override void DisableAttack()
    {
        _coll.enabled = false;
    }

    private void SetPointsSmall()
    {
        setpoints(_points, _ORIGINAL_HEIGHT / 2f, _ORIGINAL_HALFWIDTH / 2f);
        _coll.SetPath(0, _points);
        transform.localScale = new Vector3(Reach / _ORIGINAL_HEIGHT, halfwidth / _ORIGINAL_HALFWIDTH, 1f);
        transform.rotation = Quaternion.Euler(0f, 0f, _direction.AnlgeDegrees());
        transform.position = MoveComponent.Position;
        _rend.enabled = true;
        _anim.StartAnimating();
        _coll.enabled = true;

        _audio.AddSoundToQueue((int)SoundIndexes.PushAttackSoundIndex, transform.position);
    }
    private void SetPointsBig()
    {
        setpoints(_points, _ORIGINAL_HEIGHT, _ORIGINAL_HALFWIDTH);
        _coll.SetPath(0, _points);
    }

    public override void SetUpAI()
    {
        SetUpAICommon();
    }

    protected abstract class PushAttackState : AttackState
    {
        protected PushAttack _pushAttack;
        protected Coroutine _attackCoroutineTimer;

        public PushAttackState(AttackStateMachine ASM, PushAttack pushAttack) : base(ASM)
        {
            _pushAttack = pushAttack;
        }

        public override void LogicalUpdate()
        {
            throw new System.NotImplementedException();
        }

        public override void LogicalFixedUpdate()
        {
            throw new System.NotImplementedException();
        }

        public override void LogicalLateUpdate()
        {
            throw new System.NotImplementedException();
        }

    }

    protected class PushingSmallState : PushAttackState, InputAttackState
    {
        protected PushingBigState _pushingBigState;

        public PushingSmallState(AttackStateMachine ASM, PushAttack pushAttack) : base(ASM,pushAttack)
        {
        }

        public void SetTargetStates(PushingBigState pushingBigState)
        {
            _pushingBigState = pushingBigState;
        }

        public override void OnStateEnter()
        {
            _pushAttack.InitiateAttack();
            _pushAttack.SetPointsSmall();
            _attackCoroutineTimer = _pushAttack.DoActionInTime(_ASM.ChangeState, _pushingBigState, 0.5f * _TIME_OF_PUSHING);
        }

        public override void OnStateExit()
        {
            _pushAttack.StopCoroutine(_attackCoroutineTimer);
        }

        public bool ActivateAttack(bool input)
        {
            return input && _pushAttack.CooldownFinished();
        }
    }

    protected class PushingBigState : PushAttackState
    {
        public PushingBigState(AttackStateMachine ASM, PushAttack pushAttack) : base(ASM, pushAttack)
        {
        }

        public override void OnStateEnter()
        {
            _pushAttack.SetPointsBig();
            _attackCoroutineTimer = _pushAttack.DoActionInTime(_ASM.ChangeToInactive, 0.6f * _TIME_OF_PUSHING);
        }

        public override void OnStateExit()
        {
            _pushAttack.DisableAttack();
            _pushAttack.StopCoroutine(_attackCoroutineTimer);
        }
    }


}
