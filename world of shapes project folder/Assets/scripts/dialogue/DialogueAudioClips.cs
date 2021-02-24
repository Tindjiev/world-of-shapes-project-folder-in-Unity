using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;


[CreateAssetMenu(fileName = "Empty Dialogue Audio", menuName = "Dialogue/Audio Clips")]
public class DialogueAudioClips : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField]
    private AudioClip 
        NeutralMedium,
        NeutralLong,
        NeutralShort,
        Afformative,
        Negative,
        Joke,
        Question,
        Ponder,
        BriefResponse;

    [NonSerialized]
    private readonly AudioClip[] _clips = new AudioClip[9];

    public AudioClip this[int index] => _clips[index];
    public AudioClip this[DialogueStruct.Mood index] => _clips[(int)index];

    private void SetArray()
    {
        _clips[(int)DialogueStruct.Mood.NeutralMedium] = NeutralMedium;
        _clips[(int)DialogueStruct.Mood.NeutralLong] = NeutralLong;
        _clips[(int)DialogueStruct.Mood.NeutralShort] = NeutralShort;
        _clips[(int)DialogueStruct.Mood.Afformative] = Afformative;
        _clips[(int)DialogueStruct.Mood.Negative] = Negative;
        _clips[(int)DialogueStruct.Mood.Joke] = Joke;
        _clips[(int)DialogueStruct.Mood.Question] = Question;
        _clips[(int)DialogueStruct.Mood.Ponder] = Ponder;
        _clips[(int)DialogueStruct.Mood.BriefResponse] = BriefResponse;
    }

    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
        SetArray();
    }

}
