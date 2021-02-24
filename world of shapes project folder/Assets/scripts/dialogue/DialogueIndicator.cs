using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueIndicator : MonoBehaviour
{
    [SerializeField]
    private DialogueComponentBase _dialogue;

    [SerializeField]
    private SpriteRenderer _renderer;

    private void Start()
    {
        if (_dialogue == null && (_dialogue = this.SearchComponent<DialogueComponentBase>()) == null
            || _renderer == null && (_renderer = GetComponent<SpriteRenderer>()) == null
            || _dialogue.StartByNear)
        {
            gameObject.SetActive(false);
        }

    }


    private void LateUpdate()
    {
        if (_dialogue.DialogueEnabled == _renderer.enabled)
        {
            _renderer.enabled = !_dialogue.DialogueEnabled;
        }
    }
}
