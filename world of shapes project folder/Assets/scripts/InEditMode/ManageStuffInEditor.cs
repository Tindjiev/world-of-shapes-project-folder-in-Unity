using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Side = Room.Side;

public static partial class ManageStuffInEditor
{
    private static Room _roomSelected;
    public static Transform Team { get; private set; } = null;

#if UNITY_EDITOR
    [MenuItem("MyMenu/Select writer #q", false, 0)]
    public static void SelectInputWriter()
    {
        if (Application.isPlaying) return;
        Selection.objects = new Object[] { AddInEditor.EditorGameObject };
    }

    //[MenuItem("GameObject/Move/Move to room", false, 0)] //when clicking for this option selections are diselected, so it can only be done with a hotkey
    [MenuItem("MyMenu/Move to room #p", false, 0)]
    private static void SetObjectsToRoomMenuItem()
    {
        if (Application.isPlaying) return;
        SetObjectsToRoom();
    }

    public static void SetObjectsToRoom(Room room = null)
    {
        if (room == null) room = GetOneRoomFromEditorSelection(false);
        List<BaseCharacterControl> characters = new List<BaseCharacterControl>();
        foreach (var thing in Selection.objects)
        {
            BaseCharacterControl tempChar = thing.GetCharacter();
            if (tempChar != null)
            {
                characters.Add(tempChar);
            }
        }
        if (characters.Count == 0)
        {
            throw new Exception("No characters selected");
        }
        Floor floor = MonoBehaviour.FindObjectOfType<Floor>();
        foreach (var character in characters)
        {
            Room tempRoom = floor.GetRoomFromPosition(character.MoveComponent.Position);
            if (tempRoom != null)
            {
                character.MoveComponent.SetPosition(character.MoveComponent.Position - tempRoom.Position + room.Position);
            }
            else
            {
                character.MoveComponent.SetPosition(room.Position);
            }
        }
        List<Object> temp = new List<Object>(Selection.objects);
        temp.RemoveAll(x => (x as GameObject).SearchComponent<Room>() != null);
        Selection.objects = temp.ToArray();
        Debug.Log("moved characters to room", room);
    }

    [MenuItem("MyMenu/Select bodies of selected _m", false, 0)]
    public static void SelectBodies()
    {
        if (Application.isPlaying) return;
        var temp = Selection.objects;
        //SelectCharacters();
        List<Object> Characters = new List<Object>();
        foreach (var thing in temp)
        {
            BaseCharacterControl tempChar = thing.GetCharacter();
            if (tempChar == null)
            {
                //throw new Exception("Selected something that is not a character");
                continue;
            }
            GameObject tempGO = tempChar.MoveComponent.gameObject;
            if (!Characters.Contains(tempGO))
            {
                Characters.Add(tempGO);
            }
        }
        Selection.objects = Characters.ToArray();
        //SceneView.FrameLastActiveSceneView();
        Tools.current = Tool.Move;
        Debug.Log("Selected bodies of selected characters");
    }

    [MenuItem("MyMenu/Select characters of selected &m", false, 0)]
    public static void SelectCharacters()
    {
        List<Object> Characters = new List<Object>();
        foreach (var thing in Selection.objects)
        {
            BaseCharacterControl tempChar = thing.GetCharacter();
            if (tempChar == null)
            {
                //throw new Exception("Selected something that is not a character");
                continue;
            }
            GameObject tempGO = tempChar.gameObject;
            if (!Characters.Contains(tempGO))
            {
                Characters.Add(tempGO);
            }
        }
        Selection.objects = Characters.ToArray();
        SceneView.FrameLastActiveSceneView();
        Tools.current = Tool.Move;
        Debug.Log("Selected bodies of selected characters");
    }



    [MenuItem("MyMenu/Hidden selection/Select room #r", false, 0)]
    public static void SelectRoom()
    {
        if (Application.isPlaying) return;
        _roomSelected = GetOneRoomFromEditorSelection();
        Debug.Log("Room selected", _roomSelected);
    }

