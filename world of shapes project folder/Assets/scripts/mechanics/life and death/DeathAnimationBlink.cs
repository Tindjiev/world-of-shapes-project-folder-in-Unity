using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathAnimationBlink : DeathAnimationBase
{
    public const float TIME_OF_BLINK = 0.8f;
    private float _time;
    public float NumberOfBlinks = 7f;

    private bool _shouldNotBeRendered = false;
    private bool _lastFrameTimeForEnabled = true;

    private float _blinksPerSecond => NumberOfBlinks / TIME_OF_BLINK;

    protected override void InitateAnimation()
    {
        _time = 0f;
        _lastFrameTimeForEnabled = true;
        _shouldNotBeRendered = false;
    }

    private void LateUpdate()
    {
        if(System.Math.Sin(MyMathlib.TAU * _blinksPerSecond * _time) < 0)
        {
            if (_lastFrameTimeForEnabled && !_shouldNotBeRendered && !_rend.enabled)
            {
                _shouldNotBeRendered = true;
            }
            _rend.enabled = false;
            _lastFrameTimeForEnabled = false;
        }
        else
        {
            if(_lastFrameTimeForEnabled && !_rend.enabled)
            {
                _shouldNotBeRendered = true;
            }
            else if (!_shouldNotBeRendered)
            {
                _rend.enabled = true;
            }
            _lastFrameTimeForEnabled = true;
        }
        _time += Time.deltaTime;
        if (_time > TIME_OF_BLINK) Finish();
    }

    protected override void Finish()
    {
        _rend.enabled = true;
        base.Finish();
    }
}
