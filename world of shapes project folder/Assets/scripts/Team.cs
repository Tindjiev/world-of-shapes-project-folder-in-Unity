using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour, IEnumerable<BaseCharacterControl>, ISerializationCallbackReceiver
{

    [SerializeField]
    private Color _teamColor = MyColorLib.BASE_COLOR_TO_REPLACE;
    public Color TeamColor => _teamColor;

    [field: SerializeField]
    public DialogueAudioClips AudioClips { get; private set; }

    [SerializeField]
    private List<BaseCharacterControl> _canTarget = new List<BaseCharacterControl>();

    [SerializeField]
    private Team[]
        _enemyTeams = new Team[0],
        _alliedTeams = new Team[0],
        _neutralTeams = new Team[0];

    public MyLib.ReadOnlyList<Team> EnemyTeams => _enemyTeams;
    public MyLib.ReadOnlyList<Team> AlliedTeams => _alliedTeams;
    public MyLib.ReadOnlyList<Team> NeutralTeams => _neutralTeams;


    protected void Awake()
    {
        if (_canTarget == null || _canTarget.Count == 0)
        {
            SetCanTargetAllEnemiesFromScratch();
        }
    }


    public bool CheckIfCanBeTargeted(BaseCharacterControl potentialTarget)
    {
        return _canTarget.Contains(potentialTarget, true);
    }


    public void SetCanTargetAllEnemiesFromScratch()
    {
        _canTarget.Clear();
        foreach (var team in _enemyTeams)
        {
            foreach (var character in team)
            {
                _canTarget.Add(character);
            }
        }
    }


    public IEnumerator<BaseCharacterControl> GetEnumerator()
    {
        return ((IEnumerable<BaseCharacterControl>)transform.GetComponentsInChildren<BaseCharacterControl>()).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    public void CheckAndFixTeams()
    {
        MyLib.ArrayRemoveDuplicates(ref _enemyTeams);
        MyLib.ArrayRemoveDuplicates(ref _alliedTeams);
        MyLib.ArrayRemoveDuplicates(ref _neutralTeams);

        MyLib.ArrayRemoveOne(ref _enemyTeams, this);
        MyLib.ArrayRemoveOne(ref _neutralTeams, this);

        int index = _alliedTeams.FindIndex(this);
        if (index == -1)
        {
            MyLib.ArrayInsert(ref _alliedTeams, this, 0);
        }
        else
        {
            _alliedTeams[index] = _alliedTeams[0];
            _alliedTeams[0] = this;
        }
    }

    public override string ToString() => name;

    public void OnBeforeSerialize()
    {
        CheckAndFixTeams();
    }

    public void OnAfterDeserialize()
    {
        CheckAndFixTeams();
    }



    /*
#if UNITY_EDITOR

[UnityEditor.CustomEditor(typeof(teamvariables), true)]
private class TeamEditor : ExtendedEditor
{
   private static bool _showAllies = true;
   private teamvariables _team;

   void OnEnable()
   {
       _team = (teamvariables)target;
   }

   protected override void OnInspectorGUIExtend()
   {
       DrawPropertiesExcept(nameof(_enemyTeams), nameof(_alliedTeams), nameof(_neutralTeams));
       DrawProperty(nameof(_enemyTeams));


       _showAllies = UnityEditor.EditorGUILayout.Foldout(_showAllies, TidyUpString(nameof(_alliedTeams)));
       if (_showAllies)
       {
           var alliedTeamsProperty = serializedObject.FindProperty(nameof(_alliedTeams));
           var length = alliedTeamsProperty.arraySize;

           UnityEditor.EditorGUI.indentLevel++;

           var newLength = UnityEditor.EditorGUILayout.DelayedIntField("Size", length);
           UnityEditor.EditorGUI.BeginDisabledGroup(true);
           UnityEditor.EditorGUILayout.PropertyField(alliedTeamsProperty.GetArrayElementAtIndex(0));
           UnityEditor.EditorGUI.EndDisabledGroup();
           for (int i = 1; i < length; ++i)
           {
               UnityEditor.EditorGUILayout.PropertyField(alliedTeamsProperty.GetArrayElementAtIndex(i));
           }
           if (newLength != length)
           {
               alliedTeamsProperty.arraySize = newLength;
           }


           //walls.NextVisible(true);
           //walls.NextVisible(false);
           //EditorGUILayout.PropertyField(walls, new GUIContent("North Wall"), true);
           //DrawOpenCloseButtons();
           //walls.NextVisible(false);
           //EditorGUILayout.PropertyField(walls, new GUIContent("East Wall"), true);
           //DrawOpenCloseButtons();
           //walls.NextVisible(false);
           //EditorGUILayout.PropertyField(walls, new GUIContent("South Wall"), true);
           //DrawOpenCloseButtons();
           //walls.NextVisible(false);
           //EditorGUILayout.PropertyField(walls, new GUIContent("West Wall"), true);
           //DrawOpenCloseButtons();


           UnityEditor.EditorGUI.indentLevel--;
       }

       DrawProperty(nameof(_neutralTeams));
   }
}
#endif
*/
}
