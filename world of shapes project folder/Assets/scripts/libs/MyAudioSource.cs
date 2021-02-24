using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyAudioSource : MonoBehaviour
{

    public AudioClip[] clips;
    private Queue<Vector3>[] _audioQueue;

    private const int _HOW_MANY_AT_A_TIME = 20;

    protected void Start ()
    {
        _audioQueue = new Queue<Vector3>[clips.Length];
        for (int i = 0; i < clips.Length; i++)
        {
            _audioQueue[i] = new Queue<Vector3>();
        }
    }


    protected void Update ()
    {
        for (int i = 0; i < clips.Length; i++)
        {
            int len = _audioQueue[i].Count;
            int templen = len;
            for (int j = 0; j < templen && j < _HOW_MANY_AT_A_TIME; j++)
            {
                AudioSource.PlayClipAtPoint(clips[i], _audioQueue[i].Dequeue(), Mathf.Log10(len) / 2f + 1f);
                len--;
            }
        }
	}

    public void AddSoundToQueue(int soundIndex, Vector3 position)
    {
        _audioQueue[soundIndex].Enqueue(position);
    }

}
