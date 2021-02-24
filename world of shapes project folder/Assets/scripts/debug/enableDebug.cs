using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enableDebug : MonoBehaviour
{
    InputStruct movementChangeInput;

    protected void Start()
    {
        movementChangeInput = new InputStruct(Input.GetKeyDown, KeyCode.O, new KeyCode[] { KeyCode.P }, 3);
    }


    void Update()
    {
        if (movementChangeInput.CheckInput())
        {
            MoveComponent.ShowMovements = !MoveComponent.ShowMovements;
        }
    }
}
