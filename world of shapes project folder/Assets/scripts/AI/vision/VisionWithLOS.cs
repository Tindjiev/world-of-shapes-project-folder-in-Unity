using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionWithLOS : VisionCircular
{

    protected new void Awake()
    {
        base.Awake();
    }
    protected new void Start()
    {
        base.Start();
    }

    private readonly RaycastHit2D[] _results = new RaycastHit2D[1];
    private const int _LAYER_MASK = 1 << LayerNames.obstacles;
    protected new void OnTriggerStay2D(Collider2D collision)
    {
        Transform colltr = collision.transform;
        CollisionInfo collvars = collision.GetComponent<CollisionInfo>();
        if (CanAddToSeenList(collvars))
        {

#if UNITY_EDITOR
            Debug.DrawLine(_holder.Position, collision.transform.position, new Color(1f, 0.27f, 0f)); //orange
#endif

            if(Physics2D.LinecastNonAlloc(_holder.Position, collision.transform.position, _results, _LAYER_MASK) == 0)
            {
                _seen.AddLast(collvars);
            }

        }
    }

}
