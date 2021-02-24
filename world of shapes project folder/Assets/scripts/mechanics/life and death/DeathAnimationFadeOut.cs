using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class DeathAnimationBase : MonoBehaviour
{

    [field: SerializeField, ReadOnlyOnInspector]
    public LifeComponent Life { get; private set; }

    protected BaseCharacterControl _character;
    protected MoveComponent _moveComponent;

    protected SpriteRenderer _rend, _rendStart;


    protected void Awake()
    {
        enabled = false;
        Life = this.SearchComponent<LifeComponent>();
    }

    public virtual void Trigger()
    {
        enabled = true;
        if (Life.IsLifeOfCharacter)
        {
            _character = this.GetCharacter();
            _character.enabled = false;
            _moveComponent = _character.MoveComponent;
            _moveComponent.gameObject.SetActive(false);
            _moveComponent.ClearPath();
            transform.position = _moveComponent.Position;
            _rendStart = _character.Skin[0].Renderer;
            _rend = gameObject.AddComponent<SpriteRenderer>();
            _rend.sortingOrder = _rendStart.sortingOrder;
            _rend.sprite = _rendStart.sprite;
            _rend.transform.rotation = _rendStart.transform.rotation;
            _rend.enabled = true;
        }
        else
        {
            _rend = GetComponent<SpriteRenderer>();
        }
        InitateAnimation();
    }

    protected abstract void InitateAnimation();

    protected virtual void Finish()
    {
        if (Life.IsLifeOfCharacter)
        {
            _character.enabled = true;
            _moveComponent.gameObject.SetActive(true);
            Destroy(_rend);
            _rend = _rendStart;
        }
        enabled = false;
        transform.rotation = Quaternion.identity;
        Life.Death();
    }

    public void AssignDeathAnimation<DeathAnimationScript>() where DeathAnimationScript : DeathAnimationBase
    {
        if (this.IsTheExactType<DeathAnimationScript>()) return;
        gameObject.AddComponent<DeathAnimationScript>();
        Destroy(this);
    }
}


public class DeathAnimationFadeOut : DeathAnimationBase
{
    public const float SECONDS_OF_FADING = 1f;

    private float _aStart;

    protected override void InitateAnimation()
    {
        _aStart = _rend.color.a;
        _rend.DOFade(0f, SECONDS_OF_FADING).onComplete = Finish;
    }

    protected override void Finish()
    {
        _rend.color = new Color(_rend.color.r, _rend.color.b, _rend.color.g, _aStart);
        base.Finish();
    }
}
