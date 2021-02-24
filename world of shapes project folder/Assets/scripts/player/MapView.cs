using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapView : MonoBehaviour
{
    private bool _changed = false;

    private Color _roomColor;
    private Room _lastCurrentRoom = null;
    private float _cameraSize;

    private Floor _currentFloor;
    private PlayerBaseControl _player;

    private const float _TIME_TO_SWITCH = 0.4f;
    private Timer _currentRoomFlashingMapTimer = new Timer(_TIME_TO_SWITCH);

    private bool _mapField = false;
    private bool _mapActive
    {
        get => _mapField;
        set
        {
            _mapField = value;
            if (_changed)
            {
                _changed = false;
                _lastCurrentRoom.Color = _roomColor;
            }
        }
    }

    private GUIStyle _mapTextStyle = new GUIStyle();

    private void Start()
    {
        _player = ControlBase.PlayerGameObject.GetCharacter() as PlayerBaseControl;
        _currentFloor = FindObjectOfType<Floor>();


        _mapTextStyle = new GUIStyle();
        _mapTextStyle.normal.textColor = Color.red;
    }

    private void LateUpdate()
    {
        if (_mapActive)
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                MyCameraLib.SetCameraFollow(_player.MoveComponent.transform, _cameraSize);
                _mapActive = false;
                return;
            }
            ShowroomOnMap();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.M) && CameraScript.showPlayerInterface)
            {
                _cameraSize = Camera.main.orthographicSize;
                MyCameraLib.SetCameraStaticPosition(_currentFloor.GetpositionFromIndexes(_currentFloor.LengthI >> 1, _currentFloor.LengthJ >> 1), CameraScript.BIRDSEYE_VIEW);
                _mapActive = true;
                return;
            }
        }
    }

    private void ShowroomOnMap()
    {
        Room currentroom = _currentFloor.GetClosestRoomFromPosition(_player.Position);
        if (_lastCurrentRoom != null && _lastCurrentRoom != currentroom)
        {
            _lastCurrentRoom.Color = _roomColor;
            _roomColor = currentroom.Color;
        }
        _lastCurrentRoom = currentroom;
        if (_currentRoomFlashingMapTimer.CheckIfTimePassed)
        {
            _currentRoomFlashingMapTimer.StartTimer();
            if (!_changed)
            {
                _roomColor = currentroom.Color;
                currentroom.Color = _player.TeamColor;
                _changed = true;
            }
            else
            {
                currentroom.Color = _roomColor;
                _changed = false;
            }
        }
    }

    private void OnGUI()
    {
        _mapTextStyle.fontSize = Screen.width / 70;
        if (!_mapActive)
        {
            _mapTextStyle.alignment = TextAnchor.LowerRight;
            GUI.Label(new Rect(Screen.width * 0.95f, Screen.height * 0.95f, 0f, 0f), "Press M to view the layout of the floor\n\nBackspace + Escape to go back to menu\n(Resets progress)                ", _mapTextStyle);
        }
        else
        {
            _mapTextStyle.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(Screen.width >> 1, Screen.height * 0.9f, 0f, 0f), "Press M to switch to normal view", _mapTextStyle);
        }
    }

}
