using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine;

[ExecuteInEditMode]
public class MaintainCharacteristicsInEditor : MonoBehaviour
{
#if UNITY_EDITOR
#pragma warning disable CS0414

    [SerializeField]
    private Transform[] _teams = null;
    [SerializeField]
    private GameObject _nullObject = null;

    [SerializeField]
    private bool _hideWeapons = true;

    [SerializeField]
    private List<Texture2D> _generatedTextures = null;
    [SerializeField]
    private List<MyColorLib.ColoredSpriteList> _coloredSprites = null;

#pragma warning restore CS0414
    private void Awake()
    {
        if (Application.isPlaying)
        {
            enabled = false;
        }
        SetImagesList();
    }

    private void OnEnable()
    {
        CleanseGeneratedImagesNonStatic();
    }



    private void Update()
    {
        SetImagesList();
        if (MyColorLib.ColoredSprites == null || MyColorLib.ColoredSprites.Count == 0)
        {
            MyColorLib.ColoredSprites = _coloredSprites;
        }
        else if (_coloredSprites == null)
        {
            _coloredSprites = MyColorLib.ColoredSprites;
        }
        bool changedHappened = false;
        MyColorLib.teams = _teams;
        UpdateTeamColors();
        var player = FindObjectOfType<PlayerControlBattle>();
        if (player != null)
        {
            var cam = FindObjectOfType<CameraScript>();
            if (cam != null && cam.FollowTransform != player.MoveComponent.transform)
            {
                cam.FollowTransform = player.MoveComponent.transform;
                changedHappened = true;
                PrefabUtility.RecordPrefabInstancePropertyModifications(cam);
            }
        }
        //CheckAndSetDoorStates();
        HideWeaponsInEditor();
        if (changedHappened)
        {
            EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            PrefabUtility.RecordPrefabInstancePropertyModifications(this);
        }
    }

    private void SetImagesList()
    {
        if (MyColorLib.GeneratedTextures == null || MyColorLib.GeneratedTextures.Count == 0)
        {
            MyColorLib.GeneratedTextures = _generatedTextures;
        }
        else if (_generatedTextures == null)
        {
            _generatedTextures = MyColorLib.GeneratedTextures;
        }
    }


    private static void UpdateTeamColors()
    {
        foreach (var skin in FindObjectsOfType<SkinManager>())
        {
            skin.ReplaceColors();
        }
    }


    private bool _weaponsUnHidden = false;
    private void HideWeaponsInEditor()
    {
        if (_hideWeapons)
        {
            _weaponsUnHidden = false;
            foreach (var attack in FindObjectsOfType<Attack>())
            {
                SceneVisibilityManager.instance.Hide(attack.gameObject, true);
            }
        }
        else if (!_weaponsUnHidden)
        {
            _weaponsUnHidden = true;
            foreach (var attack in FindObjectsOfType<Attack>())
            {
                SceneVisibilityManager.instance.Show(attack.gameObject, true);
            }
        }
    }

    /*
    private void CheckAndSetDoorStates()
    {
        Floor floor = FindObjectOfType<Floor>();
        if (floor != null)
        {
            foreach(Room room in floor)
            {
                foreach(Door door in (IEnumerable<Door>)room)
                {
                    if (door.HasNeighbor && door.Closed != door.NeighborDoor.Closed)
                    {
                        if (door.gameObject.activeSelf != door.Closed) //if this door gameobject active != Closed means that Closed changed from inspector
                        {
                            if (door.CheckAndForceDoorState()) _changedHappened = true;
                        }
                        else if (door.NeighborDoor.gameObject.activeSelf != door.NeighborDoor.Closed)
                        {
                            if (door.NeighborDoor.CheckAndForceDoorState()) _changedHappened = true;
                        }
                    }
                    else
                    {
                        if (door.CheckAndForceDoorState()) _changedHappened = true;
                    }
                }
            }
        }
    }
    */

    public class OnSaveClass : UnityEditor.AssetModificationProcessor
    {
        public static string[] OnWillSaveAssets(string[] paths)
        {
            CleanseGeneratedImages();
            return paths;
        }
    }

    [InitializeOnLoad]
    private class UnHideWeapons
    {
        static UnHideWeapons()
        {
            foreach (var attack in FindObjectsOfType<Attack>())
            {
                SceneVisibilityManager.instance.Show(attack.gameObject, true);
            }
        }
    }


    private void CleanseGeneratedImagesNonStatic()
    {
        //Debug.Log("cleaning");
        MyColorLib.GeneratedTextures = _generatedTextures;
        MyColorLib.ColoredSprites = _coloredSprites;
        CleanseGeneratedImagesStaticPart();
    }
    public static void CleanseGeneratedImages()
    {
        var temp = FindObjectOfType<MaintainCharacteristicsInEditor>();
        if (temp != null)
        {
            temp.CleanseGeneratedImagesNonStatic();
        }
        else
        {
            CleanseGeneratedImagesStaticPart();
        }
    }

    private static void CleanseGeneratedImagesStaticPart()
    {
        foreach (var skin in FindObjectsOfType<SkinManager>())
        {
            skin.ReplaceWithOriginalColor();
        }
        for (int i = 0; i < MyColorLib.ColoredSprites.Count; ++i)
        {
            for (int j = 0; j < MyColorLib.ColoredSprites[i].ColoredSprites.Count; ++j)
            {
                var sprite = MyColorLib.ColoredSprites[i].ColoredSprites[j].sprite;
                if (sprite != null && MyColorLib.GeneratedTextures.Contains(sprite.texture))
                {
                    DestroyImmediate(sprite);
                }
            }
        }
        MyColorLib.ColoredSprites.ForEach(x => x.ColoredSprites.Clear());
        MyColorLib.ColoredSprites.Clear();
        for (int i = 0; i < MyColorLib.GeneratedTextures.Count; ++i)
        {
            DestroyImmediate(MyColorLib.GeneratedTextures[i]);
        }
        MyColorLib.GeneratedTextures.Clear();
    }

    [CustomEditor(typeof(MaintainCharacteristicsInEditor), true)]
    private class MaintainEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Reset generated textures list"))
            {
                CleanseGeneratedImages();
                EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            }
        }
    }


#endif
}