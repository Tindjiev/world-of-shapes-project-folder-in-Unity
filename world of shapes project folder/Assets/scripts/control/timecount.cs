using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timecount : MonoBehaviour
{

    public HMS clock;
    public float starttime;
    GUIStyle timetextstyle = new GUIStyle();

    protected void OnEnable()
    {
        timetextstyle.alignment = TextAnchor.UpperRight;
        timetextstyle.normal.textColor = Color.red;
        starttime = Time.time;
    }



    void OnGUI()
    {
        clock = new HMS(0, 0, Time.time - starttime);
        timetextstyle.fontSize = Screen.width / 100;
        GUI.Label(new Rect(Screen.width, 0f, 0f, 0f), "time: " + clock.ToString()/*,string.Format("time: {0}:{1:00}:{2:00}", clock.hours, clock.minutes, clock.seconds)*/, timetextstyle);
    }





}
