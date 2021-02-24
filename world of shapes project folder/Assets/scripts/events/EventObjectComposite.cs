using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventObjectComposite : EventObjectBaseClass
{

    public EventObjectBaseClass[] triggers;

    protected new void Awake()
    {
        base.Awake();
        triggers = new EventObjectBaseClass[0];
    }

    protected override bool CheckToTrigger()
    {
        for(int i = 0; i < triggers.Length; i++)
        {
            if (triggers[i] != null && !triggers[i].Triggered)
            {
                return false;
            }
        }
        return true;
    }

}
