using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chargingBody : Attack
{
    chargingAI AIvars;

    public override bool inputfunction(KeyCode key)
    {
        return false;
    }

    protected new void Awake()
    {
        AIvars = this.getvars<chargingAI>();
    }


    protected override void disableAttack()
    {
        throw new System.NotImplementedException();
    }

    protected override void initiateAttack()
    {
        throw new System.NotImplementedException();
    }

    public override bool activate(bool input)
    {
        throw new System.NotImplementedException();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform == movars.transform)
        {
            return;
        }
        /* if (collision.transform != globalvariables.playergameobject.transform.GetChild(0))
         {
             Debug.Log("hmm", collision.gameObject);
         }*/
        aboutcollisions collparameters = collision.GetComponent<aboutcollisions>();

        if (rules.collisiondamage(this, collparameters, ((chargingAI)AIvars).damage))
        {
            MyAudioSource audio = collision.GetComponent<MyAudioSource>();
            if (audio != null)
            {
                collision.GetComponent<MyAudioSource>().AudioQueue[0].Enqueue(collision.transform.position);
            }
            AIvars.lifevars.life = 0f;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform == movars.transform)
        {
            return;
        }
        Collider2D collider = collision.collider;
        aboutcollisions collparameters = collider.GetComponent<aboutcollisions>();

        if (rules.collisiondamage(this, collparameters, ((chargingAI)vars).damage))
        {
            MyAudioSource audio = collider.GetComponent<MyAudioSource>();
            if (audio != null)
            {
                collider.GetComponent<MyAudioSource>().AudioQueue[0].Enqueue(collision.transform.position);
            }
            vars.lifevars.life = 0f;
        }
    }
}
