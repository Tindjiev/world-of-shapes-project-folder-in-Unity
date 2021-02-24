using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escape : MonoBehaviour
{

    private void LateUpdate()
    {
        if (mainmenu.Escape.CheckInput())
        {
            SceneChanger.SwitchToSetScene();
        }
    }
}
