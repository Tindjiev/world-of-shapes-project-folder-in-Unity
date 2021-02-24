using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeChargeAttack : Attack
{
    //rc classes
    public SpriteRenderer rend;
    public Collider2D coll;


    private System.Action _myLateUpdate;

    private double _dwDegrees => anglespeedDEG * Time.fixedDeltaTime;

    //stats
    [SerializeField]
    private float _damage = 3f;
    public float time = 0.2f;
    public float anglespeedDEG = 1080f;
    public float maxradius = 1.5f;
    public bool canPush = false;
    public float push_distance = 3f;

    public override float Damage => _damage;

    protected new void Awake()
    {
        base.Awake();
        (rend = GetComponent<SpriteRenderer>()).enabled = false;
        (coll = GetComponent<Collider2D>()).enabled = false;


        CooldownTimer = 2f;


        _ASM.InitializeWithStates(new InactiveEnabledAttackState(_ASM, this), new ChargingState(_ASM, this));
    }
    protected new void Start()
    {
        base.Start();
    }

    public override bool InputFunction(KeyCode key)
    {
        return Input.GetKeyDown(key);
    }


    void LateUpdate()
    {
        _ASM.LogicalLateUpdate();
    }

    private void FixedUpdate()
    {
        _ASM.LogicalFixedUpdate();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        CollisionInfo collparameters = collision.GetComponent<CollisionInfo>();

        if (ApplyDamage(collparameters, _damage))
        {
            if (canPush)
            {
                Vector3 targ_sub_pos = collparameters.transform.position - MoveComponent.transform.position;
                Vector3 directionSpin = -MyMathlib.Rotate90(targ_sub_pos);
                collparameters.Entity.MoveComponent.Push(push_distance, (targ_sub_pos + directionSpin).normalized);
            }
        }
    }


    protected override void DisableAttack()
    {
        rend.enabled = false;
        coll.enabled = false;
    }

    protected override void InitiateAttack()
    {
        rend.enabled = coll.enabled = enabled = true;
        transform.localScale = new Vector3(maxradius, maxradius);
        if (Reach == 0f)
        {
            _myLateUpdate = LateUpdateStill;
        }
        else
        {
            MoveComponent.Dash(Holder.DirectionVector, Reach, time, true);
            _myLateUpdate = LateUpdateCharge;
        }
        ResetCoolDown();
    }


    private void LateUpdateCharge()
    {
        transform.position = MoveComponent.Position;
        if (!MoveComponent.IsDashing)
        {
            DisableAttack();
        }
    }
    private void LateUpdateStill()
    {
        transform.position = MoveComponent.Position;
    }

    public override void SetUpAI()
    {
        SetUpAICommon();
    }

    protected abstract class ChargeAttackState : AttackState
    {

        protected MeleeChargeAttack _chargeAttack;

        protected ChargeAttackState(AttackStateMachine ASM, MeleeChargeAttack chargeAttack) : base(ASM)
        {
            _chargeAttack = chargeAttack;
        }

        public override void LogicalUpdate()
        {
            throw new System.NotImplementedException();
        }
    }

    protected class ChargingState : ChargeAttackState, InputAttackState
    {
        public ChargingState(AttackStateMachine ASM, MeleeChargeAttack chargeAttack) : base(ASM, chargeAttack)
        {

        }

        public override void OnStateEnter()
        {
            _chargeAttack.InitiateAttack();
            _chargeAttack.DoActionInTime(_ASM.ChangeToInactive, _chargeAttack.time);
        }

        public override void LogicalFixedUpdate()
        {
            _chargeAttack.transform.Rotate(0f, 0f, -(float)_chargeAttack._dwDegrees);
        }

        public override void LogicalLateUpdate()
        {
            _chargeAttack._myLateUpdate();
        }

        public override void OnStateExit()
        {
            _chargeAttack.DisableAttack();
        }

        public bool ActivateAttack(bool input)
        {
            return input && _chargeAttack.CooldownFinished() && !_chargeAttack.MoveComponent.IsDashing;
        }
    }

}
