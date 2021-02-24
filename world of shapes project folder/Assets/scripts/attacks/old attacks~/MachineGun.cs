using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun : Attack,Projectile
{
    //class instances
    private BoxCollider2D coll;
    //private SpriteRenderer rend;
    private Rigidbody2D rb;
    private MachineBullet bulletvars;

    struct CollidedWithInfo
    {
        public float dist;
        public aboutcollisions coll;

        public CollidedWithInfo(float dist, aboutcollisions coll)
        {
            this.dist = dist;
            this.coll = coll;
        }
    }

    private mylib.MyArrayList<CollidedWithInfo> CollidedWith = new mylib.MyArrayList<CollidedWithInfo>(10,3);

    const float colltimeCD = 0.1f;
    private timelib.timer collTimer = new timelib.timer(colltimeCD);

    Vector3 currentReachPosition;

    //control variables
    private bool lastFrameInput = false;
    private float angleDirection;
    private float maxDistance;
    public float currentReach
    {
        get
        {
            //return transform.localScale.x;
            return coll.size.x;
        }
        private set
        {
            //transform.localScale = new Vector3(value, transform.localScale.y, transform.localScale.z);
            coll.size = new Vector2(value, coll.size.y);
            coll.offset = new Vector2(value / 2, 0f);
        }
    }

    //stats
    public float speed;
    public float damagePerSecond;
    public float angleSpeed;

    public float damage
    {
        get
        {
            return damagePerSecond * Time.fixedDeltaTime;
        }
    }

    protected new void Awake()
    {
        base.Awake();
        coll = GetComponent<BoxCollider2D>();
        //rend = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        maxDistance = Reach;
        disableAttack();
        bulletvars = this.getvars<MachineBullet>();
        speed = 130f;
        damagePerSecond = 10f;
        angleSpeed = 500f;
    }

    protected new void Start()
    {
        base.Start();
    }

    public override bool inputfunction(KeyCode key)
    {
        return Input.GetKey(key);
    }

    protected void Update()
    {
        if(!coll.enabled && collTimer.checkIfTimePassed())
        {
            coll.enable();
        }
        if (currentReach > maxDistance)
        {
            currentReach = maxDistance;
        }
        if (!lastFrameInput)
        {
            disableAttack();
        }
        lastFrameInput = false;
    }

    protected void LateUpdate()
    {
        //rb.MoveRotation(Quaternion.Euler(0f, 0f, angleDirection));
        transform.position = movars.position;
    }

    void changeRotation()
    {
        float rbrot = rb.rotation;
        float angleDirectiontemp = angleDirection;
        if (System.Math.Abs(angleDirectiontemp - rbrot) > 180f)
        {
            if (rbrot < 0f)
            {
                rbrot += 360f;
            }
            else
            {
                angleDirectiontemp += 360f;
            }
        }
        if (rbrot > angleDirectiontemp + angleSpeed * Time.fixedDeltaTime)
        {
            rb.rotation -= angleSpeed * Time.fixedDeltaTime;
        }
        else if (rbrot < angleDirectiontemp - angleSpeed * Time.fixedDeltaTime)
        {
            rb.rotation += angleSpeed * Time.fixedDeltaTime;
        }
        else
        {
            rb.rotation = angleDirection;
        }
    }

    protected void FixedUpdate()
    {
        changeRotation();
        if (coll.enabled)
        {
            if (CollidedWith.Count != 0)
            {
                int i = 0;
                maxDistance = GetMinDist(ref i);
                if (maxDistance > Reach)
                {
                    maxDistance = Reach;
                }
                if (CollidedWith[i].coll.lifevars != null)
                {
                    CollidedWith.array[i].coll.lifevars.addDamage(this, damage);
                }
                CollidedWith.Clear();
            }
            else
            {
                maxDistance = Reach;
            }
            //DisableCollider();
        }
        if (currentReach < maxDistance)
        {
            currentReach += speed * Time.fixedDeltaTime;
        }
    }

    float GetMinDist(ref int minIndex)
    {
        float mindist = CollidedWith[0].dist;
        minIndex = 0;
        for (int i = 1; i < CollidedWith.Count; i++)
        {
            if (CollidedWith[i].dist < mindist)
            {
                mindist = CollidedWith[i].dist;
                minIndex = i;
            }
        }
        return mindist;
    }

    void DisableCollider()
    {
        coll.disable();
        collTimer.startTimer();
    }

    protected void OnEnable()
    {
        bulletvars.enable();
    }

    public void blocked()
    {

    }

    public Attack getAttack()
    {
        return this;
    }

    protected override void initiateAttack()
    {
        lastFrameInput = true;
        angleDirection = (float)mathlib.anglevectordeg(vars.directionVector);
        if (!enabled)
        {
            enabled = true;
            //rend.enable();
            currentReach = 0f;
            rb.rotation = angleDirection;
        }
    }
    protected override void disableAttack()
    {
        enabled = false;
        //rend.disable();
        coll.disable();
    }
    public override bool activate(bool input)
    {
        if(input)
        {
            if (checkcooldown())
            {
                initiateAttack();
                return true;
            }
        }
        //disableAttack();
        return false;
    }


    void OnCollisionStay2D(Collision2D collision)
    {
        aboutcollisions collparameters = collision.collider.GetComponent<aboutcollisions>();
        //collision = collision.otherCollider;
        bool addPos = false;
        if (rules.collisiondamage(this, collparameters, 0f))
        {
            addPos = true;
        }
        else if (rules.blockattack(this, collparameters, blocked))
        {
            addPos = true;
            /*Debug.Log(collision.contactCount);
            for (int i = 0; i < collision.contactCount; i++)
            {
                Debug.Log(collision.GetContact(i).point);
            }*/
        }
        if (addPos)
        {
            Vector2 transformpos2d = (Vector2)transform.position;
            Vector2 direction = (Vector2)mathlib.polarvectdeg(angleDirection);
            float mindistsq = (collision.GetContact(0).point - transformpos2d).projectOnNormed(direction).sqrMagnitude;
            for (int i = 1; i < collision.contactCount; i++)
            {
                float tempdistsq = (collision.GetContact(i).point - transformpos2d).projectOnNormed(direction).sqrMagnitude;
                if (tempdistsq < mindistsq)
                {
                    mindistsq = tempdistsq;
                }
            }
            CollidedWith.Add(new CollidedWithInfo((float)System.Math.Sqrt(mindistsq), collparameters));
        }
    }



}
