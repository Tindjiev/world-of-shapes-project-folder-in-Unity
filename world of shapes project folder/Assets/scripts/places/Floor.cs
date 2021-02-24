using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Side = Room.Side;
using Wall = Room.Wall;

public class Floor : MonoBehaviour, IEnumerable<Room>, ISerializationCallbackReceiver
{

    private Room[,] _rooms;

    public int LengthI => _rooms.GetLength(0);
    public int LengthJ => _rooms.GetLength(1);
    public Room this[int i, int j] => _rooms[i, j];

    public Color DefaultColor;
    [SerializeField]
    private float _mutualWidth, _mutualHeight;

    public byte[] SaveDoors()
    {
        List<byte> doorsStateInfo = new List<byte>(_rooms.Length << 2);
        foreach (Room room in this)
        {
            doorsStateInfo.Add(0);
            int j = 3;
            foreach (Wall wall in (IEnumerable<Wall>)room)
            {
                if (wall.HasDoor)
                {
                    doorsStateInfo[doorsStateInfo.Count - 1] |= (byte)(wall.Door.Closed ? 0 : (1 << j));
                }
                --j;
            }
        }
        return doorsStateInfo.ToArray();
    }

    public void LoadDoors(byte[] doorsStateInfo = null)
    {
        int i = 0;
        foreach (Room room in this)
        {
            int j = 3;
            foreach (Wall wall in (IEnumerable<Wall>)room)
            {
                if (wall.HasDoor)
                {
                    if ((doorsStateInfo[i] & (1 << j)) == 0)
                    {
                        wall.Door.CloseImmediate();
                    }
                    else
                    {
                        wall.Door.OpenImmediate();
                    }
                }
                --j;
            }
            ++i;
        }
    }
    public void MakeAllRoomsDefaultColor()
    {
        foreach (Room room in this) room.Color = DefaultColor;
    }

    public Room GetClosestRoomFromPosition(Vector3 pos)
    {
        int i = GetIndexFromPosition(pos.y, _mutualHeight + Room.WallFatness);
        int j = GetIndexFromPosition(pos.x, _mutualWidth + Room.WallFatness);
        if (!_rooms.CheckOutOfRange(i, j) && _rooms[i, j] != null)
        {
            return _rooms[i, j];
        }
        float mindistsqr = 0f;
        Room minroom = null;
        foreach (Room room in _rooms)
        {
            if (room != null)
            {
                mindistsqr = (pos - room.Position).sqrMagnitude;
                minroom = room;
                break;   // assign distance of the first room
            }
        }
        foreach (Room room in _rooms)
        {
            if (room != null)
            {
                float temp = (pos - room.Position).sqrMagnitude;
                if (temp < mindistsqr)
                {
                    mindistsqr = temp;
                    minroom = room;
                }
            }
        }
        return minroom;
    }
    public Room GetRoomFromPosition(Vector3 pos) //if temp is -0.somthing then i will be 0 and it will pass the condition of the if
    {
        int i = GetIndexFromPosition(pos.y, _mutualHeight + Room.WallFatness);
        int j = GetIndexFromPosition(pos.x, _mutualWidth + Room.WallFatness);
        if (!_rooms.CheckOutOfRange(i, j) && _rooms[i, j] != null)
        {
            return _rooms[i, j];
        }
        return null;
    }

    public int GetIndexFromRoom(Room room)
    {
        int i = GetIndexFromPosition(room.Position.y, _mutualHeight + Room.WallFatness);
        int j = GetIndexFromPosition(room.Position.x, _mutualWidth + Room.WallFatness);
        return i * _rooms.GetLength(1) + j;
    }

    public int GetIndexFromRoom(Room room, int dimension)
    {
        if (dimension == 0)
            return GetIndexFromPosition(room.Position.y, _mutualHeight + Room.WallFatness);
        else if (dimension == 1)
            return GetIndexFromPosition(room.Position.x, _mutualWidth + Room.WallFatness);
        else
            return -1;
    }

    private int GetIndexFromPosition(float yx, float widthHeightPlusWallFatness)
    {
        var temp = (yx + widthHeightPlusWallFatness / 2f) / widthHeightPlusWallFatness;
        return temp < 0f ? 0 : (int)temp;
    }

    private void CreateFloor(short width, short height, float roomwidth, float roomheight, GameObject roomPrefab)
    {
        DefaultColor = new Color(0.55f, 0.27f, 0f);

        _mutualWidth = roomwidth;
        _mutualHeight = roomheight;
        short[][] parameters = new short[][] { new short[] { shape_square, 0, 0, height, width, 0 } };

        CalculateArraySize(parameters);

        addpart(parameters[0], roomwidth, roomheight, roomPrefab);

        FinalCheckForRooms();
    }
    private void CreateFloor(short[][] parameters, float width, float height, GameObject roomPrefab)
    {
        _mutualWidth = width;
        _mutualHeight = height;
        DefaultColor = new Color(0.55f, 0.27f, 0f);

        CalculateArraySize(parameters);

        for (int i = 0; i < parameters.Length; i++)
        {
            addpart(parameters[i], width, height, roomPrefab);
        }

        FinalCheckForRooms();
    }

