using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleChargerAI : AI
{

    //control
    private bool _canRotate = true;
    private float _a, _distSq = 0f; //needed to communicate between fixed and normal update
    private Vector3 _targSubPosition, _div, _fixedPosition;
    private Transform _fakeTransform;

    public float CurrentSpeed
    {
        get => MoveComponent.CurrentSpeed;
        private set => MoveComponent.SetRatioToBaseSpeed(value / MoveComponent.BaseSpeed);
    }

    //stats
    public float MaxSpeed = 15f, MinSpeed = 1f;
    public float AngularSpeed = 3f;
    public float A = 15f;
    public float Damage = 2f;

    // angle in radians: fps*th=2*pi (1 circle in a second if done each frame)
    private float _dw => AngularSpeed * Time.fixedDeltaTime;

    private Rigidbody2D _rb;
    protected new void Awake()
    {
        base.Awake();
        DirectionVector = Vector3.right;
        Life.Health = 10f;


        _rb = this.SearchComponent<Rigidbody2D>();
    }

    protected new void Start()
    {
        _fixedPosition = MoveComponent.Position;
        this.DoActionInNextFrame(() => Destroy(MoveComponent.ImageBody.gameObject));
        Physics2D.IgnoreCollision(MoveComponent.GetComponent<Collider2D>(), this.SearchComponent<Attack>().GetComponent<Collider2D>());
    }

    private void Update()
    {
        if (TargetTransform == null)
        {
            FindTarget();
            if (TargetTransform == null)
            {
                _fakeTransform = new GameObject().transform;
                TargetTransform = _fakeTransform;
                TargetTransform.position = _fixedPosition;
                TargetTransform.parent = transform;
            }
        }
        else
        {
            if (TargetTransform == _fakeTransform)
            {
                if (_distSq < 1f)
                {
                    MoveComponent.transform.position = TargetTransform.position;
                }
                FindTarget();
            }
            else
            {
                if (!CanTarget(TargetTransform.GetCharacter()))
                {
                    _fakeTransform = new GameObject().transform;
                    TargetTransform = _fakeTransform;
                    TargetTransform.position = _fixedPosition;
                    TargetTransform.parent = transform;
                }
            }/*
            MoveComponent.endpos.Clear();
            MoveComponent.endpos.Enqueue(transform.position + destination);*/
        }
    }

    private void FixedUpdate()
    {
        if (TargetTransform == null)
        {
            return;
        }
        if (MoveComponent.HasFreeMovement)
        {
            /*if (MoveComponent.path.Count != 0)
                transform.GetChild(0).rotation = Quaternion.Euler(0f, 0f, (float)mathlib.anglevectordeg(MoveComponent.path.Peek() - transform.position));*/
            MoveComponent.transform.GetChild(0).rotation = Quaternion.Euler(0f, 0f, DirectionVector.AnlgeDegrees());

            _targSubPosition = TargetTransform.position - MoveComponent.Position;
            _div = MyMathlib.MultiplyComplexConjugate(_targSubPosition, DirectionVector);
            _distSq = _targSubPosition.sqrMagnitude;
            if (_div.x > 1f)
            {
                _a = A;
            }
            else
            {
                _a = -A;
            }
            if (_canRotate)
            {
                DirectionVector.RotateRadians((_div.y > 0 ? _dw : -_dw) * 4f / (CurrentSpeed + 4f));
            }
            if (CurrentSpeed <= MaxSpeed && CurrentSpeed >= MinSpeed)
            {
                CurrentSpeed += _a * Time.fixedDeltaTime;
            }
            else if (CurrentSpeed < MinSpeed)
            {
                CurrentSpeed = MinSpeed;
                DirectionVector = _targSubPosition.normalized;
            }
            else
            {
                CurrentSpeed = MaxSpeed;
            }
            if (_canRotate && Mathf.Abs(_div.y) < 0.1f && _distSq < CurrentSpeed.Sq() * 0.09f)
            {
                _canRotate = false;
            }
            else if (!_canRotate && _div.x < 1f)
            {
                _canRotate = true;
            }
            _rb.velocity = CurrentSpeed * DirectionVector;
            //_rb.MovePosition(MoveComponent.transform.position + CurrentSpeed * Time.fixedDeltaTime * DirectionVector);
        }
        else
        {
            CurrentSpeed = 0;
        }
    }


    private void FindTarget()
    {
        float minDistSq = float.MaxValue;
        BaseCharacterControl closestTarget = null;
        foreach (BaseCharacterControl seenCharacter in (IEnumerable<BaseCharacterControl>)Vision)
        {
            if (seenCharacter.IsDead()) continue;
            var tempDistSq = (seenCharacter.Position - MoveComponent.Position).sqrMagnitude;
            if (tempDistSq < minDistSq && CanTarget(seenCharacter))
            {
                minDistSq = tempDistSq;
                closestTarget = seenCharacter;
            }
        }
        if (closestTarget == null) return;
        if (_fakeTransform != null) Destroy(_fakeTransform.gameObject);
        TargetTransform = closestTarget.MoveComponent.transform;
    }





    public void DeathSetInactive()
    {
        CurrentSpeed = 0f;
        gameObject.SetActive(false);
        Life.Health = 10f;
    }



}
