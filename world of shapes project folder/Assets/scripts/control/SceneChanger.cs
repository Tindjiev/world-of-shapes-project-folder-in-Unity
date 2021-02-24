using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public static class SceneChanger
{

    private const string _MENU_NAME = "world of shapes";
    private const string _CHARGING_TRIANGLES_PIT_NAME = "charging triangles pit";
    private const string _CHAT_ROOM_TEST_NAME = "chat room test";
    private const string _FORTRESS = "fortress";
    private const string _HOME = "home";

    private static Stack<Action> _switchToScene = new Stack<Action>();

    public static bool ChangingScene { get; private set; } = false;

    private static Action _onChangeScene = null;
    public static int SceneCount => SceneManager.sceneCountInBuildSettings;

    public static void ReloadCurrentScene()
    {
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public static void SwitchToMenu()
    {
        LoadScene(_MENU_NAME);
    }

    public static void SwitchToTrianglesPit()
    {
        LoadScene(_CHARGING_TRIANGLES_PIT_NAME);
    }

    public static void SwitchToChatTest()
    {
        LoadScene(_CHAT_ROOM_TEST_NAME);
    }

    public static void SwitchToFortress()
    {
        LoadScene(_FORTRESS);
    }

    public static void SwtichToHome()
    {
        LoadScene(_HOME);
    }

    public static void LoadScene(string sceneName)
    {
        ChangingScene = true;
        SceneManager.LoadScene(sceneName);
        ChangingScene = false;
    }

    public static void LoadScene(int sceneIndex)
    {
        ChangingScene = true;
        _onChangeScene();
        _onChangeScene = null;
        SceneManager.LoadScene(sceneIndex);
        ChangingScene = false;
    }



    public static void AddToChangeSceneEvent(Action action)
    {
        _onChangeScene += action;
    }

    public static void SetSceneToSwitchTo(Action switchToScene)
    {
        _switchToScene.Push(switchToScene);
    }

    public static void SwitchToSetScene()
    {
        if (_switchToScene.Count == 0)
        {
#if UNITY_EDITOR
            //throw new Exception("no set scene");
            Debug.Log("no set scene");
            //UnityEngine.Debug.Break();
#endif
            SwitchToMenu();
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log(_switchToScene.Count);
#endif
            _switchToScene.Pop()();
        }
    }



}
