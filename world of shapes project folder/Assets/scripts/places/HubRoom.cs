using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HubRoom : ControlBaseWithPlayer
{
    public Room hub = null;

    protected new void Awake()
    {
        base.Awake();
        AddcomponentOnDestroy = x => x.AddComponent<mainmenu>();
        if (hub == null) hub = FindObjectOfType<Room>();
        if (hub == null)
        {
            Background.SetBackgroundColor(Background.YellowishColor);
            Room.DoorType = "move";
            hub = Floor.CreateSingleRoom(gameObject, 70f, 50f, new bool[] { false, false, true });
        }
        //Background.CreateBackgroundSquare(new Rect(hub.Position, hub.Size), Background.GreenishColor);

        SetPlayer(() => BasicLib.InitializePlayer(1000f, hub.SouthSideCentre, MyColorLib.teams[MyColorLib.green], SceneChanger.SwitchToMenu));
        MyInputs.InputsOff();
        MyCameraLib.SetCameraFollow(playerTr, CameraScript._REGULAR_ORTHOGRAPHIC_SIZE);

        var trig = EventObjectPlayerPosition.AddAsGameObject(hub.transform).GetComponent<EventObjectPlayerPosition>();
        BoxCollider2D boxColl = trig.AddCollider<BoxCollider2D>();
        boxColl.size = new Vector2(Room.DoorWidth, 2f);
        trig.ActualPosition = hub.SouthSideOuter + new Vector3(0f, -boxColl.size.y / 2f);
        trig.AddAction(SceneChanger.SwitchToMenu);

        trig = EventObjectPlayerPosition.AddAsGameObject(hub.transform).GetComponent<EventObjectPlayerPosition>();
        boxColl = trig.AddCollider<BoxCollider2D>();
        boxColl.size = new Vector2(Room.DoorWidth, 1f);
        trig.ActualPosition = hub.SouthSideInner + new Vector3(0f, boxColl.size.y / 2f);
        trig.AddAction(hub.OpenSouthDoor);
        trig.Once = false;

        trig = EventObjectPlayerPosition.AddAsGameObject(hub.transform).GetComponent<EventObjectPlayerPosition>();
        boxColl = trig.AddCollider<BoxCollider2D>();
        boxColl.size = new Vector2(hub.Width, 3f);
        trig.ActualPosition = hub.SouthSideInner + new Vector3(0f, boxColl.size.y / 2f + 2f);
        trig.AddAction(hub.CloseSouthDoor);
        trig.Once = false;

        trig = EventObjectPlayerPosition.AddAsGameObject(hub.transform).GetComponent<EventObjectPlayerPosition>();
        boxColl = trig.AddCollider<BoxCollider2D>();
        boxColl.size = new Vector2(10f, 10f);
        trig.ActualPosition = hub.SouthSideCentre;
        trig.AddAction(() => playerMoveComponent.StartFromScratchNewEndpos(hub.ProportionalPosition(0f, -0.75f)));


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
        Destroy(hub);
        base.OnDestroy();
    }

}



