using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VisionBase : MonoBehaviour, IEnumerable<Transform>, IEnumerable<CollisionInfo>, IEnumerable<BaseCharacterControl>, ISerializationCallbackReceiver
{
    [SerializeField]
    protected EntityBase _holder;

    protected LinkedList<CollisionInfo> _seen = new LinkedList<CollisionInfo>();

    [SerializeField, ReadOnlyOnInspector]
    private List<CollisionInfo> _seenListForSerialize = null; //this list is used for the Inspector

    [SerializeField, HideInInspector]
    private bool _shouldBeActive = false;

    protected Coroutine _checkToRemoveSeenRoutine;

    protected const float _TIME_TO_CHECK_TO_REMOVE = 0.5f;

    protected void Awake()
    {
        if (_holder == null) _holder = this.SearchComponent<EntityBase>();
        _checkToRemoveSeenRoutine = this.DoActionInTimeRepeating(CheckAllAndRemoveFromSeens, Random.Range(0f, _TIME_TO_CHECK_TO_REMOVE), _TIME_TO_CHECK_TO_REMOVE);
    }

    protected void Start()
    {

    }

    protected virtual void CheckAllAndRemoveFromSeens()
    {
        _seen.Remove(ShouldRemoveFromSeen);
    }
    public bool ShouldRemoveFromSeen(CollisionInfo target)
    {
        return target == null || !target.gameObject.activeInHierarchy || !CanAddToSeen(target) || OutOfVisionRange(target);
    }
    public abstract bool OutOfVisionRange(CollisionInfo target);
    public virtual bool HasSeen(CollisionInfo target) => _seen.Contains(target);
    public virtual bool HasSeen(BaseCharacterControl target) => HasSeen(target.MoveComponent.GetComponent<CollisionInfo>());

    protected bool HasNotSeen(CollisionInfo tr)
    {
        return !_seen.Contains(tofind => tofind == tr, toremove => toremove == null || !toremove.gameObject.activeInHierarchy);
    }

    protected bool CanAddToSeenList(CollisionInfo withinVision)
    {
        return HasNotSeen(withinVision) && CanAddToSeen(withinVision);
    }

    private bool CanAddToSeen(CollisionInfo withinVision) => _holder.CanAddToSeen(withinVision) && CanAddToSeenExtraCondition(withinVision);

    protected virtual bool CanAddToSeenExtraCondition(CollisionInfo withinVision) => true;

    public abstract bool SetUp(); //return false if setup doesn't happen, example: its already been done


    public void Activate()
    {
        _shouldBeActive = true;
        gameObject.SetActive(true);
    }

    public void ReinstateActivation()
    {
        if (_shouldBeActive)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    protected virtual IEnumerator<CollisionInfo> ForEachAboutCollision()
    {
        for (var curr = _seen.First; curr != null;)
        {
            if (curr.Value == null || !curr.Value.gameObject.activeInHierarchy)
            {
                var toremove = curr;
                curr = curr.Next;
                _seen.Remove(toremove);
            }
            else
            {
                yield return curr.Value;
                curr = curr.Next;
            }
        }
    }

    IEnumerator<CollisionInfo> IEnumerable<CollisionInfo>.GetEnumerator()
    {
        return ForEachAboutCollision();
    }

    public virtual IEnumerator<Transform> GetEnumerator()
    {
        foreach (CollisionInfo target in (IEnumerable<CollisionInfo>)this)
        {
            yield return target.transform;
        }
    }

    IEnumerator<BaseCharacterControl> IEnumerable<BaseCharacterControl>.GetEnumerator()
    {
        foreach (CollisionInfo target in (IEnumerable<CollisionInfo>)this)
        {
            if (target.Entity is BaseCharacterControl character) yield return character;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }


    protected void OnTriggerStay2D(Collider2D collision) //it will be inherited and will be used if it has a collider
    {
        CollisionInfo collvars = collision.GetComponent<CollisionInfo>();
        if (CanAddToSeenList(collvars))
        {
            _seen.AddLast(collvars);
        }
    }

    public void OnBeforeSerialize()
    {
        _seenListForSerialize = new List<CollisionInfo>(this);
    }

    public void OnAfterDeserialize()
    {
        _seen = new LinkedList<CollisionInfo>(_seenListForSerialize);
    }
}

public static class VisionExtensions
{
    public static NewVision ReplaceVision<NewVision>(this GameObject gameObject) where NewVision : VisionOfMob
    {
        VisionOfMob oldVision = gameObject.GetComponent<VisionOfMob>(); //?? operator doesn't work for UnityEngine.Object
        if (oldVision == null) oldVision = gameObject.GetComponentInChildren<VisionOfMob>();
        if (oldVision == null) oldVision = gameObject.SearchComponent<VisionOfMob>();
        if (oldVision is NewVision) return oldVision as NewVision;
        Transform parent = oldVision.transform.parent;
        string name = oldVision.name;
        try
        {
            oldVision.gameObject.EditorDestroy();
        }
        catch (System.InvalidOperationException)
        {
            oldVision.gameObject.SetActive(false);
        }
        GameObject tempGameObject = new GameObject(name);
        tempGameObject.transform.parent = parent;
        NewVision newVision = tempGameObject.AddComponent<NewVision>();
        newVision.SetUp();
        return newVision;
    }
}

public abstract class VisionOfMob : VisionBase
{

    protected new void Awake()
    {
        base.Awake();
    }

    protected new void Start()
    {
        base.Start();
    }

}
public abstract class VisionOfNotMob : VisionBase
{

    protected new void Awake()
    {
        base.Awake();
    }

    protected new void Start()
    {
        base.Start();
    }

}
