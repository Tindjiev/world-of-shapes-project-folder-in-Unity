using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[UnityEngine.CreateAssetMenu(fileName = "Player move inputs default", menuName = "Inputs/Player move")]
public class PlayerMovementInputsSO : ScriptableObject
{

    public InputStruct InputUp = new InputStruct(Input.GetKey, KeyCode.W);
    public InputStruct InputDown = new InputStruct(Input.GetKey, KeyCode.S);
    public InputStruct InputLeft = new InputStruct(Input.GetKey, KeyCode.A);
    public InputStruct InputRight = new InputStruct(Input.GetKey, KeyCode.D);
    public InputStruct InputSprint = new InputStruct(Input.GetKey, KeyCode.LeftShift, KeyCode.RightShift);

}
