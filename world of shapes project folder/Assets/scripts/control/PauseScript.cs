using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScript : MonoBehaviour
{

    InputStruct pauseInput = new InputStruct(Input.GetKeyDown, KeyCode.P, new KeyCode[] { KeyCode.RightShift });
    public static bool paused = false;
    GameObject Root;
    float timeOfPauseStart;
    protected void Start()
    {
        Root = GameObject.Find("ToPause");
    }

    private void pause()
    {
        timeOfPauseStart = Time.time;
        paused = true;
        Root.SetActive(false);
    }

    private void unpause()
    {
        Timer._timePaused += Time.time - timeOfPauseStart;
        paused = false;
        Root.SetActive(true);
    }

    void Update()
    {
        if (pauseInput.CheckInput())
        {
            if (paused)
            {
                unpause();
            }
            else
            {
                pause();
            }
        }
    }
}
