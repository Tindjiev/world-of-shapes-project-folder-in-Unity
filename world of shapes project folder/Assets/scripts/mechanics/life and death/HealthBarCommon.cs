using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HealthVisualRepresenterBase : MonoBehaviour
{
    [field: SerializeField] public LifeComponent Life { get; protected set; }
    [field: SerializeField] public SpriteRenderer Rend { get; protected set; }
}

[ExecuteAlways]
public class HealthBarCommon : HealthVisualRepresenterBase
{

    [SerializeField]
    private Transform _livingBeing;

    private readonly Vector3 _relpos = new Vector3(0f, 2f, 0f);

    protected void Awake()
    {
        if (Life == null) Life = this.SearchComponent<LifeComponent>();
        if (Rend == null) Rend = GetComponent<SpriteRenderer>();
        if (_livingBeing == null) _livingBeing = this.SearchComponentTransform<MoveComponent>();
#if UNITY_EDITOR
        if (Life == null)
        {
            Debug.Break();
            Debug.Log("LifeComponent null", this);
        }
#endif
    }

#if UNITY_EDITOR
    private void OnEnable() => Awake();
#endif

    protected void Start()
    {
#if UNITY_EDITOR
        if (_livingBeing == null) return;
#endif
        transform.position = _livingBeing.position + _relpos;
        transform.localScale = new Vector3(Life.Health, 1f, 1f);
        Rend.color = new Color(1f, 0.61f, 0);
        Rend.enabled = true;
    }

    void LateUpdate()
    {
#if UNITY_EDITOR
        if (_livingBeing == null) return;
#endif
        transform.position = _livingBeing.position + _relpos;
        float life = Life.Health;
        if (life < 20f)
        {
            transform.localScale = new Vector3(life, 1f, 1f);
        }
        else
        {
            transform.localScale = new Vector3(20f, 1f, 1f);
            Rend.color = new Color(1f, 0.61f + (20f - life) / 100f, Rend.color.b);
        }
    }
}
