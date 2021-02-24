using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;

public class LifeComponent : MonoBehaviour
{
    [field: SerializeField]
    public EntityBase Holder { get; private set; }

    [field: SerializeField]
    public float Health { get; set; }

    [field: SerializeField]
    public float MaxHealth { get; set; }

    public bool MissingHealth => Health < MaxHealth;
    public bool HasFullHealth => !MissingHealth;

    [field: SerializeField]
    public bool IsLifeOfCharacter { get; private set; }

    public bool IsDying { get; private set; }

    [SerializeField]
    private UltEvent _death = new UltEvent(), _whileDying = new UltEvent();

    public SpriteRenderer Rend => _healthbar.Rend;
    private HealthVisualRepresenterBase _healthbar;

    public bool IsRendering
    {
        get => _healthbar != null ? _healthbar.gameObject.activeInHierarchy : false;
        set
        {
            if (_healthbar == null) return;
            _healthbar.gameObject.SetActive(value);
        }
    }

    protected void Awake()
    {
        Holder = IsLifeOfCharacter ? this.GetCharacter() : this.SearchComponent<EntityBase>();
        _healthbar = this.SearchComponent<HealthVisualRepresenterBase>();
    }

    protected void Start()
    {
        if (MaxHealth == 0f) MaxHealth = Health;
    }

    protected void LateUpdate()
    {
        if (Health <= 0f && !IsDying)
        {
            StartDeath();
        }
    }

    private void DefaultDeath()
    {
        Destroy(Holder.gameObject);
    }

    private void StartDeath()
    {
        _whileDying.Invoke();
        if (TryGetComponent(out DeathAnimationBase deathAniamtion))
        {
            IsDying = true;
            deathAniamtion.Trigger();
        }
        else
        {
            Death();
        }
    }

    public void Death()
    {
        IsDying = false;
        SetMaxHealth();
        if (_death.PersistentCallsList.Count == 0) DefaultDeath();
        else _death.Invoke();
    }

    public void SetMaxHealth() => Health = MaxHealth;

    public void AddActionOnDeath(Action action) => _death.AddPersistentCall(action);
    public void AddActionDuringDeath(Action action) => _whileDying.AddPersistentCall(action);


    public float Damage(Attack attacker, float damage)
    {
        Health -= damage;
        if (Health <= 0f)
        {
            damage += Health;
            Health = 0f;
        }
        if(Holder is BaseCharacterControl character) character.AddDamagedBy(attacker, damage);
        return damage;
    }

    public float Heal(Attack attacker, float health, bool ignoreMaxHealth = false)
    {
        Health += health;
        if (!ignoreMaxHealth && Health > MaxHealth)
        {
            health -= Health - MaxHealth;
            Health = MaxHealth;
        }
        return health;
    }
}