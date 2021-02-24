using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shieldscript : Attack
{

    //rc classes
    SpriteRenderer rend;
    Collider2D coll;
    lifescript lifevars;


    //control variables
    public bool shielding;
    bool lastframeshielding = false;
    timelib.timer closeTimer;
    Vector3 direction;
    bool waitingforCooldown = false;

   /* public bool charge = false;
    bool gotInputThisFrame;
    float time;
    float maxtime;
    bool firsttime = true;
    public bool unleashed = false;
    bool lastframecharge = false;
    Vector3 movement;
    float tempdamage;*/

    //stats
    public float size = 1f;
    public float maxhealth = 20f;


    protected new void Awake()
    {
        base.Awake();

        (rend = GetComponent<SpriteRenderer>()).enabled = false;
        (coll = GetComponent<Collider2D>()).enabled = false;
        lifevars = GetComponent<lifescript>();
        if (lifevars != null)
        {
            lifevars.death = death;
            lifevars.life = maxhealth;

            lifevars.whileDying = coll.disable;
            lifevars.whileDying += resetCoolDown;
        }


        cooldownTimer = 2.5f;
        closeTimer = new timelib.timer(cooldownTimer);
    }

    protected new void Start()
    {
        base.Start();
        rend.sprite = colorlib.GetSpriteColored(vars.teamcolor, rend.sprite, colorlib.colortochange);
    }


    public override bool inputfunction(KeyCode key)
    {
        return Input.GetKey(key);
    }

    void LateUpdate()
    {
        if (shielding)
        {
            transform.position = movars.position + Reach * direction;
            transform.rotation = Quaternion.Euler(0f, 0f, (float)mathlib.anglevectordeg(direction));
        }
        else if (lastframeshielding || !checkcooldown())
        {
            disableAttack();
            closeTimer.startTimer();
        }
        else
        {
            if (closeTimer.checkIfTimePassed())
            {
                lifevars.life = maxhealth;
            }
        }
        lastframeshielding = shielding;
        shielding = false;
    }

    protected override void initiateAttack()
    {
        rend.enabled = true;
        coll.enabled = true;
    }

    protected override void disableAttack()
    {
        rend.enabled = false;
        coll.enabled = false;
    }


    void death()
    {
        disableAttack();
        //resetCoolDown();
        lifevars.life = maxhealth;
    }

    public override bool activate(bool input)
    {
        if (input)
        {
            shielding = true;
            direction = vars.directionVector;
            if (checkcooldown())
            {
                if (!lastframeshielding || waitingforCooldown)
                {
                    initiateAttack();
                    waitingforCooldown = false;
                }
                return true;
            }
            lastframeshielding = false;
            waitingforCooldown = true;
        }
        return false;
    }


}
