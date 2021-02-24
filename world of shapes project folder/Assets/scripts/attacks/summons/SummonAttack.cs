using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonAttack : Attack
{
    [field: SerializeField, ReadOnlyOnInspector]
    public GameObject Summoned { get; private set; }

    public float MaxTimeSummoned = 15f;
    public float StartingHealth = 10f;


    private BaseCharacterControl _summonedCharacter;
    public BaseCharacterControl SummonedCharacter => _summonedCharacter != null ? _summonedCharacter : _summonedCharacter = Summoned.GetCharacter();
    public MoveComponent SummonedMoveComponent => SummonedCharacter.MoveComponent;

    public override float Damage => 0f;

    protected new void Awake()
    {
        base.Awake();

        Summoned = AddInEditor.MobCreate.sprayer(Vector3.zero, transform, StartingHealth);
        Summoned.SetActive(false);
        SummonedCharacter.Life.AddActionOnDeath(() => Summoned.SetActive(false));
        SummonedCharacter.Life.AddActionOnDeath(() => _ASM.ChangeToInactive());

        Summoned.SearchComponent<MoveAround_ChillMode>().SetCentreTarget(MoveComponent);

        Summoned.SearchComponent<CollisionInfo>().SetEntity(Holder);

        _ASM.InitializeWithStates(new InactiveBaseAttackState(_ASM), new SummonedState(_ASM, this));
    }

    protected new void Start()
    {
        base.Start();
    }

    public override bool InputFunction(KeyCode key) => Input.GetKeyDown(key);

    protected override void InitiateAttack()
    {
        Summoned.SetActive(true);
        SummonedMoveComponent.SetPosition(MoveComponent.Position);
        SummonedCharacter.SetAggroMode(AggroMode.FollowCanTargetFromAnother, Holder);
        SummonedCharacter.Life.Health = StartingHealth;

    }


    protected override void DisableAttack()
    {
        SummonedCharacter.SearchComponent<Attack>().enabled = false;
        SummonedCharacter.Life.Health = 0f;
        ResetCoolDown();
    }

    public override void SetUpAI()
    {
        throw new System.NotImplementedException();
    }

    protected abstract class SummonAttackState : AttackState
    {
        protected SummonAttack _summonAttack;

        protected SummonAttackState(AttackStateMachine ASM, SummonAttack summonAttack) : base(ASM)
        {
            _summonAttack = summonAttack;
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

    protected class SummonedState : SummonAttackState, InputAttackState
    {

        private Coroutine _secondsSummoned;

        public SummonedState(AttackStateMachine ASM, SummonAttack summonAttack) : base(ASM, summonAttack)
        {
        }

        public override void OnStateEnter()
        {
            _summonAttack.InitiateAttack();
            _secondsSummoned = _summonAttack.DoActionInTime(_ASM.ChangeToInactive, _summonAttack.MaxTimeSummoned);
        }

        public override void OnStateExit()
        {
            _summonAttack.StopCoroutine(_secondsSummoned);
            _summonAttack.DisableAttack();
        }

        public bool ActivateAttack(bool input)
        {
            return input && _summonAttack.CooldownFinished();
        }
    }

}
