using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprayAttackParticle : MonoBehaviour, Projectile
{
    //class variables
    private SprayAttack _sprayAttack;
    private SpriteRenderer _rend;
    private Collider2D _coll;
    private Rigidbody2D _rb;

    //control variables
    private float _time;
    private float _maxTime;
    private Vector3 _movement;
    private float _damage;
    private bool _firstTime;


    protected void Awake()
    {
        _sprayAttack = this.SearchComponent<SprayAttack>();

        _rend = GetComponent<SpriteRenderer>();
        _coll = GetComponent<Collider2D>();
        _rb = GetComponent<Rigidbody2D>();

        gameObject.SetActive(false);
    }

    protected void Start()
    {
    }

    protected void FixedUpdate()
    {
        if (_time > 0f)
        {
            if (_firstTime)
            {
                _firstTime = false;
                _coll.enabled = true;
                _rend.enabled = true;
                transform.position = _sprayAttack.MoveComponent.Position;
                _rb.velocity = _movement;
            }
            if (_time > _maxTime)
            {
                ProjectileStop();
            }
        }
        _time += Time.fixedDeltaTime;
    }

    protected void OnEnable()
    {
        _firstTime = true;
        _coll.enabled = false;
        _rend.enabled = false;

        var angle = _sprayAttack.AngleDirection + Random.Range(-_sprayAttack.AngleSpreadRAD, _sprayAttack.AngleSpreadRAD) / 2f;
        _movement = MyMathlib.PolarVectorRad(_sprayAttack.Speed, angle) + _sprayAttack.MoveComponent.Velocity;
        transform.eulerAngles = new Vector3(0, 0, angle * MyMathlib.RAD_TO_DEG);

        _maxTime = _sprayAttack.Reach / _sprayAttack.Speed;
        _time = Random.Range(-_maxTime * 0.75f, 0); //wait a bit so the fire rate can be uniform and not all come out at the same time

        _damage = _sprayAttack.Damage;
    }


    public void ProjectileStop()
    {
        gameObject.SetActive(false);
        _coll.enabled = false;
        _rend.enabled = false;
    }

    public void Blocked() => ProjectileStop();
    public Attack GetAttack() => _sprayAttack;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        CollisionInfo collparameters = collision.GetComponent<CollisionInfo>();

        this.CheckToBlockAttack(collparameters);

        _sprayAttack.ApplyDamage(collparameters, _damage);

    }
}