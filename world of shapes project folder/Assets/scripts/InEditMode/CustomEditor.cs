using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public abstract class ExtendedEditor : Editor
{
    protected List<ValueAndChanged> _changedList = new List<ValueAndChanged>();
    public sealed override void OnInspectorGUI()
    {
        if (targets.Length == 1)
        {
            _changedList.Clear();
            Undo.RecordObject(target, "revert changes on " + target.name);
            ShowScriptOnInspector();
            OnInspectorGUIExtend(target);
            serializedObject.ApplyModifiedProperties();
            if (GUI.changed && !Application.isPlaying)
            {
                ApplyChanges(target);
                EditorUtility.SetDirty(target);
                PrefabUtility.RecordPrefabInstancePropertyModifications(target);
            }
        }
        else if (TargetAreTheSameType())
        {
            _changedList.Clear();
            var currentTarget = targets[0];
            Undo.RecordObject(currentTarget, "revert changes on " + currentTarget.name);
            int undoID = Undo.GetCurrentGroup();
            ShowScriptOnInspector();
            OnInspectorGUIExtend(currentTarget);
            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
            {
                ApplyChanges(currentTarget);
                if (!Application.isPlaying)
                {
                    EditorUtility.SetDirty(currentTarget);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(currentTarget);
                }
                for (int i = targets.Length - 1; i > 0; --i)
                {
                    currentTarget = targets[i];
                    Undo.RecordObject(currentTarget, "revert changes on " + currentTarget.name);
                    Undo.CollapseUndoOperations(undoID);
                    ApplyChanges(currentTarget);
                    if (!Application.isPlaying)
                    {
                        EditorUtility.SetDirty(currentTarget);
                        PrefabUtility.RecordPrefabInstancePropertyModifications(currentTarget);
                    }
                }
            }
        }
    }

    protected abstract void ApplyChanges(Object currentTarget);

    private bool TargetAreTheSameType()
    {
        var firstTarget = targets[0];
        for(int i = targets.Length - 1; i > 0; --i)
        {
            if (targets[i].GetType() != firstTarget.GetType())
            {
                return false;
            }
        }
        return true;
    }

    protected void AddNewToList<T>(T newValue, T oldValue)
    {
        _changedList.Add(EqualityComparer<T>.Default.Equals(oldValue, newValue) ? new ValueAndChanged(null, false) : new ValueAndChanged(newValue, true));
    }

    private void ShowScriptOnInspector()
    {
        var propertyScript = serializedObject.FindProperty("m_Script");
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(propertyScript, new GUIContent("Script"), true);
        EditorGUI.EndDisabledGroup();
    }

    protected abstract void OnInspectorGUIExtend(Object currentTarget);


    protected void DrawProperties()
    {
        foreach (var property in serializedObject.ForEach())
        {
            DrawField(property);
        }
    }
    protected void DrawPropertiesExcept(string exclude)
    {
        foreach (var property in serializedObject.ForEach())
        {
            if (exclude != property.name && exclude != GetBackingFieldString(property.name))
            {
                DrawField(property);
            }
        }
    }
    protected void DrawProperty(string propertyName)
    {
        foreach (var property in serializedObject.ForEach())
        {
            if (propertyName == property.name || propertyName == GetBackingFieldString(property.name))
            {
                DrawField(property);
            }
        }
    }
    protected string GetBackingFieldString(string str) => "<" + str + ">k__BackingField";
    protected void DrawPropertiesExcept(IEnumerable<string> exclude)
    {
        if (exclude == null)
        {
            DrawProperties();
            return;
        }
        foreach (var property in serializedObject.ForEach())
        {
            bool excluded = false;
            foreach (var str in exclude)
            {
                if (str == property.name || str == GetBackingFieldString(property.name))
                {
                    excluded = true;
                    break;
                }
            }
            if (!excluded)
            {
                DrawField(property);
            }
        }
    }
    protected void DrawPropertiesExcept(params string[] exclude)
    {
        DrawPropertiesExcept((IEnumerable<string>)exclude);
    }

    protected void DrawField(SerializedProperty property, bool includeChildren = true, params GUILayoutOption[] options)
    {
        EditorGUILayout.PropertyField(property, new GUIContent(TidyUpString(property.name)), includeChildren, options);
    }

    protected static string TidyUpString(string str)
    {
        int i1 = str.IndexOf('<'), i2 = str.IndexOf('>');
        if (i1 != -1 && i2 != -1)
        {
            str = str.Substring(++i1, i2 - i1);
        }
        if (str[0] == '_')
        {
            str = str.Remove(0, 1);
        }
        str = str.Replace('_', ' ');
        string newStr = char.IsLetter(str[0]) && char.IsLower(str[0]) ? char.ToUpper(str[0]).ToString() : str[0].ToString();
        for (int i = 1; i < str.Length; ++i)
        {
            if (!char.IsLetter(str[i]))
            {
                newStr += str[i];
            }
            else if (char.IsLower(str[i]))
            {
                newStr += str[i - 1] == ' ' ? char.ToUpper(str[i]) : str[i];
            }
            else if (char.IsLetter(str[i - 1]) && char.IsLower(str[i - 1])) //if str[i] is a letter and not lower then its upper
            {
                newStr += " " + str[i];
            }
            else
            {
                newStr += str[i];
            }
        }
        return newStr;
    }

    protected struct ValueAndChanged
    {
        public readonly object Value;
        public readonly bool Changed;

        public ValueAndChanged(object value, bool changed)
        {
            Value = value;
            Changed = changed;
        }
    }

}


