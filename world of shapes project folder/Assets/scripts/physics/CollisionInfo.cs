using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionInfo : MonoBehaviour 
{
    public bool StopChargers = false; // can block and vanish chargers
    public bool BlockAttacks = false; // can block attacks
    public bool Damagable = false;    // if gmbjct has health and can be damaged
    public bool Blockable = false;    // if gmbjct is blocked by obstacles

    private EntityBase _entity;
    public EntityBase Entity => _entity == null ?
                (_entity = this.GetCharacter()) == null ? _entity = this.SearchComponent<EntityBase>() : _entity :
                _entity;

    private LifeComponent _health; //this is need for things like shields which have their own health
    public LifeComponent Health => _health != null ? _health : _health = this.SearchComponent<LifeComponent>();

    public void SetEntity(EntityBase entity) => _entity = entity;
}
