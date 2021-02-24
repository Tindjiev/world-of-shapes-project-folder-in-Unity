using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UltEvents;
using Dialogue;

public class DialogueComponent : DialogueComponentBase
{
    [field: SerializeField, ReadOnlyOnInspector]
    public BaseCharacterControl Character { get; private set; }

    private Transform _holder;
    public Transform Holder
    {
        get => _holder != null ? _holder : _holder = Character.SkinTransform;
        private set => _holder = value == null ? Character.SkinTransform : value;
    }
    private CircleCollider2D _coll;
    public float Radius
    {
        get => _coll.radius;
        set => _coll.radius = value;
    }


    //visual
    private GUIStyle _dialogueStyle;
    private Texture2D _backgroundBox;
    private Color _backgroundcolor;
    private Color _textColor;

    //shaking control
    private float _time;
    private Vector3 _centre, _shakingOffset;
    private Vector3 _centreInWorld => Holder.position - Holder.localPosition + _centre;

    private bool _nearField;
    protected bool _near
    {
        get => _nearField;
        set
        {
            _nearField = value;
            if (!value) _showinputkey = false;
        }
    }

    protected new void Awake()
    {
        base.Awake();
        _dialogueStyle = new GUIStyle();
        _dialogueStyle.alignment = TextAnchor.MiddleCenter;
        _coll = GetComponent<CircleCollider2D>();
        if (Character == null) Character = this.GetCharacter();
    }

    protected new void Start()
    {
        base.Start();
        _backgroundcolor = Holder.GetTeam().TeamColor;
        _textColor = _backgroundcolor.InvertColor();
        _backgroundBox = new Texture2D(1,1).RectangleTextureSetColor(_backgroundcolor);
        _dialogueStyle.normal.textColor = _textColor;
    }

    protected void LateUpdate()
    {
        BaseUpdate();

        _lateframespeaking = DialogueEnabled;
    }

    protected new void OnEnable()
    {
        base.OnEnable();
        _coll.enabled = true;
    }
    protected new void OnDisable()
    {
        base.OnDisable();
        _coll.enabled = false;
    }
    protected new void OnDestroy()
    {
        base.OnDestroy();
        Destroy(_backgroundBox);
    }



    public new void BaseUpdate()
    {
        transform.position = Holder.position;
        base.BaseUpdate();
        if (_lateframespeaking)
        {
            if (_time < 1.5f)
            {
                _shakingOffset = new Vector3(0f, 0.5f * Expmx2mx4mx(_time - 1) * Mathf.Abs(Mathf.Sin(MyMathlib.TAU * _time)));
                Holder.localPosition = _centre + _shakingOffset;
                _time += 4f * Time.deltaTime;
                if (_time >= 1.5f)
                {
                    Holder.localPosition = _centre;
                }
            }
        }
        _lateframespeaking = DialogueEnabled;
    }


    protected override void EndDialogue()
    {
        base.EndDialogue();
        if (Repeat == DialogueRepetition.DeleteAfterOnce)
        {
            Destroy(gameObject);
        }
        else if (StartConvWithInteraction)
        {
            Holder = null;
            _backgroundBox.RectangleTextureSetColor(_backgroundcolor);
            _dialogueStyle.normal.textColor = _textColor;
        }
    }

    private float Expmx2mx4mx(float x)
    { //e^(-x^2-x^4-x)
        return Mathf.Exp(-x * x * (1 + x * x) - x);
    }

