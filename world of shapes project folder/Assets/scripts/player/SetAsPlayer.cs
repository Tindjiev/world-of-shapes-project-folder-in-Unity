using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAsPlayer : MonoBehaviour
{
    void Awake()
    {
        ControlBase.PlayerGameObject = gameObject;
        Destroy(this);
    }
}
