using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

public abstract class DialogueComponentBase : MonoBehaviour
{
    protected static List<DialogueComponentBase> _activedialogues = new List<DialogueComponentBase>();
    private static List<DialogueComponentBase> _globalDialogueComponentBaseList = new List<DialogueComponentBase>();
    protected static bool _chatEnable = true;
    public static bool ChatEnable
    {
        get => _chatEnable;
        set
        {
            if (_chatEnable && !value)
            {
                _globalDialogueComponentBaseList.ForEach(x => x.enabled = false);
            }
            else if (!_chatEnable && value)
            {
                _globalDialogueComponentBaseList.ForEach(x => x.enabled = true);
            }
            _chatEnable = value;
        }
    }

    public enum DialogueRepetition
    {
        RepeatButNotAutomatically,
        RepeatAutomatically,
        DeleteAfterOnce,
    }

    public enum DialogueStart
    {
        ByInteraction,
        ByEnteringRange,
    }
    public bool StartConvWithInteraction => StartConv == DialogueStart.ByInteraction;
    public bool StartByNear => StartConv == DialogueStart.ByEnteringRange;

    public const float timeToBeAbleToContinueDialogue = 0.6f;
    protected Timer _timerToPressContinueDialogue = new Timer(timeToBeAbleToContinueDialogue);
    protected Timer _timerToAutoContinueDialogue = new Timer(1f);

    public DialogueGraph Dialogue;

    //options
    public bool Auto = false;
    public bool Standingstill = true;
    public DialogueRepetition Repeat = DialogueRepetition.RepeatButNotAutomatically;
    public DialogueStart StartConv = DialogueStart.ByInteraction;

    //inputs
    public static InputStruct StartConversationInput = new InputStruct(Input.GetKeyDown, KeyCode.LeftControl, KeyCode.RightControl);
    public static InputStruct NextInput = new InputStruct(Input.GetKeyDown, KeyCode.Return, KeyCode.KeypadEnter);


    [NonSerialized]
    protected bool _showinputkey = false, _lateframespeaking = false, _speaking = false, _waitingAnswer = false;

    [NonSerialized]
    protected DialogueGraph.Node _currentDialogueNode, _lastDialogueNode;
    public bool DialogueEnabled
    {
        get => _speaking;
        protected set
        {
            if (value) SpeakEnable();
            else SpeakDisable();
        }
    }


    protected void Awake()
    {
        _globalDialogueComponentBaseList.Add(this);
        DialogueEnabled = false;
    }

    protected void Start()
    {

    }

    private void LateUpdate()
    {
        BaseUpdate();

        _lateframespeaking = DialogueEnabled;
    }

    protected void OnEnable()
    {

    }
    protected void OnDisable()
    {
        SpeakDisable();
    }
    protected void OnDestroy()
    {
        _globalDialogueComponentBaseList.Remove(this);
    }


    protected void BaseUpdate()
    {
        if (_lateframespeaking)
        {
            if (_waitingAnswer ||
                (!Auto && _timerToPressContinueDialogue.CheckIfTimePassed && NextInput.CheckInput()) ||
                (Auto && _timerToAutoContinueDialogue.CheckIfTimePassed))
            {
                if (!_currentDialogueNode.HasNeighbors)
                {
                    EndDialogue();
                }
                else if (_currentDialogueNode.NumberOfNeighbors == 1)
                {
                    _lastDialogueNode = _currentDialogueNode;
                    _currentDialogueNode = _currentDialogueNode[0];
                    NewSentance();
                }
                else
                {
                    _waitingAnswer = true;
                    int input = MyInputs.GetNumberPressed(Input.GetKeyDown) - 1;
                    if (input > -1 && input < _currentDialogueNode.NumberOfNeighbors)
                    {
                        _waitingAnswer = false;
                        _lastDialogueNode = _currentDialogueNode;
                        _currentDialogueNode = _currentDialogueNode[input];
                        NewSentance();
                    }
                }
            }
        }
    }


    protected virtual void EndDialogue()
    {
        if (_currentDialogueNode != null && _currentDialogueNode.Value.OnNext != null)
        {
            _currentDialogueNode.Value.OnNext.Invoke();
        }
        DialogueEnabled = false;
    }

    protected virtual bool SpeakEnable()
    {
        if (DialogueEnabled) return false;

        _speaking = true;   //using field to avoid inf loop

        _lastDialogueNode = null;
        _currentDialogueNode = Dialogue.Root;
        _activedialogues.Add(this);

        NewSentance();
        return true;
    }


    protected virtual void NewSentance()
    {
        if (Auto)
        {
            _timerToAutoContinueDialogue.StartTimer();
        }
        else
        {
            _timerToPressContinueDialogue.StartTimer();
        }
        if(_lastDialogueNode != null && _lastDialogueNode.Value.OnNext != null)
        {
            _lastDialogueNode.Value.OnNext.Invoke();
        }
        if (_currentDialogueNode.Value.OnSpeak != null)
        {
            _currentDialogueNode.Value.OnSpeak.Invoke();
        }
        if (_currentDialogueNode.Value.DestroyDialogueAtEnd)
        {
            Repeat = DialogueRepetition.DeleteAfterOnce;
        }
    }

    protected virtual bool SpeakDisable()
    {
        if (!DialogueEnabled) return false;

        _speaking = false;   //using field to avoid inf loop
        
        _activedialogues.Remove(this);
        return true;
    }

    public virtual void StartConversation(Transform target)
    {
        DialogueEnabled = true;
    }


#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(DialogueComponentBase), true), UnityEditor.CanEditMultipleObjects]
    private class DialogueComponentEditor : ExtendedEditor
    {
        private static bool _showResetButton = false;
        protected override void OnInspectorGUIExtend(UnityEngine.Object currentTarget)
        {
            var dialogue = (DialogueComponentBase)currentTarget;
            DrawPropertiesExcept(nameof(Dialogue));

            DrawProperty(nameof(Dialogue));

            if (targets.Length == 1)
            {
                if (dialogue.Dialogue.PartOfComponent == null) dialogue.Dialogue.PartOfComponent = dialogue;
                if (GUILayout.Button("Edit dialogue graph"))
                {
                    DialogueGraphEditor.ShowEditor(dialogue.Dialogue);
                }
                if ((_showResetButton = UnityEditor.EditorGUILayout.Foldout(_showResetButton, "Reset Button")) && GUILayout.Button("Reset dialogue graph"))
                {
                    dialogue.Dialogue.Clear();
                    dialogue.Dialogue = new DialogueGraph();
                }
            }

        }

        protected override void ApplyChanges(UnityEngine.Object currentTarget)
        {
        }
    }
#endif

}
