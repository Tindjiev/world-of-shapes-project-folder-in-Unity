using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionOfRoom : VisionOfNotMob
{
    [SerializeField]
    protected Room _room;
    [SerializeField]
    protected BoxCollider2D _coll;

    [SerializeField]
    private Team _team;
    [SerializeField]
    private TeamAction _whatToDoWithTeam;

    public float Height
    {
        get => _coll.size.y;
        set => _coll.size = new Vector2(_coll.size.x, value);
    }
    public float Width
    {
        get => _coll.size.x;
        set => _coll.size = new Vector2(value, _coll.size.y);
    }
    protected new void Awake()
    {
        base.Awake();
        if (_room == null)
        {
            _room = this.SearchComponent<Room>();
        }
        if (_coll == null && (_coll = GetComponent<BoxCollider2D>()) == null)
        {
            _coll = gameObject.AddComponent<BoxCollider2D>();
            _coll.isTrigger = true;
        }
        Height = _room.Height - 1f;
        Width = _room.Width - 1f;
    }

    protected new void Start()
    {
        base.Start();
    }

    public override bool OutOfVisionRange(CollisionInfo target)
    {
        if (target == null) return false;
        return !_room.WithinRoom(target.transform.position);
    }

    protected override bool CanAddToSeenExtraCondition(CollisionInfo withinVision)
    {
        switch (_whatToDoWithTeam)
        {
            case TeamAction.SeeEverything:
                return true;
            case TeamAction.SeeOnlyThisTeam:
                return withinVision.Entity.Team == _team;
            case TeamAction.SeeOnlyEnemyTeams:
                return _team.EnemyTeams.Contains(withinVision.Entity.Team);
            case TeamAction.SeeOnlyNonAlliedTeams:
                return !_team.AlliedTeams.Contains(withinVision.Entity.Team);
        }
        return false;
    }

    public override bool SetUp()
    {
        gameObject.SetActive(false);
        if (_room == null)
        {
            Room room = this.SearchComponent<Room>();
            if (room == null)
            {
                throw new Exception("vision of room set in a non room");
            }
            _room = room;
            for (int i = transform.childCount - 1; i > -1; --i)
            {
                transform.GetChild(i).EditorDestroy();
            }
            var temp = new GameObject("collider of vision");
            temp.transform.parent = transform;
            _coll = temp.AddComponent<BoxCollider2D>();
            _coll.size = new Vector2(_room.Width - Room.WallFatness - 1f, _room.Height - Room.WallFatness - 1f);
            return true;
        }
        if (_coll == null)
        {
            for (int i = transform.childCount - 1; i > -1; --i)
            {
                transform.GetChild(i).EditorDestroy();
            }
            var temp = new GameObject("collider of vision");
            temp.transform.parent = transform;
            _coll = temp.AddComponent<BoxCollider2D>();
            _coll.size = new Vector2(_room.Width - Room.WallFatness - 1f, _room.Height - Room.WallFatness - 1f);
            return true;
        }
        return false;
    }

    public void SetTeam(Team team, TeamAction whatToDoWithTeam)
    {
        _team = team;
        _whatToDoWithTeam = whatToDoWithTeam;
    }


    public enum TeamAction
    {
        SeeEverything,
        SeeOnlyThisTeam,
        SeeOnlyEnemyTeams,
        SeeOnlyNonAlliedTeams,
    }
}