    private void FinalCheckForRooms()
    {
        CleanNonExistantRooms();
        MakeAllRoomsDefaultColor();
        CleanDoorsOfNonExistantNeighbors();
    }

    private void CalculateArraySize(short[][] parameters)
    {
        int maxi = 0, maxj = 0;
        for (int i = 0; i < parameters.Length; i++)
        {
            int temp = parameters[i][1] + parameters[i][3];
            if (temp > maxi)
            {
                maxi = temp;
            }
            temp = parameters[i][2] + parameters[i][4];
            if (temp > maxj)
            {
                maxj = temp;
            }
        }
        _rooms = new Room[maxi, maxj];
    }

    public static Floor CreateFloor(GameObject gameObject, short floorwidth, short floorheight, float roomwidth, float roomheight, GameObject roomPrefab = null)
    {
        Transform flortr = new GameObject("floor").transform;
        flortr.parent = gameObject.transform;
        Floor floor = flortr.gameObject.AddComponent<Floor>();
        floor.CreateFloor(floorwidth, floorheight, roomwidth, roomheight, RoomPrefab(roomPrefab));
        return floor;
    }
    public static Floor CreateFloor(GameObject gameObject, short[][] parameters, float width, float height, GameObject roomPrefab = null)
    {
        Transform flortr = new GameObject("floor").transform;
        flortr.parent = gameObject.transform;
        Floor floor = flortr.gameObject.AddComponent<Floor>();
        floor.CreateFloor(parameters, width, height, RoomPrefab(roomPrefab));
        return floor;
    }
    public static Floor CreateFloor(GameObject gameObject, FloorGenerationInfoBase[] parameters, float width, float height, GameObject roomPrefab = null)
    {
        short[][] parametersArray = new short[parameters.Length][];
        for (int i = 0; i < parameters.Length; ++i)
        {
            parametersArray[i] = parameters[i].GenerateParameters();
        }
        return CreateFloor(gameObject, parametersArray, width, height, roomPrefab);
    }

    public static Room CreateSingleRoom(GameObject gameObject, float w, float h, Vector3 position, bool[] doorsExistance = null, GameObject roomPrefab = null)
    {
        if (doorsExistance == null)
        {
            return CreateFloor(gameObject, 1, 1, w, h).GetComponentInChildren<Room>();
        }
        else
        {
            Transform floorTr = new GameObject("floor").transform;
            floorTr.parent = gameObject.transform;
            var floor = floorTr.gameObject.AddComponent<Floor>();
            floor._rooms = new Room[1, 1];
            floor._rooms[0, 0] = Room.CreateRoom(RoomPrefab(roomPrefab), floorTr, position, w, h, doorsExistance);
            return floor._rooms[0, 0];
        }
    }

    public static Room CreateSingleRoom(GameObject gameObject, float w, float h, bool[] doorsExistance = null, GameObject roomPrefab = null)
    {
        return CreateSingleRoom(gameObject, w, h, default, doorsExistance, roomPrefab);
    }


