using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;

public class EventObjectCustomCondition : EventObjectBaseClass
{
    [SerializeField]
    private UltEvent _condition;

    private bool _triggered = false;

    protected override bool CheckToTrigger()
    {
        _condition.Invoke();
        return _triggered;
    }

    protected new void Awake()
    {
        base.Awake();
    }

    private void CheckConditionResult(bool conditionResult)
    {
        _triggered = conditionResult;
    }

}
