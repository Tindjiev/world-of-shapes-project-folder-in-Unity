using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealWepBasic : Heal,Projectile
{
    private SpriteRenderer rend;
    private Collider2D coll;

    private move targetmo;
    private Transform targettr;

    protected Vector3 targetPos
    {
        get
        {
            if (targetmo != null)
            {
                return targetmo.position;
            }
            else
            {
                return targettr.position;
            }
        }
    }

    //control
    private float v;
    private bool dampingPhase;
    private float b
    {
        get
        {
            return 2.5f / timeReach;
        }
    }

    //stats
    public float heal = 2f;
    public float timeReach = 0.5f;
    public float vconst = 20f;
    private float distance
    {
        get
        {
            return Reach;
        }
        set
        {
            Reach = value;
        }
    }

    protected new void Awake()
    {
        base.Awake();
        rend = GetComponent<SpriteRenderer>();
        coll = GetComponent<Collider2D>();
        disableAttack();
        distance = 7f;
        cooldownTimer = 2f;
    }

    public override bool inputfunction(KeyCode key)
    {
        return Input.GetKeyDown(key);
    }

    protected new void Start()
    {
        base.Start();
        rend.sprite = colorlib.GetSpriteColored(this.getvarsteam().teamcolor, rend.sprite, colorlib.colortochange);
    }

    protected void Update()
    {

    }

    protected void FixedUpdate()
    {
        if (dampingPhase)
        {
            transform.position += v * (targetPos - transform.position).normalized * Time.fixedDeltaTime;
            v -= b * v * Time.fixedDeltaTime;
            if (v < vconst)
            {
                dampingPhase = false;
            }
        }
        else
        {
            transform.position += vconst * (targetPos - transform.position).normalized * Time.fixedDeltaTime;
        }

    }


    public override bool activate(bool input)
    {
        if (input && checkcooldown() && (targettr = vars.targetTr) != null)
        {
            targetmo = targettr.getvars<move>();
            if (targetmo != null)
            {
                targettr = targetmo.transform;
            }
            initiateAttack();
            return true;
        }
        return false;
    }


    protected override void disableAttack()
    {
        enabled = false;
        rend.enabled = false;
        coll.enabled = false;
    }

    protected override void initiateAttack()
    {
        transform.position = movars.position;
        v = Reach * b;
        dampingPhase = true;
        enabled = true;
        rend.enabled = true;
        coll.enabled = true;
        resetCoolDown();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform == targettr)
        {
            disableAttack();
            //targettr.getvars<lifescript>().life += heal;
            targettr.getvars<lifescript>().heal(this, heal);
        }
    }

    public void blocked()
    {

    }

    public Attack getAttack()
    {
        return this;
    }
}
