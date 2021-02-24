using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Side = Room.Side;

public class DemoForAstar : ControlBase
{
    private Floor _floor;

    protected new void Awake()
    {
        base.Awake();
        AddcomponentOnDestroy = x => x.AddComponent<mainmenu>();

        Room.DoorType = "move";
        Room.WallFatness = 10f;
    }

    protected new void Start()
    {
        base.Start();
        foreach(var obj in FindObjectsOfType<Rigidbody2D>())
        {
            Destroy(obj);
        }
        _textStyle = new GUIStyle();
        _textStyle.fontSize = Screen.height * 5 / 100;
        _textStyle.alignment = TextAnchor.MiddleCenter;


        _textStyle.normal.textColor = Color.white;

        if ((_floor = FindObjectOfType<Floor>()) == null)
        {
            Room.DoorType = "move";
            Room.WallFatness = 10f;
            _floor = Floor.CreateFloor(gameObject, (short)(10 * Screen.width / 1920), 10, 50f, 50f);
        }

        MoveComponent.ShowMovements = true;

        Camera.main.orthographicSize = CameraScript.BIRDSEYE_VIEW;
        if (_floor.LengthI.IsEven())
        {
            MyCameraLib.SetCameraPosition(_floor[_floor.LengthI >> 1, _floor.LengthJ - 1].Position - new Vector3(0f, _floor[0, 0].Height / 2f));
        }
        else
        {
            MyCameraLib.SetCameraPosition(_floor[_floor.LengthI >> 1, _floor.LengthJ - 1].Position);
        }

    }

    private bool _chooseStart = true;
    private Room _startRoom = null, _endRoom = null;
    private Room _lastTempRoom = null;
    private Side _lastSide = Side.North;

    protected new void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (_chooseStart)
            {
                Room temproom = _floor.GetClosestRoomFromPosition(MyInputs.GetMousePositionFromscreen);
                if (temproom == _startRoom)
                {
                    _startRoom.Color = _floor.DefaultColor;
                    _startRoom = null;
                }
                else
                {
                    _startRoom = _floor.GetClosestRoomFromPosition(MyInputs.GetMousePositionFromscreen);
                    _chooseStart = false;
                    _floor.MakeAllRoomsDefaultColor();
                    _startRoom.Color = Color.white;
                }
            }
            else
            {
                _endRoom = _floor.GetClosestRoomFromPosition(MyInputs.GetMousePositionFromscreen);
                _startRoom.Color = Color.black;
                if (_startRoom != _endRoom)
                {
                    Findpath();
                    _chooseStart = true;
                }
            }
        }
        else if (Input.GetKey(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Mouse1))
        {
            Room closestroom = _floor.GetClosestRoomFromPosition(MyInputs.GetMousePositionFromscreen);
            Side side = Room.FindSide(MyInputs.GetMousePositionFromscreen - closestroom.Position, 1f);
            if (CheckIfDifferentWall(closestroom, _lastTempRoom, side, _lastSide) || Input.GetKeyDown(KeyCode.Mouse1))
            {
                if (!closestroom.CheckIfDoorActuallyOpen(side))
                {
                    int i = _floor.GetIndexFromRoom(closestroom, 0);
                    int j = _floor.GetIndexFromRoom(closestroom, 1);
                    Room neighbor = null;
                    switch (side)
                    {
                        case Side.North:
                            if (i + 1 < _floor.LengthI)
                            {
                                neighbor = _floor[i + 1, j];
                            }
                            break;
                        case Side.East:
                            if (j + 1 < _floor.LengthJ)
                            {
                                neighbor = _floor[i, j + 1];
                            }
                            break;
                        case Side.South:
                            if (i - 1 >= 0)
                            {
                                neighbor = _floor[i - 1, j];
                            }
                            break;
                        case Side.West:
                            if (j - 1 >= 0)
                            {
                                neighbor = _floor[i, j - 1];
                            }
                            break;
                    }
                    if (neighbor != null) // neighbor != null is when it is not on the side/edge of the floor
                    {
                        closestroom.OpenDoor(side);
                        _floor.MakeAllRoomsDefaultColor();
                        Findpath();
                    }
                }
                else
                {
                    closestroom.CloseDoor(side);
                    _floor.MakeAllRoomsDefaultColor();
                    Findpath();
                }
                _lastTempRoom = closestroom;
                _lastSide = side;
            }
        }
    }

    private bool CheckIfDifferentWall(Room newRoom, Room oldRoom, Side newSide, Side oldSide)
    {
        if (newRoom == null || oldRoom == null)
        {
            return true;
        }
        for (int i = 0; i < 4; i++)
        {
            if (oldRoom[i] == newRoom && (Side)i == oldSide && oldSide.OppositeSide() == newSide)
            {
                return false;
            }
        }
        return newRoom != oldRoom || newSide != oldSide;
    }


    protected void OnGUI()
    {
        _textStyle.fontSize = Screen.width * 2 / 110;
        GUI.Label(new Rect(Screen.width * 0.75f, Screen.height * 0.25f, 0f, 0f), string.Format("Left click to choose start room and goal room\nRight click to block/unblock walls\n" +
                                                                                            "\nGreen: path\nDarker green: diagonal filling\nRed: visited rooms\nYellow: rooms that were pending to be visited\n\n\n" +
                                                                                            mainmenu.Escape + " to go back"), _textStyle);

    }


    private void Findpath()
    {
        if (_startRoom == null || _endRoom == null)
        {
            return;
        }
        _floor.FindRoomPath(_startRoom, _endRoom, !Input.GetKey(KeyCode.LeftShift));
        _startRoom.Color = Color.white;
        _endRoom.Color = Color.black;
    }


    protected new void OnDestroy()
    {
        foreach (Room room in _floor)
        {
            Destroy(room.gameObject);
        }
        Destroy(_floor);
        MoveComponent.ShowMovements = false;
        base.OnDestroy();
    }

}
