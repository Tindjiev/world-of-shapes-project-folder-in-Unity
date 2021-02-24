using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Dialogue;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;

public class DialogueGraphEditor : EditorWindow
{
    private List<DialogueGraphNodeWindow> _windows = new List<DialogueGraphNodeWindow>();
    private List<Texture2D> _generatedTextures = new List<Texture2D>();

    private DialogueGraphNodeWindow _nodeSelected = null;

    private static DialogueGraphEditor _graphEditor = null;

    private Texture2D _normalNodeBackground = null, _selectedNodeBackground = null, _rootNodeBackground = null;

    private GUIStyle _deleteButtonStyle = null, _attachButtonStyle = null, _detachButtonStyle = null, _setAsRootButtonStyle = null;
    private const float  _WINDOW_WIDTH_FULL = 300f, _WINDOW_HEIGHT_FULL = 410f;
    private static readonly Rect _WINDOW_RECT = new Rect(10f, 10f, _WINDOW_WIDTH_FULL, _WINDOW_HEIGHT_FULL);

    [SerializeField, HideInInspector]
    private bool _hasBeenSetUp = false;
    private static bool _newGraph = false;
    public static void ShowEditor(DialogueGraph graph)
    {
        _staticGraph = graph != null ? graph : new DialogueGraph();
        _newGraph = true;

        _graphEditor = GetWindow<DialogueGraphEditor>();
        _graphEditor._windows.Clear();
        int i = 0, j = 0;
        foreach (var node in (IEnumerable<DialogueGraph.Node>)graph)
        {
            _graphEditor._windows.Add(new DialogueGraphNodeWindow(node,
                new Rect(_WINDOW_RECT.position + new Vector2((_WINDOW_WIDTH_FULL + 5f) * i, (_WINDOW_HEIGHT_FULL + 5f) * j), _WINDOW_RECT.size)));
            ++i;
            if (300 * i > Screen.width << 1)
            {
                i = 0;
                j++;
            }
        }
        AssemblyReloadEvents.beforeAssemblyReload += _graphEditor.Close;

    }

    private void OnEnable()
    {
        if (_newGraph || _graph == null)
        {
            _graph = _staticGraph;
        }

        _hasBeenSetUp = false;
    }

    private void SetUpFields()
    {

        _normalNodeBackground = CreateTexture(Color.cyan * 0.7f);

        _selectedNodeBackground = CreateTexture(new Color(1f, 1f, 0.5f));

        _rootNodeBackground = CreateTexture(new Color(0f, 1f, 0.5f) * 0.7f);

        _setAsRootButtonStyle = new GUIStyle(GUI.skin.button);
        _setAsRootButtonStyle.normal.background = CreateTexture(new Color(0f, 1f, 0.5f) * 0.8f);
        _setAsRootButtonStyle.active.background = CreateTexture(new Color(0f, 1f, 0.5f));

        _deleteButtonStyle = new GUIStyle(GUI.skin.button);
        _deleteButtonStyle.normal.background = CreateTexture(Color.red);
        _deleteButtonStyle.active.background = CreateTexture(Color.black);
        _deleteButtonStyle.active.textColor = Color.white;

        _attachButtonStyle = new GUIStyle(GUI.skin.button);
        _attachButtonStyle.normal.textColor = Color.green * 0.7f;
        _attachButtonStyle.hover.textColor = Color.green * 0.7f;
        _attachButtonStyle.active.textColor = Color.green * 0.7f;

        _detachButtonStyle = new GUIStyle(GUI.skin.button);
        _detachButtonStyle.normal.textColor = Color.red;
        _detachButtonStyle.hover.textColor = Color.red;
        _detachButtonStyle.active.textColor = Color.red;

    }

    private Texture2D CreateTexture(Color color)
    {
        var texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        _generatedTextures.Add(texture);
        return texture;
    }

    [SerializeField]
    private DialogueGraph _graph = null;
    private static DialogueGraph _staticGraph = null;//used to transfer graph from static method to _graph field

    [Serializable]
    public class DialogueGraphNodeWindow
    {
        public DialogueGraph.Node Node;
        public Rect Rect;

        public bool FirstTime = true;

        public DialogueStruct Dialogue => Node.Value;

        private static Dictionary<DialogueGraph.Node, DialogueGraphNodeWindow> _dict = new Dictionary<Graph<DialogueStruct>.Node, DialogueGraphNodeWindow>();

        public static DialogueGraphNodeWindow GetNodeWindow(DialogueGraph.Node node) => node != null ? _dict[node] : null;

        public DialogueGraphNodeWindow(DialogueGraph.Node node, Rect rect)
        {
            Node = node;
            Rect = rect;
            _dict.Remove(node);
            _dict.Add(node, this);
        }
    }

    private void OnGUI()
    {
        if (!_hasBeenSetUp)
        {
            _hasBeenSetUp = true;
            SetUpFields();
        }

        BeginWindows();

        DrawGraphPropertiesAndOptions();

        int i = 0;

        foreach (var windowNode in _windows)
        {
            windowNode.Rect = GUI.Window(i, windowNode.Rect, (id) => DrawNodeWindowPropertiesAndOptions(_windows[id]), "Window " + i);
            foreach (DialogueGraph.Node neighbor in (IEnumerable<DialogueGraph.Node>)windowNode.Node)
            {
                DrawNodeCurve(windowNode.Rect, GetNodeWindow(neighbor).Rect);
            }
            ++i;
        }

        EndWindows();
    }

