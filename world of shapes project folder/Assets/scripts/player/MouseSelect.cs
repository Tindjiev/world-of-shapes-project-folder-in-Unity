using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseSelect : MonoBehaviour
{
    private Collider2D _coll;

    private InputStruct ClickInput = new InputStruct(Input.GetKeyDown, KeyCode.Mouse0);

    public Transform TransformClicked { get; private set; }
    private Transform _trasnformCollidedWith;
    public Transform GetCollidedWith => _trasnformCollidedWith;

    void Start()
    {
        _coll = this.SearchComponent<Collider2D>();
        _coll.enabled = true;
    }


    private void LateUpdate()
    {
        SelectTransform(null);
        if (ClickInput.CheckInput())
        {
            SelectTransform(_trasnformCollidedWith);
        }
        _trasnformCollidedWith = null;
    }

    private void SelectTransform(Transform tr)
    {
        if (tr == null)
        {
            TransformClicked = null;
            return;
        }
        TransformClicked = tr;
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        _trasnformCollidedWith = collision.transform;
    }

}