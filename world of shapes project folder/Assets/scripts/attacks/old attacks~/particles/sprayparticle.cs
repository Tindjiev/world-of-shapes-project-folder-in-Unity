using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sprayparticle : MonoBehaviour,Projectile
{
    //class variables
    lifescript lifevars;
    sprayattack sprayvars;

    //rc classes
    SpriteRenderer rend;
    Collider2D coll;

    //control variables
    float time;
    float maxtime;
    Vector3 movement;
    float damage;
    bool firsttime;


    protected void Awake()
    {
        lifevars = this.getvars<lifescript>();
        sprayvars = this.getvars<sprayattack>();

        rend = GetComponent<SpriteRenderer>();
        coll = GetComponent<Collider2D>();

        gameObject.SetActive(false);
    }

    protected void Start()
    {
        rend.sprite = colorlib.GetSpriteColored(this.getvarsteam().teamcolor, rend.sprite, colorlib.colortochange);
    }

    void FixedUpdate()
    {
        if (time > 0f)
        {
            if (firsttime)
            {
                firsttime = false;
                coll.enabled = rend.enabled = true;
                transform.position = sprayvars.movars.position;
            }
            transform.position += movement * Time.fixedDeltaTime;
            if (time > maxtime)
            {
                ProjectileStop();
            }
        }
        time += Time.fixedDeltaTime;
    }

    protected void OnEnable()
    {
        firsttime = true;
        coll.enabled = rend.enabled = false;

        float angle = sprayvars.angledirection + Random.Range(-sprayvars.AngleSpreadRAD, sprayvars.AngleSpreadRAD);
        movement = mathlib.polarvectrad(sprayvars.speed, angle) + sprayvars.movars.movement;
        transform.eulerAngles = new Vector3(0, 0, angle * (float)mathlib.radtodeg);

        maxtime = sprayvars.Reach / sprayvars.speed;
        time = Random.Range(-maxtime * 0.75f, 0); //wait a bit so the fire rate can be uniform and not all come out at the same time
        transform.position = sprayvars.movars.position;

        damage = sprayvars.damage;
    }


    public void ProjectileStop()
    {
        gameObject.SetActive(false);
        coll.enabled = rend.enabled = false;
    }

    public void blocked()
    {
        ProjectileStop();
    }

    public Attack getAttack()
    {
        return sprayvars;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {

        aboutcollisions collparameters = collision.GetComponent<aboutcollisions>();

        rules.blockattack(this, collparameters, blocked);

        rules.collisiondamage(sprayvars, collparameters, damage);

    }
}