    private void DrawGraphPropertiesAndOptions()
    {
        if (GUILayout.Button("Create Node"))
        {
            _windows.Add(new DialogueGraphNodeWindow(new DialogueGraph.Node(_graph, new DialogueStruct("", DialogueStruct.Mood.ChooseClip), false, _graph.LastAddedNode), _WINDOW_RECT));
        }

        var state = GUI.enabled;
        GUI.enabled = false;
        EditorGUILayout.Toggle("Costed distances", _graph.CostedConnections);
        EditorGUILayout.IntField("Number of Nodes", _graph.Size);
        GUI.enabled = state;
    }

    private void DrawNodeWindowPropertiesAndOptions(DialogueGraphNodeWindow windowNode)
    {
        if (windowNode == _nodeSelected)
        {
            GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), _selectedNodeBackground, ScaleMode.StretchToFill);
            if (GUILayout.Button("Unselect"))
            {
                _nodeSelected = null;
            }
        }
        else
        {
            if (windowNode.Node.IsRoot)
            {
                GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), _rootNodeBackground, ScaleMode.StretchToFill);
            }
            else
            {
                GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), _normalNodeBackground, ScaleMode.StretchToFill);
            }
            if (GUILayout.Button("Select"))
            {
                _nodeSelected = windowNode;
            }
            if (_nodeSelected != null)
            {
                if (GUILayout.Button("Attach", _attachButtonStyle))
                {
                    if (ConnectNodes(_nodeSelected, windowNode))
                    {
                        _nodeSelected = null;
                    }
                }
                if (GUILayout.Button("Detach", _detachButtonStyle))
                {
                    DisconnectNodes(_nodeSelected, windowNode);
                    _nodeSelected = null;
                }
            }
        }
        try
        {
            if (GUILayout.Button("Set as root", _setAsRootButtonStyle)) windowNode.Node.SetAsRoot();

            DrawNodeProperties(windowNode);

            if (GUILayout.Button("Delete", _deleteButtonStyle))
            {
                if (windowNode.Node.RemoveFromGraph())
                {
                    _windows.Remove(windowNode);
                    ClearSelected(windowNode);
                }
            }


            GUI.DragWindow();
        }
        catch (Exception e)
        {
            Debug.Log("Error:\n" + e);
            if (windowNode.Node.RemoveFromGraph())
            {
                _windows.Remove(windowNode);
                ClearSelected(windowNode);
            }
        }
    }

    private void DrawNodeProperties(DialogueGraphNodeWindow windowNode)
    {
        var node = windowNode.Node;
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.Toggle("Is root", node.IsRoot);
        EditorGUILayout.IntField("Number of neighbors", node.NumberOfNeighbors);
        EditorGUI.EndDisabledGroup();

        var graphSerializedObject = new SerializedObject(_graph.PartOfComponent);
        var nodesProperty = graphSerializedObject.GetIterator();
        while (nodesProperty.Next(true))
        {
            if (nodesProperty.name == "_nodesSerialized") break;
        }

        int length = nodesProperty.arraySize;
        for (int i = 0; i < length; ++i)
        {
            var nodeProperty = nodesProperty.GetArrayElementAtIndex(i);
            if (nodeProperty.FindPropertyRelative("_id").intValue == node.ID)
            {
                nodeProperty.Next(true);
                nodeProperty.Next(false);
                EditorGUI.BeginChangeCheck();
                nodeProperty.DrawProperty();

                if (EditorGUI.EndChangeCheck() || windowNode.FirstTime)
                {
                    windowNode.FirstTime = false;
                    windowNode.Rect.height = nodeProperty.GetHeight() + 150f;

                    graphSerializedObject.ApplyModifiedProperties();
                    PrefabUtility.RecordPrefabInstancePropertyModifications(_graph.PartOfComponent);
                }
                return;
            }
        }
    }

    private bool ConnectNodes(DialogueGraphNodeWindow start, DialogueGraphNodeWindow end)
    {
        return start.Node.AddNeighbor(end.Node);
    }

    private void DisconnectNodes(DialogueGraphNodeWindow start, DialogueGraphNodeWindow end)
    {
        start.Node.RemoveNeighbor(end.Node);
    }

    private void DrawNodeCurve(Rect start, Rect end)
    {
        Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
        Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);
        Vector3 startTan = startPos + Vector3.right * 50;
        Vector3 endTan = endPos + Vector3.left * 50;
        Color shadowCol = new Color(0, 0, 0, 0.06f);


        for (int i = 0; i < 3; i++)
        {// Draw a shadow
            Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
        }


        Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 1);
    }


    private DialogueGraphNodeWindow GetNodeWindow(DialogueGraph.Node node)
    {
        DialogueGraphNodeWindow.GetNodeWindow(node);

        var temp = DialogueGraphNodeWindow.GetNodeWindow(node);
        if (temp == null)
        {
            temp = _windows.Find(x => x.Node == node);
            if (temp == null)
            {
                _windows.Add(temp = new DialogueGraphNodeWindow(node, _WINDOW_RECT));
            }
            else
            {
                _windows.Remove(temp);
                _windows.Add(temp = new DialogueGraphNodeWindow(temp.Node, temp.Rect));
            }
        }
        return temp;
    }


    private void ClearSelected(DialogueGraphNodeWindow windowNode)
    {
        if (windowNode == _nodeSelected)
        {
            _nodeSelected = null;
        }
    }

    private void OnDestroy()
    {
        AssemblyReloadEvents.beforeAssemblyReload -= _graphEditor.Close;
        foreach (var texture in _generatedTextures)
        {
            DestroyImmediate(texture);
        }
        foreach (var dialogueComponent in FindObjectsOfType<DialogueComponent>())
        {
            if (dialogueComponent.Dialogue == _graph)
            {
                PrefabUtility.RecordPrefabInstancePropertyModifications(dialogueComponent);
            }
        }
        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    }
}
#endif