    [MenuItem("MyMenu/Hidden selection/Diselect room &r", false, 0)]
    public static void DiselectRoom()
    {
        if (Application.isPlaying) return;
        _roomSelected = null;
        Debug.Log("Room diselected");
    }

    [MenuItem("MyMenu/Hidden selection/Select team #t", false, 0)]
    public static void SelectTeam()
    {
        if (Application.isPlaying) return;
        Team = GetTeamFromSelection();
        if (Team == null) throw new Exception("No team selected");
        Debug.Log("Team selected", Team);
    }

    [MenuItem("MyMenu/Hidden selection/Diselect team &t", false, 0)]
    public static void DiselectTeam()
    {
        if (Application.isPlaying) return;
        Team = null;
        Debug.Log("Team diselected");
    }




    public static Room GetOneRoomFromEditorSelection(bool ErrorOnOtherThanRoom = true)
    {
        Room room = null;
        foreach (var thing in Selection.objects)
        {
            Room tempRoom = thing.SearchComponent<Room>();
            if (ErrorOnOtherThanRoom && tempRoom == null)
            {
                throw new EditorExceptions.NotRoomException();
            }
            else if (room != null && room != tempRoom)
            {
                throw new EditorExceptions.MoreThanOneRoomException();
            }
            room = tempRoom;
        }
        if (room == null)
        {
            throw new EditorExceptions.NoRoomException();
        }
        return room;
    }
    public static IEnumerable<Room> GetManyRoomsFromEditorSelection(bool ErrorOnOtherThanRoom = true)
    {
        List<Room> rooms = new List<Room>();
        foreach (var thing in Selection.objects)
        {
            Room tempRoom = thing.SearchComponent<Room>();
            if (ErrorOnOtherThanRoom && tempRoom == null)
            {
                throw new EditorExceptions.NotRoomException();
            }
            if (tempRoom != null)
            {
                rooms.Add(tempRoom);
            }
        }
        if (rooms.Count == 0)
        {
            throw new EditorExceptions.NoRoomException();
        }
        return rooms.Distinct();
    }

    public static Room GetRoomFromBothSelections(bool ErrorOnOtherThanRoom = true)
    {
        Room room;
        try
        {
            room = GetOneRoomFromEditorSelection(ErrorOnOtherThanRoom);
        }
        catch (EditorExceptions.NoRoomException)
        {
            room = null;
        }
        if (room == null)
        {
            room = _roomSelected;
            if (room == null) throw new EditorExceptions.NoRoomException();
        }
        return room;
    }



    public static Transform GetTeamFromSelection()
    {
        Transform team = null;
        foreach (var thing in Selection.objects)
        {
            Team teamTemp = (thing as GameObject).GetTeam();
            if (teamTemp == null) continue;
            if (team != null && team != teamTemp.transform)
            {
                throw new EditorExceptions.MoreThanOneRoomException();
            }
            team = teamTemp.transform;
        }
        return team;
    }
    public static BaseCharacterControl GetCharacterFromSelection()
    {
        foreach (var thing in Selection.objects)
        {
            var charTemp = (thing as GameObject).GetCharacter();
            if (charTemp != null)
            {
                return charTemp;
            }
        }
        return null;
    }



    private static void OpenCloseDoors(Side side, bool openDoor)
    {
        EditorLib.DoMenuItemFunctionOnce(() =>
        {
            GetManyRoomsFromEditorSelection().MyForEach(x => {
                x.ChangeDoorStateImmediate(side, openDoor);
                EditorUtility.SetDirty(x);
                if (x.HasNeighbor(side)) EditorUtility.SetDirty(x[side]);
            });
        });
    }
    private static void OpenCloseDoors(bool openDoor)
    {
        EditorLib.DoMenuItemFunctionOnce(() =>
        {
            GetManyRoomsFromEditorSelection().MyForEach(room => {
                room.ChangeDoorStateImmediate(openDoor);
                EditorUtility.SetDirty(room);
                foreach(var neighbor in room) EditorUtility.SetDirty(neighbor);
            });

        });
    }

    [MenuItem("GameObject/Room Changes/North/Open", false, 0)]
    public static void OpenNorthDoor()
    {
        OpenCloseDoors(Side.North, true);
    }