public static class MyInspectorLib
{
    public static IEnumerable<SerializedProperty> ForEach(this SerializedObject serObj, bool enterChildren = false)
    {
        var property = serObj.GetIterator();
        property.NextVisible(true);
        if (property.name != "m_Script") //skipping m_Script
        {
            yield return property;
        }
        while (property.NextVisible(enterChildren))
        {
            yield return property;
        }
    }
    private static IEnumerable<SerializedProperty> ForEachGetEverything(this SerializedObject serObj, bool enterChildren)
    {
        var property = serObj.GetIterator();
        if (property.Next(true))
        {
            do
            {
                yield return property;
            } while (property.Next(enterChildren));
        }
    }
    private static IEnumerable<SerializedProperty> ForEachGetEverythingOnlyVisible(this SerializedObject serObj, bool enterChildren)
    {
        var property = serObj.GetIterator();
        if (property.NextVisible(true))
        {
            do
            {
                yield return property;
            } while (property.NextVisible(enterChildren));
        }
    }
    public static IEnumerable<SerializedProperty> ForEach(this SerializedObject serObj, bool enterChildren, bool getEverything, bool onlyVisible = false)
    {
        if (!getEverything)
        {
            return serObj.ForEach(enterChildren);
        }
        else if (onlyVisible)
        {
            return serObj.ForEachGetEverything(enterChildren);
        }
        else
        {
            return serObj.ForEachGetEverythingOnlyVisible(enterChildren);
        }
    }
}
public static class DrawProperyMethods
{
    public static void DrawProperty(this SerializedProperty property)
    {
        var next = property.Copy();
        var propertyCopy = property.Copy();
        if (next.Next(false))
        {
            if (propertyCopy.Next(true)) do
                {
                    EditorGUILayout.PropertyField(propertyCopy, true);
                    propertyCopy.Next(false);
                } while (!SerializedProperty.EqualContents(propertyCopy, next));
        }
        else
        {
            if (propertyCopy.Next(true)) do
                {
                    EditorGUILayout.PropertyField(propertyCopy, true);
                } while (propertyCopy.Next(false));
        }
    }
    public static void DrawPropertyExcept(this SerializedProperty property, params string[] exceptions)
    {
        var next = property.Copy();
        var propertyCopy = property.Copy();
        if (next.Next(false))
        {
            if (propertyCopy.Next(true)) do
                {
                    bool excluded = false;
                    foreach (var str in exceptions)
                    {
                        if (str == property.name)
                        {
                            excluded = true;
                            break;
                        }
                    }
                    if (!excluded) EditorGUILayout.PropertyField(propertyCopy, true);
                    propertyCopy.Next(false);
                } while (!SerializedProperty.EqualContents(propertyCopy, next));
        }
        else
        {
            if (propertyCopy.Next(true)) do
                {
                    bool excluded = false;
                    foreach (var str in exceptions)
                    {
                        if (str == property.name)
                        {
                            excluded = true;
                            break;
                        }
                    }
                    if (!excluded) EditorGUILayout.PropertyField(propertyCopy, true);
                } while (propertyCopy.Next(false));
        }
    }

    public static float GetHeight(this SerializedProperty property)
    {
        float height = 0f;
        var next = property.Copy();
        var propertyCopy = property.Copy();
        if (next.Next(false))
        {
            if (propertyCopy.Next(true)) do
                {
                    height += EditorGUI.GetPropertyHeight(propertyCopy, true);
                    propertyCopy.Next(false);
                } while (!SerializedProperty.EqualContents(propertyCopy, next));
        }
        else
        {
            if (propertyCopy.Next(true)) do
                {
                    height += EditorGUI.GetPropertyHeight(propertyCopy, true);
                } while (propertyCopy.Next(false));
        }
        return height;
    }
}

public class ReadOnlyOnInspector : PropertyAttribute { }
[CustomPropertyDrawer(typeof(ReadOnlyOnInspector))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginDisabledGroup(true);
        EditorGUI.PropertyField(position, property, label);
        EditorGUI.EndDisabledGroup();
    }
}

public class ReadOnlyOnInspectorDuringPlay : PropertyAttribute { }
[CustomPropertyDrawer(typeof(ReadOnlyOnInspectorDuringPlay))]
public class ReadOnlyDuringPlayDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (Application.isPlaying)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(position, property, label);
            EditorGUI.EndDisabledGroup();
        }
        else
        {
            EditorGUI.PropertyField(position, property, label);
        }
    }
}

#else

public class ReadOnlyOnInspector : System.Attribute { }

public class ReadOnlyOnInspectorDuringPlay : System.Attribute { }

#endif


