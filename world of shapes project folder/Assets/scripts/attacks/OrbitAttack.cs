using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitAttack : Attack
{
    public Vector3 TempDirectionOfParticle { get; set; }
    public Vector3 Centre { get; private set; }
    public bool OrbitThrown { get; private set; }
    public float Radius { get; private set; }
    public Vector3 ParticleAngleDiffVector { get; private set; }
    public Vector3 Directionrotatespeed { get; private set; }

    private int _lastNum = 0;
    private float _lastFixedFramerate;
    private float _radiusSpeed = 10f;

    private float _dwDegrees => AngularSpeedDegrees * Time.fixedDeltaTime;

    //stats
    [SerializeField]
    private float _damage;
    public int NumberOfParticles = 15;
    public float Speed = 30f;
    public float AngularSpeedDegrees = 120f;
    public float MaxRadius = 1.5f;
    public float ParticleSize = 1f;

    public override float Damage => _damage;

    private OribtAttackSpawnedState _spawnState = null;

    protected new void Awake()
    {
        base.Awake();


        enabled = false;

        _spawnState = new OribtAttackSpawnedState(_ASM, this);
        OrbitAttackThrownState s2 = new OrbitAttackThrownState(_ASM, this);

        _spawnState.SetTargetStates(s2);
        s2.SetTargetStates(_spawnState);
        _ASM.InitializeWithStates(new InactiveEnabledDisableAttackState(_ASM, this), _spawnState);
    }

    protected new void Start()
    {
        base.Start();
        CheckAndSetParticleNumber(NumberOfParticles);
    }

    public override bool InputFunction(KeyCode key)
    {
        return Input.GetKeyDown(key);
    }

    private void LateUpdate()
    {
        _ASM.LogicalLateUpdate();
    }

    private void FixedUpdate()
    {
        _ASM.LogicalFixedUpdate();
    }

    private void SetRadius()
    {
        Radius = Radius < MaxRadius ? Radius + _radiusSpeed * Time.fixedDeltaTime : MaxRadius;
    }

    private void CheckAndFixDt()
    {
        if (_lastFixedFramerate != Time.fixedDeltaTime)
        {
            Directionrotatespeed = MyMathlib.PolarVectorDeg(_dwDegrees);
            _lastFixedFramerate = Time.fixedDeltaTime;
#if UNITY_EDITOR
            Debug.Break();
            Debug.Log("framerate changed");
#endif
        }
    }

    public void SetDamage(float newDamage) => _damage = newDamage;

    private void SetParticleNumber(int num)
    {
        if (transform.childCount != 0)
        {
            if (num > transform.childCount)
            {
                Transform tempOriginal = transform.GetChild(0);
                var skin = GetComponent<SkinManager>();
                for (int i = transform.childCount; i < num; i++)
                {
                    var temp = Instantiate(tempOriginal, transform);
                    temp.name = tempOriginal.name;
                    var temp2 = new SkinManager.ImageInfo(temp.GetComponent<SpriteRenderer>(), skin[0].OriginalImage, skin[0].CurrentColor);
                    temp2.ReplaceColorOfSprites(skin[0].CurrentColor);
                    skin.AddRenderer(temp2);
                }
            }
            else
            {
                var skin = GetComponent<SkinManager>();
                for (int i = transform.childCount - 1; i >= num; i--)
                {
                    skin.RemoveRenderer(transform.GetChild(i).GetComponent<SpriteRenderer>());
                    Destroy(transform.GetChild(i).gameObject);
                }
            }
        }
        else
        {
            Transform tempOriginal = Resources.Load<Transform>("Prefabs/" + BuildPathToWeapon<OrbitAttack>()).transform.GetChild(0);
            var skin = GetComponent<SkinManager>();
            for (int i = 0; i < num; i++)
            {
                var temp = Instantiate(tempOriginal, transform);
                temp.name = tempOriginal.name;
                var temp2 = new SkinManager.ImageInfo(temp.GetComponent<SpriteRenderer>(), skin[0].OriginalImage, skin[0].CurrentColor);
                temp2.ReplaceColorOfSprites(skin[0].CurrentColor);
                skin.AddRenderer(temp2);
            }
        }
    }

    private void CheckAndSetParticleNumber(int num)
    {
        if (_lastNum != num)
        {
            SetParticleNumber(num);
            _lastNum = num;
        }
    }


    private bool CheckActiveChildren() //return false if all children are inactive
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf)
            {
                return true;
            }
        }
        return false;
    }

    protected override void DisableAttack()
    {
        OrbitThrown = false;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
    protected override void InitiateAttack()
    {
        enabled = true;
        Radius = 0f;
        TempDirectionOfParticle = Vector3.right;
        ParticleAngleDiffVector = MyMathlib.PolarVectorRad(MyMathlib.TAU / NumberOfParticles);
        Directionrotatespeed = MyMathlib.PolarVectorDeg(_dwDegrees);
        _lastFixedFramerate = Time.fixedDeltaTime;
        CheckAndSetParticleNumber(NumberOfParticles);
        for (int i = 0; i < NumberOfParticles; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void RespawnParticles()
    {
        _ASM.ChangeToInactive();
        _ASM.ChangeState(_spawnState);
    }

    public override void SetUpAI()
    {
        if (GetComponent<OrbitAutoAttack>() == null)
        {
            gameObject.AddComponent<OrbitAutoAttack>();
        }
    }

    protected abstract class OrbitAttackState : AttackState
    {
        protected OrbitAttack _orbitAttack;

        protected OrbitAttackState(AttackStateMachine ASM, OrbitAttack orbitAttack) : base(ASM)
        {
            _orbitAttack = orbitAttack;
        }

        public override void LogicalUpdate()
        {
            throw new System.NotImplementedException();
        }
    }

    protected class OribtAttackSpawnedState : OrbitAttackState, InputAttackState
    {

        protected OrbitAttackThrownState _orbitAttackThrownState;

        public OribtAttackSpawnedState(AttackStateMachine ASM, OrbitAttack orbitAttack) : base(ASM, orbitAttack)
        {
        }

        public void SetTargetStates(OrbitAttackThrownState orbitAttackThrownState)
        {
            _orbitAttackThrownState = orbitAttackThrownState;
        }

        public override void OnStateEnter()
        {
            _orbitAttack.InitiateAttack();
            _ASM.SetInputAttackState(_orbitAttackThrownState);
        }

        public override void LogicalFixedUpdate()
        {
            _orbitAttack.CheckAndFixDt();
            _orbitAttack.SetRadius();
        }

        public override void LogicalLateUpdate()
        {
            if (_orbitAttack.CheckActiveChildren())
            {
                _orbitAttack.transform.position = _orbitAttack.MoveComponent.Position;
            }
            else
            {
                _ASM.ChangeToInactive();
            }
        }

        public override void OnStateExit()
        {
            _ASM.SetInputAttackState(this);
        }

        public bool ActivateAttack(bool input)
        {
            return input && !_orbitAttack.enabled;
        }
    }

    protected class OrbitAttackThrownState : OrbitAttackState, InputAttackState
    {

        protected OribtAttackSpawnedState _orbitAttackSpawnedState;
        private Coroutine _timeOnAir;

        public OrbitAttackThrownState(AttackStateMachine ASM, OrbitAttack orbitAttack) : base(ASM, orbitAttack)
        {
        }

        public void SetTargetStates(OribtAttackSpawnedState orbitAttackSpawnedState)
        {
            _orbitAttackSpawnedState = orbitAttackSpawnedState;
        }

        public override void OnStateEnter()
        {
            _orbitAttack.OrbitThrown = true;
            _orbitAttack.Centre = _orbitAttack.MoveComponent.Position;
            _orbitAttack.TempDirectionOfParticle = _orbitAttack.Holder.DirectionVector != Vector3.zero ? _orbitAttack.Holder.DirectionVector : Vector3.right;
            _timeOnAir = _orbitAttack.DoActionInTime(_ASM.ChangeToInactive, _orbitAttack.Reach / _orbitAttack.Speed);
        }

        public override void LogicalFixedUpdate()
        {
            _orbitAttack.CheckAndFixDt();
            _orbitAttack.Centre += _orbitAttack.Speed * _orbitAttack.TempDirectionOfParticle * Time.fixedDeltaTime;
        }

        public override void LogicalLateUpdate()
        {
            if (_orbitAttack.CheckActiveChildren())
            {
                _orbitAttack.transform.position = _orbitAttack.Centre;
            }
            else
            {
                _ASM.ChangeToInactive();
            }
        }

        public override void OnStateExit()
        {
            _orbitAttack.StopCoroutine(_timeOnAir);
        }

        public bool ActivateAttack(bool input)
        {
            return input && _orbitAttack.Radius == _orbitAttack.MaxRadius;
        }

    }

}
