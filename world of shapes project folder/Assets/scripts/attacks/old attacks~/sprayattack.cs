using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sprayattack : Attack
{

    //class variables
    AudioSource audsourceloop;

    //control variables
    public float angledirection;
    int lastnum = 0;
    bool gotinput;

    //stats
    public int numberOfParticles = 8;
    public float damage = 1.25f;
    public float damageToSelfpersecond = 1f;
    public float MIN_life_for_damageToSelf = 0.4f;
    public float speed = 50f;
    public float AngleSpreadRAD = (float)mathlib.PI / 10f;

    public float damageToSelf
    {
        get
        {
            return damageToSelfpersecond * Time.deltaTime;
        }
    }

    protected new void Awake()
    {
        base.Awake();
        audsourceloop = transform.GetChild(0).GetComponent<AudioSource>();

        check_n_setparticlesnumber(numberOfParticles);
        //        defaultImage = imagelib.SpriteToTexture2D(transform.GetChild(0).GetComponent<SpriteRenderer>().sprite);


    }

    public override bool inputfunction(KeyCode key)
    {
        return Input.GetKey(key);
    }

    void LateUpdate()
    {
        if (vars.lifevars.life > MIN_life_for_damageToSelf)
        {
            vars.lifevars.addDamage(this, damageToSelf);
        }
        if (!gotinput)
        {
            disableAttack();
            return;
        }
        audsourceloop.transform.position = movars.position;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (!transform.GetChild(i).gameObject.activeSelf)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        check_n_setparticlesnumber(numberOfParticles);
        gotinput = false;
    }

    protected void OnDisable()
    {
        audsourceloop.enabled = false;
        gotinput = false;
    }


    void setparticlesnumber(int num)  // num + 1 because of sound gameobject
    {
        if (transform.childCount > 1)
        {
            if (num + 1 > transform.childCount)
            {
                Transform temp = transform.GetChild(1);
                for (int i = transform.childCount; i < num + 1; i++)
                {
                    Instantiate(temp, transform).name = temp.name;
                }
            }
            else
            {
                for (int i = transform.childCount - 1; i >= num + 1; i--)
                {
                    Destroy(transform.GetChild(i).gameObject);
                }
            }
        }
        else
        {
            Transform temp = Resources.Load<Transform>("Prefabs/attacks/sprayattack").transform.GetChild(1);
            for (int i = 0; i < num; i++)
            {
                Instantiate(temp, transform).name = temp.name;
            }
        }
    }


    void check_n_setparticlesnumber(int num)
    {
        if (lastnum != num)
        {
            setparticlesnumber(num);
            lastnum = num;
        }
    }

    protected override void initiateAttack()
    {
        gotinput = true;
        angledirection = (float)mathlib.anglevectorrad(vars.directionVector);
        audsourceloop.enabled = true;
        enabled = true;
    }

    protected override void disableAttack()
    {
        enabled = false;
    }

    public override bool activate(bool inputspray)
    {
        if (inputspray)
        {
            initiateAttack();
            return true;
        }
        else
        {
            return gotinput = false;
        }
    }

}
