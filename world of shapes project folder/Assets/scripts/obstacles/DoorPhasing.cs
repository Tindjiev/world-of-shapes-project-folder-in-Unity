using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Side = Room.Side;

public class DoorPhasing : Door
{
    [SerializeField]
    private SpriteRenderer _rend;

    private const float _TIME_TO_PHASE = 0.5f;

    protected new void Awake()
    {
        _rend = GetComponent<SpriteRenderer>();
        AdjustToNewWidthHeight();
        base.Awake();
    }

    public override void AdjustToNewWidthHeight()
    {
        switch (Side)
        {
            case Side.North:
                transform.position = Room.NorthSideCentre;
                transform.localScale = new Vector3(Room.DoorWidth * 2f, Room.WallFatness);
                break;
            case Side.East:
                transform.position = Room.EastSideCentre;
                transform.localScale = new Vector3(Room.WallFatness, Room.DoorWidth * 2f);
                break;
            case Side.South:
                transform.position = Room.SouthSideCentre;
                transform.localScale = new Vector3(Room.DoorWidth * 2f, Room.WallFatness);
                break;
            case Side.West:
                transform.position = Room.WestSideCentre;
                transform.localScale = new Vector3(Room.WallFatness, Room.DoorWidth * 2f);
                break;
        }
    }



    private void StartClosing()
    {
        _closing = true;
        gameObject.SetActive(true);
        _rend.DOFade(1f, _TIME_TO_PHASE).onComplete = CloseImmediate;
    }
    private void StartOpening()
    {
        _opening = true;
        _rend.DOFade(0f, _TIME_TO_PHASE).onComplete = CloseImmediate;
    }

    protected override void CommandCloseSpecific()
    {
        gameObject.SetActive(true);
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

    protected override void CloseImmediateSpecific()
    {
        _rend.color = new Color(_rend.color.r, _rend.color.g, _rend.color.b, 1f);
        gameObject.SetActive(true);
    }

    protected override void OpenImmediateSpecific()
    {
        _rend.color = new Color(_rend.color.r, _rend.color.g, _rend.color.b, 0f);
        gameObject.SetActive(false);
    }
}
