using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GenerateAstarFloor : MonoBehaviour
{
    [SerializeField]
    private GameObject _roomPrefab = null;

    void Awake()
    {
        enabled = false;
    }


    void Update()
    {
        enabled = false;
        Floor.SetRoomPrefab(_roomPrefab);
        if (FindObjectOfType<Floor>() != null)
        {
            DestroyImmediate(FindObjectOfType<Floor>().gameObject);
        }
        Room.DoorType = "move";
        Room.WallFatness = 10f;
        //        floor = floor.createfloor(gameObject, new short[][] { new short[] { floor.shape_square, 0, 0, 10, (short)(10 * Screen.width / 1920), 0 } });
        Floor fl = Floor.CreateFloor(gameObject, (short)(10 * Screen.width / 1920), 10, 50f, 50f);
        fl.transform.parent = transform;
        Camera.main.orthographicSize = CameraScript.BIRDSEYE_VIEW;
        return;
        /*
        if (fl.rooms.GetLength(0).isEven())
        {
            Cameralib.SetCameraPosition(fl.rooms[fl.rooms.GetLength(0) >> 1, fl.rooms.GetLength(1) - 1].position - new Vector3(0f, fl.rooms[0, 0].height / 2f));
        }
        else
        {
            Cameralib.SetCameraPosition(fl.rooms[fl.rooms.GetLength(0) >> 1, fl.rooms.GetLength(1) - 1].position);
        }
        */
    }
}
