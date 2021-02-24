using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



[ExecuteInEditMode]
public class FloorGenerator : MonoBehaviour
{

#if UNITY_EDITOR
    [SerializeField]
    GameObject _gameObjectToAddFloor = null;

    [SerializeField]
    private float _rommWidth = 50f, _roomHeight = 50f;
    [SerializeField]
    private bool DoorsOpen = true;
    [SerializeField]
    private Floor.FloorGenerationInfoBase[] _partsOfFloor = null;
    [SerializeField]
    private byte[] _doorsStateInfo;

    [SerializeField]
    private bool _generateFloor = false, _setAllDoors = false, _saveDoors = false, _loadDoors = false, _fixFloor = false;


    private void Update()
    {
        if (_fixFloor)
        {
            _fixFloor = false;
            var floor = Rules.Floor;
            if (floor != null) floor.FixFloor();
        }
        if (_generateFloor)
        {
            _generateFloor = false;
            _setAllDoors = true;
            if (FindObjectOfType<Floor>() != null)
            {
                DestroyImmediate(FindObjectOfType<Floor>().gameObject);
            }
            List<Floor.FloorGenerationInfoBase> partsOfFloor = new List<Floor.FloorGenerationInfoBase>();
            foreach (var thing in _partsOfFloor)
            {
                if (thing.Include) partsOfFloor.Add(thing);
            }
            if (partsOfFloor.Count == 0) return;
            Room.WallFatness = 10f;
            var floor = Floor.CreateFloor(_gameObjectToAddFloor, partsOfFloor.ToArray(), _rommWidth, _roomHeight);

            foreach (var room in FindObjectsOfType<Room>())
            {
                PrefabUtility.RecordPrefabInstancePropertyModifications(room);
            }
            DIRT();
        }
        if (_setAllDoors)
        {
            _setAllDoors = false;
            Floor floor = FindObjectOfType<Floor>();
            if (floor != null)
            {
                floor.SetAllDoors(DoorsOpen);
            }
            DIRT();
        }
        if (_saveDoors)
        {
            _saveDoors = false;
            Floor floor = FindObjectOfType<Floor>();
            if (floor != null)
            {
                _doorsStateInfo = floor.SaveDoors();
            }
            DIRT();
        }
        if (_loadDoors)
        {
            _loadDoors = false;
            Floor floor = FindObjectOfType<Floor>();
            if (floor != null)
            {
                floor.LoadDoors(_doorsStateInfo);
            }
            foreach(Room room in floor)
            {
                foreach(Door door in (IEnumerable<Door>)room)
                {
                    EditorUtility.SetDirty(door.gameObject);
                }
            }
            DIRT();
        }
    }


    private void DIRT()
    {
        PrefabUtility.RecordPrefabInstancePropertyModifications(this);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        //EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

#endif
}