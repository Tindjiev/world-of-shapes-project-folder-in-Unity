using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class orbitparticle : MonoBehaviour,Projectile
{

    //class variables
    orbitattack orbitvars;

    //rc variables
    Collider2D coll;


    //control variables
    Vector3 direction;
    float damage;

    protected void Awake()
    {
        orbitvars = this.getvars<orbitattack>();
        coll = GetComponent<Collider2D>();
        coll.disable();
        if (!orbitvars.enabled)
        {
            endenable();
        }
    }

    protected void Start()
    {
        
        SpriteRenderer rend = GetComponent<SpriteRenderer>();
        rend.sprite = colorlib.GetSpriteColored(orbitvars.vars.teamcolor, rend.sprite, colorlib.colortochange);
        
    }


    protected void OnEnable()
    {
        transform.position = orbitvars.movars.position + orbitvars.radius * (direction = orbitvars.tempdirection = mathlib.rotatevector(orbitvars.tempdirection, orbitvars.anglediff_between_particles_Vector));
        float size = orbitvars.particleSize;
        transform.localScale = new Vector3(size, size, 1f);
        damage = orbitvars.damage;
        //coll.enabled = rend.enabled = true;
        coll.disable();
    }


    void LateUpdate()
    {
        if (!coll.enabled && orbitvars.radius == orbitvars.maxradius)
        {
            coll.enable();
        }
        if (!orbitvars.orbitthrown)
        {
            transform.position = orbitvars.movars.position + orbitvars.radius * (direction = mathlib.rotatevector(direction, orbitvars.Directionrotatespeed));
        }
        else
        {
            transform.position = orbitvars.centre + orbitvars.radius * (direction = mathlib.rotatevector(direction, orbitvars.Directionrotatespeed));
        }
    }

    void endenable()
    {
        gameObject.SetActive(false);
    }



    public void blocked()
    {
        endenable();
    }

    public Attack getAttack()
    {
        return orbitvars;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        aboutcollisions collparameters = collision.GetComponent<aboutcollisions>();

        rules.blockattack(this, collparameters, blocked);

        if (rules.collisiondamage(orbitvars, collparameters, damage))
        {
            endenable();
        }
    }
}
