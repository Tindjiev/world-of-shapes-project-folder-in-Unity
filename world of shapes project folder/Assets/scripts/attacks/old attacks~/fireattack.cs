using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireattack : Attack
{


    //rc classes
    public PolygonCollider2D coll; // public is needed for the editor
    AudioSource audsourceloop;
    public AudioClip firestart; //public is needed here so I can instert audiofile as a prefab in editor

    //control variables
    public bool firing;
    bool chargingAI;
    bool firsttime = true;
    timelib.timer chargingTimer;
    float lastfixedframerate;
    Vector3 DirectionCurrent;
    Vector3 Directionrotatespeed;
    Vector2[] points;
    Transform[] particles = new Transform[0];
    int numberOfParticles = 15;
    public const float originalheight = 4f;
    public const float originalhalfwidth = 2f;

    public double dwDEG
    {
        get
        {
            return anglespeeedDEG * Time.fixedDeltaTime;
        }
    }

    //stats
    public float damagepersecond = 5f;
    public float halfwidth;     //essentially how wide it can Reach
    public float chargeTimeNeeded = 0.8f;
    public double anglespeeedDEG = 120.0;

    public float damage
    {
        get
        {
            return damagepersecond * Time.fixedDeltaTime;
        }
    }


    protected new void Awake()
    {
        base.Awake();
        Reach = originalheight;
        halfwidth = originalhalfwidth;

        coll = GetComponent<PolygonCollider2D>();
        coll.enabled = false;
        points = new Vector2[6];
        audsourceloop = GetComponent<AudioSource>();
        setparticlesnumber(numberOfParticles);

        cooldownTimer = 2f;
        chargingTimer = new timelib.timer(chargeTimeNeeded);
    }

    protected new void Start()
    {
        base.Start();
    }

    public override bool inputfunction(KeyCode key)
    {
        return Input.GetKey(key);
    }

    void LateUpdate()
    {
        if (firing)
        {
            Vector3 DirectionTarget = vars.directionVector;
            transform.position = movars.position;
            firing = false;
            if (firsttime) //initiate attack
            {
                initiateAttack();
            }
            else if (chargingTimer.checkIfTimePassed()) //check time of chargingAI
            {
                chargingAIFinishedAndFireFull();
            }


            followDirection(DirectionTarget);

            for (int i = 0; i < transform.childCount; i++)  //enabling particles
            {
                if (!transform.GetChild(i).gameObject.activeSelf)
                {
                    transform.GetChild(i).gameObject.SetActive(true);
                }
            }
        }
        else if (chargingAI)
        {
            enabled = false;
        }
        else if (coll.enabled) //end attack,  firing==false thus we need to disable collider,audio etc
        {
            disableAttack();
        }
    }


    void OnTriggerStay2D(Collider2D collision)
    {
        aboutcollisions collparameters = collision.GetComponent<aboutcollisions>();

        rules.collisiondamage(this, collparameters, damage);
    }

    void setpoints(Vector2[] points, float h, float w)
    {
        points[0] = new Vector2(h, -w / 3f);
        points[1] = new Vector2(h * 5f / 6f, -w);
        points[2] = new Vector2(0f, -0.5f);
        points[3] = new Vector2(0f, 0.5f);
        points[4] = new Vector2(h * 5f / 6f, w);
        points[5] = new Vector2(h, w / 3f);
    }

    public bool checkborders(Vector3 pos)
    {
        Vector2 points5 = points[5];
        points5.x *= transform.localScale.x;
        points5.y *= transform.localScale.y;
        Vector2 points4 = points[4];
        points4.x *= transform.localScale.x;
        points4.y *= transform.localScale.y;
        return pos.x > Reach ||
            System.Math.Abs(pos.y) > (points5.y - points4.y) / (points5.x - points4.x) * (pos.x - points5.x) + points5.y ||
            System.Math.Abs(pos.y) > pos.x * points4.y / points4.x + 1f;
    }


    public void followDirection(Vector3 DirectionTarget)
    {
        if (lastfixedframerate != Time.fixedDeltaTime)
        {
            Directionrotatespeed = mathlib.polarvectdeg(dwDEG);
            lastfixedframerate = Time.fixedDeltaTime;
            Debug.Break();
            Debug.Log("huh...");
        }
        Vector2 div = mathlib.mulcmplxvectconj(DirectionTarget, DirectionCurrent);
        if (div.y >= 4f * Time.fixedDeltaTime)
        {
            transform.Rotate(0f, 0f, (float)dwDEG);
            DirectionCurrent = mathlib.mulcmplxvect(DirectionCurrent, Directionrotatespeed);
        }
        else if (div.y <= -4f * Time.fixedDeltaTime)
        {
            transform.Rotate(0f, 0f, -(float)dwDEG);
            DirectionCurrent = mathlib.mulcmplxvectconj(DirectionCurrent, Directionrotatespeed);
        }
        else if (div.x > 0f)    // because if target direction is oposite of flamethrow direction then that still means y==0, so we need to avoid that by ensuring x>0
        {
            transform.rotation = Quaternion.Euler(0f, 0f, (float)mathlib.anglevectordeg(DirectionTarget));
            DirectionCurrent = DirectionTarget;
        }
        else        //here flamethrow direction is exact opposite of wanted direction and wouln't move without this case, so I chose to just rotate anti-clockwise in this frame
        {
            transform.Rotate(0f, 0f, (float)dwDEG);
            DirectionCurrent = mathlib.mulcmplxvect(DirectionCurrent, Directionrotatespeed);
        }
    }

    void setparticlesnumber(int num)
    {
        if (transform.childCount != 0 || particles.Length != 0)
        {
            if (num > particles.Length)
            {
                Transform temp;
                if (particles.Length == 0)
                {
                    temp = transform.GetChild(0);
                }
                else
                {
                    temp = particles[0];
                }
                int oldLength = particles.Length;
                System.Array.Resize(ref particles, num);
                for (int i = oldLength; i < num; i++)
                {
                    particles[i] = Instantiate(temp, transform);
                }
            }
            else
            {
                for (int i = particles.Length - 1; i >= num; i--)
                {
                    Destroy(particles[i].gameObject);
                }
                System.Array.Resize(ref particles, num);
            }
        }
        else
        {
            Transform temp = Resources.Load<Transform>("Prefabs/attacks/fireattack").transform.GetChild(0);
            particles = new Transform[num];
            for (int i = particles.Length; i < num; i++)
            {
                particles[i] = Instantiate(temp, transform);
            }
        }
    }

    protected override void initiateAttack()
    {
        firsttime = false;
        chargingAI = true;
        DirectionCurrent = vars.directionVector;
        if (DirectionCurrent == Vector3.zero)
        {
            DirectionCurrent = Vector3.right;
        }
        transform.rotation = Quaternion.Euler(0f, 0f, (float)mathlib.anglevectordeg(DirectionCurrent));
        Directionrotatespeed = mathlib.polarvectdeg(dwDEG);
        lastfixedframerate = Time.fixedDeltaTime;
        setpoints(points, originalheight / 3f, originalhalfwidth / 3f);   //the collider is always set to its original size with the originals and changes by the localscale
        transform.localScale = new Vector3(Reach / originalheight, halfwidth / originalhalfwidth, 1f);
        AudioSource.PlayClipAtPoint(firestart, transform.position, 1f);
        chargingTimer.startTimer();
    }

    protected override void disableAttack()
    {
        coll.enabled = false;
        audsourceloop.enabled = false;
        firsttime = true;
        resetCoolDown();
        enabled = false;
    }

    public override bool activate(bool inputfire)
    {
        if (inputfire && checkcooldown())
        {
            if (enabled)
            {
                return firing = true;
            }
            else
            {
                return firsttime = firing = enabled = true;
            }
        }
        else
        {
            return false;
        }
    }


    void chargingAIFinishedAndFireFull()
    {
        setpoints(points, originalheight, originalhalfwidth);
        coll.SetPath(0, points);
        coll.enabled = true;
        audsourceloop.enabled = true;
        chargingAI = false;
    }


}