    public const int shape_square = 0;
    public List<short[]> shapes = new List<short[]>();
    public void addpart(short[] parameters, float width, float height, GameObject roomPrefab)
    {
        shapes.Add(parameters);
        switch (parameters[0])
        {
            case shape_square:
                int istart = parameters[1], jstart = parameters[2];
                int ilength = parameters[3] + istart, jlength = parameters[4] + jstart;
                bool edgeneighbors = parameters[5] != 0;
                for (int i = istart; i < ilength; i++)  //main part
                {
                    for (int j = jstart; j < jlength; j++)
                    {
                        bool[] doors = new bool[] { i != ilength - 1, j != jlength - 1, i != istart, j != jstart }; //initiate doors by checking if in edge of local square, true if it isn't
                        Room[] neighbors = new Room[] { null, null, null, null };
                        if (doors[0] || (i != _rooms.GetLength(0) - 1 && (edgeneighbors || (_rooms[i + 1, j] != null && _rooms[i + 1, j].CheckIfDoorExist(Side.South)))))
                        {
                            //if it isn't on localedge then, aka inside the square then no need to check anything
                            //if it isn't then check if it is on the edge of the whole square of the floor and then check if the localedges have neighbors by default or neighbors with doors pre exist
                            doors[0] = true;
                            neighbors[0] = _rooms[i + 1, j];
                        }
                        if (doors[1] || (j != _rooms.GetLength(1) - 1 && (edgeneighbors || (_rooms[i, j + 1] != null && _rooms[i, j + 1].CheckIfDoorExist(Side.West)))))
                        {
                            doors[1] = true;
                            neighbors[1] = _rooms[i, j + 1];
                        }
                        if (doors[2] || (i != 0 && (edgeneighbors || (_rooms[i - 1, j] != null && _rooms[i - 1, j].CheckIfDoorExist(Side.North)))))
                        {
                            doors[2] = true;
                            neighbors[2] = _rooms[i - 1, j];
                        }
                        if (doors[3] || (j != 0 && (edgeneighbors || (_rooms[i, j - 1] != null && _rooms[i, j - 1].CheckIfDoorExist(Side.East)))))
                        {
                            doors[3] = true;
                            neighbors[3] = _rooms[i, j - 1];
                        }

                        if (_rooms[i, j] == null)
                        {
                            _rooms[i, j] = Room.CreateRoom(roomPrefab, transform, GetpositionFromIndexes(i, j), width, height, doors, neighbors);
                        }
                        else
                        {
                            _rooms[i, j].AddNewNeighbors(neighbors);
                        }

                        if (doors[0]) _rooms[i + 1, j] = neighbors[0];
                        if (doors[1]) _rooms[i, j + 1] = neighbors[1];
                        if (doors[2]) _rooms[i - 1, j] = neighbors[2];
                        if (doors[3]) _rooms[i, j - 1] = neighbors[3];
                    }
                }
                for (int i = 6; i < parameters.Length && parameters[i] != -1; i += 2) //2nd part, forcing doors existances
                {
                    int itemp = parameters[i] / parameters[4] + istart, jtemp = parameters[i] % parameters[4] + jstart;
                    if (_rooms.CheckOutOfRange(itemp, jtemp)) continue;
                    Room temproom = _rooms[itemp, jtemp];
                    Room[] neighbors = new Room[] { null, null, null, null };
                    if (itemp != _rooms.GetLength(0) - 1)
                    {
                        neighbors[0] = _rooms[itemp + 1, jtemp];
                    }
                    if (jtemp != _rooms.GetLength(1) - 1)
                    {
                        neighbors[1] = _rooms[itemp, jtemp + 1];
                    }
                    if (itemp != 0)
                    {
                        neighbors[2] = _rooms[itemp - 1, jtemp];
                    }
                    if (jtemp != 0)
                    {
                        neighbors[3] = _rooms[itemp, jtemp - 1];
                    }
                    temproom.ChangeDoorExistance(Side.North, (parameters[i + 1] & 8) != 0, ref neighbors[0], roomPrefab);
                    temproom.ChangeDoorExistance(Side.East, (parameters[i + 1] & 4) != 0, ref neighbors[1], roomPrefab);
                    temproom.ChangeDoorExistance(Side.South, (parameters[i + 1] & 2) != 0, ref neighbors[2], roomPrefab);
                    temproom.ChangeDoorExistance(Side.West, (parameters[i + 1] & 1) != 0, ref neighbors[3], roomPrefab);
                    if (itemp != _rooms.GetLength(0) - 1)
                    {
                        _rooms[itemp + 1, jtemp] = neighbors[0];
                    }
                    if (jtemp != _rooms.GetLength(1) - 1)
                    {
                        _rooms[itemp, jtemp + 1] = neighbors[1];
                    }
                    if (itemp != 0)
                    {
                        _rooms[itemp - 1, jtemp] = neighbors[2];
                    }
                    if (jtemp != 0)
                    {
                        _rooms[itemp, jtemp - 1] = neighbors[3];
                    }
                }
                //3rd part is done cleaningnonexistantroom method below;
                break;
        }
    }


