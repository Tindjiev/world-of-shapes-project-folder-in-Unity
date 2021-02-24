using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lightsOutTest : ControlBase
{

    private LightsOutScript _puzzle;
    private LightsOutSolver _solver;

    private InputStruct _previousInput;

    protected new void Awake()
    {
        base.Awake();
        AddcomponentOnDestroy = x => x.AddComponent<mainmenu>();
        /*float[,] A = new float[3, 3];
        int k = 0;
        for(int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                A[i, j] = ++k;
            }
        }

        A[2, 0] = 2f;
        A[2, 1] = 3f;
        A[2, 2] = 2f;
        float[] C = BaseLib.leftDivision(A, new float[] { 1f, 0f, 0f });
        float[,] CC = BaseLib.rightDivision(A, A);

        Debug.Log(C.ArrayToString());
        Debug.Log(CC.ArrayToString());
        Debug.Log(A.getInverse().ArrayToString());*/

        /* bool[,] a = new bool[3, 3];
         for(int i = 0; i < 3; i++)
         {
             for (int j = 0; j < 3; j++)
             {
                 a[i, j] = i == j;
             }
         }*/

    }
    protected new void Start()
    {
        base.Start();
        Background.SetBackgroundColor(Color.black);
        _textStyle = new GUIStyle();
        _textStyle.alignment = TextAnchor.MiddleCenter;
        _textStyle.normal.textColor = Color.white;

        _puzzle = gameObject.AddComponent<LightsOutScript>();
        _previousInput = mainmenu.Escape;
        mainmenu.Escape = new InputStruct(KeyCode.Escape);
        _puzzle.Open();
        (_solver = gameObject.AddComponent<LightsOutSolver>()).enabled = false;
        int keypressed = MyInputs.GetNumberPressed(Input.GetKey);
        if (Input.GetKey(KeyCode.P))
        {
            keypressed += 10;
        }
        else if (keypressed < 2)
        {
            keypressed = 10;
        }
        //Debug.Log(keypressed);
        _puzzle.CreateBoard(keypressed, keypressed, Color.red, Color.green);
        LightsOutScript.ActiveDdebug = true;
        LightsOutScript.Square.EdgeSizeRatio = 50f / 1920f;
    }



    protected new void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.S))
        {
            _solver.enabled = !_solver.enabled;
        }
    }

    protected void OnGUI()
    {
        _textStyle.fontSize = Screen.width * 2 / 110;
        GUI.Label(new Rect(Screen.width * 0.75f + 30, Screen.height * 0.25f, 0f, 0f), string.Format("Turn all squares green\nLeft click to switch squares normally\n" +
                                                                                            "Right click to force single square change (cheating)\n" +
                                                                                            "Press S to enable/disable solver\n\n"), _textStyle);
                                                                                            //mainmenu.Escape + " to go back to menu"), _textStyle);

    }

    protected new void OnDestroy()
    {
        Destroy(_puzzle);
        LightsOutScript.ActiveDdebug = false;
        base.OnDestroy();
        mainmenu.Escape = _previousInput;
    }
}
