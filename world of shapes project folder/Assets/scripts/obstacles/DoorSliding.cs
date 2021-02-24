using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Wall = Room.Wall;
using Side = Room.Side;

public abstract class Door : MonoBehaviour
{
    [SerializeField, ReadOnlyOnInspector]
    protected bool _isOpen;

    protected bool _closing, _opening;


    public Room Room;

    private Wall _wallField;
    protected Wall _wall
    {
        get
        {
            if (_wallField == null)
            {
                FindWall();
            }
            return _wallField;
        }
    }
    public bool Exists => _wall.HasDoor;
    public bool HasNeighbor => _wall.HasNeighbor;
    public Room Neighbor => _wall.Neighbor;
    public Side Side => _wall.Side;
    public Door NeighborDoor => _wall.NeighborDoor;

    protected void Awake()
    {
        if (Room == null) Room = this.SearchComponent<Room>();
    }

    public abstract void AdjustToNewWidthHeight();

    private void FindWall()
    {
        foreach (Wall wall in (IEnumerable<Wall>)Room)
        {
            if (wall.Door == this)
            {
                _wallField = wall;
                return;
            }
        }
    }

    public bool ActuallyOpen
    {
        get
        {
            if (Neighbor == null)
            {
                return false;
            }
            Door Neighbordoor = NeighborDoor;
            return _isOpen && Neighbordoor != null && Neighbordoor._isOpen && Neighbordoor.Neighbor == Room;
        }
    }

    public bool Closed
    {
        get => !_isOpen;
        protected set => _isOpen = !value;
    }

    public void DestroyDoorIfNoNeighbor()
    {
        if (Neighbor == null)
        {
            _wall.DoorDestroy();
        }
    }



    public void CommandClose()
    {
        if (!Exists || _opening || _closing) return;
        CommandCloseSpecific();
    }
    protected abstract void CommandCloseSpecific();

    public void CommandOpen()
    {
        if (!Exists || _opening || _closing) return;
        CommandOpenSpecific();
    }
    protected abstract void CommandOpenSpecific();



    public void CloseImmediate()
    {
        _isOpen = false;
        _opening = false;
        _closing = false;
        CloseImmediateSpecific();
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
        }
#endif
    }
    protected abstract void CloseImmediateSpecific();
    public void CloseImmediate(bool NeighborToo)
    {
        if (!Exists) return;
        CloseImmediate();
        if (NeighborToo && Neighbor != null)
        {
            NeighborDoor.CloseImmediate();
        }
    }

    public void OpenImmediate()
    {
        _isOpen = true;
        _opening = false;
        _closing = false;
        OpenImmediateSpecific();
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
        }
#endif
    }
    protected abstract void OpenImmediateSpecific();
    public void OpenImmediate(bool NeighborToo)
    {
        if (!Exists) return;
        OpenImmediate();
        if (NeighborToo && Neighbor != null)
        {
            NeighborDoor.OpenImmediate();
        }
    }

}

public class DoorSliding : Door
{
    private const float _TIME_TO_MOVE = 0.5f;

    [SerializeField, HideInInspector]
    private float _widthItRemembers, _heightItRemembers;

    [SerializeField, HideInInspector]
    private Vector3 _scaleCloseItRemembers = Vector3.zero, _scaleOpenItRemembers = Vector3.zero, _closePositionItRemembers = Vector3.zero, _openPositionItRemembers = Vector3.zero;

    private Vector3 _scaleClose
    {
        get
        {
            IfHeightWidthDifferentAdjust();
            return _scaleCloseItRemembers;
        }
    }
    private Vector3 _scaleOpen
    {
        get
        {
            IfHeightWidthDifferentAdjust();
            return _scaleOpenItRemembers;
        }
    }
    private Vector3 _closePosition
    {
        get
        {
            IfHeightWidthDifferentAdjust();
            return _closePositionItRemembers;
        }
    }
    private Vector3 _openPosition
    {
        get
        {
            IfHeightWidthDifferentAdjust();
            return _openPositionItRemembers;
        }
    }

    protected new void Awake()
    {
        base.Awake();
        AdjustToNewWidthHeight();
    }

    private void IfHeightWidthDifferentAdjust()
    {
        if (Room.Width != _widthItRemembers || Room.Height != _heightItRemembers)
        {
            _widthItRemembers = Room.Width;
            _heightItRemembers = Room.Height;
            AdjustToNewWidthHeight();
        }
    }

    public override void AdjustToNewWidthHeight()
    {
        Vector3 close = Vector3.zero, open = Vector3.zero;
        switch (Side)
        {
            case Side.North:
                open = (close = Room.NorthSideCentre - Room.Position) + new Vector3(Room.DoorWidth, 0f);
                _scaleOpenItRemembers = transform.localScale = new Vector3(Room.DoorWidth, Room.WallFatness);
                _scaleCloseItRemembers = new Vector3(_scaleOpen.x * 2f, _scaleOpen.y, _scaleOpen.z);
                break;
            case Side.East:
                open = (close = Room.EastSideCentre - Room.Position) + new Vector3(0f, Room.DoorWidth);
                _scaleOpenItRemembers = transform.localScale = new Vector3(Room.WallFatness, Room.DoorWidth);
                _scaleCloseItRemembers = new Vector3(_scaleOpen.x, _scaleOpen.y * 2f, _scaleOpen.z);
                break;
            case Side.South:
                open = (close = Room.SouthSideCentre - Room.Position) + new Vector3(Room.DoorWidth, 0f);
                _scaleOpenItRemembers = transform.localScale = new Vector3(Room.DoorWidth, Room.WallFatness);
                _scaleCloseItRemembers = new Vector3(_scaleOpen.x * 2f, _scaleOpen.y, _scaleOpen.z);
                break;
            case Side.West:
                open = (close = Room.WestSideCentre - Room.Position) + new Vector3(0f, Room.DoorWidth);
                _scaleOpenItRemembers = transform.localScale = new Vector3(Room.WallFatness, Room.DoorWidth);
                _scaleCloseItRemembers = new Vector3(_scaleOpen.x, _scaleOpen.y * 2f, _scaleOpen.z);
                break;
        }
        _closePositionItRemembers = close;
        _openPositionItRemembers = open;
    }


    private void StartClosing()
    {
        _closing = true;
        gameObject.SetActive(true);
        transform.DOLocalMove(_closePosition, _TIME_TO_MOVE).onComplete = CloseImmediate;
    }
    private void StartOpening()
    {
        _opening = true;
        transform.localScale = _scaleOpen;
        transform.DOLocalMove(_openPosition, _TIME_TO_MOVE).onComplete = OpenImmediate;
    }

    protected override void CommandCloseSpecific()
    {
        if (!Closed)
        {
            Closed = true;
            StartClosing();
        }
    }

    protected override void CommandOpenSpecific()
    {
        if (Closed)
        {
            Closed = false;
            StartOpening();
        }
    }
    protected override void  CloseImmediateSpecific()
    {
        transform.localPosition = _closePosition;
        transform.localScale = _scaleClose;
        gameObject.SetActive(true);
    }
    protected override void OpenImmediateSpecific()
    {
        transform.localPosition = _openPosition;
        transform.localScale = _scaleOpen;
        gameObject.SetActive(false);
    }

}
