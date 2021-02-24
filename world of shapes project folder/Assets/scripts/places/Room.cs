using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Room : EntityBase, IEnumerable<Room>, IEnumerable<Door>, IEnumerable<Room.Wall>
{
    public static string DoorType = "phase";
    protected SpriteRenderer[] _rends;
    [SerializeField]
    protected BoxCollider2D _visionCollider;
    [SerializeField]
    protected VisionOfRoom _vision;
    public bool RendsEnabled { get; private set; } = true;
    private int _lastChildCount = 0;

    public override Vector3 Position => transform.position;

    [SerializeField]
    private Color _color = new Color32(140, 69, 0, 255);
    public Color Color
    {
        get => _color;
        set
        {
            _color = value;
            foreach (Wall wall in (IEnumerable<Wall>)this)
            {
                wall.ColorWall(value);
            }
        }
    }


    public enum Side
    {
        Invalid = -1,
        North = 0,
        East = 1,
        South = 2,
        West = 3,
    }

    public Room this[int index] => _walls[index].Neighbor;
    public Room this[Side index] => _walls[(int)index].Neighbor;

    [System.Serializable]
    public class Wall
    {
        [SerializeField]
        private Side _side = Side.Invalid;
        [SerializeField]
        private Room _room = null;

        public Side Side => _side;
        public Room Room => _room;

        [SerializeField]
        private Transform _block1 = null, _block2 = null;

        public Door Door;

        public bool HasDoor => !_noDoor;

        private Vector3 _block1Pos = Vector3.zero, _block1Scale = Vector3.zero;

        public Wall(Side side, Room room)
        {
            _side = side;
            _room = room;
        }

        public void SetNewHeight(float newHeight)
        {
            switch (Side)
            {
                case Side.North:
                    _block1.localPosition = new Vector3(_block1.localPosition.x, newHeight / 2f, _block1.localPosition.z);
                    _block2.localPosition = new Vector3(_block2.localPosition.x, newHeight / 2f, _block2.localPosition.z);
                    _block1Pos.y = newHeight / 2f;
                    Door.transform.localPosition = new Vector3(Door.transform.localPosition.x, newHeight / 2f, Door.transform.localPosition.z);
                    break;
                case Side.South:
                    _block1.localPosition = new Vector3(_block1.localPosition.x, newHeight / -2f, _block1.localPosition.z);
                    _block2.localPosition = new Vector3(_block2.localPosition.x, newHeight / -2f, _block2.localPosition.z);
                    _block1Pos.y = newHeight / -2f;
                    Door.transform.localPosition = new Vector3(Door.transform.localPosition.x, newHeight / -2f, Door.transform.localPosition.z);
                    break;
                default:
                    newHeight += WallFatness;
                    if (_noDoor)
                    {
                        _block1.localScale = new Vector3(_block1.localScale.x, newHeight, _block1.localScale.z);
                        _block2.localScale = new Vector3(_block2.localScale.x, _block1Scale.y = (newHeight - DoorWidth) / 2f, _block2.localScale.z);
                        _block2.localPosition = new Vector3(_block2.localPosition.x, (newHeight - _block1Scale.y) / -2f, _block2.localPosition.z);
                        _block1Pos.y = (newHeight - _block1Scale.y) / 2f;
                    }
                    else
                    {
                        _block1.localScale = new Vector3(_block1.localScale.x, (newHeight - DoorWidth) / 2f, _block1.localScale.z);
                        _block1.localPosition = new Vector3(_block1.localPosition.x, (newHeight - _block1.localScale.y) / 2f, _block1.localPosition.z);
                        _block2.localScale = new Vector3(_block2.localScale.x, (newHeight - DoorWidth) / 2f, _block2.localScale.z);
                        _block2.localPosition = new Vector3(_block2.localPosition.x, (newHeight - _block2.localScale.y) / -2f, _block2.localPosition.z);
                    }
                    break;
            }
        }

        public void SetNewWidth(float newWidth)
        {
            switch (Side)
            {
                case Side.East:
                    _block1.localPosition = new Vector3(newWidth / 2f, _block1.localPosition.y, _block1.localPosition.z);
                    _block2.localPosition = new Vector3(newWidth / 2f, _block2.localPosition.y, _block2.localPosition.z);
                    _block1Pos.x = newWidth / 2f;
                    Door.transform.localPosition = new Vector3(newWidth / 2f, Door.transform.localPosition.y, Door.transform.localPosition.z);
                    break;
                case Side.West:
                    _block1.localPosition = new Vector3(newWidth / -2f, _block1.localPosition.y, _block1.localPosition.z);
                    _block2.localPosition = new Vector3(newWidth / -2f, _block2.localPosition.y, _block2.localPosition.z);
                    _block1Pos.x = newWidth / -2f;
                    Door.transform.localPosition = new Vector3(newWidth / -2f, Door.transform.localPosition.y, Door.transform.localPosition.z);
                    break;
                default:
                    newWidth += WallFatness;
                    if (_noDoor)
                    {
                        _block1.localScale = new Vector3(newWidth, _block1.localScale.y, _block1.localScale.z);
                        _block2.localScale = new Vector3(_block1Scale.x = (newWidth - DoorWidth) / 2f, _block2.localScale.y, _block2.localScale.z);
                        _block2.localPosition = new Vector3((newWidth - _block1Scale.x) / -2f, _block2.localPosition.y, _block2.localPosition.z);
                        _block1Pos.x = (newWidth - _block1Scale.x) / 2f;
                    }
                    else
                    {
                        _block1.localScale = new Vector3((newWidth - DoorWidth) / 2f, _block1.localScale.y, _block1.localScale.z);
                        _block1.localPosition = new Vector3((newWidth - _block1.localScale.x) / -2f, _block1.localPosition.y, _block1.localPosition.z);
                        _block2.localScale = new Vector3((newWidth - DoorWidth) / 2f, _block2.localScale.y, _block2.localScale.z);
                        _block2.localPosition = new Vector3((newWidth - _block2.localScale.x) / 2f, _block2.localPosition.y, _block2.localPosition.z);
                    }
                    break;
            }
        }

        public void DoorCreate()
        {
            if (HasDoor) return;
            _noDoor = false;
            Door.gameObject.SetActive(true);
            _block2.gameObject.SetActive(true);
            _block1.localScale = _block1Pos;
            _block1.localPosition = _block1Scale;
        }

        public void DoorCreate(Room neighbor)
        {
            DoorCreate();
            Neighbor = neighbor;
        }

        public void DoorDestroy()
        {
            if (!HasDoor) return;
            _noDoor = true; // necessary to also avoid loops
            if (Neighbor != null)
            {
                Neighbor._walls[(int)Side.OppositeSide()].DoorDestroy();
            }
            Door.gameObject.SetActive(false);
            _block2.gameObject.SetActive(false);

            _block1Pos = _block1.localScale;
            _block1Scale = _block1.localPosition;
            switch (Side)
            {
                case Side.North:
                    _block1.localScale = new Vector3(Room.Width + WallFatness, WallFatness);
                    _block1.localPosition = new Vector3(0f, Room.Height / 2f);
                    break;
                case Side.East:
                    _block1.localScale = new Vector3(WallFatness, Room.Height + WallFatness);
                    _block1.localPosition = new Vector3(Room.Width / 2f, 0f);
                    break;
                case Side.South:
                    _block1.localScale = new Vector3(Room.Width + WallFatness, WallFatness);
                    _block1.localPosition = new Vector3(0f, Room.Height / -2f);
                    break;
                case Side.West:
                    _block1.localScale = new Vector3(WallFatness, Room.Height + WallFatness);
                    _block1.localPosition = new Vector3(Room.Width / -2f, 0f);
                    break;
            }
        }

        public Room Neighbor;

        public bool HasNeighbor => HasDoor && Neighbor != null && Neighbor.HasNeighborDontCheckOpposite(Side.OppositeSide());

        public bool DoorActuallyOpen => HasDoor && Door.ActuallyOpen;

        public Wall NeighborWall => Neighbor._walls[(int)_side.OppositeSide()];
        public Door NeighborDoor => Neighbor._walls[(int)_side.OppositeSide()].Door;

        public void ColorWall(Color newColor)
        {
            _block1.GetComponent<SpriteRenderer>().color = newColor;
            if (_block2 != null)
            {
                _block2.GetComponent<SpriteRenderer>().color = newColor;
            }
        }


        [SerializeField, ReadOnlyOnInspector]
        private bool _noDoor = false;

        public void FixHasDoor()
        {
            _noDoor = !_block2.gameObject.activeSelf;
        }
    }


    public static float WallFatness = 10f;
    public static float DoorWidth = 20f;


    [field: SerializeField]
    protected Wall[] _walls { get; private set; } = new Wall[4];

    #region WidthHeight

    [SerializeField, HideInInspector]
    private float _height = 50f, _width = 50f;
    public float Height
    {
        get => _height;
        protected set
        {
            _height = value;
            foreach (Wall wall in _walls)
            {
                wall.SetNewHeight(value);
            }
            _visionCollider.size = new Vector2(_visionCollider.size.x, value - WallFatness - 1f);
        }
    }

    public float Width
    {
        get => _width;
        set
        {
            _width = value;
            foreach (Wall wall in _walls)
            {
                wall.SetNewWidth(value);
            }
            _visionCollider.size = new Vector2(value - WallFatness - 1f, _visionCollider.size.y);
        }
    }
    public float WidthInner => Width - WallFatness / 2f;
    public float WidthOuter => Width + WallFatness / 2f;
    public float HeightInner => Height - WallFatness / 2f;
    public float HeightOuter => Height + WallFatness / 2f;

    #endregion


    protected void Awake()
    {
        SetAsSkinTransform();
        enabled = false;
    }

    void GetAndSetRenderers()
    {
        _rends = GetComponentsInChildren<SpriteRenderer>();
    }

    protected void Start()
    {
        _vision.ReinstateActivation();
        GetAndSetRenderers();
        _lastChildCount = transform.childCount;
    }

    void Update()
    {
        if (transform.childCount != _lastChildCount)
        {
            GetAndSetRenderers();
        }
        _lastChildCount = transform.childCount;
        float x, y;
        x = transform.position.x - MyCameraLib.Camera.transform.position.x;
        y = transform.position.y - MyCameraLib.Camera.transform.position.y;
        if ((System.Math.Abs(x - y) + System.Math.Abs(x + y) > 10f * Camera.main.orthographicSize) == RendsEnabled)
        {
            RendsEnabled = !RendsEnabled;
            for (int i = 0; i < _rends.Length; i++)
            {
                _rends[i].enabled = RendsEnabled;
            }
        }
    }

    public bool CheckIfDoorExist(Side side)
    {
        return _walls[(int)side].HasDoor;
    }

    public bool CheckIfDoorActuallyOpen(Side side)
    {
        return _walls[(int)side].DoorActuallyOpen;
    }

    #region RoomCreate

    public void CreateRoom(float width, float height, bool[] doorexistance, Room[] neighbors, GameObject prefab)
    {
        Height = height;
        Width = width;
        if (doorexistance.Length > (int)Side.North && neighbors.Length > (int)Side.North && doorexistance[(int)Side.North])
        {
            if (neighbors[(int)Side.North] != null)
            {
                MakeWallWithDoor(Side.North, neighbors[(int)Side.North]);
                neighbors[(int)Side.North].MakeWallWithDoor(Side.South, this);
            }
            else
            {
                neighbors[(int)Side.North] = Room.CreateRoom(prefab, transform.parent, new Vector3(0f, height + WallFatness) + transform.position, width, height, new bool[] { false, false, true, false }, new Room[] { null, null, this, null });
                MakeNorthWallWithDoor(neighbors[(int)Side.North]);
            }
        }
        else
        {
            MakeNorthwallnodoor();
        }
        if (doorexistance.Length > (int)Side.East && neighbors.Length > (int)Side.East && doorexistance[(int)Side.East])
        {
            if (neighbors[(int)Side.East] != null)
            {
                MakeWallWithDoor(Side.East, neighbors[(int)Side.East]);
                neighbors[(int)Side.East].MakeWallWithDoor(Side.West, this);
            }
            else
            {
                neighbors[(int)Side.East] = Room.CreateRoom(prefab, transform.parent, new Vector3(width + WallFatness, 0f) + transform.position, width, height, new bool[] { false, false, false, true }, new Room[] { null, null, null, this });
                MakeEastWallWithDoor(neighbors[(int)Side.East]);
            }
        }
        else
        {
            MakeEastwallnodoor();
        }
        if (doorexistance.Length > (int)Side.South && neighbors.Length > (int)Side.South && doorexistance[(int)Side.South])
        {
            if (neighbors[(int)Side.South] != null)
            {
                MakeWallWithDoor(Side.South, neighbors[(int)Side.South]);
                neighbors[(int)Side.South].MakeWallWithDoor(Side.North, this);
            }
            else
            {
                neighbors[(int)Side.South] = Room.CreateRoom(prefab, transform.parent, new Vector3(0f, height + WallFatness) + transform.position, width, height, new bool[] { true, false, false, false }, new Room[] { this, null, null, null });
                MakeSouthWallWithDoor(neighbors[(int)Side.South]);
            }
        }
        else
        {
            MakeSouthwallnodoor();
        }
        if (doorexistance.Length > (int)Side.West && neighbors.Length > (int)Side.West && doorexistance[(int)Side.West])
        {
            if (neighbors[(int)Side.West] != null)
            {
                MakeWallWithDoor(Side.West, neighbors[(int)Side.West]);
                neighbors[(int)Side.West].MakeWallWithDoor(Side.East, this);
            }
            else
            {
                neighbors[(int)Side.West] = Room.CreateRoom(prefab, transform.parent, new Vector3(width + WallFatness, 0f) + transform.position, width, height, new bool[] { false, true, false, false }, new Room[] { null, this, null, null });
                MakeWestWallWithDoor(neighbors[(int)Side.West]);
            }
        }
        else
        {
            MakeWestwallnodoor();
        }

    }


    public void CreateRoom(float width, float height, bool[] doorexistance)
    {
        Height = height;
        Width = width;
        if (doorexistance == null)
        {
            doorexistance = new bool[0];
        }
        if (doorexistance.Length > (int)Side.North && doorexistance[(int)Side.North])
        {
            MakeNorthWallWithDoor();
        }
        else
        {
            MakeNorthwallnodoor();
        }
        if (doorexistance.Length > (int)Side.East && doorexistance[(int)Side.East])
        {
            MakeEastWallWithDoor();
        }
        else
        {
            MakeEastwallnodoor();
        }
        if (doorexistance.Length > (int)Side.South && doorexistance[(int)Side.South])
        {
            MakeSouthWallWithDoor();
        }
        else
        {
            MakeSouthwallnodoor();
        }
        if (doorexistance.Length > (int)Side.West && doorexistance[(int)Side.West])
        {
            MakeWestWallWithDoor();
        }
        else
        {
            MakeWestwallnodoor();
        }

    }

    #endregion

    public bool HasNeighbor(Side side) => _walls[(int)side].HasNeighbor;
    private bool HasNeighborDontCheckOpposite(Side side) => _walls[(int)side].HasDoor && _walls[(int)side].Neighbor != null;

    public void AddNewNeighbors(params Room[] neighbors)
    {
        for (int side = 0; side < neighbors.Length; ++side)
        {
            if (neighbors[side] != null)
            {
                if (_walls[side].Neighbor != null)
                {
                    if (_walls[side].Neighbor != neighbors[side])
                    {
                        throw new System.Exception("this already has a neighbor on this side");
                    }
                    else
                    {
                        return;
                    }
                }
                MakeWallWithDoor((Side)side, neighbors[side]);
                neighbors[side].MakeWallWithDoor(((Side)side).OppositeSide(), this);
            }
        }
    }

    public void SetNeighbor(Room neighbor, Side side, bool onNeighborToo = true)
    {
        _walls[(int)side].Neighbor = neighbor;
        if (onNeighborToo) _walls[(int)side].NeighborWall.Neighbor = this;
    }

    public void FixWallsHasDoor()
    {
        foreach(var wall in _walls)
        {
            wall.FixHasDoor();
        }
    }

    #region ChangeDoorExistance

    public void ChangeDoorExistance(Side side, bool makedoor, ref Room newneighbor, GameObject prefab)
    {
        if (makedoor)
        {
            if (newneighbor != null)
            {
                MakeWallWithDoor(side, newneighbor);
                newneighbor.MakeWallWithDoor(side.OppositeSide(), this); //( x&3 ) == ( X%4 )
            }
            else
            {
                MakeWallWithDoor(side, newneighbor);
                side = side.OppositeSide();
                newneighbor = Room.CreateRoom(prefab, transform.parent, new Vector3(Width + WallFatness, 0f) + transform.position, Height, Width, new bool[] { side == Side.North, side == Side.East, side == Side.South, side == Side.West }, new Room[] { this, this, this, this });
            }
        }
        else
        {
            if (_walls[(int)side].Neighbor != null)
            {
                _walls[(int)side].Neighbor.MakeWallNoDoor(side.OppositeSide());
            }
            MakeWallNoDoor(side);
        }
    }
    public void ChangeDoorExistance(Side side, bool makedoor)
    {
        if (makedoor)
        {
            if (_walls[(int)side].Neighbor != null)
                _walls[(int)side].Neighbor.MakeWallWithDoor(side.OppositeSide());
            MakeWallWithDoor(side);
        }
        else
        {
            if (_walls[(int)side].Neighbor != null)
                _walls[(int)side].Neighbor.MakeWallNoDoor(side.OppositeSide());
            MakeWallNoDoor(side);
        }
    }


    public void ChangeDoorExistance(Side side)
    {
        if (!CheckIfDoorExist(side))
        {
            MakeWallWithDoor(side);
        }
        else
        {
            MakeWallWithDoor(side);
        }
    }

    public void ChangeDoorExistance(Side side, Room newneighbor)
    {
        if (!CheckIfDoorExist(side))
        {
            MakeWallWithDoor(side, newneighbor);
        }
        else
        {
            MakeWallNoDoor(side);
        }
    }

    #endregion

    #region MakeWall

    private void MakeWallWithDoor(Side side)
    {
        switch (side)
        {
            case Side.North:
                MakeNorthWallWithDoor();
                break;
            case Side.East:
                MakeEastWallWithDoor();
                break;
            case Side.South:
                MakeSouthWallWithDoor();
                break;
            case Side.West:
                MakeWestWallWithDoor();
                break;
        }
    }
    private void MakeWallWithDoor(Side side, Room neighbor)
    {
        switch (side)
        {
            case Side.North:
                MakeNorthWallWithDoor(neighbor);
                break;
            case Side.East:
                MakeEastWallWithDoor(neighbor);
                break;
            case Side.South:
                MakeSouthWallWithDoor(neighbor);
                break;
            case Side.West:
                MakeWestWallWithDoor(neighbor);
                break;
        }
    }

    private void MakeNorthWallWithDoor()
    {
        _walls[(int)Side.North].DoorCreate();
    }

    private void MakeEastWallWithDoor()
    {
        _walls[(int)Side.East].DoorCreate();
    }

    private void MakeSouthWallWithDoor()
    {
        _walls[(int)Side.South].DoorCreate();
    }

    private void MakeWestWallWithDoor()
    {
        _walls[(int)Side.West].DoorCreate();
    }

    private void MakeNorthWallWithDoor(Room neighbor)
    {
        _walls[(int)Side.North].DoorCreate(neighbor);
    }

    private void MakeEastWallWithDoor(Room neighbor)
    {
        _walls[(int)Side.East].DoorCreate(neighbor);
    }

    private void MakeSouthWallWithDoor(Room neighbor)
    {
        _walls[(int)Side.South].DoorCreate(neighbor);
    }

    private void MakeWestWallWithDoor(Room neighbor)
    {
        _walls[(int)Side.West].DoorCreate(neighbor);
    }

    private void MakeWallNoDoor(Side side)
    {

        switch (side)
        {
            case Side.North:
                MakeNorthwallnodoor();
                break;
            case Side.East:
                MakeEastwallnodoor();
                break;
            case Side.South:
                MakeSouthwallnodoor();
                break;
            case Side.West:
                MakeWestwallnodoor();
                break;
        }
    }

    private void MakeNorthwallnodoor()
    {
        _walls[(int)Side.North].DoorDestroy();
    }

    private void MakeEastwallnodoor()
    {
        _walls[(int)Side.East].DoorDestroy();
    }

    private void MakeSouthwallnodoor()
    {
        _walls[(int)Side.South].DoorDestroy();
    }

    private void MakeWestwallnodoor()
    {
        _walls[(int)Side.West].DoorDestroy();
    }

    #endregion

    #region OpenCloseDoor

    public void CloseAllDoors()
    {
        CloseNorthDoor();
        CloseEastDoor();
        CloseSouthDoor();
        CloseWestDoor();
    }
    public void OpenAllDoors()
    {
        OpenNorthDoor();
        OpenEastDoor();
        OpenSouthDoor();
        OpenWestDoor();
    }


    public void CloseNorthDoor()
    {
        CloseDoor(Side.North);
    }
    public void CloseEastDoor()
    {
        CloseDoor(Side.East);
    }
    public void CloseSouthDoor()
    {
        CloseDoor(Side.South);
    }
    public void CloseWestDoor()
    {
        CloseDoor(Side.West);
    }

    public void OpenNorthDoor()
    {
        OpenDoor(Side.North);
    }
    public void OpenEastDoor()
    {
        OpenDoor(Side.East);
    }
    public void OpenSouthDoor()
    {
        OpenDoor(Side.South);
    }
    public void OpenWestDoor()
    {
        OpenDoor(Side.West);
    }



    public void CloseDoor(Side side)
    {
        if (CheckIfDoorExist(side))
        {
            CloseDoorDontCheck(side);
        }
    }

    public void OpenDoor(Side side)
    {
        if (CheckIfDoorExist(side))
        {
            OpenDoorDontCheck(side);
        }
    }

    private void CloseDoorDontCheck(Side side)
    {
        Wall wall = _walls[(int)side];
        wall.Door.CommandClose();
        if (wall.Neighbor != null)
        {
            wall.NeighborDoor.CommandClose();
        }
    }

    private void OpenDoorDontCheck(Side side)
    {
        Wall wall = _walls[(int)side];
        wall.Door.CommandOpen();
        if (wall.Neighbor != null)
        {
            wall.NeighborDoor.CommandOpen();
        }
    }


    public void CloseDoor(Side[] sides)
    {
        for (int i = 0; i < sides.Length; i++)
        {
            CloseDoor(sides[i]);
        }
    }

    public void OpenDoor(Side[] sides)
    {
        for (int i = 0; i < sides.Length; i++)
        {
            OpenDoor(sides[i]);
        }
    }

    public void ChangeDoorState(Side side)
    {
        ChangeDoorState(side, !_walls[(int)side].Door.Closed);
    }

    public void ChangeDoorState(Side side, bool openDoor)
    {
        if (CheckIfDoorExist(side))
        {
            if (openDoor)
            {
                OpenDoorDontCheck(side);
            }
            else
            {
                CloseDoorDontCheck(side);
            }
        }
    }
    public void ChangeDoorState(Side[] sides)
    {
        for (int i = 0; i < sides.Length; i++)
        {
            ChangeDoorState(sides[i]);
        }
    }

    public void ChangeDoorState(bool[] doorsInfo)
    {
        for (int i = 0; i < doorsInfo.Length; i++)
        {
            ChangeDoorState((Side)i, doorsInfo[i]);
        }
    }

    /* public void ChangeDoorState(short opendoors)
     {
         for (int i = 0; i < 4; i++)
         {
             ChangeDoorState(3 - i, (opendoors & (1 << i)) != 0);
         }
     }*/

    #endregion

    #region OpenCloseDoorImmediate

    private void CloseDoorDontCheckImmediate(Side side)
    {
        Wall wall = _walls[(int)side];
        wall.Door.CloseImmediate();
        if (wall.HasNeighbor)
        {
            wall.NeighborDoor.CloseImmediate();
        }
    }

    private void OpenDoorDontCheckImmediate(Side side)
    {
        Wall wall = _walls[(int)side];
        wall.Door.OpenImmediate();
        if (wall.HasNeighbor)
        {
            wall.NeighborDoor.OpenImmediate();
        }
    }

    public void CloseDoorImmediate(Side side)
    {
        if (CheckIfDoorExist(side)) CloseDoorDontCheckImmediate(side);
    }

    public void OpenDoorImmediate(Side side)
    {
        if (CheckIfDoorExist(side)) OpenDoorDontCheckImmediate(side);
    }
    public void ChangeDoorStateImmediate(Side side, bool openDoor)
    {
        if (CheckIfDoorExist(side))
        {
            if (openDoor)
            {
                OpenDoorDontCheckImmediate(side);
            }
            else
            {
                CloseDoorDontCheckImmediate(side);
            }
        }
    }

    public void ChangeDoorStateImmediate(bool openDoor)
    {
        ChangeDoorStateImmediate(new bool[] { openDoor, openDoor, openDoor, openDoor });
    }


    public void ChangeDoorStateImmediate(bool[] doorsInfo)
    {
        for (int i = 0; i < doorsInfo.Length; i++)
        {
            ChangeDoorStateImmediate((Side)i, doorsInfo[i]);
        }
    }

    #endregion

    public bool IsNeighboring(Room room)
    {
        foreach (Room Neighbor in this)
        {
            if (room == Neighbor) return true;
        }
        return false;
    }

    public bool IsNeighboringWithOpenDoor(Room room)
    {
        foreach (var wall in (IEnumerable<Wall>)this)
        {
            if (wall.Neighbor == room && wall.DoorActuallyOpen) return true;
        }
        return false;
    }


    protected void OnDestroy()
    {
        foreach (Wall wall in _walls)
        {
            if (wall != null) wall.DoorDestroy();
        }
    }

    #region StaticRoomCreate

    public static Room CreateRoom(GameObject prefab, Transform parent, Vector3 position, float w, float h, bool[] doorExistance, Room[] neighbors)
    {
        Room room = BasicLib.MyInstantiatePrefab(prefab, prefab.name, parent).GetComponent<Room>();
        room.transform.position = position;
        room.CreateRoom(w, h, doorExistance, neighbors, prefab);
        return room;
    }


    public static Room CreateRoom(GameObject prefab, Transform parent, Vector3 position, float w, float h, bool[] doorExistance)
    {
        Room room = BasicLib.MyInstantiatePrefab(prefab, prefab.name, parent).GetComponent<Room>();
        room.transform.position = position;
        room.CreateRoom(w, h, doorExistance);
        return room;
    }

    #endregion

    #region EdgesInnerOuterMiddle

    public Rect Rect => new Rect(Position, Size);

    public Vector2 Size => new Vector2(Width, Height);

    public Vector3 NorthSideCentre => transform.position + new Vector3(0f, Height / 2f);
    public Vector3 EastSideCentre => transform.position + new Vector3(Width / 2f, 0f);
    public Vector3 SouthSideCentre => transform.position - new Vector3(0f, Height / 2f);
    public Vector3 WestSideCentre => transform.position - new Vector3(Width / 2f, 0f);

    public Vector3 NorthSideInner => transform.position + new Vector3(0f, (Height - WallFatness) / 2f);
    public Vector3 EastSideInner => transform.position + new Vector3((Width - WallFatness) / 2f, 0f);
    public Vector3 SouthSideInner => transform.position - new Vector3(0f, (Height - WallFatness) / 2f);
    public Vector3 WestSideInner => transform.position - new Vector3((Width - WallFatness) / 2f, 0f);

    public Vector3 NorthSideOuter => transform.position + new Vector3(0f, (Height + WallFatness) / 2f);
    public Vector3 EastSideOuter => transform.position + new Vector3((Width + WallFatness) / 2f, 0f);
    public Vector3 SouthSideOuter => transform.position - new Vector3(0f, (Height + WallFatness) / 2f);
    public Vector3 WestSideOuter => transform.position - new Vector3((Width + WallFatness) / 2f, 0f);

    public const float inner = -1f;
    public const float centre = 0f;
    public const float outer = 1f;
    public Vector3 getSidePos(Side side, float inner_outer)
    {
        Vector3 add;
        switch (side)
        {
            case Side.North:
                add = new Vector3(0f, (Height + inner_outer * WallFatness) / 2f);
                break;
            case Side.East:
                add = new Vector3((Width + inner_outer * WallFatness) / 2f, 0f);
                break;
            case Side.South:
                add = new Vector3(0f, (Height + inner_outer * WallFatness) / -2f);
                break;
            case Side.West:
                add = new Vector3((Width + inner_outer * WallFatness) / -2f, 0f);
                break;
            default:
                add = Vector3.zero;
                break;
        }
        return transform.position + add;
    }


    public Vector3 ProportionalPosition(Vector2 xy)
    {
        return ProportionalPosition(xy.x, xy.y);
    }

    public Vector3 ProportionalPosition(float x, float y) // pos + x*(east-pos) + y*(north-pos)
    {
        return EastSideInner * x + NorthSideInner * y - Position * (x + y - 1f);
    }

    public bool WithinRoom(Vector3 pos)
    {
        pos -= Position;
        pos.x /= Width - WallFatness;
        pos.y /= Height - WallFatness;
        return System.Math.Abs(pos.x - pos.y) + System.Math.Abs(pos.x + pos.y) <= 1f;
    }

    #endregion

    #region VisionStuff
    public override bool CanAddToSeen(CollisionInfo target)
    {
        return true;
    }

    public void EnableVision() => _vision.Activate();

    public bool ObjectInRoomVisible(CollisionInfo target) => _vision.HasSeen(target);


    #endregion

    #region Enumerators

    public IEnumerator<Room> GetEnumerator()
    {
        if (_walls[0].HasNeighbor)
        {
            yield return _walls[0].Neighbor;
        }
        if (_walls[1].HasNeighbor)
        {
            yield return _walls[1].Neighbor;
        }
        if (_walls[2].HasNeighbor)
        {
            yield return _walls[2].Neighbor;
        }
        if (_walls[3].HasNeighbor)
        {
            yield return _walls[3].Neighbor;
        }
    }


    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    IEnumerator<Door> IEnumerable<Door>.GetEnumerator()
    {
        if (_walls[0].HasDoor)
        {
            yield return _walls[0].Door;
        }
        if (_walls[1].HasDoor)
        {
            yield return _walls[1].Door;
        }
        if (_walls[2].HasDoor)
        {
            yield return _walls[2].Door;
        }
        if (_walls[3].HasDoor)
        {
            yield return _walls[3].Door;
        }
    }

    IEnumerator<Wall> IEnumerable<Wall>.GetEnumerator()
    {
        yield return _walls[0];
        yield return _walls[1];
        yield return _walls[2];
        yield return _walls[3];
    }
    #endregion

    #region CustomEditor



#if UNITY_EDITOR


    [CustomEditor(typeof(Room), true), CanEditMultipleObjects]
    private class RoomEditor : ExtendedEditor
    {
        private static bool _showWallsOnInspector = true;
        private const string _WALLS_PROPERTY_NAME = "<" + nameof(_walls) + ">k__BackingField";

        protected override void OnInspectorGUIExtend(Object currentTarget)
        {
            DrawPropertiesExcept(_WALLS_PROPERTY_NAME, nameof(_color));
            var room = (Room)target;
            DrawWidthHeight(room);
            DrawColor(room);
            DrawWalls();


        }

        private void DrawWidthHeight(Room room)
        {
            var temp = room.Width;
            AddNewToList(EditorGUILayout.FloatField("Width", temp), temp);
            temp = room.Height;
            AddNewToList(EditorGUILayout.FloatField("Height", temp), temp);
        }

        private void DrawColor(Room room)
        {
            var temp = room.Color;
            AddNewToList(EditorGUILayout.ColorField("Color of room", room.Color), temp);
        }


        private void DrawWalls()
        {
            _showWallsOnInspector = EditorGUILayout.Foldout(_showWallsOnInspector, "Walls");
            if (_showWallsOnInspector)
            {
                EditorGUI.indentLevel++;
                var walls = serializedObject.FindProperty(_WALLS_PROPERTY_NAME);
                walls.NextVisible(true);
                walls.NextVisible(false);
                EditorGUILayout.PropertyField(walls, new GUIContent("North Wall"), true);
                DrawOpenCloseButtons(Side.North);
                walls.NextVisible(false);
                EditorGUILayout.PropertyField(walls, new GUIContent("East Wall"), true);
                DrawOpenCloseButtons(Side.East);
                walls.NextVisible(false);
                EditorGUILayout.PropertyField(walls, new GUIContent("South Wall"), true);
                DrawOpenCloseButtons(Side.South);
                walls.NextVisible(false);
                EditorGUILayout.PropertyField(walls, new GUIContent("West Wall"), true);
                DrawOpenCloseButtons(Side.West);
                EditorGUI.indentLevel--;
            }
        }

        private void DrawOpenCloseButtons(Side side)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Open", GUILayout.Width(Screen.width >> 1)))
            {
                foreach (var roomObject in targets)
                {
                    ((Room)roomObject).OpenDoorImmediate(side);
                }
            }
            if (GUILayout.Button("Close", GUILayout.Width(Screen.width >> 1)))
            {
                foreach (var roomObject in targets)
                {
                    ((Room)roomObject).CloseDoorImmediate(side);
                }
            }
            GUILayout.EndHorizontal();
        }

        protected override void ApplyChanges(Object currentTarget)
        {
            var room = (Room)currentTarget;
            if (_changedList[0].Changed)
            {
                room.Width = (float)_changedList[0].Value;
            }
            if (_changedList[1].Changed)
            {
                room.Height = (float)_changedList[1].Value;
            }
            if (_changedList[2].Changed)
            {
                room.Color = (Color)_changedList[2].Value;
            }
        }
    }


