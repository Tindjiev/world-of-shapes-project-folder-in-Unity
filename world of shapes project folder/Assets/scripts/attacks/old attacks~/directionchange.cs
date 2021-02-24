using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class directionchange : Attack,Projectile
{


    //rc classes
    SpriteRenderer rend;
    CircleCollider2D coll;
    TargetInterface targetgui = null;

    public Sprite preSpiked;
    public Sprite Spiked;


    //control variables
    bool changed;
    float control;
    float maxcontrol;
    Vector3 v;
    float tempdamage;

    //stats
    public float damage = 7f;
    public float speed = 20f;
    public float size = 2f;



    protected new void Awake()
    {
        base.Awake();
        (coll = GetComponent<CircleCollider2D>()).enabled = false;
        (rend = GetComponent<SpriteRenderer>()).enabled = false;
        preSpiked = colorlib.GetSpriteColored(vars.teamcolor, preSpiked, colorlib.colortochange);
        Spiked = colorlib.GetSpriteColored(vars.teamcolor, Spiked, colorlib.colortochange);
        rend.sprite = preSpiked;
        cooldownTimer = 2.5f;

        if (vars.gameObject == globalvariables.playergameobject)
        {
            targetgui = somefunctions.InstantiatePrefabTr("gui/targetinterface", transform).GetComponent<TargetInterface>();
            targetgui.setCentre(transform);
            targetgui.gameObject.SetActive(false);
        }
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
        transform.position += v * Time.fixedDeltaTime;
        if (control > maxcontrol)
        {
            disableAttack();
        }
        control += Time.fixedDeltaTime;
    }


    /*
                 int nmax = (int)(maxcontrol / Time.fixedDeltaTime);
            v += 2f * Reach / (nmax * Time.fixedDeltaTime * (nmax - 1)) * direction;
            control++;
            if (control > maxcontrol / Time.fixedDeltaTime)
            {
                rend.enabled = coll.enabled = enabled = changed = false;
                resetCoolDown();
            }
         */



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


        if (changed)
        {
            rules.collisiondamage(this, collparameters, tempdamage);
        }
    }



    protected override void initiateAttack()
    {
        v = speed / 5f * vars.directionVector;
        transform.position = movars.position;
        tempdamage = damage;
        changed = false;
        maxcontrol = 3f;
        control = 0f;
        rend.sprite = preSpiked;
        coll.radius = 0.3f * 12f / 22f;

        if (targetgui != null)
        {
            targetgui.gameObject.SetActive(true);
            targetgui.transform.localScale = new Vector3(0.15f, 0.15f);
        }

        coll.enabled = true;
        rend.enabled = true;
        enabled = true;
    }


    protected override void disableAttack()
    {
        rend.enabled = coll.enabled = enabled = false;
        resetCoolDown();
        if (!changed)
        {
            changed = false;
            if (targetgui != null)
            {
                targetgui.gameObject.SetActive(false);
            }
        }
    }

    public override bool activate(bool input)
    {
        if (input && checkcooldown())
        {
            if (!enabled)
            {
                initiateAttack();
                return true;
            }
            else if (!changed)
            {
                v = speed * (vars.targetPosition - transform.position).normalized;
                maxcontrol = Reach / speed;
                control = 0f;
                rend.sprite = Spiked;
                coll.radius = 0.3f;

                if (targetgui != null)
                {
                    targetgui.gameObject.SetActive(false);
                }
                return changed = true;
            }
        }
        return false;
    }
    

}
