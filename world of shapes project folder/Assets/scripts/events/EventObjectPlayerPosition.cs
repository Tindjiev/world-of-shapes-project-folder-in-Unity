using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EventObjectPlayerPosition : EventObjectBaseClass
{
    private bool _entered;

    [SerializeField]
    private Collider2D _eventCollider;
    public Collider2D EventCollider
    {
        get
        {
            if (_eventCollider == null)
            {
                _eventCollider = GetComponent<Collider2D>();
                if (_eventCollider == null || _eventCollider.gameObject.layer != LayerNames.eventObjects)
                {
                    _eventCollider = BasicLib.InstantiatePrefabTr("events/event trigger collider", transform).GetComponent<Collider2D>();
                }
            }
            return _eventCollider;
        }
    }

    public Transform TransformWithCollider => EventCollider.transform;

    public Vector3 ActualPosition
    {
        get => TransformWithCollider.position;
        set => TransformWithCollider.position = value;
    }

    private void Start()
    {
#if UNITY_EDITOR
        if (_eventCollider.gameObject.layer != LayerNames.eventObjects)
        {
            Debug.Log("hmm collider not on event layer", this);
            Debug.Break();
        }
#endif
    }

    public T AddCollider<T>() where T : Collider2D
    {
        if (_eventCollider != null)
        {
            if (_eventCollider.transform != transform && _eventCollider.transform.childCount == 0 && _eventCollider.GetComponents<Component>().Length <= 1)
            {
#if UNITY_EDITOR
                if (_eventCollider.GetComponents<Component>().Length == 0) Debug.Log("hmm weird", this);
#endif
                DestroyImmediate(_eventCollider.gameObject);
            }
        }
        T coll = gameObject.AddComponent<T>();
        _eventCollider = coll;
        return coll;
    }

    protected void OnDestroy()
    {
        Destroy(TransformWithCollider.gameObject);
    }

    protected override bool CheckToTrigger()
    {
        if (_entered)
        {
            _entered = false;
            return true;
        }
        return false;
    }

    public static GameObject AddAsGameObject(Transform parent)
    {
        var temp = BasicLib.InstantiatePrefabGmbjct("events/event object collider", parent).AddEventComponent<EventObjectPlayerPosition>();
        temp._eventCollider = temp.GetComponent<Collider2D>();
        return temp.gameObject;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _entered = true;
    }

}
