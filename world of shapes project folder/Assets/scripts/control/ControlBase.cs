using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;
using System;

public abstract class ControlBase : MonoBehaviour
{

    public static GameObject PlayerGameObject = null;

    protected GUIStyle _textStyle;
    public timecount timevars;

    public Action<GameObject> AddcomponentOnDestroy = x => { };

    public MyLib.voidfunction saveProgress = MyLib.DoNothing, loadProgress = MyLib.DoNothing;
    protected void Awake()
    {
        MyColorLib.teams = new Transform[] { 
            GameObject.Find("green").transform,
            GameObject.Find("blue").transform,
            GameObject.Find("red").transform,
            GameObject.Find("yellow").transform,
            GameObject.Find("cyan").transform,
            GameObject.Find("purple").transform,
        };
        //ControlBase.control = gameObject;
    }

    protected void Start()
    {

    }
    protected void OnDestroy()
    {
        AddcomponentOnDestroy(gameObject);
    }
    protected void Update()
    {
        if (mainmenu.Escape.CheckInput())
        {
            SceneChanger.SwitchToSetScene();
        }
    }


    public void destroythis()
    {
        Destroy(this);
        for (int i = 0; i < MyColorLib.teams.Length; i++)
        {
            for (int j = 0; j < MyColorLib.teams[i].childCount; j++)
            {
                Destroy(MyColorLib.teams[i].GetChild(j).gameObject);
            }
        }
        Background.getRidOfAddedBackgrounds();
        //getRidOfThings(EventObjectBaseClass.triggersOutSofar);
    }
    public void destroythis(Action<GameObject> AddcomponentFunction)
    {
        AddcomponentOnDestroy = AddcomponentFunction;
        destroythis();
    }


    public static void getRidOfThings<T>(LinkedList<T> list) where T : Object
    {
        for (LinkedListNode<T> Node = list.First; Node != null; Node = Node.Remove())
        {
            if (Node.Value != null)
            {
                Destroy(Node.Value);
            }
        }
    }
}


public abstract class ControlBaseWithPlayer : ControlBase
{
    private BaseCharacterControl playervars_private;
    public GameObject player { get; private set; }
    public Transform playerTr { get; private set; }
    public MoveComponent playerMoveComponent { get; private set; }
    public BaseCharacterControl playervars
    {
        get
        {
            return playervars_private;
        }
        set
        {
            playervars_private = value;
            playerMoveComponent = playervars_private.MoveComponent;
            player = playerMoveComponent.gameObject;
            playerTr = playerMoveComponent.transform;
        }
    }
    public Vector3 playerPos
    {
        get
        {
            return playerMoveComponent.Position;
        }
    }

    protected new void Awake()
    {
        base.Awake();
    }
    protected new void Start()
    {
        base.Start();
    }

    protected new void Update()
    {
        base.Update();
    }

    protected new void OnDestroy()
    {
        base.OnDestroy();
    }

    public void SetPlayer(MyLib.GameObjectfunction initializePlayerFunction)
    {
        playervars = initializePlayerFunction().SearchComponent<PlayerControlBattle>();
    }

}