    private void OnGUI()
    {
        string stringToProject;
        Vector3 posInWorld;
        if (DialogueEnabled)
        {
            stringToProject = _currentDialogueNode.Value.Dialogue;
            posInWorld = _centreInWorld + new Vector3(0f, 1f);
        }
        else if (_showinputkey)
        {
            stringToProject = StartConversationInput.ToString();
            posInWorld = Holder.position + new Vector3(0f, 1f);
        }
        else
        {
            return;
        }
        int fontsize = Screen.width * 2 / 100;
        if (fontsize > 25)
        {
            _dialogueStyle.fontSize = 25;
        }
        else
        {
            _dialogueStyle.fontSize = fontsize;
        }
        Rect rectOfText = BasicLib.GetRectForGUI(posInWorld, 0f, 0f);        //prep for text
        GUIContent guicont = new GUIContent(stringToProject);
        Rect rectOfBackground = DrawText(rectOfText, _backgroundBox, guicont, _dialogueStyle);

        if (!Auto && _speaking)
        {
            _dialogueStyle.alignment = TextAnchor.UpperCenter;
            _dialogueStyle.fontSize >>= 1;
            rectOfText = new Rect(rectOfBackground.position + rectOfBackground.size / 2f, Vector2.zero);
            guicont = new GUIContent(NextInput.ToString());
            DrawText(rectOfText, _backgroundBox, guicont, _dialogueStyle);

            _dialogueStyle.alignment = TextAnchor.MiddleCenter;
            _dialogueStyle.fontSize <<= 1;
        }

        if (_waitingAnswer)
        {
            _dialogueStyle.alignment = TextAnchor.UpperLeft;
            Rect pos = BasicLib.GetRectForGUI(_currentDialogueNode[0].Value.Speaker.SkinTransform.position - new Vector3(0f, 2f, 0f), 0f, 0f);
            for (int i = 0; i < _currentDialogueNode.NumberOfNeighbors; i++)
            {
                GUIContent gc = new GUIContent((i + 1).ToString() + ": " + _currentDialogueNode[i].Value.Dialogue);
                pos.y += DrawText(pos, _backgroundBox, gc, _dialogueStyle).size.y;
            }
            _dialogueStyle.alignment = TextAnchor.MiddleCenter;
        }
    }

    private Rect DrawText(Rect rectOfText, Texture2D texture, GUIContent guicont, GUIStyle textstyle)
    {
        Rect rectOfBackground = new Rect(rectOfText.position, _dialogueStyle.CalcSize(guicont));
        rectOfBackground.x -= 2f * Screen.width / 1920f;
        rectOfBackground.width += 5f * Screen.width / 1920f;
        GUI.DrawTexture(rectOfBackground.RectangleCentre(textstyle), texture);
        GUI.Label(rectOfText, guicont, textstyle);
        return rectOfBackground;
    }

    protected override bool SpeakEnable()
    {
        if (!base.SpeakEnable()) return false;
        _centre = Holder.localPosition;
        if (Standingstill) Character.enabled = false;
        if (!Auto) MyInputs.InputsOff();
        return true;
    }


    protected override void NewSentance()
    {
        base.NewSentance();
        if (_currentDialogueNode.Value.Speaker == null)
        {
            Holder = null;
            _currentDialogueNode.Value.Speaker = Character;
        }
        Holder = _currentDialogueNode.Value.Speaker.SkinTransform;
        _time = 0f;
        switch (_currentDialogueNode.Value.SetColor)
        {
            case DialogueStruct.ColorChoice.ByTeam:
                if (_currentDialogueNode.Value.Speaker == Character)
                {
                    _backgroundBox.RectangleTextureSetColor(_backgroundcolor);
                    _dialogueStyle.normal.textColor = _textColor;
                }
                else
                {
                    _backgroundBox.RectangleTextureSetColor(_currentDialogueNode.Value.Speaker.TeamColor);
                    _dialogueStyle.normal.textColor = _currentDialogueNode.Value.Speaker.TeamColor.InvertColor();
                }
                break;
            case DialogueStruct.ColorChoice.ByChoice:
                _backgroundBox.RectangleTextureSetColor(_currentDialogueNode.Value.BackgroundColor);
                _dialogueStyle.normal.textColor = _currentDialogueNode.Value.BackgroundColor.InvertColor();
                break;
        }
        if (_currentDialogueNode.Value.MoodTone == DialogueStruct.Mood.ChooseClip)
        {
            if(_currentDialogueNode.Value.Clip != null) AudioSource.PlayClipAtPoint(_currentDialogueNode.Value.Clip, _centreInWorld, _currentDialogueNode.Value.Volume);
        }
        else
        {
            AudioSource.PlayClipAtPoint(_currentDialogueNode.Value.AutoClip, _centreInWorld, _currentDialogueNode.Value.Volume);
        }
    }

