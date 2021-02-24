using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAttack : Attack
{
    private PolygonCollider2D _coll;
    private AudioSource _audioSourceLoop;

    [SerializeField]
    private AudioClip _fireStartAudioClip;

    //control variables
    private bool _gotInput;
    private Timer _chargingTimer;
    private float _lastFixedFrameRate;
    private Vector3 _currentDirection;
    private Vector3 _directionRotationSpeed;
    private Vector2[] _points;
    private Transform[] _particles = new Transform[0];
    public const float _ORIGINAL_HEIGHT = 4f;
    public const float _ORIGINAL_HALFWIDTH = 2f;

    private static int _numberOfParticles = 15;

    private float _dwDegrees => AngularSpeeedDegrees * Time.fixedDeltaTime;

    //stats
    public float DamagePerSecond = 5f;
    public float HalfWidth;
    public float TimeOfCharge = 0.8f;
    public float AngularSpeeedDegrees = 120f;

    public override float Damage => DamagePerSecond * Time.fixedDeltaTime;


    protected new void Awake()
    {
        base.Awake();
        Reach = _ORIGINAL_HEIGHT;
        HalfWidth = _ORIGINAL_HALFWIDTH;

        _coll = GetComponent<PolygonCollider2D>();
        _coll.enabled = false;
        _points = new Vector2[6];
        _audioSourceLoop = GetComponent<AudioSource>();

        _chargingTimer = new Timer(TimeOfCharge);


        FireChargingState s1 = new FireChargingState(_ASM, this);
        FiringState s2 = new FiringState(_ASM, this);
        s1.SetTargetStates(s2);
        s2.SetTargetStates(s1);
        _ASM.InitializeWithStates(new InactiveEnabledAttackState(_ASM, this), s1);

        ClearCooldown();
    }

    protected new void Start()
    {
        SetParticlesNumber(_numberOfParticles);
        base.Start();
    }

    public override bool InputFunction(KeyCode key)
    {
        return Input.GetKey(key);
    }

    private void LateUpdate()
    {
        _ASM.LogicalLateUpdate();
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        CollisionInfo collparameters = collision.GetComponent<CollisionInfo>();

        ApplyDamage(collparameters, Damage);
    }

    private void Setpoints(Vector2[] points, float h, float w)
    {
        points[0] = new Vector2(h, -w / 3f);
        points[1] = new Vector2(h * 5f / 6f, -w);
        points[2] = new Vector2(0f, -0.5f);
        points[3] = new Vector2(0f, 0.5f);
        points[4] = new Vector2(h * 5f / 6f, w);
        points[5] = new Vector2(h, w / 3f);
    }

    public bool Checkborders(in Vector3 pos)
    {
        Vector2 points5 = _points[5];
        points5.x *= transform.localScale.x;
        points5.y *= transform.localScale.y;
        Vector2 points4 = _points[4];
        points4.x *= transform.localScale.x;
        points4.y *= transform.localScale.y;
        return pos.x > Reach ||
            System.Math.Abs(pos.y) > (points5.y - points4.y) / (points5.x - points4.x) * (pos.x - points5.x) + points5.y ||
            System.Math.Abs(pos.y) > pos.x * points4.y / points4.x + 1f;
    }


    public void FollowDirection(in Vector3 DirectionTarget)
    {
        if (_lastFixedFrameRate != Time.fixedDeltaTime)
        {
            _directionRotationSpeed = MyMathlib.PolarVectorDeg(_dwDegrees);
            _lastFixedFrameRate = Time.fixedDeltaTime;
            Debug.Break();
            Debug.Log("huh...");
        }
        Vector2 div = MyMathlib.MultiplyComplexConjugate(DirectionTarget, _currentDirection);
        if (div.y >= 4f * Time.fixedDeltaTime)
        {
            transform.Rotate(0f, 0f, (float)_dwDegrees);
            _currentDirection = MyMathlib.MultiplyComplex(_currentDirection, _directionRotationSpeed);
        }
        else if (div.y <= -4f * Time.fixedDeltaTime)
        {
            transform.Rotate(0f, 0f, -(float)_dwDegrees);
            _currentDirection = MyMathlib.MultiplyComplexConjugate(_currentDirection, _directionRotationSpeed);
        }
        else if (div.x > 0f)    // because if target direction is oposite of flamethrow direction then that still means y==0, so we need to avoid that by ensuring x>0
        {
            transform.rotation = Quaternion.Euler(0f, 0f, DirectionTarget.AnlgeDegrees());
            _currentDirection = DirectionTarget;
        }
        else        //here flamethrow direction is exact opposite of wanted direction and wouln't move without this case, so I chose to just rotate anti-clockwise in this frame
        {
            transform.Rotate(0f, 0f, (float)_dwDegrees);
            _currentDirection = MyMathlib.MultiplyComplex(_currentDirection, _directionRotationSpeed);
        }
    }

    private void SetParticlesNumber(int num)
    {
        if (transform.childCount != 0 || _particles.Length != 0)
        {
            if (num > _particles.Length)
            {
                Transform temp;
                if (_particles.Length == 0)
                {
                    temp = transform.GetChild(0);
                }
                else
                {
                    temp = _particles[0];
                }
                int oldLength = _particles.Length;
                System.Array.Resize(ref _particles, num);
                var skin = GetComponent<SkinManager>();
                for (int i = oldLength; i < num; i++)
                {
                    _particles[i] = Instantiate(temp, transform);
                    _particles[i].name = temp.name;
                    var temp2 = new SkinManager.ImageInfo(_particles[i].GetComponent<SpriteRenderer>(), skin[0].OriginalImage, skin[0].CurrentColor);
                    temp2.ReplaceColorOfSprites(skin[0].CurrentColor);
                    skin.AddRenderer(temp2);
                }
            }
            else if(num < _particles.Length)
            {
                var skin = GetComponent<SkinManager>();
                for (int i = _particles.Length - 1; i >= num; i--)
                {
                    skin.RemoveRenderer(_particles[i].GetComponent<SpriteRenderer>());
                    Destroy(_particles[i].gameObject);
                }
                System.Array.Resize(ref _particles, num);
            }
        }
        else
        {
            Transform temp = Resources.Load<Transform>("Prefabs/" + BuildPathToWeapon<FireAttack>()).transform.GetChild(0);
            _particles = new Transform[num];
            var skin = GetComponent<SkinManager>();
            for (int i = _particles.Length; i < num; i++)
            {
                _particles[i] = Instantiate(temp, transform);
                _particles[i].name = temp.name;
                var temp2 = new SkinManager.ImageInfo(_particles[i].GetComponent<SpriteRenderer>(), skin[0].OriginalImage, skin[0].CurrentColor);
                temp2.ReplaceColorOfSprites(skin[0].CurrentColor);
                skin.AddRenderer(temp2);
            }
        }
    }

    protected override void InitiateAttack()
    {
        _currentDirection = Holder.DirectionVector;
        if (_currentDirection == Vector3.zero)
        {
            _currentDirection = Vector3.right;
        }
        transform.rotation = Quaternion.Euler(0f, 0f, _currentDirection.AnlgeDegrees());
        _directionRotationSpeed = MyMathlib.PolarVectorDeg(_dwDegrees);
        _lastFixedFrameRate = Time.fixedDeltaTime;
        Setpoints(_points, _ORIGINAL_HEIGHT / 3f, _ORIGINAL_HALFWIDTH / 3f);   //the collider is always set to its original size with the originals and changes by the localscale
        transform.localScale = new Vector3(Reach / _ORIGINAL_HEIGHT, HalfWidth / _ORIGINAL_HALFWIDTH, 1f);
        AudioSource.PlayClipAtPoint(_fireStartAudioClip, transform.position, 1f);
        _chargingTimer.StartTimer();
    }

    protected override void DisableAttack()
    {
        _coll.enabled = false;
        _audioSourceLoop.enabled = false;
        ResetCoolDown();
    }

    private void ChargingFinishedAndStartFire()
    {
        Setpoints(_points, _ORIGINAL_HEIGHT, _ORIGINAL_HALFWIDTH);
        _coll.SetPath(0, _points);
        _coll.enabled = true;
        _audioSourceLoop.enabled = true;
    }

    public override void SetUpAI()
    {
        SetUpAICommon();
    }

    protected abstract class FireAttackState : AttackState
    {
        protected FireAttack _fireAttack;
        protected FireAttackState(AttackStateMachine ASM, FireAttack fireAttack) : base(ASM)
        {
            _fireAttack = fireAttack;
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
            Vector3 DirectionTarget = _fireAttack.Holder.DirectionVector;
            _fireAttack.transform.position = _fireAttack.MoveComponent.Position;


            _fireAttack.FollowDirection(DirectionTarget);

            for (int i = 0; i < _fireAttack.transform.childCount; i++)  //enabling particles
            {
                if (!_fireAttack.transform.GetChild(i).gameObject.activeSelf)
                {
                    _fireAttack.transform.GetChild(i).gameObject.SetActive(true);
                }
            }
            if (!_fireAttack._gotInput)
            {
                _ASM.ChangeToInactive();
            }
            else
            {
                _fireAttack._gotInput = false;
            }
        }
    }

    protected class FireChargingState : FireAttackState, InputAttackState
    {
        protected FiringState _firingState;
        private Coroutine _waitSecondsToFire;

        public FireChargingState(AttackStateMachine ASM, FireAttack fireAttack) : base(ASM, fireAttack)
        {

        }

        public void SetTargetStates(FiringState firingState)
        {
            _firingState = firingState;
        }

        public override void OnStateEnter()
        {
            _fireAttack.InitiateAttack();
            _waitSecondsToFire = _fireAttack.DoActionInTime(_ASM.ChangeState, _firingState, _fireAttack.TimeOfCharge);
        }

        public override void OnStateExit()
        {
            _fireAttack.StopCoroutine(_waitSecondsToFire);
        }

        public bool ActivateAttack(bool input)
        {
            return _fireAttack._gotInput = input && _fireAttack.CooldownFinished();
        }



    }

    protected class FiringState : FireAttackState, InputAttackState
    {
        protected FireChargingState _fireChargingState;

        public FiringState(AttackStateMachine ASM, FireAttack fireAttack) : base(ASM, fireAttack)
        {

        }

        public void SetTargetStates(FireChargingState fireChargingState)
        {
            _fireChargingState = fireChargingState;
        }

        public override void OnStateEnter()
        {
            _ASM.SetInputAttackState(this);
            _fireAttack.ChargingFinishedAndStartFire();
        }

        public override void OnStateExit()
        {
            _ASM.SetInputAttackState(_fireChargingState);
            _fireAttack.DisableAttack();
        }

        public bool ActivateAttack(bool input)
        {
            return _fireAttack._gotInput = input && _fireAttack.CooldownFinished();
        }
    }

}
