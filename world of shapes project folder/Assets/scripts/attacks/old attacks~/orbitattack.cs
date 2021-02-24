using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class orbitattack : Attack
{


    //control variables
    public bool orbitthrown = false;
    public float radius;
    public float angledirection;
    int lastnum = 0;
    float lastfixedframerate;
    public Vector3 tempdirection;
    public Vector3 anglediff_between_particles_Vector;
    public Vector3 Directionrotatespeed;
    public Vector3 centre;
    float time;
    float maxtime;

    public double dwDEG
    {
        get
        {
            return anglespeedDEG * Time.fixedDeltaTime;
        }
    }

    //stats
    public int numberOfParticles = 15;
    public float damage = 3f;
    public float speed = 30f;
    public float anglespeedDEG = 120f;
    public float maxradius = 1.5f;
    public float particleSize = 1f;

    protected new void Awake()
    {
        base.Awake();

        check_n_setparticlesnumber(numberOfParticles);
    }

    protected new void Start()
    {
        base.Start();
    }

    public override bool inputfunction(KeyCode key)
    {
        return Input.GetKeyDown(key);
    }

    void LateUpdate()
    {
        if (!checkactivechildren())
        {
            disableAttack();
        }
        else if (!orbitthrown)
        {
            centre = movars.position;
            maxtime = Reach / speed;
            time = 0f;
        }
        if (lastfixedframerate != Time.fixedDeltaTime)
        {
            Directionrotatespeed = mathlib.polarvectdeg(dwDEG);
            lastfixedframerate = Time.fixedDeltaTime;
            Debug.Break();
            Debug.Log("huh...");
        }
    }

    void FixedUpdate()
    {
        if (!orbitthrown)
        {
            if (radius < maxradius)
            {
                radius += 10f * Time.fixedDeltaTime;
            }
            else
            {
                radius = maxradius;
            }
        }
        else
        {
            if (time > maxtime)
            {
                orbitthrown = false;
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
            }
            time += Time.fixedDeltaTime;
            centre += speed * tempdirection * Time.fixedDeltaTime;
        }
    }




    void setparticlesnumber(int num)
    {
        if (transform.childCount != 0)
        {
            if (num > transform.childCount)
            {
                Transform temp = transform.GetChild(0);
                for (int i = transform.childCount; i < num; i++)
                {
                    Instantiate(temp, transform).name = temp.name;
                }
            }
            else
            {
                for (int i = transform.childCount - 1; i >= num; i--)
                {
                    Destroy(transform.GetChild(i).gameObject);
                }
            }
        }
        else
        {
            Transform temp = Resources.Load<Transform>("Prefabs/attacks/sprayattack").transform.GetChild(0);
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


    bool checkactivechildren() //return false if all children are inactive
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf)
            {
                return true;
            }
        }
        return false;
    }

    protected override void disableAttack()
    {
        orbitthrown = enabled = false;
    }

    public override bool activate(bool input)
    {
        if (input && !orbitthrown)
        {
            if (enabled)
            {
                if (radius < maxradius)
                {
                    return false;
                }
                tempdirection = vars.directionVector;
                return orbitthrown = true;
            }
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
        enabled = true;
        tempdirection = Vector3.right;
        radius = 0f;
        anglediff_between_particles_Vector = mathlib.polarvectrad(mathlib.TAU / numberOfParticles);
        Directionrotatespeed = mathlib.polarvectdeg(dwDEG);
        lastfixedframerate = Time.fixedDeltaTime;
        check_n_setparticlesnumber(numberOfParticles);
        for (int i = 0; i < numberOfParticles; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }

}
