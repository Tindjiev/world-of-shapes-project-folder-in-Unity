using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionChangeAttack : Attack, Projectile, SkinManager.IMultipleSprites
{
    //rc classes
    private SpriteRenderer _rend;
    private CircleCollider2D _coll;
    private DirectionInterface _directionGUI = null;

    [SerializeField]
    private Sprite _preSpikedSprite, _spikedSprite;


    //control variables
    private bool _changed;
    private Vector3 _v;
    private float _tempDamage;

    //stats
    [SerializeField]
    public float _damage = 7f;
    public float Speed = 20f;
    public float Size = 2f;
    public float SecondsBeforeChange = 3f;

    public override float Damage => _damage;

    protected new void Awake()
    {
        base.Awake();
        (_coll = GetComponent<CircleCollider2D>()).enabled = false;
        (_rend = GetComponent<SpriteRenderer>()).enabled = false;

        CooldownTimer = 2.5f;

        this.DoActionInNextFrame(() =>
        {
            if (Holder.gameObject == ControlBase.PlayerGameObject)
            {
                _directionGUI = BasicLib.InstantiatePrefabTr("gui/" + typeof(DirectionInterface).Name, transform).GetComponent<DirectionInterface>();
                _directionGUI.gameObject.SetActive(false);
                _directionGUI.transform.localScale = new Vector3(0.15f, 0.15f);
            }
        });



        AttackOnAirState s1 = new AttackOnAirState(_ASM, this);
        ChagnedDirectionState s2 = new ChagnedDirectionState(_ASM, this);
        s1.SetTargetStates(s2);
        _ASM.InitializeWithStates(new InactiveEnabledDisableAttackState(_ASM, this), s1);

        ClearCooldown();
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


    public void Blocked()
    {
        _ASM.ChangeToInactive();
    }

    public Attack GetAttack()
    {
        return this;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        CollisionInfo collparameters = collision.GetComponent<CollisionInfo>();
        this.CheckToBlockAttack(collparameters);


        if (_changed)
        {
            ApplyDamage(collparameters, _tempDamage);
        }
    }



    protected override void InitiateAttack()
    {
        _v = Speed / 5f * Holder.DirectionVector;
        transform.position = MoveComponent.Position;
        _tempDamage = _damage;
        _changed = false;
        _rend.sprite = _preSpikedSprite;
        _coll.radius = 0.3f * 12f / 22f;

        if (_directionGUI != null)
        {
            _directionGUI.gameObject.SetActive(true);
        }

        _coll.enabled = true;
        _rend.enabled = true;
    }


    protected override void DisableAttack()
    {
        _rend.enabled = false;
        _coll.enabled = false;
        ResetCoolDown();
        if (!_changed && _directionGUI != null)
        {
            _directionGUI.gameObject.SetActive(false);
        }
    }

    public void SetSprites(params Sprite[] sprites)
    {
        _preSpikedSprite = sprites[0];
        _spikedSprite = sprites[1];
    }

    public IEnumerator<Sprite> GetEnumerator()
    {
        yield return _preSpikedSprite;
        yield return _spikedSprite;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override void SetUpAI()
    {
        throw new System.NotImplementedException();
    }

    protected abstract class DirectionChangeAttackState : AttackState
    {

        protected DirectionChangeAttack _directionChange;

        protected DirectionChangeAttackState(AttackStateMachine ASM, DirectionChangeAttack directionChange) : base(ASM)
        {
            _directionChange = directionChange;
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


    protected class AttackOnAirState : DirectionChangeAttackState, InputAttackState
    {
        protected ChagnedDirectionState _chagnedDirectionState;
        private Coroutine _disableInSeconds;

        public AttackOnAirState(AttackStateMachine ASM, DirectionChangeAttack directionChange) : base(ASM, directionChange)
        {
        }

        public void SetTargetStates(ChagnedDirectionState chagnedDirectionState)
        {
            _chagnedDirectionState = chagnedDirectionState;
        }

        public override void OnStateEnter()
        {
            _directionChange.InitiateAttack();
            _ASM.SetInputAttackState(_chagnedDirectionState);
            _disableInSeconds = _directionChange.DoActionInTime(_ASM.ChangeToInactive, _directionChange.SecondsBeforeChange);
        }

        public override void LogicalFixedUpdate()
        {
            _directionChange.transform.position += _directionChange._v * Time.fixedDeltaTime;
        }

        public override void OnStateExit()
        {
            _ASM.SetInputAttackState(this);
            _directionChange.StopCoroutine(_disableInSeconds);
        }

        public bool ActivateAttack(bool input)
        {
            return input && _directionChange.CooldownFinished();
        }
    }


    protected class ChagnedDirectionState : DirectionChangeAttackState, InputAttackState
    {
        private Coroutine _disableInSeconds;
        public ChagnedDirectionState(AttackStateMachine ASM, DirectionChangeAttack directionChange) : base(ASM, directionChange)
        {
        }

        public override void OnStateEnter()
        {
            _directionChange._v = _directionChange.Speed * (_directionChange.Holder.TargetPosition - _directionChange.transform.position).normalized;
            _directionChange._rend.sprite = _directionChange._spikedSprite;
            _directionChange._coll.radius = 0.3f;
            _directionChange._changed = true;

            if (_directionChange._directionGUI != null)
            {
                _directionChange._directionGUI.gameObject.SetActive(false);
            }
            _disableInSeconds = _directionChange.DoActionInTime(_ASM.ChangeToInactive, _directionChange.Reach / _directionChange.Speed);
        }

        public override void LogicalFixedUpdate()
        {
            _directionChange.transform.position += _directionChange._v * Time.fixedDeltaTime;
        }

        public override void OnStateExit()
        {
            _directionChange.StopCoroutine(_disableInSeconds);
            _directionChange._changed = false;
        }

        public bool ActivateAttack(bool input)
        {
            return input;
        }
    }

}
