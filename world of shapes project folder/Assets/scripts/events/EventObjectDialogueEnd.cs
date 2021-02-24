using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventObjectDialogueEnd : EventObjectBaseClass
{

    public DialogueComponent Dialouge;

    protected override bool CheckToTrigger()
    {
        return Dialouge == null;
    }

}
