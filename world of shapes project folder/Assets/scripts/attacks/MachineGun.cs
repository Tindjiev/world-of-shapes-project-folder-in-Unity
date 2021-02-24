using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun : Attack, Projectile
{
    private Rigidbody2D _rigidBody;
    private MachineBullet _bullet;

    private float _angleDirection;
    private float _maxDistance;

    public float CurrentReach { get; private set; }

    //stats
    public float Speed = 130f;
    public float DamagePerSecond = 10f;
    public float AngleSpeedDeg = 45f;

    public override float Damage => DamagePerSecond * Time.fixedDeltaTime;

    protected new void Awake()
    {
        base.Awake();
        _rigidBody = GetComponent<Rigidbody2D>();
        _maxDistance = Reach;
        DisableAttack();
        _bullet = GetComponentInChildren<MachineBullet>();

        _ASM.InitializeWithStates(new InactiveEnabledAttackState(_ASM, this), new MachinGunFiringState(_ASM, this));
    }

    protected new void Start()
    {
        base.Start();
        if (Holder is PlayerBaseControl) AngleSpeedDeg = 400f;
    }

    public override bool InputFunction(KeyCode key)
    {
        return Input.GetKey(key);
    }

    protected void LateUpdate()
    {
        _ASM.LogicalLateUpdate();
    }

    private void ChangeRotation()
    {
        float rbRotDegrees = _rigidBody.rotation;
        float angleDirectiontemp = _angleDirection;
        if (System.Math.Abs(angleDirectiontemp - rbRotDegrees) > 180f)
        {
            if (rbRotDegrees < 0f)
            {
                rbRotDegrees += 360f;
            }
            else
            {
                angleDirectiontemp += 360f;
            }
        }
        if (rbRotDegrees > angleDirectiontemp + AngleSpeedDeg * Time.fixedDeltaTime)
        {
            _rigidBody.SetRotation(_rigidBody.rotation - AngleSpeedDeg * Time.fixedDeltaTime);
        }
        else if (rbRotDegrees < angleDirectiontemp - AngleSpeedDeg * Time.fixedDeltaTime)
        {
            _rigidBody.SetRotation(_rigidBody.rotation + AngleSpeedDeg * Time.fixedDeltaTime);
        }
        else
        {
            _rigidBody.SetRotation(_angleDirection);
        }
    }

    private void SetCurrentReach()
    {
        if (CurrentReach < _maxDistance)
        {
            CurrentReach += Speed * Time.fixedDeltaTime;
        }
        else
        {
            CurrentReach = _maxDistance;
        }
        if (CurrentReach > 1f)
        {
            Vector2 direction = MyMathlib.PolarVector2Deg(_rigidBody.rotation);
            Vector2 currentPosition = transform.position;
            var results = Physics2D.LinecastAll(currentPosition + direction, currentPosition + (CurrentReach + 1f) * direction, gameObject.layer.GetLayerMask());
            if (results.Length > 0)
            {
                CollisionInfo collHit = null;
                float minDistSq = CurrentReach.Sq();
                for (int i = results.Length - 1; i >= 0; --i)
                {
                    var collInfo = results[i].collider.GetComponent<CollisionInfo>();
                    if (ApplyDamage(collInfo, 0f) || this.CheckToBlockAttack(collInfo))
                    {
                        float tempdistsq = (results[i].point - currentPosition).ProjectOnNormalisedVector(direction).sqrMagnitude;
                        if (tempdistsq < minDistSq)
                        {
                            minDistSq = tempdistsq;
                            collHit = collInfo;
                        }
                    }
                }
                if (collHit != null)
                {
                    _maxDistance = Mathf.Sqrt(minDistSq) + 0.1f;
                    ApplyDamage(collHit, Damage);
                }
                else
                {
                    _maxDistance = Reach;
                }
            }
            else
            {
                _maxDistance = Reach;
            }
            if (CurrentReach > _maxDistance)
            {
                CurrentReach = _maxDistance;
            }
        }
    }

    protected void FixedUpdate()
    {
        _ASM.LogicalFixedUpdate();
    }

    public void Blocked()
    {
    }

    public Attack GetAttack() => this;

    protected override void InitiateAttack()
    {
        _angleDirection = Holder.DirectionVector.AnlgeDegrees();
        CurrentReach = 0f;
        _rigidBody.rotation = _angleDirection;
        _bullet.enabled = true;
    }

    protected override void DisableAttack()
    {
    }

    public override void SetUpAI()
    {
        SetUpAICommon();
    }

    protected abstract class MachineGunAttackState: AttackState
    {
        protected MachineGun _machineGun;

        protected MachineGunAttackState(AttackStateMachine ASM, MachineGun machineGun) : base(ASM)
        {
            _machineGun = machineGun;
        }

        public override void LogicalUpdate()
        {
            throw new System.NotImplementedException();
        }
    }

    protected class MachinGunFiringState : MachineGunAttackState, InputAttackState
    {
        private bool _gotInput = false;
        public MachinGunFiringState(AttackStateMachine ASM, MachineGun machineGun) : base(ASM, machineGun)
        {

        }

        public override void OnStateEnter()
        {
            _machineGun.InitiateAttack();
        }

        public override void LogicalFixedUpdate()
        {
            _machineGun._angleDirection = _machineGun.Holder.DirectionVector.AnlgeDegrees();
            _machineGun.ChangeRotation();
            _machineGun.SetCurrentReach();
        }

        public override void LogicalLateUpdate()
        {
            _machineGun.transform.position = _machineGun.MoveComponent.Position;
            if (!_gotInput)
            {
                _ASM.ChangeToInactive();
            }
            else
            {
                _gotInput = false;
            }
        }

        public override void OnStateExit()
        {

        }

        public bool ActivateAttack(bool input)
        {
            return _gotInput = input;
        }
    }

}
