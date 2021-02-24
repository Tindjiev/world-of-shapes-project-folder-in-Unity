using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grid : MonoBehaviour
{
    public Transform gridtr;
    public bool gridon = true;
    bool toenable = false;
    Transform[,] grids;
    int i, j;
    public int gridsizew = 100;
    public int gridsizeh = 100;

    protected void OnEnable()
    {
        grids = new Transform[gridsizew, gridsizeh];
        for (i = 0; i < gridsizew; i++)
        {
            for (j = 0; j < gridsizeh; j++)
            {
                grids[i, j] = Instantiate(gridtr, new Vector3(i - gridsizew / 2, j - gridsizeh / 2, 0f), Quaternion.identity, transform);
                grids[i, j].gameObject.SetActive(true);
            }
        }
        InvokeRepeating("pseudoupdate", 0.5f, 0.05f);
    }

    protected void OnDisable()
    {
        for (i = 0; i < gridsizew; i++)
        {
            for (j = 0; j < gridsizeh; j++)
            {
                Destroy(grids[i, j]);
            }
        }
    }


    void pseudoupdate()
    {
        if (gridon == toenable)  // (!gridon && !toenable) || (gridon && toenable)  XNOR
        {
            for (i = 0; i < gridsizew; i++)
            {
                for (j = 0; j < gridsizeh; j++)
                {
                    grids[i, j].GetComponent<SpriteRenderer>().enabled = toenable;
                }
            }
            toenable = !toenable;
        }
    }

}
