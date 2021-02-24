using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineBullet : MonoBehaviour
{
    public List<Transform> _bullets = new List<Transform>(10);
    public float Size = 0.5f;

    private int _activeBullets;
    private MachineGun _machineGun;
    private SkinManager _skin;

    private void Awake()
    {
        _machineGun = this.SearchComponent<MachineGun>();
        _skin = this.SearchComponent<SkinManager>();
        _bullets.Add(transform.GetChild(0));
        _bullets[0].localPosition = Size * Vector3.right;
        OnDisable();
    }

    private void LateUpdate()
    {
        transform.localPosition = new Vector3(Random.value * Size, 0f);
        while ((_activeBullets + 1) * Size > _machineGun.CurrentReach && _activeBullets > 0)
        {
            _bullets[--_activeBullets].gameObject.SetActive(false);
        }
        while (_activeBullets * Size < _machineGun.CurrentReach - Size)
        {
            if (_activeBullets >= _bullets.Count)
            {
                var temp = Instantiate(_bullets[0], transform);
                _bullets.Add(temp);
                var temp2 = new SkinManager.ImageInfo(temp.GetComponent<SpriteRenderer>(), _skin[0].OriginalImage, _skin[0].CurrentColor);
                temp2.ReplaceColorOfSprites(_skin[0].CurrentColor);
                _skin.AddRenderer(temp2);
                _bullets[_activeBullets].localPosition = (_activeBullets + 1) * Size * Vector3.right;
            }
            _bullets[_activeBullets++].gameObject.SetActive(true);
        }
        enabled = _machineGun.enabled;
    }

    protected void OnDisable()
    {
        _bullets.ForEach(x => x.gameObject.SetActive(false));
        _activeBullets = 0;
    }
}
