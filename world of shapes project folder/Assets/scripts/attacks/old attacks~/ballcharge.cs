using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballcharge : Attack,Projectile
{
    //rc classes
    public SpriteRenderer rend;
    public Collider2D coll;


    //control variables
    public bool charge = false;
    bool gotInputThisFrame;
    float time;
    float maxtime;
    bool firsttime = true;
    public bool unleashed = false;
    bool lastframecharge = false;
    Vector3 movement;
    Vector3 direction;
    float tempdamage;

    //stats
    public float maxsize = 3f;
    public float shrinkrate = 1f;
    public float damage = 3f;
    public float speed = 20f;
    public bool canPassThroughBodies = true;


    protected new void Awake()
    {
        base.Awake();
        (rend = GetComponent<SpriteRenderer>()).enabled = false;
        (coll = GetComponent<Collider2D>()).enabled = false;

        cooldownTimer = 1f;


       /* EntityNotCharacter test = gameObject.AddComponent<EntityNotCharacter>();
        attack test2=null;
        try
        {
            test2 = (attack)test;
        }
        catch (System.InvalidCastException)
        {
            Debug.Log("fail");
        }
        Debug.Log(test);
        Debug.Log(test2);*/
    }

    protected new void Start()
    {
        base.Start();
        rend.sprite = colorlib.GetSpriteColored(this.getvarsteam().teamcolor, rend.sprite, colorlib.colortochange);
    }

    public override bool inputfunction(KeyCode key)
    {
        return Input.GetKey(key);
    }

    //Vector3 startpos;
    void FixedUpdate()
    {
        if (lastframecharge)
        {
            chargingAIAttackFixedUpdate();
        }
        else if (unleashed)
        {
            unleashedFixedUpdate();
        }
    }

    void Update()
    {
        if (charge)
        {
            lastframecharge = true;
            charge = false;      // if input key is still pressed then next frame charge will be true again
            if (firsttime)
            {
                transform.localScale = new Vector3(1f, 1f);
                rend.enabled = true;
                tempdamage = damage;
                firsttime = false;
            }
        }
        else if (lastframecharge)   //this will be checked if charge is false, aka if input key is stopped being pressed
        {
            if (!firsttime)
            {
                unleashAttack();
                time = 0f;
                //maxcontrol = Reach / speed * (1f / (transform.localScale.x * transform.localScale.x + 1) + 0.5f);
                coll.enabled = firsttime = unleashed = true;
                lastframecharge = false;
                resetCoolDown();
            }
        }
    }

    void LateUpdate()
    {
        if (!unleashed)
        {
            chargingAIAttackLateUpdate();
        }
        gotInputThisFrame = false;
    }



    void chargingAIAttackFixedUpdate()
    {
        if (transform.localScale.x < maxsize)
        {
            transform.localScale += new Vector3(shrinkrate * Time.fixedDeltaTime, shrinkrate * Time.fixedDeltaTime);
        }
    }
    void chargingAIAttackLateUpdate()
    {
        transform.position = movars.position + movars.transform.localScale.magnitude * 0.2f * direction;
        if (!gotInputThisFrame)
        {
            coll.enabled = rend.enabled = enabled = charge = false;
        }
    }

    void unleashedFixedUpdate()
    {
        transform.position += movement * Time.fixedDeltaTime;
        if (time > maxtime)
        {
            coll.enabled = rend.enabled = enabled = unleashed = false;
        }
        time += Time.fixedDeltaTime;
    }

    void chargingAIAttackUpdate()
    {

    }

    void unleashedUpdate()
    {

    }

    public void unleashAttack()
    {
        unleashed = true;
        coll.enabled = true;
        maxtime = Reach / speed;
        movement = speed * direction;

    }

    public void blocked()
    {
        time = maxtime;
    }

    public Attack getAttack()
    {
        return this;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        aboutcollisions collparameters = collision.GetComponent<aboutcollisions>();

        rules.blockattack(this, collparameters, blocked);

        if (rules.collisiondamage(this, collparameters, tempdamage * transform.localScale.magnitude) && !canPassThroughBodies)
        {
            blocked();
        }
    }

    protected override void initiateAttack()
    {
        direction = vars.directionVector;
        if (!enabled)
        {
            firsttime = enabled = true;
        }
        charge = true;
        gotInputThisFrame = true;
    }

    protected override void disableAttack()
    {
        time = maxtime;
        charge = false;
    }

    public override bool activate(bool input)
    {
        if (input && !unleashed && checkcooldown())
        {
            initiateAttack();
            return true;
        }
        else
        {
            return charge = false;
        }
    }







}
