using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireParticle : MonoBehaviour
{

    private FireAttack _fireAttack;

    private const float _MAX_TIME_ON_AIR = 2f;

    private Vector3 _v;
    private Coroutine _coroutineTimer;

    private Vector3 _angleTemp;


    protected void Awake()
    {
        _fireAttack = GetComponentInParent<FireAttack>();
        gameObject.SetActive(false);
    }

    protected void OnEnable()
    {
        _v = new Vector3(Random.Range(FireAttack._ORIGINAL_HEIGHT / 0.3f, FireAttack._ORIGINAL_HEIGHT / 0.6f), Random.Range(-FireAttack._ORIGINAL_HALFWIDTH / 0.6f, FireAttack._ORIGINAL_HALFWIDTH / 0.6f));
        _v = MyMathlib.RotateVector(_v, _angleTemp = MyMathlib.PolarVectorDeg(transform.parent.rotation.eulerAngles.z)) + _fireAttack.MoveComponent.Velocity;
        transform.position = MyMathlib.RotateVector(Vector3.right, _angleTemp) + transform.parent.position;

        _coroutineTimer = this.DoActionInTime(() => gameObject.SetActive(false), _MAX_TIME_ON_AIR);
    }
    private void LateUpdate()
    {
        transform.position += _v * Time.deltaTime;
        if (_fireAttack.Checkborders(MyMathlib.RotateVector(transform.position - transform.parent.position, MyMathlib.Conjugatevect(_angleTemp)) / 1.35f))
        {
            gameObject.SetActive(false);
            StopCoroutine(_coroutineTimer);
        }
        transform.localScale = new Vector3(1f, 1f);
    }

}
