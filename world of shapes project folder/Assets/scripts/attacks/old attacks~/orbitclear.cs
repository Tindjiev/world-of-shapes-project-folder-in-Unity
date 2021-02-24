using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class orbitclear : Attack,Projectile
{


    //rc classes
    SpriteRenderer rend;
    Collider2D coll;
    MyAudioSource Audio;


    //control variables
    public float side = 1;
    bool firsttime = true;
    float time;
    float maxtime;
    float radius;
    Vector3 rotatespeed;
    Vector3 direction;
    float directionmagnitude;
    const int punchhitSoundIndex = 0;



    //stats
    public float damage = 3.5f;
    public float size = 2f;
    public float angvel = 2f * (float)mathlib.TAU;
    public float minReach = 1.5f;

    protected new void Awake()
    {
        base.Awake();
        Audio = GetComponent<MyAudioSource>();
        (coll = GetComponent<Collider2D>()).enabled = false;
        (rend = GetComponent<SpriteRenderer>()).enabled = false;
        rend.sprite = colorlib.GetSpriteColored(vars.teamcolor, rend.sprite, colorlib.colortochange);

        cooldownTimer = 1f;
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
        if (firsttime)
        {
            firsttime = false;
            transform.position = movars.position + (radius = getradius()) * (direction = -side * mathlib.rotate90(direction));
            transform.localScale = new Vector3(size, size, 1f);
            coll.enabled = rend.enabled = true;
            //float secondsforcircle = cooldown;
            //double angvel = mathlib.TAU * Time.fixedDeltaTime / secondsforcircle;
            rotatespeed = mathlib.polarvectrad(angvel * side * Time.fixedDeltaTime);
            //maxcontrol = secondsforcircle;
            maxtime = (float)mathlib.TAU / angvel;
            time = 0;
            resetCoolDown();
            //Debug.Log(direction.magnitude);
        }
        else
        {
            time += Time.fixedDeltaTime;
            direction = mathlib.mulcmplxvect(direction, rotatespeed);
        }
        if (time > maxtime)
        {
            disableAttack();
        }
    }

    void LateUpdate()
    {
        transform.position = movars.position + radius * direction;
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

        rules.blockattack(this, collparameters, blocked);

        if (rules.collisiondamage(this, collparameters, damage))
        {
            move collmovars = collision.getvars<move>();
            collmovars.push(10f, side * mathlib.rotate90(direction));
            //collmovars.push(10f, (collmovars.position - transform.position).normalized);
            //collmovars.push(10f, collision.transform.position - transform.position);

            //           collvars.control = collvars.controlpushed_once;
            //           collvars.endpos = colltr.position + 2f * radius * new Vector3(-direction.y, direction.x) * (float)side /*+ 0.2f * somefunctions.getunitvector2(colltr.position - transform.position)*/;
            Audio.AddSoundToQueue(punchhitSoundIndex, transform.position);
        }
    }

    float getradius()
    {
        float f = directionmagnitude;
        if (f > 1.5f)
        {
            if (f < Reach)
            {
                return f;
            }
            else
            {
                return Reach;
            }
        }
        else
        {
            return minReach;
        }
    }

    protected override void initiateAttack()
    {
        direction = vars.targetPosition - movars.position;
        directionmagnitude = direction.magnitude;
        direction /= directionmagnitude;
        side = -side;
        enabled = true;
    }

    protected override void disableAttack()
    {
        coll.enabled = rend.enabled = enabled = false;
        firsttime = true;
    }

    public override bool activate(bool input)
    {
        if (input && checkcooldown())
        {
            initiateAttack();
            return true;
        }
        return false;
    }



}
