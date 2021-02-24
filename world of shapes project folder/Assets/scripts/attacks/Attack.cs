using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack : EntityPartOfCharacter
{
    #region PrefabPath
    private const string _PREFAB_PATH = "attacks";
    public static string BuildPathToWeaponz(string specificWeaponPath)
    {
        return _PREFAB_PATH + "/" + specificWeaponPath;
    }
    public static string BuildPathToWeapon<T>() where T : Attack
    {
        return _PREFAB_PATH + "/" + typeof(T).Name;
    }
    #endregion

    [field: SerializeField]
    public Texture2D DefaultImage { get; private set; }

    [field: SerializeField]
    public Timer Cooldown { get; private set; } = new Timer();

    public float CooldownTimer
    {
        get => Cooldown.TotalCooldown;
        set => Cooldown.SetCounter(value);
    }

    public float Reach = 10f;
    public abstract float Damage { get; }

    protected AttackStateMachine _ASM;

    protected void Awake()
    {
        _ASM = new AttackStateMachine();
    }
    protected void Start()
    {

    }

    public bool Activate(bool input) => _ASM.CheckInput(input);

    protected abstract void InitiateAttack();
    protected abstract void DisableAttack();

    public abstract bool InputFunction(KeyCode key);


    public bool CooldownFinished()  => Cooldown.CheckIfTimePassed; //returns true if attack is NOT on cooldown
    public bool IsOnCooldown() => Cooldown.CheckIfTimeNotPassed; //returns true if attack IS on cooldown
    public void ResetCoolDown() => Cooldown.StartTimer();
    public void ResetCoolDown(float tempCooldown) => Cooldown.StartTimerWithTempCounter(tempCooldown);


    public void ReduceCdTimeRemaining(float SecondsToReduce)
    {
        Cooldown.ReduceTimeRemaining(SecondsToReduce);
    }

    public void ClearCooldown()
    {
        Cooldown.ReduceTimeRemaining(Cooldown.TimeRemaining);
    }

    public bool ApplyDamage(CollisionInfo colliderInfo, float damage)
    {
        if (colliderInfo == null || !colliderInfo.Damagable) return false;
        if (colliderInfo.Entity is BaseCharacterControl collCharacter && collCharacter.DamageMode != DamageMode.Invulnerable && Holder.CanDamage(collCharacter))
        {
            LifeComponent collHealth = colliderInfo.Health;
            if (collHealth != null)
            {
                if (damage != 0f && collHealth.Health > 0f)
                {
                    if (Holder.DamageMode == DamageMode.Undamageable)
                    {
                        collHealth.Damage(this, 0f);
                    }
                    else
                    {
                        collHealth.Damage(this, damage);
                    }
                }
                return true;
            }
#if UNITY_EDITOR
            else
            {
                Debug.Log("no collHealth, colliderInfo: " + colliderInfo, colliderInfo);
                Debug.Log("no collHealth, colliderInfo: " + this, this);
                Debug.Break();
                return false;
            }
#endif
        }
        return false;
    }


    #region AISetup

    public abstract void SetUpAI();

    protected void SetUpAICommon()
    {
        DestroyAIMode<FleeModeClass>();
        GetCheckNullAndSetMode<AttackFromDistance_AttackMode>();
    }

    protected void DestroyAIMode<Mode>() where Mode : AIModeClass
    {
        AI ai = Holder as AI;
        var mode = ai.GetMode<Mode>();
        if (mode == null) mode = ai.GetComponent<Mode>();
        if (mode != null) DestroyImmediate(mode);
    }

    protected Mode GetCheckNullAndSetMode<Mode>() where Mode : AIModeClass
    {
        if (typeof(Mode).IsExactTypeOrSubClassOf<ChillModeClass>())
        {
            return GetCheckNullAndSetModeBase<Mode, ChillModeClass>();
        }
        else if (typeof(Mode).IsExactTypeOrSubClassOf<AttackModeClass>())
        {
            return GetCheckNullAndSetModeBase<Mode, AttackModeClass>();
        }
        else if (typeof(Mode).IsExactTypeOrSubClassOf<FleeModeClass>())
        {
            return GetCheckNullAndSetModeBase<Mode, FleeModeClass>();
        }
        else if (typeof(Mode).IsExactTypeOrSubClassOf<ObjectiveModeClass>())
        {
            return GetCheckNullAndSetModeBase<Mode, ObjectiveModeClass>();
        }
        return null;
    }

    private Mode GetCheckNullAndSetModeBase<Mode, BaseMode>() where Mode : AIModeClass where BaseMode : AIModeClass
    {
        if (!(Holder is AI ai)) return null;
        var mode = ai.GetMode<BaseMode>();
        if (mode == null) mode = ai.GetComponent<BaseMode>();
        if (mode == null)
        {
            return ai.AssignModeScript<Mode>();
        }
        else if (mode.GetType() != typeof(Mode))
        {
            DestroyImmediate(mode);
            return ai.AssignModeScript<Mode>();
        }
        return mode as Mode;
    }

    #endregion



    #region StateMachineRelated

    protected class AttackStateMachine : StateMachine
    {
        protected InactiveBaseAttackState _inactiveAttackBaseState;
        protected InputAttackState _inputAttackState;

        public void InitializeWithStates(InactiveBaseAttackState inactiveAttackBaseState, InputAttackState inputAttackState)
        {
            _inputAttackState = inputAttackState;
            SetStartingState(_inactiveAttackBaseState = inactiveAttackBaseState);
            Initialize();
        }

        public bool CheckInput(bool input)
        {
            if (_inputAttackState.ActivateAttack(input))
            {
                ChangeState(_inputAttackState);
                return true;
            }
            return false;
        }

        public void ChangeToInactive()
        {
            ChangeState(_inactiveAttackBaseState);
        }

        public void SetInputAttackState(InputAttackState inputAttackState)
        {
            _inputAttackState = inputAttackState;
        }

    }

    protected abstract class AttackState : IState
    {
        protected readonly AttackStateMachine _ASM;

        protected AttackState(AttackStateMachine ASM)
        {
            _ASM = ASM;
        }
        public abstract void OnStateEnter();
        public abstract void LogicalUpdate();
        public abstract void LogicalFixedUpdate();
        public abstract void LogicalLateUpdate();
        public abstract void OnStateExit();
    }

    protected class InactiveBaseAttackState : AttackState
    {
        public InactiveBaseAttackState(AttackStateMachine ASM) : base(ASM)
        {
        }

        public override void OnStateEnter()
        {
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

        public override void OnStateExit()
        {
        }
    }

    protected class InactiveEnabledAttackState : InactiveBaseAttackState
    {

        protected Attack _attack;

        public InactiveEnabledAttackState(AttackStateMachine ASM, Attack attack) : base(ASM)
        {
            _attack = attack;
        }

        public override void OnStateEnter()
        {
            _attack.enabled = false;
        }

        public override void OnStateExit()
        {
            _attack.enabled = true;
        }
    }

    protected class InactiveEnabledDisableAttackState : InactiveEnabledAttackState
    {

        public InactiveEnabledDisableAttackState(AttackStateMachine ASM, Attack attack) : base(ASM, attack)
        {
        }

        public override void OnStateEnter()
        {
            base.OnStateEnter();
            _attack.DisableAttack();
        }
    }

    protected interface InputAttackState : IState
    {
        bool ActivateAttack(bool input);
    }

    #endregion

}



public interface Projectile
{
    void Blocked();
    Attack GetAttack();
}

public static class ProjectileExtensions
{
    public static bool CheckToBlockAttack(this Projectile projectile, CollisionInfo collisionInfo)
    {
        if (collisionInfo == null || !collisionInfo.BlockAttacks) return false;
        if (!(collisionInfo.Entity is BaseCharacterControl collCharacter) || collCharacter.CanDamage(projectile.GetAttack().Holder))
        {
            projectile.Blocked();
            return true;
        }
        return false;
    }
}


public abstract class UltAttack : Attack
{

}