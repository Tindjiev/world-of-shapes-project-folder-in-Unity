using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiangleChargerBodyAttack : Attack
{
    private TriangleChargerAI _AIvars;

    public override float Damage => _AIvars.Damage;

    protected new void Awake()
    {
        _AIvars = this.SearchComponent<TriangleChargerAI>();
    }

    public override bool InputFunction(KeyCode key)
    {
        throw new System.NotImplementedException();
    }

    protected override void DisableAttack()
    {
        throw new System.NotImplementedException();
    }

    protected override void InitiateAttack()
    {
        throw new System.NotImplementedException();
    }

    public override void SetUpAI()
    {
        throw new System.NotImplementedException();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (ApplyDamage(collision.GetComponent<CollisionInfo>(), _AIvars.Damage))
        {
            _AIvars.Life.Health = 0f;
        }
    }
}