#endif

    #endregion



    public static Vector3 PickPositionInRoomDoorSide(Room CurrentRoom, Room NextRoom)
    {
        float rng = DoorWidth * (Random.value - 0.5f) * 0.65f;
        if (CurrentRoom[0] == NextRoom)
        {
            return new Vector3(CurrentRoom.Position.x + rng, CurrentRoom.Position.y + (CurrentRoom.Height / 2f + Room.WallFatness));
        }
        else if (CurrentRoom[1] == NextRoom)
        {
            return new Vector3(CurrentRoom.Position.x + (CurrentRoom.Width / 2f + Room.WallFatness), CurrentRoom.Position.y + rng);
        }
        else if (CurrentRoom[2] == NextRoom)
        {
            return new Vector3(CurrentRoom.Position.x + rng, CurrentRoom.Position.y - (CurrentRoom.Height / 2f + Room.WallFatness));
        }
        else if (CurrentRoom[3] == NextRoom)
        {
            return new Vector3(CurrentRoom.Position.x - (CurrentRoom.Width / 2f + Room.WallFatness), CurrentRoom.Position.y + rng);
        }
        else
        {
            return CurrentRoom.Position;
        }
    }



    public static Side FindSide(Vector3 currpos_sub_centre, float heigh_div_width)
    {
        if (currpos_sub_centre.y > heigh_div_width * currpos_sub_centre.x)
        {
            if (currpos_sub_centre.y > -heigh_div_width * currpos_sub_centre.x)
            {
                return Side.North;
            }
            else
            {
                return Side.West;
            }
        }
        else
        {
            if (currpos_sub_centre.y > -heigh_div_width * currpos_sub_centre.x)
            {
                return Side.East;
            }
            else
            {
                return Side.South;
            }
        }
    }


}

public static class SideExtensionMethods
{
    public static Room.Side OppositeSide(this Room.Side side) => (Room.Side)(((int)side + 2) & 3);
}