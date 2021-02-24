using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class room : MonoBehaviour
{
    public class door
    {
        public bool exists;
        public bool isOpen;
        public room neighbor;
        int side;
        room room;

        public bool actuallyOpen
        {
            get
            {
                if (neighbor == null)
                {
                    return false;
                }
                door neighbordoor = neighbor.doors[(side + 2) & 3]; //( x&3 ) == ( X%4 )
                return exists && isOpen && neighbordoor.exists && neighbordoor.isOpen && neighbordoor.neighbor == room;
            }
        }

        public door(int SideToBePut, room ParentRoom)
        {
            side = SideToBePut;
            exists = true;
            isOpen = true;
            neighbor = null;
            room = ParentRoom;
        }

    }

    public struct wall
    {
        public Transform part1;
        public Transform part2;
    }

    public float height = 100f;
    public float width = 100f;
    public const float wallfatness = 10f;
    public const float doorwidth = 20f;
    public const bool defaultdoorstate = true;
    public door[] doors;
    public wall[] walls;

    void Awake()
    {
        walls = new wall[4];
        doors = new door[] { new door(0, this), new door(1, this), new door(2, this), new door(3, this) };
        if (pathlib.edge == null)
        {
            pathlib.edge = somefunctions.InstantiatePrefabTr("obstacles/edge", "edge", null);
            pathlib.edge.gameObject.SetActive(false);
        }
    }


    public bool checkifneighborexist(room room, int side)
    {
        return room.doors[side].exists;
    }


    public void createroom(float h, float w, bool[] doorexistance, room[] neighbors)
    {
        height = h;
        width = w;
        if (doorexistance.Length > 0 && neighbors.Length > 0 && doorexistance[0] && checkifneighborexist(neighbors[0], 2))
        {
            makeNorthwallwithdoor(null);
            doors[0].neighbor = neighbors[0];
        }
        else
        {
            makeNorthwallnodoor(null);
        }
        if (doorexistance.Length > 1 && neighbors.Length > 1 && doorexistance[1] && checkifneighborexist(neighbors[1], 3))
        {
            makeEastwallwithdoor(null);
            doors[1].neighbor = neighbors[1];
        }
        else
        {
            makeEastwallnodoor(null);
        }
        if (doorexistance.Length > 2 && neighbors.Length > 2 && doorexistance[2] && checkifneighborexist(neighbors[2], 0))
        {
            makeSouthwallwithdoor(null);
            doors[2].neighbor = neighbors[2];
        }
        else
        {
            makeSouthwallnodoor(null);
        }
        if (doorexistance.Length > 3 && neighbors.Length > 3 && doorexistance[3] && checkifneighborexist(neighbors[3], 1))
        {
            makeWestwallwithdoor(null);
            doors[3].neighbor = neighbors[3];
        }
        else
        {
            makeWestwallnodoor(null);
        }

    }

    public void changedoorexistance(int side)
    {
        if (!doors[side].exists)
        {
            changedoorexistancemakedoor(side);
        }
        else
        {
            doors[side].neighbor.changedoorexistancedestroydoor((side + 2) & 3); //( x&3 ) == ( X%4 )
            changedoorexistancedestroydoor(side);
        }
    }

    public void changedoorexistance(int side, room newneighbor)
    {
        if (!doors[side].exists)
        {
            changedoorexistancemakedoor(side, newneighbor);
            if (newneighbor != null)
            {
                newneighbor.changedoorexistancemakedoor((side + 2) & 3, this); //( x&3 ) == ( X%4 )
            }
        }
        else
        {
            doors[side].neighbor.changedoorexistancedestroydoor((side + 2) & 3); //( x&3 ) == ( X%4 )
            changedoorexistancedestroydoor(side);
        }
    }

    public void changedoorexistance(int side, bool makedoor)
    {
        if (makedoor)
        {
            changedoorexistancemakedoor(side);
        }
        else
        {
            doors[side].neighbor.changedoorexistancedestroydoor((side + 2) & 3); //( x&3 ) == ( X%4 )
            changedoorexistancedestroydoor(side);
        }
    }

    public void changedoorexistance(int side, bool makedoor, room newneighbor)
    {
        if (makedoor)
        {
            changedoorexistancemakedoor(side, newneighbor);
            if (newneighbor != null)
            {
                newneighbor.changedoorexistancemakedoor((side + 2) & 3, this); //( x&3 ) == ( X%4 )
            }
        }
        else
        {
            doors[side].neighbor.changedoorexistancedestroydoor((side + 2) & 3); //( x&3 ) == ( X%4 )
            changedoorexistancedestroydoor(side);
        }
    }

    void changedoorexistancemakedoor(int side)
    {
        if (!doors[side].exists)
        {
            switch (side)
            {
                case 0:
                    makeNorthwallwithdoor(walls[side].part1);
                    break;
                case 1:
                    makeEastwallwithdoor(walls[side].part1);
                    break;
                case 2:
                    makeSouthwallwithdoor(walls[side].part1);
                    break;
                case 3:
                    makeWestwallwithdoor(walls[side].part1);
                    break;
            }
        }
    }

    void changedoorexistancemakedoor(int side, room newneighbor)
    {
        if (!doors[side].exists)
        {
            switch (side)
            {
                case 0:
                    makeNorthwallwithdoor(walls[side].part1);
                    break;
                case 1:
                    makeEastwallwithdoor(walls[side].part1);
                    break;
                case 2:
                    makeSouthwallwithdoor(walls[side].part1);
                    break;
                case 3:
                    makeWestwallwithdoor(walls[side].part1);
                    break;
            }
            doors[side].neighbor = newneighbor;
        }
    }

    void changedoorexistancedestroydoor(int side)
    {
        if (doors[side].exists)
        {
            Destroy(walls[side].part2.gameObject);
            switch (side)
            {
                case 0:
                    makeNorthwallnodoor(walls[side].part1);
                    break;
                case 1:
                    makeEastwallnodoor(walls[side].part1);
                    break;
                case 2:
                    makeSouthwallnodoor(walls[side].part1);
                    break;
                case 3:
                    makeWestwallnodoor(walls[side].part1);
                    break;
            }
        }
    }



    void makeNorthwallwithdoor(Transform edge)
    {
        if (edge == null)
        {
            edge = Instantiate(pathlib.edge, transform);
            edge.gameObject.SetActive(true);
        }
        edge.name = "edge_n2";
        edge.transform.localScale = new Vector3((width + wallfatness - doorwidth) / 2f, wallfatness);
        edge.transform.localPosition = new Vector3((-width - wallfatness - doorwidth) / 4f, height / 2f);
        walls[0].part1 = edge;
        edge = Instantiate(edge, transform);
        edge.name = "edge_n2";
        edge.transform.localPosition = new Vector3((width + wallfatness + doorwidth) / 4f, height / 2f);
        walls[0].part2 = edge;
        doors[0].exists = true;
        doors[0].isOpen = defaultdoorstate;
    }

    void makeEastwallwithdoor(Transform edge)
    {
        if (edge == null)
        {
            edge = Instantiate(pathlib.edge, transform);
            edge.gameObject.SetActive(true);
        }
        edge.name = "edge_e2";
        edge.transform.localScale = new Vector3(wallfatness, (height + wallfatness - doorwidth) / 2f);
        edge.transform.localPosition = new Vector3(width / 2f, (-height - wallfatness - doorwidth) / 4f);
        walls[1].part1 = edge;
        edge = Instantiate(edge, transform);
        edge.name = "edge_e2";
        edge.transform.localPosition = new Vector3(width / 2f, (height + wallfatness + doorwidth) / 4f);
        walls[1].part2 = edge;
        doors[1].exists = true;
        doors[1].isOpen = defaultdoorstate;
    }


    void makeSouthwallwithdoor(Transform edge)
    {
        if (edge == null)
        {
            edge = Instantiate(pathlib.edge, transform);
            edge.gameObject.SetActive(true);
        }
        edge.name = "edge_s1";
        edge.transform.localScale = new Vector3((width + wallfatness - doorwidth) / 2f, wallfatness);
        edge.transform.localPosition = new Vector3((-width - wallfatness - doorwidth) / 4f, -height / 2f);
        walls[2].part1 = edge;
        edge = Instantiate(edge, transform);
        edge.name = "edge_s2";
        edge.transform.localPosition = new Vector3((width + wallfatness + doorwidth) / 4f, -height / 2f);
        walls[2].part2 = edge;
        doors[2].exists = true;
        doors[2].isOpen = defaultdoorstate;
    }

    void makeWestwallwithdoor(Transform edge)
    {
        if (edge == null)
        {
            edge = Instantiate(pathlib.edge, transform);
            edge.gameObject.SetActive(true);
        }
        edge.name = "edge_w2";
        edge.transform.localScale = new Vector3(wallfatness, (height + wallfatness - doorwidth) / 2f);
        edge.transform.localPosition = new Vector3(-width / 2f, (-height - wallfatness - doorwidth) / 4f);
        walls[3].part1 = edge;
        edge = Instantiate(edge, transform);
        edge.name = "edge_w2";
        edge.transform.localPosition = new Vector3(-width / 2f, (height + wallfatness + doorwidth) / 4f);
        walls[3].part2 = edge;
        doors[3].exists = true;
        doors[3].isOpen = defaultdoorstate;
    }



    void makeNorthwallnodoor(Transform edge)
    {
        if (edge == null)
        {
            edge = Instantiate(pathlib.edge, transform);
            edge.gameObject.SetActive(true);
        }
        edge.name = "edge_n";
        edge.transform.localScale = new Vector3(width + wallfatness, wallfatness);
        edge.transform.localPosition = new Vector3(0f, height / 2f);
        walls[0].part1 = edge;
        doors[0].exists = false;
        doors[0].isOpen = false;
    }

    void makeEastwallnodoor(Transform edge)
    {
        if (edge == null)
        {
            edge = Instantiate(pathlib.edge, transform);
            edge.gameObject.SetActive(true);
        }
        edge.name = "edge_e";
        edge.transform.localScale = new Vector3(wallfatness, wallfatness + height);
        edge.transform.localPosition = new Vector3(width / 2f, 0f);
        walls[1].part1 = edge;
        doors[1].exists = false;
        doors[1].isOpen = false;
    }

    void makeSouthwallnodoor(Transform edge)
    {
        if (edge == null)
        {
            edge = Instantiate(pathlib.edge, transform);
            edge.gameObject.SetActive(true);
        }
        edge.name = "edge_s";
        edge.transform.localScale = new Vector3(width + wallfatness, wallfatness);
        edge.transform.localPosition = new Vector3(0f, -height / 2f);
        walls[2].part1 = edge;
        doors[2].exists = false;
        doors[2].isOpen = false;
    }

    void makeWestwallnodoor(Transform edge)
    {
        if (edge == null)
        {
            edge = Instantiate(pathlib.edge, transform);
            edge.gameObject.SetActive(true);
        }
        edge.name = "edge_w";
        edge.transform.localScale = new Vector3(wallfatness, wallfatness + height);
        edge.transform.localPosition = new Vector3(-width / 2f, 0f);
        walls[3].part1 = edge;
        doors[3].exists = false;
        doors[3].isOpen = false;
    }

    void OnDestroy()
    {

        foreach (wall wall in walls)
        {
            if (wall.part1 != null)
            {
                Destroy(wall.part1.gameObject);
            }
            if (wall.part2 != null)
            {
                Destroy(wall.part2.gameObject);
            }
        }
    }

    public static room createroom(GameObject gameObject, Vector3 position, float h, float w, bool[] doorexistance, room[] neighbors)
    {
        room room;
        room = gameObject.AddComponent<room>();
        room.transform.position = position;
        room.createroom(h, w, doorexistance, neighbors);
        return room;
    }








}
