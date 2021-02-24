using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineBullet : MonoBehaviour
{
    public mylib.MyArrayList<Transform> bullets = new mylib.MyArrayList<Transform>(10, 5);
    int activeBullets;
    public float size;
    MachineGun machinevars;
    void Awake()
    {
        machinevars = this.getvars<MachineGun>();
        size = 0.5f;
        bullets.Add(transform.GetChild(0));
        bullets[0].localPosition = size * Vector3.right;
        OnDisable();
    }

    void LateUpdate()
    {
        transform.localPosition = new Vector3(Random.value * size, 0f);
        //Debug.Log((activeBullets * size).ToString() + " " + machinevars.currentReach.ToString());
        while ((activeBullets + 1) * size > machinevars.currentReach && activeBullets > 0)
        {
            bullets[--activeBullets].gameObject.SetActive(false);
        }
        while (activeBullets * size < machinevars.currentReach - size)
        {
            if (activeBullets >= bullets.Count)
            {
                bullets.Add(Instantiate(bullets[0], transform));
                bullets[activeBullets].localPosition = (activeBullets + 1) * size * Vector3.right;
            }
            bullets[activeBullets++].gameObject.SetActive(true);
        }
        enabled = machinevars.enabled;
    }

    protected void OnDisable()
    {
        bullets.ForEach(x => x.gameObject.SetActive(false));
        activeBullets = 0;
    }
}
