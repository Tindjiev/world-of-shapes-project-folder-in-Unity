using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UltEvents;

public abstract class EventObjectBaseClass : MonoBehaviour
{

    public bool Triggered { get; private set; } = false;
    public bool Once = true;

    [SerializeField]
    private UltEvent _triggerFunction;

    public GameObject[] GameObjectsToAffect;

    [SerializeField]
    private float _timeToActivate;


    private bool _triggering = false;

    protected void Awake()
    {
        if (_triggerFunction == null)
        {
            _triggerFunction = new UltEvent();
        }
    }
    protected void LateUpdate()
    {
        if (!_triggering && CheckToTrigger())
        {
            Trigger();
        }
    }

    public void Trigger()
    {
        _triggering = true;
        if (_timeToActivate == 0f)
        {
            DoTriggers();
        }
        else
        {
            this.DoActionInTime(DoTriggers, _timeToActivate);
        }
    }

    public void Trigger(float delay)
    {
        SetDelay(delay);
        Trigger();
    }


    private void DoTriggers()
    {
        Triggered = true;
        _triggerFunction?.Invoke();
        if (Once) Destroy(this);
        else _triggering = false;
    }

    public void AddAction(System.Action action)
    {
        _triggerFunction.AddPersistentCall(action);
    }

    protected abstract bool CheckToTrigger();


    public void SetDelay(float time)
    {
        _timeToActivate = time;
    }
}

public static class EventExtensions
{

    public static EventObjectType AddEventComponent<EventObjectType>(this GameObject gameObject) where EventObjectType : EventObjectBaseClass
    {
        EventObjectType eventTemp = gameObject.AddComponent<EventObjectType>();
        return eventTemp;
    }

    public static EventObjectType AddEventComponent<EventObjectType>(this GameObject gameObject, float secondsDelay) where EventObjectType : EventObjectBaseClass
    {
        EventObjectType eventTemp = gameObject.AddEventComponent<EventObjectType>();
        eventTemp.SetDelay(secondsDelay);
        return eventTemp;
    }



    public static EventObjectType AddEventComponent<EventObjectType>(this Component component) where EventObjectType : EventObjectBaseClass
    {
        return component.gameObject.AddEventComponent<EventObjectType>();
    }
    public static EventObjectType AddEventComponent<EventObjectType>(this Component component, float secondsDelay) where EventObjectType : EventObjectBaseClass
    {
        return component.gameObject.AddEventComponent<EventObjectType>(secondsDelay);
    }




}

public class EventObjectNull : EventObjectBaseClass
{

    protected new void Awake()
    {
        base.Awake();
        enabled = false;
    }

    protected override bool CheckToTrigger()
    {
        return false;
    }

}