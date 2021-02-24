using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolRooms_ChillMode : ChillModeClass
{

    private int _curretIndex = 0;

    [SerializeField]
    private Room[] _rooms = new Room[0];

    public MyLib.ReadOnlyList<Room> Rooms => _rooms;

    protected new void Awake()
    {
        base.Awake();

        if (_rooms == null || _rooms.Length == 0)
        {
            _rooms = new Room[1];
            _rooms[0] = Rules.Floor.GetClosestRoomFromPosition(AICharacter.Position);
        }
        else
        {
            for (int i = 0; i < _rooms.Length; ++i)
            {
                if (Rules.Floor.GetClosestRoomFromPosition(AICharacter.Position) == _rooms[i])
                {
                    _curretIndex = i;
                    break;
                }
            }
        }
    }

    public override void OnStateEnter()
    {
    }

    public override void LogicalUpdate()
    {
        if (!AICharacter.MoveComponent.HasPath)
        {
            Room currentroom = Rules.Floor.GetClosestRoomFromPosition(AICharacter.Position);
            if (currentroom == _rooms[_curretIndex] && ++_curretIndex >= _rooms.Length)
            {
                _curretIndex = 0;
            }
            AICharacter.MoveComponent.StartFromScratchNewEndpos(_rooms[_curretIndex].Position);
        }
    }

    public override void OnStateExit()
    {
    }

    public void SetRooms(params Room[] rooms) => _rooms = rooms;
    public void SetRooms(IEnumerable<Room> rooms) => _rooms = new List<Room>(rooms).ToArray();
}

