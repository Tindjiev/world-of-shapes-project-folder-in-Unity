using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventObjectLightsOut : EventObjectBaseClass
{

    public LightsOutScript LightsOut;

    protected override bool CheckToTrigger()
    {
        return LightsOut.Solved;
    }

}
