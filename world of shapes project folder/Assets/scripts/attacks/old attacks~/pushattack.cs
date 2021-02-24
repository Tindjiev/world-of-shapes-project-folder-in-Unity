using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushAttack : Attack
{

    //rc classes
    public PolygonCollider2D coll; // public is needed for the editor
    SpriteRenderer rend;
    MyAnimator anim;
    MyAudioSource Audio;
    
    //control variables
    //float time;
    float timeneeded;
    Vector3 direction;
    Vector2[] points;
    const float originalheight = 10f;
    const float originalhalfwidth = 4f;
    const int PushAttackSoundIndex = 0;
    const int PushHitSoundIndex = 1;

    //stats
    public float push_distance = 10f;
    public float halfwidth;     //essentially how wide it can push
    public float damage = 1f;


    protected new void Awake()
    {
        base.Awake();
        coll = GetComponent<PolygonCollider2D>();
        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<MyAnimator>();
        Audio = GetComponent<MyAudioSource>();

        coll.enabled = false;
        rend.enabled = false;
        enabled = false;
        points = new Vector2[6];

        //activate = pushplayerinput;
        cooldownTimer = 1f;
    }

    public override bool inputfunction(KeyCode key)
    {
        return Input.GetKeyDown(key);
    }
    void Update()
    {
        if (!coll.enabled)
        {
            //Debug.Break();
            setpoints(points, originalheight / 2f, originalhalfwidth / 2f);
            coll.SetPath(0, points);
            transform.localScale = new Vector3(Reach / originalheight, halfwidth / originalhalfwidth, 1f);
            transform.rotation = Quaternion.Euler(0f, 0f, (float)mathlib.anglevectordeg(direction));

            time = 0f;
            timeneeded = 0.1f;
            transform.position = movars.position;
            rend.enabled = true;
            anim.RenderOnlyIfAnimate = true;
            anim.animate = true;
            coll.enabled = true;

            Audio.AddSoundToQueue(PushAttackSoundIndex, transform.position);

            resetCoolDown();
        }
        else if (time > 1.1f * timeneeded)
        {
            disableAttack();
        }
        else if (time > 0.5f * timeneeded)
        {
            setpoints(points, originalheight, originalhalfwidth);
            coll.SetPath(0, points);
        }
        time += Time.deltaTime;
    }
    

    void OnTriggerEnter2D(Collider2D collision)
    {

        aboutcollisions collparameters = collision.GetComponent<aboutcollisions>();

        if (rules.collisiondamage(this, collparameters, damage))
        {
            move collmovars = collision.getvars<move>();
            if (collmovars != null)
            {
                Audio.AddSoundToQueue(PushHitSoundIndex, collmovars.position);
                collmovars.push(push_distance, ((collision.transform.position - transform.position).normalized + 3f * direction).normalized);
            }
        }
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

    protected override void initiateAttack()
    {
        direction = vars.directionVector;
        enabled = true;

        resetCoolDown();
    }

    protected override void disableAttack()
    {
        coll.enabled = false;
        enabled = false;
    }
    public override bool activate(bool inputpush)
    {
        if (inputpush && checkcooldown())
        {
            initiateAttack();
            return true;
        }
        return false;
    }

    private void SetPointsSmall()
    {
        setpoints(points, originalheight / 2f, originalhalfwidth / 2f);
        coll.SetPath(0, points);
        transform.localScale = new Vector3(Reach / originalheight, halfwidth / originalhalfwidth, 1f);
        transform.rotation = Quaternion.Euler(0f, 0f, (float)mathlib.anglevectordeg(direction));

        timeneeded = 0.1f;
        transform.position = movars.position;
        rend.enabled = true;
        anim.RenderOnlyIfAnimate = true;
        anim.animate = true;
        coll.enabled = true;

        Audio.AddSoundToQueue(PushAttackSoundIndex, transform.position);
    }
    private void SetPointsBig()
    {
        setpoints(points, originalheight, originalhalfwidth);
        coll.SetPath(0, points);
    }


    protected abstract class PushAttackState : AttackState
    {
        protected PushAttack _pushAttack;
        public PushAttackState(AttackStateMachine ASM, PushAttack pushAttack) : base(ASM)
        {
            _pushAttack = pushAttack;
        }

        public override void OnStateEnter()
        {

        }

        public override void OnStateExit()
        {

        }

    }

    protected class WaitingInputForPush : WaitingInput
    {

        public WaitingInputForPush(AttackStateMachine ASM, Attack attack) : base(ASM, attack)
        {
        }
    }

    protected class PushingSmallState : PushAttackState
    {
        protected PushingBigState _pushingBigState;

        public PushingSmallState(AttackStateMachine ASM, PushAttack pushAttack) : base(ASM, pushAttack)
        {
        }

        public override void OnStateEnter()
        {
            _pushAttack.initiateAttack();
            _pushAttack.SetPointsSmall();
            _pushAttack.DoActionInTime(()=> _ASM.ChangeState(_pushingBigState) , 0.5f * _pushAttack.timeneeded);
        }

        public override void LogicalUpdate()
        {
        }

        public override void OnStateExit()
        {
        }
    }

    protected class PushingBigState : PushAttackState
    {
        protected PushingSmallState _pushingSmallState;
        protected WaitingInputForPush _waitingInput;

        public PushingBigState(AttackStateMachine ASM, PushAttack pushAttack) : base(ASM, pushAttack)
        {
        }

        public override void OnStateEnter()
        {
            _pushAttack.SetPointsBig();
            _pushAttack.DoActionInTime(() => _ASM.ChangeState(_waitingInput), 0.6f * _pushAttack.timeneeded);
        }

        public override void LogicalUpdate()
        {
        }

        public override void OnStateExit()
        {
            _pushAttack.disableAttack();
        }
    }


}