    protected override bool SpeakDisable()
    {
        if (!base.SpeakDisable()) return false;
        _nearDialogues.Remove(this);
        _near = false;
        if (Standingstill) Character.enabled = true;
        if (!Auto) MyInputs.InputsOn();
        return true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (StartByNear)
        {
            if (!_speaking)
            {
                switch (Repeat)
                {
                    case DialogueRepetition.RepeatButNotAutomatically:
                        if (!_near)
                        {
                            StartConversation(collision.SearchComponentTransform<MoveComponent>());
                        }
                        break;
                    case DialogueRepetition.RepeatAutomatically:
                        StartConversation(collision.SearchComponentTransform<MoveComponent>());
                        break;
                    case DialogueRepetition.DeleteAfterOnce:
                        StartConversation(collision.SearchComponentTransform<MoveComponent>());
                        break;
                }
                _near = true;
            }
            return;
        }
        if (!StartConvWithInteraction) return;
        if (!_near && !_nearDialogues.Contains(this))
        {
            _near = true;
            _nearDialogues.AddLast(this);
        }
        if (!CheckinNearDialogues(collision.transform))
        {
            _showinputkey = false;
            return;
        }
        _showinputkey = true;
        if (!_speaking && StartConversationInput.CheckInput())
        {
            StartConversation(collision.SearchComponentTransform<MoveComponent>());
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        _near = false;
        if (!_speaking) _nearDialogues.Remove(this);
    }

    private static LinkedList<DialogueComponent> _nearDialogues = new LinkedList<DialogueComponent>(); //for player standing near multiple speakers

    private bool CheckinNearDialogues(Transform neartr)
    {
        DialogueComponent mintr = null;
        float min = float.MaxValue;
        for (LinkedListNode<DialogueComponent> curr = _nearDialogues.First; curr != null;)
        {
            DialogueComponent currdialogue = curr.Value;
            if (currdialogue._speaking)
            {
                return false;
            }
            if (currdialogue.Holder == null)
            {
                LinkedListNode<DialogueComponent> toRemove = curr;
                curr = curr.Next;
                _nearDialogues.Remove(toRemove);
                continue;
            }
            float tempdist = (currdialogue.Holder.position - neartr.position).sqrMagnitude;
            if (min > tempdist)
            {
                min = tempdist;
                mintr = currdialogue;
            }
            curr = curr.Next;
        }
        return mintr == this;
    }

}

namespace Dialogue
{
    [Serializable]
    public class DialogueGraph : Graph<DialogueStruct>
    {
#if UNITY_EDITOR
        [NonSerialized]
        public DialogueComponentBase PartOfComponent;
#endif
        public DialogueGraph()
        {
        }
        public DialogueGraph(DialogueStruct[] dialogue) : base(dialogue)
        {
        }
        public DialogueGraph(DialogueStruct[][] dialogue, int[,] linkInfo) : base(dialogue, linkInfo)
        {
        }
    }

    [Serializable]
    public struct DialogueStruct
    {
        public string Dialogue;
        public Mood MoodTone;
        public AudioClip Clip;
        public BaseCharacterControl Speaker;
        [Range(0f, 3f)]
        public float Volume;
        [SerializeField]
        public UltEvent OnSpeak;
        [SerializeField]
        public UltEvent OnNext;
        public bool DestroyDialogueAtEnd;
        public ColorChoice SetColor;
        public Color BackgroundColor;

        public AudioClip AutoClip => Clip == null ? Speaker.Team.AudioClips[MoodTone] : Clip;