    [MenuItem("GameObject/Room Changes/North/Close", false, 0)]
    public static void CloseNorthDoor()
    {
        OpenCloseDoors(Side.North, false);
    }

    [MenuItem("GameObject/Room Changes/East/Open", false, 0)]
    public static void OpenEastDoor()
    {
        OpenCloseDoors(Side.East, true);
    }

    [MenuItem("GameObject/Room Changes/East/Close", false, 0)]
    public static void CloseEastDoor()
    {
        OpenCloseDoors(Side.East, false);
    }

    [MenuItem("GameObject/Room Changes/South/Open", false, 0)]
    public static void OpenSouthDoor()
    {
        OpenCloseDoors(Side.South, true);
    }

    [MenuItem("GameObject/Room Changes/South/Close", false, 0)]
    public static void CloseSouthDoor()
    {
        OpenCloseDoors(Side.South, false);
    }

    [MenuItem("GameObject/Room Changes/West/Open", false, 0)]
    public static void OpenWestDoor()
    {
        OpenCloseDoors(Side.West, true);
    }

    [MenuItem("GameObject/Room Changes/West/Close", false, 0)]
    public static void CloseWestDoor()
    {
        OpenCloseDoors(Side.West, false);
    }

    [MenuItem("GameObject/Room Changes/All Doors/Open", false, 0)]
    public static void OpenAllDoors()
    {
        OpenCloseDoors(true);
    }

    [MenuItem("GameObject/Room Changes/All Doors/Close", false, 0)]
    public static void CloseAllDoors()
    {
        OpenCloseDoors(false);
    }
#endif

    public static void EditorDestroy(this Object obj)
    {
        if (obj is Transform)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Object.DestroyImmediate((obj as Transform).gameObject);
            }
            else
            {
                Object.Destroy((obj as Transform).gameObject);
            }
#else
            Object.Destroy((obj as Transform).gameObject);
#endif
        }
        else
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Object.DestroyImmediate(obj);
            }
            else
            {
                Object.Destroy(obj);
            }
#else
            Object.Destroy(obj);
#endif
        }
    }

    private static T SearchComponent<T>(this Object obj) where T : Component
    {
        return SearchComponentExtensions.SearchComponent<T>(obj as GameObject);
    }
    private static BaseCharacterControl GetCharacter(this Object obj)
    {
        return SearchComponentExtensions.GetCharacter(obj as GameObject);
    }

    private static void MyForEach<T>(this IEnumerable<T> Iter, Action<T> action)
    {
        foreach(var thing in Iter)
        {
            action(thing);
        }
    }
}



public static class EditorExceptions
{
    public abstract class EditorException : Exception
    {
        protected EditorException(string message) : base(message)
        {
        }
    }
    public abstract class RoomSelectionException : EditorException
    {
        protected RoomSelectionException(string message) : base(message)
        {
        }
    }

    public class NoRoomException : RoomSelectionException
    {
        public NoRoomException() : base("No room selected")
        {
        }
        protected NoRoomException(string message) : base(message)
        {
        }
    }
    public class MoreThanOneRoomException : RoomSelectionException
    {
        public MoreThanOneRoomException() : base("More than one room selected")
        {
        }
        protected MoreThanOneRoomException(string message) : base(message)
        {
        }
    }
    public class NotRoomException : RoomSelectionException
    {
        public NotRoomException() : base("Selected something thatis not a room")
        {
        }
        protected NotRoomException(string message) : base(message)
        {
        }
    }

    public class NothingSelected : EditorException
    {
        public NothingSelected() : base("There isn't anything selected")
        {
        }
        protected NothingSelected(string message) : base(message)
        {
        }
    }

    public class SomethingSelected : EditorException
    {
        public SomethingSelected() : base("There is something selected")
        {
        }
        protected SomethingSelected(string message) : base(message)
        {
        }
    }

    public class MoreThanOneSelected : EditorException
    {
        public MoreThanOneSelected() : base("There is more than one Object selected")
        {
        }
        protected MoreThanOneSelected(string message) : base(message)
        {
        }
    }
}