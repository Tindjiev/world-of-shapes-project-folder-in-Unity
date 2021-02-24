using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionOthers : VisionOfMob
{
    [SerializeField]
    protected List<VisionAndMatch> _visions = new List<VisionAndMatch>();

    [System.Serializable]
    protected struct VisionAndMatch
    {
        [field: SerializeField]
        public VisionBase Vision { get; private set; }

        [field: SerializeField]
        public bool MatchCanTargets { get; private set; }  //check if it should just get the vision's seens raw or bother check if it can be seen/targeted

        public VisionAndMatch(VisionBase vision, bool matchCanTargets)
        {
            Vision = vision;
            MatchCanTargets = matchCanTargets;
        }
    }

    protected new void Awake()
    {
        base.Awake();
    }
    protected new void Start()
    {
        base.Start();
    }

    private void Update()
    {
        for (int i = _visions.Count - 1; i >= 0; --i)
        {
            if (_visions[i].Vision == null)
            {
                _visions.RemoveAt(i);
            }
            else if (!_visions[i].MatchCanTargets)
            {
                foreach (CollisionInfo target in (IEnumerable<CollisionInfo>)_visions[i].Vision)
                {
                    if (CanAddToSeenList(target))
                    {
                        _seen.AddLast(target);
                    }
                }
            }
        }
    }

    public override bool SetUp() => false;

    public void AddVision(VisionBase vision, bool matchCanTargets = false)
    {
        vision.Activate();
        _visions.Add(new VisionAndMatch(vision, matchCanTargets));
    }

    public override bool OutOfVisionRange(CollisionInfo target)
    {
        if (target == null) return false;
        for (int i = _visions.Count - 1; i >= 0; --i)
        {
            if (!_visions[i].Vision.OutOfVisionRange(target))
            {
                return false;
            }
        }
        return true;
    }

    protected override IEnumerator<CollisionInfo> ForEachAboutCollision()
    {
        for (int i = _visions.Count - 1; i >= 0; --i)
        {
            if (_visions[i].Vision != null && _visions[i].MatchCanTargets)
            {
                foreach (CollisionInfo target in (IEnumerable<CollisionInfo>)_visions[i].Vision)
                {
                    yield return target;
                }
            }
        }
        IEnumerator<CollisionInfo> iterator = base.ForEachAboutCollision();
        while (iterator.MoveNext())
        {
            yield return iterator.Current;
        }
    }
}