    private void CleanNonExistantRooms()
    {
        bool[,] validrooms = new bool[_rooms.GetLength(0), _rooms.GetLength(1)];
        for (int i = 0; i < validrooms.GetLength(0); i++)       //initialize them as invalid;
        {
            for (int j = 0; j < validrooms.GetLength(1); j++)
            {
                validrooms[i, j] = false;
            }
        }
        foreach (short[] parameters in shapes)
        {
            switch (parameters[0])
            {
                case shape_square:
                    int istart = parameters[1], jstart = parameters[2];
                    int ilength = parameters[3] + istart, jlength = parameters[4] + jstart;
                    int i;
                    for (i = istart; i < ilength; i++) //validate the rooms within the local square
                    {
                        for (int j = jstart; j < jlength; j++)
                        {
                            validrooms[i, j] = true;
                        }
                    }
                    i = GetStartInArrayForNonExistance(parameters);
                    if (i != -1)
                    {
                        for (; i < parameters.Length && parameters[i] != -1; i++)//covering the 3rd part in which forced non existant rooms get invalidated
                        {
                            validrooms[parameters[i] / parameters[4] + istart, parameters[i] % parameters[4] + jstart] = false;
                        }
                    }
                    break;
            }
        }

        for (int i = 0; i < validrooms.GetLength(0); i++)//destroy invalid rooms
        {
            for (int j = 0; j < validrooms.GetLength(1); j++)
            {
                if (validrooms[i, j] == false && _rooms[i, j] != null)
                {
#if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        DestroyImmediate(_rooms[i, j].gameObject);
                    }
                    else
                    {
                        Destroy(_rooms[i, j].gameObject);
                    }
#else
                    Destroy(_rooms[i, j].gameObject);
#endif
                    _rooms[i, j] = null;
                }
            }
        }
    }


    private void CleanDoorsOfNonExistantNeighbors() //after all doors without neighbors have been destroyed, this is for the doors that should exist because of part2
    {
        foreach (Room room in this)
        {
            foreach (Door door in (IEnumerable<Door>)room)
            {
                door.DestroyDoorIfNoNeighbor();
            }
        }
        foreach (short[] parameters in shapes)
        {
            switch (parameters[0])
            {
                case shape_square:
                    if (parameters.Length < 7 || parameters[6] == -1)
                    {
                        continue;
                    }
                    int istart = parameters[1], jstart = parameters[2];
                    for (int i = 6; i < parameters.Length && parameters[i] != -1; i += 2) //2nd part, forcing doors existances
                    {
                        int itemp = parameters[i] / parameters[4] + istart, jtemp = parameters[i] % parameters[4] + jstart;
                        if (_rooms.CheckOutOfRange(itemp, jtemp)) continue;
                        Room temproom = _rooms[itemp, jtemp];
                        temproom.ChangeDoorExistance(Side.North, (parameters[i + 1] & 8) != 0);
                        temproom.ChangeDoorExistance(Side.East, (parameters[i + 1] & 4) != 0);
                        temproom.ChangeDoorExistance(Side.South, (parameters[i + 1] & 2) != 0);
                        temproom.ChangeDoorExistance(Side.West, (parameters[i + 1] & 1) != 0);
                    }
                    break;
            }
        }

    }

    private int GetStartInArrayForNonExistance(short[] parameters)
    {
        int i;
        for (i = 6; i < parameters.Length && parameters[i] != -1; i += 2) ;
        if (i == parameters.Length) return -1;
        return i + 1;
    }

    public Vector3 GetpositionFromIndexes(int i, int j)
    {
        if (_rooms.CheckOutOfRange(i, j) || _rooms[i, j] == null)
        {
            return new Vector3(j * (_mutualWidth + Room.WallFatness), i * (_mutualHeight + Room.WallFatness));
        }
        return _rooms[i, j].Position;
    }

    public void ManageDoors(short[] info)
    {
        for (int ii = 0; ii + 2 < info.Length; ii += 3)
        {
            int i = info[ii], j = info[ii + 1], doors = info[ii + 2];
            if (!_rooms.CheckOutOfRange(i, j) && _rooms[i, j] != null)
            {
                _rooms[i, j].ChangeDoorState(new bool[] { (doors & 8) != 0, (doors & 4) != 0, (doors & 2) != 0, (doors & 1) != 0 });
            }
        }
    }
    public void SetAllDoors(bool doorsOpen)
    {
        foreach (var room in this)
        {
            room.ChangeDoorStateImmediate(doorsOpen);
        }
    }

    private static GameObject _roomPrefabField;
    private static GameObject _roomPrefab
    {
        get => _roomPrefabField != null ? _roomPrefabField : _roomPrefabField = Resources.Load<GameObject>("Prefabs/room");
        set => _roomPrefabField = value;
    }

    private static GameObject RoomPrefab(GameObject roomPrefab) => roomPrefab == null ? _roomPrefab : roomPrefab;

    public static void SetRoomPrefab(GameObject roomPrefab) => _roomPrefab = roomPrefab;

    public IEnumerator<Room> GetEnumerator()
    {
        foreach (Room room in _rooms)
        {
            if (room != null)
            {
                yield return room;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }


    [System.Serializable]
    private struct GroupOfRooms //wrapper class to serialize Rooms[,]
    {
        public Room[] Rooms;
        public GroupOfRooms(Room[] rooms)
        {
            Rooms = rooms;
        }
    }
    [SerializeField]
    private GroupOfRooms[] _storeRooms;

    public void OnBeforeSerialize()
    {
        StoreRooms();
    }

    public void OnAfterDeserialize()
    {
        LoadRooms();
    }

    public void LoadRooms()
    {
        if (_storeRooms != null && _storeRooms.Length != 0)
        {
            _rooms = new Room[_storeRooms.Length, _storeRooms[0].Rooms.Length];
            for (int i = 0; i < _rooms.GetLength(0); ++i)
            {
                for (int j = 0; j < _rooms.GetLength(1); ++j)
                {
                    _rooms[i, j] = _storeRooms[i].Rooms[j];
                }
            }
        }
    }

    public void StoreRooms()
    {
        if (_rooms != null)
        {
            _storeRooms = new GroupOfRooms[_rooms.GetLength(0)];
            for (int i = 0; i < _rooms.GetLength(0); ++i)
            {
                _storeRooms[i] = new GroupOfRooms(new Room[_rooms.GetLength(1)]);
                for (int j = 0; j < _rooms.GetLength(1); ++j)
                {
                    _storeRooms[i].Rooms[j] = _rooms[i, j];
                }
            }
        }
    }

    [System.Serializable]
    public class FloorGenerationInfoBase
    {
        [SerializeField]
        private bool _include = false;

        public bool Include => _include;


        [SerializeField]
        private int _origini = 0, _originj = 0, _height = 0, _width = 0;

        [SerializeField]
        private bool _edgesOpen = false;

        [SerializeField]
        private RoomIndexAndWallsInfo[] RoomsAndDoors = new RoomIndexAndWallsInfo[0];
        [SerializeField]
        private RoomsIndexes[] RoomsToRemove = new RoomsIndexes[0];

        public short[] GenerateParameters()
        {
            short[] parametersArray = new short[6 + ((RoomsAndDoors != null && RoomsAndDoors.Length != 0) ? (RoomsAndDoors.Length << 1) + 1 : 1) +
                                                    ((RoomsToRemove != null && RoomsToRemove.Length != 0) ? RoomsToRemove.Length : 1)];
            parametersArray[0] = (short)0;
            parametersArray[1] = (short)_origini;
            parametersArray[2] = (short)_originj;
            parametersArray[3] = (short)_height;
            parametersArray[4] = (short)_width;
            parametersArray[5] = _edgesOpen ? (short)1 : (short)0;

            int i, istart;
            if (RoomsAndDoors == null || RoomsAndDoors.Length == 0)
            {
                parametersArray[6] = -1;
                i = 7;
            }
            else
            {
                for (istart = i = 6; ((i - istart) >> 1) < RoomsAndDoors.Length; i += 2)
                {
                    parametersArray[i] = (short)(RoomsAndDoors[(i - istart) >> 1].RoomIndexes.i * _width + RoomsAndDoors[(i - istart) >> 1].RoomIndexes.j);
                    parametersArray[i + 1] = RoomsAndDoors[(i - istart) >> 1].Bits;
                }
                parametersArray[i++] = -1;
            }
            if (RoomsToRemove == null || RoomsToRemove.Length == 0)
            {
                parametersArray[i++] = -1;
            }
            else
            {
                for (istart = i; i < RoomsToRemove.Length + istart; ++i)
                {
                    parametersArray[i] = (short)(RoomsToRemove[i - istart].i * _width + RoomsToRemove[i - istart].j);
                }
            }
            //string s = "";
            //foreach (var ss in parametersArray)
            //{
            //    s += ss + " ";
            //}
            //Debug.Log(s);
            return parametersArray;
        }

    }

    [System.Serializable]
    public struct RoomIndexAndWallsInfo
    {
        public RoomsIndexes RoomIndexes;
        public bool North, East, South, West;

        public short Bits => (short)((North ? 8 : 0) |
            (East ? 4 : 0) | (South ? 2 : 0) | (West ? 1 : 0));

    }

    [System.Serializable]
    public struct RoomsIndexes
    {
        public int i, j;
    }



    #region pathFinding


    private Uinf _visitedRooms = new Uinf(0);

    private void ClearVistsedRooms()
    {
        if (_visitedRooms.Length != _rooms.Length)
        {
            _visitedRooms = new Uinf(_rooms.Length);
        }
        else
        {
            _visitedRooms.ResetAllBits();
        }
    }
    private void AddToVistsedRooms(Room room) => _visitedRooms.SetBit(GetIndexFromRoom(room));
    private bool HasBeenVisited(Room room) => _visitedRooms[GetIndexFromRoom(room)];

    private static RoomDist FindMinInNextAndRemoveIt()
    {
        float minDist = -1f;
        foreach (var element in _next)
        {
            minDist = element.Key;
            break;
        }
        if (minDist == -1f) return null;
        int indexOfLast = _next[minDist].Count - 1;
        var temp = _next[minDist][indexOfLast];
        if (indexOfLast == 0)
        {
            _next.Remove(minDist);
        }
        else
        {
            _next[minDist].RemoveAt(indexOfLast);
        }
        return temp;
    }


    private static bool ReplaceInNext(Room r, Room from, float cost, float h)
    {
        float newDistance = cost + h;
        foreach (var element in _next)
        {
            int index = element.Value.FindIndex(x => x.Room == r);
            if (index != -1)
            {//if it is alraedy in next replace it if it has smaller distance, if it's not then just add it
                if (element.Key > newDistance)
                {
                    if (index == 0)
                    {
                        _next.Remove(element.Key);
                    }
                    else
                    {
                        element.Value.RemoveAt(index);
                    }
                    AddToNext(new RoomDist(r, from, cost, h));
                    return true;
                }
                return false;
            }
        }
        AddToNext(new RoomDist(r, from, cost, h));
        return true;

        void AddToNext(RoomDist roomDist)
        {
            if (_next.ContainsKey(roomDist.CostPlusH))
            {
                _next[roomDist.CostPlusH].Add(roomDist);
            }
            else
            {
                _next.Add(roomDist.CostPlusH, new List<RoomDist> { roomDist });
            }
        }
    }

    private static void ClearNext()
    {
        foreach (var element in _next)
        {
            element.Value.Clear();
        }
        _next.Clear();
    }

    private static SortedDictionary<float, List<RoomDist>> _next = new SortedDictionary<float, List<RoomDist>>();
    private static List<Room> _path = new List<Room>();



    private class RoomDist
    {
        public readonly Room Room;
        public readonly Room From;
        public readonly float CostPlusH;
        public readonly float Cost;

        public RoomDist(Room room, Room from, float cost, float h)
        {
            Room = room;
            From = from;
            Cost = cost;
            CostPlusH = cost + h;
        }
    }

    public bool FindRoomPath(Room start, Room end, out LinkedList<Room> roompath)
    {
        roompath = new LinkedList<Room>();
        if (start == null || end == null)
        {
            return false;
        }

        int starti = -1;
        for (int i = 0; i < _path.Count; i++)
        {
            if (start == _path[i])
            {
                starti = i;
                break;
            }
        }
        if (starti != -1)
        {
            int endi = -1;
            roompath.Clear();
            roompath.AddLast(_path[starti]);
            for (int i = starti + 1; i < _path.Count; i++)
            {
                if (!RoomsAreNeighborsAstar(_path[i], _path[i - 1]))
                {
                    break;
                }
                roompath.AddLast(_path[i]);
                if (end == _path[i])
                {
                    endi = i;
                    break;
                }
            }
            if (endi != -1)
            {
                return true;
            }
        }

        ClearVistsedRooms();
        ClearNext();
        _path.Clear();

        Astar(start, end, 0f);
        _path.Reverse(); // in the Astar the path is produced as a stack meaning the startroom is at the end of the list, so it must reversed

        ClearVistsedRooms();
        ClearNext();

        roompath = new LinkedList<Room>(_path);

        return _path.Count != 0;
    }

    public void FindRoomPath(Room start, Room end, bool debug)
    {
        if (start == null || end == null)
        {
            return;
        }
        ClearVistsedRooms();
        ClearNext();
        _path.Clear();
        if (debug)
        {
            AstarDebug(start, end, 0f);
            _path.Reverse(); // in the Astar the path is produced as a stack meaning the startroom is at the end of the list, so i reverse it
        }
        else
        {
            Astar(start, end, 0f);
            _path.Reverse(); // in the Astar the path is produced as a stack meaning the startroom is at the end of the list, so i reverse it
        }
        ClearVistsedRooms();
        ClearNext();
    }


    public Room Astar(Room curr, Room goal, float d)
    {
        if (curr == goal)
        {
            AddToPath(goal);
            return goal;
        }
        else if (RoomsAreNeighborsAstar(curr, goal))
        {
            AddToPath(goal);
            AddToPath(curr);
            return goal;
        }
        AddToVistsedRooms(curr);
        AddToNext(curr, goal, d);
        if (_next.Count == 0) return null;

        RoomDist nextrd = FindMinInNextAndRemoveIt();
        return EndAstar(curr, goal, nextrd, Astar(nextrd.Room, goal, nextrd.Cost));
    }

    private void AddToNext(Room curr, Room goal, float d)
    {
        foreach (Door door in (IEnumerable<Door>)curr)
        {
            if (door.ActuallyOpen && !HasBeenVisited(door.Neighbor))
            {
                if (ReplaceInNext(door.Neighbor, curr, d + Distance(curr, door.Neighbor), Distance(goal, door.Neighbor)))
                {
                    if (((int)door.Side & 1) == 0)
                    {
                        Room tempNeighbor = door.Neighbor[1];
                        if (door.Neighbor.CheckIfDoorActuallyOpen(Side.East) && !HasBeenVisited(tempNeighbor))
                        {
                            ReplaceInNext(tempNeighbor, curr, d + Distance(curr, tempNeighbor), Distance(goal, tempNeighbor));
                        }
                        tempNeighbor = door.Neighbor[3];
                        if (door.Neighbor.CheckIfDoorActuallyOpen(Side.West) && !HasBeenVisited(tempNeighbor))
                        {
                            ReplaceInNext(tempNeighbor, curr, d + Distance(curr, tempNeighbor), Distance(goal, tempNeighbor));
                        }
                    }
                    else
                    {
                        Room tempNeighbor = door.Neighbor[0];
                        if (door.Neighbor.CheckIfDoorActuallyOpen(Side.North) && !HasBeenVisited(tempNeighbor))
                        {
                            ReplaceInNext(tempNeighbor, curr, d + Distance(curr, tempNeighbor), Distance(goal, tempNeighbor));
                        }
                        tempNeighbor = door.Neighbor[2];
                        if (door.Neighbor.CheckIfDoorActuallyOpen(Side.South) && !HasBeenVisited(tempNeighbor))
                        {
                            ReplaceInNext(tempNeighbor, curr, d + Distance(curr, tempNeighbor), Distance(goal, tempNeighbor));
                        }
                    }
                }
            }
        }
    }


    private static Room EndAstar(Room curr, Room goal, RoomDist nextrd, Room roomreturned)
    {
        if (roomreturned != null)
        {
            if (roomreturned == goal) //this means that nextrd.room is on path
            {
                if (nextrd.From == curr) //curr being nextrd.comefrom means that curr must be also on path
                {
                    FillCorners(curr, nextrd.Room, goal, false);
                    AddToPath(curr);
                    return goal;
                }
                else
                {
                    return nextrd.From; //return the next room of the path that we must find
                }
            }
            else if (roomreturned == curr) //if temprd.room is not path then we need to check if curr is on path else we return the room we are looking for
            {
                FillCorners(curr, _path[_path.Count - 1], goal, false);
                AddToPath(curr);
                return goal;
            }
            else
            {
                return roomreturned;   //keep looking for the next room of the path
            }
        }
        return null;
    }



    public Room AstarDebug(Room curr, Room goal, float d)
    {
        if (curr == goal)
        {
            AddToPath(goal);
            ColorPath(goal);
            return goal;
        }
        else if (RoomsAreNeighborsAstar(curr, goal))
        {
            AddToPath(goal);
            ColorPath(goal);
            AddToPath(curr);
            ColorPath(curr);
            return goal;
        }
        AddToVistsedRooms(curr);
        ColorVisited(curr);
        //Debug.Log("current", curr);
        AddToNextDebug(curr, goal, d);
        if (_next.Count == 0) return null;

        RoomDist nextrd = FindMinInNextAndRemoveIt();
        return EndAstarDebug(curr, goal, nextrd, AstarDebug(nextrd.Room, goal, nextrd.Cost));
    }

    private void AddToNextDebug(Room curr, Room goal, float d)
    {
        foreach (Door door in (IEnumerable<Door>)curr)
        {
            if (door.ActuallyOpen && !HasBeenVisited(door.Neighbor))
            {
                if (ReplaceInNext(door.Neighbor, curr, d + Distance(curr, door.Neighbor), Distance(goal, door.Neighbor)))
                {
                    ColorNext(door.Neighbor);
                    if (((int)door.Side & 1) == 0)
                    {
                        Room tempNeighbor = door.Neighbor[1];
                        if (door.Neighbor.CheckIfDoorActuallyOpen(Side.East) && !HasBeenVisited(tempNeighbor))
                        {
                            if (ReplaceInNext(tempNeighbor, curr, d + Distance(curr, tempNeighbor), Distance(goal, tempNeighbor)))
                                ColorNext(door.Neighbor);
                        }
                        tempNeighbor = door.Neighbor[3];
                        if (door.Neighbor.CheckIfDoorActuallyOpen(Side.West) && !HasBeenVisited(tempNeighbor))
                        {
                            if (ReplaceInNext(tempNeighbor, curr, d + Distance(curr, tempNeighbor), Distance(goal, tempNeighbor)))
                                ColorNext(door.Neighbor);
                        }
                    }
                    else
                    {
                        Room tempNeighbor = door.Neighbor[0];
                        if (door.Neighbor.CheckIfDoorActuallyOpen(Side.North) && !HasBeenVisited(tempNeighbor))
                        {
                            if (ReplaceInNext(tempNeighbor, curr, d + Distance(curr, tempNeighbor), Distance(goal, tempNeighbor)))
                                ColorNext(door.Neighbor);
                        }
                        tempNeighbor = door.Neighbor[2];
                        if (door.Neighbor.CheckIfDoorActuallyOpen(Side.South) && !HasBeenVisited(tempNeighbor))
                        {
                            if (ReplaceInNext(tempNeighbor, curr, d + Distance(curr, tempNeighbor), Distance(goal, tempNeighbor)))
                                ColorNext(door.Neighbor);
                        }
                    }
                }
            }
        }
    }

    private static Room EndAstarDebug(Room curr, Room goal, RoomDist nextrd, Room roomreturned)
    {
        if (roomreturned != null)
        {
            if (roomreturned == goal) //this means that nextrd.room is on path
            {
                if (nextrd.From == curr) //curr being nextrd.comefrom means that curr must be also on path
                {
                    FillCorners(curr, nextrd.Room, goal, true);
                    AddToPath(curr);
                    ColorPath(curr);
                    return goal;
                }
                else
                {
                    return nextrd.From; //return the next room of the path that we must find
                }
            }
            else if (roomreturned == curr) //if temprd.room is not path then we need to check if curr is on path else we return the room we are looking for
            {
                FillCorners(curr, _path[_path.Count - 1], goal, true);
                AddToPath(curr);
                ColorPath(curr);
                return goal;
            }
            else
            {
                return roomreturned;   //keep looking for the next room of the path
            }
        }
        return null;
    }


    public static float Distance(Room r1, Room r2) => (r1.transform.position - r2.transform.position).magnitude;

    public static void AddToPath(Room r) => _path.Add(r);



    public static bool RoomsAreNeighborsAstar(Room r1, Room r2)
    {
        if (r1 == null || r2 == null) return false;
        foreach (Door door in (IEnumerable<Door>)r1)
        {
            if (door.ActuallyOpen && door.Neighbor == r2)
            {
                return true;
            }
        }
        return false;
    }

    private static void FillCorners(Room curr, Room nextroom, Room goal, bool debug)
    {
        if (RoomsAreNeighborsAstar(curr, nextroom)) return;
        float mindist = float.MaxValue;
        Room closestroom = null;
        foreach (Door door in (IEnumerable<Door>)curr)
        {
            if (door.ActuallyOpen && RoomsAreNeighborsAstar(door.Neighbor, nextroom))
            {
                float tempdist = (door.Neighbor.transform.position - goal.transform.position).sqrMagnitude;
                if (tempdist < mindist)
                {
                    mindist = tempdist;
                    closestroom = door.Neighbor;
                }
            }
        }
        if (closestroom != null)
        {
            AddToPath(closestroom);
            if (debug)
                closestroom.Color = new Color(0f, 0.75f, 0f);
        }
    }

    private static void ColorPath(Room r) => r.Color = Color.green;

    private static void ColorVisited(Room r) => r.Color = Color.red;

    private static void ColorNext(Room r) => r.Color = Color.yellow;


    #endregion




    public void FixFloor()
    {
        Room[] rooms = FindObjectsOfType<Room>();
        if (rooms.Length == 0)
        {
            Debug.Log("No rooms found");
            return;
        }
        float mutualWidth = rooms[0].Width;
        float mutualHeight = rooms[0].Height;

        float minx = float.MaxValue, miny = float.MaxValue, maxx = float.MinValue, maxy = float.MinValue;
        foreach (var room in rooms)
        {
            if (room.Position.x < minx)
            {
                minx = room.Position.x;
            }
            if (room.Position.y < miny)
            {
                miny = room.Position.y;
            }
            if (room.Position.x > maxx)
            {
                maxx = room.Position.x;
            }
            if (room.Position.y > maxy)
            {
                maxy = room.Position.y;
            }
        }
        int jlength = GetIndexFromPosition(maxx - minx, mutualWidth + Room.WallFatness) + 1;
        int ilength = GetIndexFromPosition(maxy - miny, mutualHeight + Room.WallFatness) + 1;
        _rooms = new Room[ilength, jlength];

        foreach (var room in rooms)
        {
            int j = GetIndexFromPosition(room.Position.x - minx, mutualWidth + Room.WallFatness);
            int i = GetIndexFromPosition(room.Position.y - miny, mutualHeight + Room.WallFatness);
            _rooms[i, j] = room;
        }

        for (int i = 0; i < ilength; i++)  //main part
        {
            for (int j = 0; j < jlength; j++)
            {
                if (_rooms[i, j] == null) continue;
                _rooms[i, j].FixWallsHasDoor();
                if (i + 1 < ilength && _rooms[i + 1, j] != null)
                {
                    _rooms[i + 1, j].FixWallsHasDoor();
                    if (_rooms[i, j].CheckIfDoorExist(Side.North) && _rooms[i + 1, j].CheckIfDoorExist(Side.South))
                    {
                        _rooms[i, j].SetNeighbor(_rooms[i + 1, j], Side.North);
                    }
                }
                if (j + 1 < jlength && _rooms[i, j + 1] != null)
                {
                    _rooms[i, j + 1].FixWallsHasDoor();
                    if (_rooms[i, j].CheckIfDoorExist(Side.East) && _rooms[i, j + 1].CheckIfDoorExist(Side.West))
                    {
                        _rooms[i, j].SetNeighbor(_rooms[i, j + 1], Side.East);
                    }
                }
            }
        }

    }
}