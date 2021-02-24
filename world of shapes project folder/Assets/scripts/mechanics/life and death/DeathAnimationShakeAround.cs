using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathAnimationShakeAround : DeathAnimationBase
{
    public const float SECONDS_OF_SHAKING = 2f;
    private float _time, _shakeAmplitude = 0.3f, _wa = 10f;
    private Vector2 _direction;
    private Vector3 _centre;


    protected override void InitateAnimation()
    {
        if (Life.IsLifeOfCharacter)
        {
            _centre = transform.position;
        }
        else
        {
            _centre = _moveComponent.Position;
        }
        _time = 1f;
        _direction = new Vector2(Random.Range(-_shakeAmplitude, _shakeAmplitude), Random.Range(-_shakeAmplitude, _shakeAmplitude));
    }

    void FixedUpdate()
    {
        transform.position = _centre + (_time * _time * _wa < 10f * MyMathlib.TAU ?
             new Vector3(_direction.x * Mathf.Cos(_wa * _time * _time * _time), _direction.y * Mathf.Cos(_wa * _time * _time * _time)) :
             new Vector3(Random.Range(-_shakeAmplitude, _shakeAmplitude), Random.Range(-_shakeAmplitude, _shakeAmplitude)));
        _time += Time.fixedDeltaTime;
        if (_time > SECONDS_OF_SHAKING + 1f) Finish();
    }
}