        public enum ColorChoice
        {
            ByTeam = 0,
            ByChoice,
        }
        public enum Mood
        {
            ChooseClip = -1,
            NeutralMedium = 0,
            NeutralLong,
            NeutralShort,
            Afformative,
            Negative,
            Joke,
            Question,
            Ponder,
            BriefResponse,
        }
        public DialogueStruct(string dialogue, Mood mood) : this()
        {
            Dialogue = dialogue;
            MoodTone = mood;
            Volume = 1f;
            OnSpeak = new UltEvent();
            OnNext = new UltEvent();
            BackgroundColor = MyColorLib.BASE_COLOR_TO_REPLACE;
        }
        public DialogueStruct(string dialogue, Mood mood, BaseCharacterControl speaker) : this(dialogue, mood)
        {
            Speaker = speaker;
        }
        public DialogueStruct(string dialogue, Mood mood, GameObject speaker) : this(dialogue, mood, speaker.GetCharacter())
        {
        }
        public DialogueStruct(string dialogue, Mood mood, BaseCharacterControl speaker, Action triggerFunction) : this(dialogue, mood, speaker)
        {
            if (OnNext == null) OnNext = new UltEvent();
            OnNext.AddPersistentCall(triggerFunction);
        }

        public override string ToString()
        {
            return '\"' + Dialogue + "\" with volume " + Volume.ToString("0.00") + " from " + Speaker + " and voice: " + Clip;
        }
    }

    public static class DialogueStaticClass
    {
        private static DialogueComponent SetDialogue(this BaseCharacterControl mainSpeaker)
        {
            return BasicLib.InstantiatePrefabTr("chatObject", mainSpeaker.MoveComponent.transform).GetComponent<DialogueComponent>();
        }
        public static DialogueComponent SetDialogue(this BaseCharacterControl mainSpeaker, DialogueStruct[] dialogue)
        {
            var temp = mainSpeaker.SetDialogue();
            temp.Dialogue = new DialogueGraph(dialogue);
            return temp;
        }
        public static DialogueComponent SetDialogue(this BaseCharacterControl mainSpeaker, DialogueStruct[] dialogue, bool auto, bool StandStill, DialogueComponentBase.DialogueStart startConv, DialogueComponentBase.DialogueRepetition repeat)
        {
            DialogueComponent temp = mainSpeaker.SetDialogue(dialogue);
            temp.Auto = auto;
            temp.Standingstill = StandStill;
            temp.StartConv = startConv;
            temp.Repeat = repeat;
            return temp;
        }
        public static DialogueComponent SetDialogue(this BaseCharacterControl mainSpeaker, DialogueStruct[] dialogue, bool auto, bool StandStill, DialogueComponentBase.DialogueStart startConv, DialogueComponentBase.DialogueRepetition repeat, float radius)
        {
            DialogueComponent temp = mainSpeaker.SetDialogue(dialogue, auto, StandStill, startConv, repeat);
            temp.Radius = radius;
            return temp;
        }

        public static DialogueComponent SetDialogue(this BaseCharacterControl mainSpeaker, DialogueStruct[][] dialogues, int[,] linkInfo)
        {
            DialogueComponent temp = mainSpeaker.SetDialogue();
            temp.Dialogue = new DialogueGraph(dialogues, linkInfo);
            return temp;
        }
        public static DialogueComponent setDialogue(this BaseCharacterControl mainSpeaker, DialogueStruct[][] dialogues, int[,] linkInfo, bool auto, bool StandStill, DialogueComponentBase.DialogueStart startConv, DialogueComponentBase.DialogueRepetition repeat)
        {
            DialogueComponent temp = mainSpeaker.SetDialogue(dialogues, linkInfo);
            temp.Auto = auto;
            temp.Standingstill = StandStill;
            temp.StartConv = startConv;
            temp.Repeat = repeat;
            return temp;
        }
        public static DialogueComponent setDialogue(this BaseCharacterControl mainSpeaker, DialogueStruct[][] dialogues, int[,] linkInfo, bool auto, bool StandStill, DialogueComponentBase.DialogueStart startConv, DialogueComponentBase.DialogueRepetition repeat, float radius)
        {
            DialogueComponent temp = mainSpeaker.setDialogue(dialogues, linkInfo, auto, StandStill, startConv, repeat);
            temp.Radius = radius;
            return temp;
        }

    }
}