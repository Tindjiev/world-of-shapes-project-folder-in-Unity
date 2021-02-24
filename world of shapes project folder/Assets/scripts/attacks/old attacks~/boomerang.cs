using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boomerang : Attack,Projectile
{


    //rc classes
    SpriteRenderer rend;
    Collider2D coll;


    //control variables
    bool outofReach;
    float control;
    float maxcontrol;
    Vector3 direction;
    Vector3 v;
    float directionmagnitude;
    const float spinRatio = 4f;

    //stats
    public float damage = 7f;
    public float size = 2f;
    public float drift = 1f;

    public float wdrift
    {
        get
        {
            return drift * (float)mathlib.TAU;
        }
    }

    protected new void Awake()
    {
        base.Awake();
        (coll = GetComponent<Collider2D>()).enabled = false;
        (rend = GetComponent<SpriteRenderer>()).enabled = false;
        rend.sprite = colorlib.GetSpriteColored(vars.teamcolor, rend.sprite, colorlib.colortochange);

        cooldownTimer = 0.75f;
    }

    protected new void Start()
    {
        base.Start();
    }

    public override bool inputfunction(KeyCode key)
    {
        return Input.GetKeyDown(key);
    }

    void FixedUpdate()
    {
        control += Time.fixedDeltaTime;
        UpdatePhysics();
        if (control > maxcontrol)
        {
            disableAttack();
        }
    }

    public void blocked()
    {
        disableAttack();
    }

    public Attack getAttack()
    {
        return this;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {

        aboutcollisions collparameters = collision.GetComponent<aboutcollisions>();

        if (rules.collisiondamage(this, collparameters, damage))
        {

        }
        rules.blockattack(this, collparameters, blocked);
    }

    float getDistance()
    {
        float f = directionmagnitude;
        if (f > 1.5f)
        {
            if (f < Reach)
            {
                outofReach = false;
                return f;
            }
            else
            {
                outofReach = true;
                return Reach;
            }
        }
        else
        {
            return 1.5f;
        }
    }



    public override bool activate(bool input)
    {
        if (input && checkcooldown() && !enabled)
        {
            direction = vars.targetPosition - movars.position;
            directionmagnitude = direction.magnitude;
            direction /= directionmagnitude;
            drift = -drift;
            initiateAttack();
            return enabled = true;
        }
        return false;
    }

    protected override void disableAttack()
    {
        coll.enabled = rend.enabled = enabled = false;
        resetCoolDown();
    }


    protected override void initiateAttack()
    {
        v = getDistance() / 2f * wdrift * mathlib.rotate90(-direction);
        transform.position = movars.position;
        coll.enabled = rend.enabled = true;
        //maxcontrol = spin / 2f;
        maxcontrol = 3f;
        control = 0;
        //resetCoolDown();
    }

    public void UpdatePhysics()
    {
        updatev();
        transform.position += v * Time.fixedDeltaTime;
        transform.Rotate(0f, 0f, drift * 360f * Time.fixedDeltaTime * spinRatio);
    }

    void updatev()
    {
        //v = v * (float)System.Math.Sqrt(1 - spin * spin * Time.fixedDeltaTime * Time.fixedDeltaTime) + spin * mathlib.rotate90(v) * Time.fixedDeltaTime;
        //v += wspin * mathlib.rotate90(v) * Time.fixedDeltaTime;
        //v = (v + wspin * Time.fixedDeltaTime * mathlib.rotate90(v)) / (1 + wspin * wspin * Time.fixedDeltaTime * Time.fixedDeltaTime);

        //v *= Random.Range(0.98f, 1.02f);
        if (outofReach)
        {
            //v *= 1.003f;
            v += wdrift * Time.fixedDeltaTime * mathlib.rotate90(v);
        }
        else
        {
            float w2T2div4 = wdrift * wdrift * Time.fixedDeltaTime * Time.fixedDeltaTime / 4f;
            v = ((1f - w2T2div4) * v + wdrift * Time.fixedDeltaTime * mathlib.rotate90(v)) / (1f + w2T2div4);
        }
    }


}
