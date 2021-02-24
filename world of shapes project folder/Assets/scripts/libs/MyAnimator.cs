using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyAnimator : MonoBehaviour, SkinManager.IMultipleSprites
{

    [SerializeField]
    private Sprite[] _sprites = new Sprite[0];

    [SerializeField]
    private float[] _timeMoments = new float[0];

    [SerializeField]
    private SpriteRenderer _rend;


    public bool Loop = false;
    public bool RenderOnlyIfAnimate = true;

    private float _time;

    public bool IsAnimating
    {
        get => enabled;
        private set
        {
            enabled = value;
            if (RenderOnlyIfAnimate) _rend.enabled = value;
        }
    }

    public float TotalTime => _timeMoments[_timeMoments.Length - 1];

	protected void Awake ()
    {
	}

    private void Start()
    {
        if (_rend == null)  _rend = this.SearchComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        for (int i = _sprites.Length - 1; i >= 0; i--)
        {
            if (_time > _timeMoments[i])
            {
                _rend.sprite = _sprites[i];
                break;
            }
        }
        if (_time > TotalTime)
        {
            if (!Loop) StopAnimating();
            else _time = 0f;
        }
        _time += Time.deltaTime;
    }

    public void StartAnimating()
    {
        ContinueAnimating();
        _time = 0f;
    }

    public void StopAnimating()
    {
        IsAnimating = false;
    }

    public void ContinueAnimating()
    {
        IsAnimating = true;
    }

    public void SetSprites(params Sprite[] sprites)
    {
        _sprites = sprites;
    }

    public IEnumerator<Sprite> GetEnumerator()
    {
        return ((IEnumerable<Sprite>)_sprites).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
