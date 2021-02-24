using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprayAttack : Attack
{
    private AudioSource _audioSourceLoop;

    public float AngleDirection { get; private set; }
    private int _lastNum = 0;

    //stats
    [SerializeField]
    private float _damage = 1.25f;
    public int NumberOfParticles = 8;
    public float DamageToSelfPerSecond = 1f;
    public float MinLifeForDamageToSelf = 0.4f;
    public float Speed = 50f;
    public float AngleSpreadDegrees = 40f;
    public float AngleSpreadRAD => AngleSpreadDegrees * MyMathlib.DEG_TO_RAD;

    public override float Damage => _damage;
    public float DamageToSelf => DamageToSelfPerSecond * Time.deltaTime;


    protected new void Awake()
    {
        base.Awake();
        _audioSourceLoop = transform.GetChild(0).GetComponent<AudioSource>();

        _ASM.InitializeWithStates(new InactiveEnabledAttackState(_ASM, this), new SprayingState(_ASM, this));
    }

    protected new void Start()
    {
        base.Start();
        CheckAndSetParticleNumber(NumberOfParticles);
    }

    public override bool InputFunction(KeyCode key)
    {
        return Input.GetKey(key);
    }

    private void LateUpdate()
    {
        _ASM.LogicalLateUpdate();
    }

    protected void OnDisable()
    {
        _audioSourceLoop.enabled = false;
    }


    private void SetParticleNumber(int num)  // num + 1 because of sound gameobject
    {
        if (transform.childCount > 1)
        {
            if (num + 1 > transform.childCount)
            {
                Transform tempOriginal = transform.GetChild(1);
                var skin = GetComponent<SkinManager>();
                for (int i = transform.childCount; i < num + 1; i++)
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
                for (int i = transform.childCount - 1; i >= num + 1; i--)
                {
                    skin.RemoveRenderer(transform.GetChild(i).GetComponent<SpriteRenderer>());
                    Destroy(transform.GetChild(i).gameObject);
                }
            }
        }
        else
        {
            Transform tempOriginal = Resources.Load<Transform>("Prefabs/" + BuildPathToWeapon<SprayAttack>()).transform.GetChild(1);
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

    protected override void InitiateAttack()
    {
        _audioSourceLoop.enabled = true;
    }

    protected override void DisableAttack()
    {
        enabled = false;
    }

    public override void SetUpAI()
    {
        SetUpAICommon();
    }

    protected abstract class SprayAttackState : AttackState
    {
        protected SprayAttack _sprayAttack;

        protected SprayAttackState(AttackStateMachine ASM, SprayAttack sprayAttack) : base(ASM)
        {
            _sprayAttack = sprayAttack;
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

    protected class SprayingState : SprayAttackState, InputAttackState
    {
        private bool _gotInput = false;
        public SprayingState(AttackStateMachine ASM, SprayAttack sprayAttack) : base(ASM, sprayAttack)
        {

        }

        public override void OnStateEnter()
        {
            _sprayAttack.InitiateAttack();
        }

        public override void LogicalLateUpdate()
        {
            BaseCharacterControl vars = _sprayAttack.Holder; 
            if (vars.Life.Health > _sprayAttack.MinLifeForDamageToSelf)
            {
                vars.Life.Damage(_sprayAttack, _sprayAttack.DamageToSelf);
            }
            if (!_gotInput)
            {
                _ASM.ChangeToInactive();
                return;
            }
            _sprayAttack.AngleDirection = MyMathlib.AngleRadians(vars.DirectionVector);
            _sprayAttack._audioSourceLoop.transform.position = _sprayAttack.MoveComponent.Position;
            Transform transform = _sprayAttack.transform;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (!transform.GetChild(i).gameObject.activeSelf)
                {
                    transform.GetChild(i).gameObject.SetActive(true);
                }
            }
            _sprayAttack.CheckAndSetParticleNumber(_sprayAttack.NumberOfParticles);
            _gotInput = false;
        }

        public override void OnStateExit()
        {
            _sprayAttack.DisableAttack();
        }

        public bool ActivateAttack(bool input)
        {
            return _gotInput = input;
        }
    }


}
