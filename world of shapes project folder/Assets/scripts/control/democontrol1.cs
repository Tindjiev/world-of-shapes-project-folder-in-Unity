using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class democontrol1 : ControlBaseWithPlayer
{

    private Transform[] _chargers;
    private int _enemiesNum = 0;

    private GUIStyle _hitTextStyle = new GUIStyle();


    private Action _update;
    private Action _updateGUI;

    private Room _room;

    private Transform _playerTeam, _enemyTeam;

    protected new void Awake()
    {
        base.Awake();
        AddcomponentOnDestroy = x => x.AddComponent<mainmenu>();
    }

    protected new void Start()
    {
        base.Start();

        _room = gameObject.AddComponent<Room>();

        _chargers = new Transform[0];

        MyCameraLib.SetCameraPosition(_room.Position);

        _textStyle = new GUIStyle();
        _textStyle.normal.textColor = Color.black;
        _textStyle.alignment = TextAnchor.MiddleCenter;

        _update = pregameupdate;
        _hitTextStyle.alignment = TextAnchor.MiddleCenter;
        _hitTextStyle.normal.textColor = Color.yellow;

        _updateGUI = pregameupdategui;

        _playerTeam = MyColorLib.teams[MyColorLib.green].transform;
        _enemyTeam = GameObject.Find("purple").GetTeam().transform;
    }


    protected new void Update()
    {
        base.Update();
        _update();
    }

    


    private void OnGUI()
    {
        _updateGUI();
    }

    void pregameupdate()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                _enemiesNum -= 10;
            }
            else if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                _enemiesNum += 10;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                _enemiesNum--;
            }
            else if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                _enemiesNum++;
            }
        }
        if (_enemiesNum < 0)
        {
            _enemiesNum = 0;
        }

        if (Input.GetKey(KeyCode.B) && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            SetUpGame();
            _update = GameUpdate;
            _updateGUI = GameUpdateGUI;
        }
    }

    void pregameupdategui()
    {

        _textStyle.fontSize = Screen.width * 2 / 100;
        GUI.Label(new Rect(Screen.width >> 1, Screen.height * 0.25f, 0f, 0f), string.Format("Press Backspace + Esc to go back whenever\n" +
                                                                                            "put 0 enemies to learn and test the controls\nleft click to increase enemies, right click to decrease\n" +
                                                                                            "adds/removes 10 enemies with shift pressed\n press B + enter when done"
                                                                                            ), _textStyle);




        _textStyle.fontSize = Screen.width * 4 / 100;
        GUI.Label(new Rect(Screen.width >> 1, Screen.height >> 1, 0f, 0f), string.Format("enemies: {0}", _enemiesNum), _textStyle);



    }

    private void SetUpGame()
    {
        if (_room == null || (_room = FindObjectOfType<Room>()) == null)
        {
            _room = Floor.CreateSingleRoom(gameObject, 100f, 100f);
        }
        AddInEditor.CreatePlayer(_playerTeam);
        ControlBase.PlayerGameObject.GetComponentInChildren<LifeComponent>().AddActionOnDeath(SceneChanger.SwitchToSetScene);
        ControlBase.PlayerGameObject.GetComponentInChildren<LifeComponent>().Health = 100f;
        ControlBase.PlayerGameObject.GetCharacter().AddAttacks();

        MyCameraLib.SetCameraFollow(ControlBase.PlayerGameObject.SearchComponentTransform<MoveComponent>(), CameraScript._REGULAR_ORTHOGRAPHIC_SIZE);


        _chargers = new Transform[_enemiesNum];
        for (int i = 0; i < _enemiesNum; i++)
        {
            _chargers[i] = AddInEditor.MobCreate.Charger(_enemyTeam).transform;
            _chargers[i].gameObject.SetActive(false);
            _chargers[i].SearchComponent<LifeComponent>().AddActionOnDeath(_chargers[i].SearchComponent<TriangleChargerAI>().DeathSetInactive);
            _chargers[i].gameObject.ReplaceVision<VisionOthers>().AddVision(_room.GetComponentInChildren<VisionOfRoom>(true), _room);
        }


        _hitTextStyle.alignment = TextAnchor.UpperCenter;
        _hitTextStyle.normal.textColor = Color.red;

        timevars = gameObject.AddComponent<timecount>();




    }



    private void GameUpdate()
    {

        for (int i = 0; i < _enemiesNum; i++)
        {
            if (!_chargers[i].gameObject.activeSelf)
            {
                _chargers[i].SearchComponentTransform<MoveComponent>().transform.position = RandomPosition();
                _chargers[i].gameObject.SetActive(true);
            }
        }
    }

    private void GameUpdateGUI()
    {
        if (_enemiesNum == 0)
        {

            _textStyle.fontSize = Screen.width * 2 / 100;
            GUI.Label(new Rect(Screen.width >> 1, Screen.height * 3 / 4, 0f, 0f), string.Format("WASD to move, hold down leftshift to move faster\n" +
                                                                                                "double press leftshit for a small dash in the direction you are MOVING to\n\n" +
                                                                                                "press the corresponding number on your keyboard to switch holding slot\n" +
                                                                                                "scroll to change the weapon of your current holding slot\n" +
                                                                                                "Your weapon holding state is saved\n\n" +
                                                                                                "leftctrl + mouse scroll zooms in/out"), _textStyle);

        }


    }





    protected new void OnDestroy()
    {
        Destroy(_room);
        Destroy(GetComponent<timecount>());
        base.OnDestroy();
    }

    private Vector3 RandomPosition()
    {
        return new Vector3(_room.Width / 2f * SpawnDistribution(), _room.Height / 2f * SpawnDistribution()) + _room.Position;
    }

    private float SpawnDistribution()
    {
        float r = UnityEngine.Random.value - 0.5f;

        return Mathf.Sign(r) * (2f * r * r + 0.5f);

    }


}
