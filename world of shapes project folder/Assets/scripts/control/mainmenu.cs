using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainmenu : ControlBase
{
    MenuBackground mb;

    public static InputStruct Escape = new InputStruct(Input.GetKeyDown, KeyCode.Escape, new KeyCode[] { KeyCode.Backspace });

    protected new void Awake()
    {
        base.Awake();
    }


    protected new void Start()
    {
        base.Start();
        this.DoActionInNextFrame(_setCamera);
        mb = gameObject.AddComponent<MenuBackground>();
        _textStyle = new GUIStyle();
        _textStyle.normal.textColor = Color.red;
        _textStyle.alignment = TextAnchor.MiddleCenter;

        CameraScript.showPlayerInterface = true;


        SceneChanger.SetSceneToSwitchTo(SceneChanger.SwitchToMenu);
    }

    private void _setCamera()
    {
        MyCameraLib.SetCameraPosition(Vector2.zero);
        Camera.main.orthographicSize = 30f;
    }


    protected new void OnDestroy()
    {
        Destroy(mb);
        base.OnDestroy();
    }

    public void IntializeHome()
    {
        SceneChanger.SwtichToHome();
    }
    public void IntializeAstar()
    {
        SceneChanger.LoadScene("Astar test");
    }
    public void IntializeTrianglesPit()
    {
        SceneChanger.SwitchToTrianglesPit();
    }
    public void IntializeFortress()
    {
        SceneChanger.SwitchToFortress();
    }
    public void IntializeLightsOut()
    {
        destroythis();
        gameObject.AddComponent<lightsOutTest>();
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

}
