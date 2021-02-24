using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class summonBaseClass : Attack
{

    //public GameObject prefab;
    public GameObject summoned;
    public bool isSummoned { get; private set; } = false;


    //stats
    public float maxTimeSummoned = 10f;
    public float StartingHealth = 5f;
    timelib.timer summonedTimer = new timelib.timer();

    private move private_summovars;
    public move summovars
    {
        get
        {
            if (private_summovars == null)
            {
                return private_summovars = summoned.getvars<move>();
            }
            return private_summovars;
        }
    }

    private BaseCharacterControl private_sumvars;
    public BaseCharacterControl sumvars
    {
        get
        {
            if (private_sumvars == null)
            {
                return private_sumvars = summoned.getvars<BaseCharacterControl>();
            }
            return private_sumvars;
        }
    }


    protected new void Awake()
    {
        base.Awake();


        //summoned = Instantiate(prefab, transform);
        summoned = mobcreate.sprayer(Vector3.zero, transform, StartingHealth);
        summoned.SetActive(false);
        sumvars.lifevars.death = () => summoned.SetActive(false);

        summoned.getvars<MoveAround_ChillMode>().setTarget(movars);

        summoned.getvars<aboutcollisions>().setVars(vars);
    }

    protected new void Start()
    {
        base.Start();
    }

    public override bool inputfunction(KeyCode key)
    {
        return Input.GetKeyDown(key);
    }


    void Update()
    {
        BaseUpdate();
    }




    void BaseUpdate()
    {
        if (summonedTimer.checkIfTimePassed() || vars.lifevars.isDying)
        {
            disableAttack();
        }
        if (!summoned.activeInHierarchy)
        {
            attackEnd();
        }
    }


    protected override void initiateAttack()
    {
        summoned.SetActive(true);
        summovars.SetPosition(movars.position);
        sumvars.candamage = vars.candamage;
        sumvars.cantarget = vars.candamage;
        sumvars.lifevars.life = StartingHealth;
        sumvars.getvars<AI>().mode = -1;
        sumvars.getvars<Attack>().enabled = false;
        summonedTimer.startTimerAndSetCounter(maxTimeSummoned);

        enabled = true;
        isSummoned = true;

    }


    protected override void disableAttack()
    {
        //summoned.gameObject.SetActive(false);
        sumvars.lifevars.startdeath();
        attackEnd();
    }

    private void attackEnd()
    {
        resetCoolDown();
        enabled = false;
        isSummoned = false;
    }

    public override bool activate(bool input)
    {
        if(input && !isSummoned && checkcooldown())
        {
            initiateAttack();
            return true;
        }
        return false;
    }






}
