using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chargeattack : Attack
{
    //rc classes
    public SpriteRenderer rend;
    public Collider2D coll;


    //control variables
    float control;
    float radius;
    mylib.voidfunction MyLateUpdate;

    public double dwDEG
    {
        get
        {
            return anglespeedDEG * Time.fixedDeltaTime;
        }
    }

    //stats
    public float damage = 3f;
    //public float speed = 15f;
    public float time = 0.2f;
    public float anglespeedDEG = 1080f;
    public float maxradius = 1.5f;
    public bool canPush = false;
    public float push_distance = 3f;

    protected new void Awake()
    {
        base.Awake();
        (rend = GetComponent<SpriteRenderer>()).enabled = false;
        (coll = GetComponent<Collider2D>()).enabled = false;


        cooldownTimer = 2f;
    }

    public override bool inputfunction(KeyCode key)
    {
        return Input.GetKeyDown(key);
    }
    protected new void Start()
    {
        rend.sprite = colorlib.GetSpriteColored(this.getvarsteam().teamcolor, rend.sprite, colorlib.colortochange);
    }


    void LateUpdate()
    {
        MyLateUpdate();
    }

    void FixedUpdate()
    {
        transform.Rotate(0f, 0f, -(float)dwDEG);
    }


    void OnTriggerEnter2D(Collider2D collision)
    {

        aboutcollisions collparameters = collision.GetComponent<aboutcollisions>();

        if (rules.collisiondamage(this, collparameters, damage))
        {
            if (canPush)
            {
                Vector3 targ_sub_pos = collparameters.transform.position - movars.transform.position;
                Vector3 directionSpin = -mathlib.rotate90(targ_sub_pos);
                collparameters.vars.movars.push(push_distance, (targ_sub_pos + directionSpin).normalized);
            }
        }
    }


    protected override void disableAttack()
    {
        rend.enabled = false;
        coll.enabled = false;
        enabled = false;
    }

    public override bool activate(bool input)
    {
        if (input && checkcooldown() && !movars.dashing)
        {
            initiateAttack();
            return true;
        }
        else
        {
            return false;
        }
    }

    protected override void initiateAttack()
    {
        rend.enabled = coll.enabled = enabled = true;
        transform.localScale = new Vector3(maxradius, maxradius);
        if (Reach == 0f)
        {
            MyLateUpdate = LateUpdateStill;
            control = 0f;
        }
        else
        {
            movars.dash(vars.directionVector, Reach, time, true);
            MyLateUpdate = LateUpdateCharge;
        }
        resetCoolDown();
    }


    void LateUpdateCharge()
    {
        transform.position = movars.position;
        if (!movars.dashing)
        {
            disableAttack();
        }
    }
    void LateUpdateStill()
    {
        transform.position = movars.position;
        if (control > time)
        {
            disableAttack();
        }
        control += Time.fixedDeltaTime;
    }

}
