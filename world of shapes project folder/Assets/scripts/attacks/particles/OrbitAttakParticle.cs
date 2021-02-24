using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitAttakParticle : MonoBehaviour, Projectile
{
    private OrbitAttack _orbitAttack;

    private Collider2D _coll;

    private Vector3 _direction;
    private float _damage;

    protected void Awake()
    {
        _orbitAttack = this.SearchComponent<OrbitAttack>();
        _coll = GetComponent<Collider2D>();
        _coll.enabled = false;
        if (!_orbitAttack.enabled) EndEnable();
    }

    protected void OnEnable()
    {
        transform.position = _orbitAttack.MoveComponent.Position + _orbitAttack.Radius * (_direction = _orbitAttack.TempDirectionOfParticle = MyMathlib.RotateVector(_orbitAttack.TempDirectionOfParticle, _orbitAttack.ParticleAngleDiffVector));
        float size = _orbitAttack.ParticleSize;
        transform.localScale = new Vector3(size, size, 1f);
        _damage = _orbitAttack.Damage;
        _coll.enabled = false;
    }


    private void LateUpdate()
    {
        if (!_coll.enabled && _orbitAttack.Radius == _orbitAttack.MaxRadius)
        {
            _coll.enabled = true;
        }
        if (!_orbitAttack.OrbitThrown)
        {
            transform.position = _orbitAttack.MoveComponent.Position + _orbitAttack.Radius * (_direction = MyMathlib.RotateVector(_direction, _orbitAttack.Directionrotatespeed));
        }
        else
        {
            transform.position = _orbitAttack.Centre + _orbitAttack.Radius * (_direction = MyMathlib.RotateVector(_direction, _orbitAttack.Directionrotatespeed));
        }
    }

    private void EndEnable()
    {
        gameObject.SetActive(false);
    }


    public void Blocked() => EndEnable();
    public Attack GetAttack() => _orbitAttack;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CollisionInfo collparameters = collision.GetComponent<CollisionInfo>();

        this.CheckToBlockAttack(collparameters);

        if (_orbitAttack.ApplyDamage(collparameters, _damage))
        {
            EndEnable();
        }
    }
}
