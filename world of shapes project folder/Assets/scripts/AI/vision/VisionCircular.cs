using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionCircular : VisionOfMob
{
    [SerializeField]
    protected CircleCollider2D _coll;

    public float RadiusOfVision
    {
        get => _coll.radius;
        protected set => _coll.radius = value;
    }

    private System.Func<bool> _checkToInactive = () => false, _checkToActive = () => true;

    protected new void Awake()
    {
        base.Awake();
    }
    protected new void Start()
    {
        base.Start();
        if (_coll == null)
        {
            GameObject tempGameObject = new GameObject("collider of vision");
            tempGameObject.transform.parent = transform;
            _coll = tempGameObject.AddComponent<CircleCollider2D>();
            _coll.isTrigger = true;
        }
        if (_holder is AI)
        {
            AI aivars = _holder as AI;
            _checkToInactive = aivars.CheckModeChillNot;
            _checkToActive = aivars.CheckModeChill;
        }
        Physics2D.IgnoreCollision(_coll, _holder.MoveComponent.GetComponent<Collider2D>());
    }
    
    protected void Update()
    {
        if (_coll.enabled)
        {
            if (_checkToInactive())
            {
                _coll.enabled = false;
            }
        }
        else if(_checkToActive())
        {
            _coll.enabled = true;
        }
    }

    protected override void CheckAllAndRemoveFromSeens()
    {
        if (_coll.enabled)
        {
            base.CheckAllAndRemoveFromSeens();
        }
    }

    public override bool OutOfVisionRange(CollisionInfo target)
    {
        if (target == null) return false;
        return (target.transform.position - _holder.transform.position).sqrMagnitude > RadiusOfVision.Sq() * (1.2f * 1.2f);
    }

    public override bool SetUp()
    {
        if (_coll == null)
        {
            for(int i = transform.childCount - 1; i > -1; --i)
            {
                transform.GetChild(i).EditorDestroy();
            }
            var temp = new GameObject("collider of vision");
            temp.transform.parent = transform;
            _coll = temp.AddComponent<CircleCollider2D>();
            _coll.radius = 35f;
            return true;
        }
        return false;
    }


#if UNITY_EDITOR

    [UnityEditor.CustomEditor(typeof(VisionCircular), true), UnityEditor.CanEditMultipleObjects]
    private class VisionEditor : ExtendedEditor
    {
        protected override void OnInspectorGUIExtend(Object currentTarget)
        {
            var vision = (VisionCircular)currentTarget;
            DrawProperties();
            if (vision._coll != null)
            {
                AddNewToList(UnityEditor.EditorGUILayout.FloatField("Radius Of Vision", vision.RadiusOfVision), vision.RadiusOfVision);
            }
        }

        protected override void ApplyChanges(Object currentTarget)
        {
            var vision = (VisionCircular)currentTarget;
            if (_changedList[0].Changed)
            {
                vision.RadiusOfVision = (float)_changedList[0].Value;
            }
        }
    }
#endif

}
