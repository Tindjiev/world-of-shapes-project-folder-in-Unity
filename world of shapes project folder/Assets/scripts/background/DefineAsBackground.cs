using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefineAsBackground : MonoBehaviour
{
    void Awake()
    {
        Background.backgroundField = GetComponent<SpriteRenderer>();
        Destroy(this);
    }

}
