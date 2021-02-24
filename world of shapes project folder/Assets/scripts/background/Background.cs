using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Background
{
    public static SpriteRenderer backgroundField;
    static LinkedList<GameObject> addedObjects = new LinkedList<GameObject>();

    public static Transform backgroundFieldTr
    {
        get
        {
            return backgroundField.transform;
        }
        set
        {
            SpriteRenderer rend = value.GetComponent<SpriteRenderer>();
            if (rend != null)
            {
                backgroundField = rend;
            }
        }
    }
    public static Color backgroundColor
    {
        get
        {
            return backgroundField.color;
        }
        set
        {
            backgroundField.color = value;
        }
    }

    public static Color SetBackgroundColor(Color newcolor)
    {
        return backgroundColor = newcolor;
    }

    public static Color GreenishColor => new Color(0.5f, 1f, 0.5f);
    public static Color YellowishColor => new Color(0.9f, 0.9f, 0.5f);

    public static void CreateBackgroundSquare(Rect PosAndSize, Color color)
    {
        GameObject gotemp = new GameObject("backgroundthing");
        addedObjects.AddLast(gotemp);
        gotemp.transform.parent = backgroundFieldTr.parent;
        SpriteRenderer rend = gotemp.AddComponent<SpriteRenderer>();
        rend.sprite = backgroundFieldTr.GetComponent<SpriteRenderer>().sprite;
        rend.color = color;
        rend.sortingOrder = backgroundField.sortingOrder + 1;
        gotemp.transform.position = PosAndSize.position;
        gotemp.transform.localScale = PosAndSize.size;
    }

    public static void getRidOfAddedBackgrounds()
    {
        ControlBase.getRidOfThings(addedObjects);
        /*
        for (LinkedListNode<GameObject> Node = addedObjects.First; Node != null; Node = Node.Next)
        {
            Object.Destroy(Node.Value);
        }
        addedObjects.Clear();*/
    }


}

