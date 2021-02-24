using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class room : MonoBehaviour
{
    public static string doortype = "phase";

    public SpriteRenderer[] rends;
    public bool rendsenabled = true;
    int lastchildcount;

    Color pirvateColor;
    public Color color
    {
        get
        {
            return pirvateColor;
        }
        set
        {
            for (int l = 0; l < walls.Length; l++)
            {
                walls[l].part1.GetComponent<SpriteRenderer>().color = value;
                if (walls[l].part2 != null)
                {
                    walls[l].part2.GetComponent<SpriteRenderer>().color = value;
                }
            }
            pirvateColor = value;
        }
    }

    public const int north = 0, east = 1, south = 2, west = 3;

    [System.Serializable]
    public class door
    {
        public bool exists;
        public bool isOpen;
        public room neighbor;
        public int side;
        room room;
        public DoorBase doorvars;

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
            exists = false;
            neighbor = null;
            room = ParentRoom;
        }


        public void destroy()
        {
            if (doorvars != null)
            {
                Destroy(doorvars.gameObject);
            }
            exists = false;
            isOpen = false;
        }

        public void create()
        {
            if (doorvars == null)
            {
                doorvars = somefunctions.InstantiatePrefabGmbjct("obstacles/door " + doortype, room.transform).GetComponent<DoorBase>();
            }
            doorvars.doorvars = this;
            doorvars.room = room;
            exists = true;
            isOpen = defaultdoorstate;
        }

        public void create(room neighbor)
        {
            if (doorvars == null)
            {
                doorvars = somefunctions.InstantiatePrefabGmbjct("obstacles/door " + doortype, room.transform).GetComponent<DoorBase>();
            }
            doorvars.doorvars = this;
            doorvars.room = room;
            exists = true;
            isOpen = defaultdoorstate;
            this.neighbor = neighbor;
        }

    }

    [System.Serializable]
    public struct wall
    {
        public Transform part1;
        public Transform part2;
    }

    public float height = 100f;
    public float width = 100f;
    public static float wallfatness = 7f;
    public static float doorwidth = 20f;
    public static bool defaultdoorstate = true;
    public door[] doors;
    public wall[] walls;

    public float widthinner
    {
        get
        {
            return width - wallfatness / 2f;
        }
    }
    public float widthouter
    {
        get
        {
            return width + wallfatness / 2f;
        }
    }
    public float heightinner
    {
        get
        {
            return height - wallfatness / 2f;
        }
    }
    public float heightouter
    {
        get
        {
            return height + wallfatness / 2f;
        }
    }

    protected void Awake()
    {
        enabled = false;
        walls = new wall[4];
        doors = new door[] { new door(0, this), new door(1, this), new door(2, this), new door(3, this) };
        if (pathlib.edge == null)
        {
            //pathlib.edge = somefunctions.InstantiatePrefabTr("obstacles/edge", "edge", null);
            pathlib.edge = somefunctions.InstantiatePrefabTr("obstacles/edge", null);
            pathlib.edge.gameObject.SetActive(false);
        }
    }

    void getrenderers()
    {
        rends = GetComponentsInChildren<SpriteRenderer>();
    }

    protected void Start()
    {
        getrenderers();
        lastchildcount = transform.childCount;
    }

    void Update()
    {
        if (transform.childCount != lastchildcount)
        {
            getrenderers();
        }
        lastchildcount = transform.childCount;
        float x, y;
        x = transform.position.x - Cameralib.camera.transform.position.x;
        y = transform.position.y - Cameralib.camera.transform.position.y;
        if ((System.Math.Abs(x - y) + System.Math.Abs(x + y) > 10f * Camera.main.orthographicSize) == rendsenabled)
        {
            rendsenabled = !rendsenabled;
            for(int i = 0; i < rends.Length; i++)
            {
                rends[i].enabled = rendsenabled;
            }
        }
    }


    public bool checkifdoorexist(int side)
    {
        return doors[side].exists;
    }


    public void createroom(float w, float h, bool[] doorexistance, room[] neighbors)
    {
        height = h;
        width = w;
        if (doorexistance.Length > north && neighbors.Length > north && doorexistance[north])
        {
            if (neighbors[north] != null)
            {
                changedoorexistancemakedoor(north, neighbors[north]);
                neighbors[north].changedoorexistancemakedoor(south, this);
            }
            else
            {
                neighbors[north] = room.createroom(transform.parent, new Vector3(0f, height + wallfatness) + transform.position, width, height, new bool[] { false, false, true, false }, new room[] { null, null, this, null });
                //makeNorthwallwithdoor(neighbors[north]);
            }
        }
        else if (!doors[north].exists)
        {
            makeNorthwallnodoor();
        }
        if (doorexistance.Length > east && neighbors.Length > east && doorexistance[east])
        {
            if (neighbors[east] != null)
            {
                changedoorexistancemakedoor(east, neighbors[east]);
                neighbors[east].changedoorexistancemakedoor(west, this);
            }
            else
            {
                neighbors[east] = room.createroom(transform.parent, new Vector3(width + wallfatness, 0f) + transform.position, width, height, new bool[] { false, false, false, true }, new room[] { null, null, null, this });
                //makeEastwallwithdoor(neighbors[east]);
            }
        }
        else if (!doors[east].exists)
        {
            makeEastwallnodoor();
        }
        if (doorexistance.Length > south && neighbors.Length > south && doorexistance[south])
        {
            if (neighbors[south] != null)
            {
                changedoorexistancemakedoor(south, neighbors[south]);
                neighbors[south].changedoorexistancemakedoor(north, this);
            }
            else
            {
                neighbors[south] = room.createroom(transform.parent, new Vector3(0f, height + wallfatness) + transform.position, width, height, new bool[] { true, false, false, false }, new room[] { this, null, null, null });
                //makeSouthwallwithdoor(neighbors[south]);
            }
        }
        else if (!doors[south].exists)
        {
            makeSouthwallnodoor();
        }
        if (doorexistance.Length > west && neighbors.Length > west && doorexistance[west])
        {
            if (neighbors[west] != null)
            {
                changedoorexistancemakedoor(west, neighbors[west]);
                neighbors[west].changedoorexistancemakedoor(east, this);
            }
            else
            {
                neighbors[west] = room.createroom(transform.parent, new Vector3(width + wallfatness, 0f) + transform.position, width, height, new bool[] { false, true, false, false }, new room[] { null, this, null, null });
                //makeWestwallwithdoor(neighbors[west]);
            }
        }
        else if (!doors[west].exists)
        {
            makeWestwallnodoor();
        }

    }


    public void createroom(float w, float h, bool[] doorexistance)
    {
        if (doorexistance == null)
        {
            doorexistance = new bool[0];
        }
        height = h;
        width = w;
        if (doorexistance.Length > north && doorexistance[north])
        {
            makeNorthwallwithdoor();
        }
        else
        {
            makeNorthwallnodoor();
        }
        if (doorexistance.Length > east && doorexistance[east])
        {
            makeEastwallwithdoor();
        }
        else
        {
            makeEastwallnodoor();
        }
        if (doorexistance.Length > south && doorexistance[south])
        {
            makeSouthwallwithdoor();
        }
        else
        {
            makeSouthwallnodoor();
        }
        if (doorexistance.Length > west && doorexistance[west])
        {
            makeWestwallwithdoor();
        }
        else
        {
            makeWestwallnodoor();
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

    public void changedoorexistance(int side, bool makedoor,ref room newneighbor)
    {
        if (makedoor)
        {
            if (newneighbor != null)
            {
                changedoorexistancemakedoor(side, newneighbor);
                newneighbor.changedoorexistancemakedoor((side + 2) & 3, this); //( x&3 ) == ( X%4 )
            }
            else
            {
                side = (side + 2) & 3;
                newneighbor = room.createroom(transform.parent, new Vector3(width + wallfatness, 0f) + transform.position, height, width, new bool[] { side == 0, side == 1, side == 2, side == 3 }, new room[] { this, this, this, this });
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
                case north:
                    makeNorthwallwithdoor(walls[side].part1);
                    break;
                case east:
                    makeEastwallwithdoor(walls[side].part1);
                    break;
                case south:
                    makeSouthwallwithdoor(walls[side].part1);
                    break;
                case west:
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
                case north:
                    makeNorthwallwithdoor(walls[side].part1);
                    break;
                case east:
                    makeEastwallwithdoor(walls[side].part1);
                    break;
                case south:
                    makeSouthwallwithdoor(walls[side].part1);
                    break;
                case west:
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
                case north:
                    makeNorthwallnodoor(walls[side].part1);
                    break;
                case east:
                    makeEastwallnodoor(walls[side].part1);
                    break;
                case south:
                    makeSouthwallnodoor(walls[side].part1);
                    break;
                case west:
                    makeWestwallnodoor(walls[side].part1);
                    break;
            }
        }
    }




    void makeNorthwallwithdoor(room neighbor)
    {
        Transform edge = Instantiate(pathlib.edge, transform);
        edge.gameObject.SetActive(true);
        edge.name = "edge_n1";
        edge.transform.localScale = new Vector3((width + wallfatness - doorwidth) / 2f, wallfatness);
        edge.transform.localPosition = new Vector3((-width - wallfatness - doorwidth) / 4f, height / 2f);
        walls[north].part1 = edge;
        edge = Instantiate(edge, transform);
        edge.name = "edge_n2";
        edge.transform.localPosition = new Vector3((width + wallfatness + doorwidth) / 4f, height / 2f);
        walls[north].part2 = edge;
        doors[north].create(neighbor);
    }

    void makeEastwallwithdoor(room neighbor)
    {
        Transform edge = Instantiate(pathlib.edge, transform);
        edge.gameObject.SetActive(true);
        edge.name = "edge_e1";
        edge.transform.localScale = new Vector3(wallfatness, (height + wallfatness - doorwidth) / 2f);
        edge.transform.localPosition = new Vector3(width / 2f, (-height - wallfatness - doorwidth) / 4f);
        walls[east].part1 = edge;
        edge = Instantiate(edge, transform);
        edge.name = "edge_e2";
        edge.transform.localPosition = new Vector3(width / 2f, (height + wallfatness + doorwidth) / 4f);
        walls[east].part2 = edge;
        doors[east].create(neighbor);
    }


    void makeSouthwallwithdoor(room neighbor)
    {
        Transform edge = Instantiate(pathlib.edge, transform);
        edge.gameObject.SetActive(true);
        edge.name = "edge_s1";
        edge.transform.localScale = new Vector3((width + wallfatness - doorwidth) / 2f, wallfatness);
        edge.transform.localPosition = new Vector3((-width - wallfatness - doorwidth) / 4f, -height / 2f);
        walls[south].part1 = edge;
        edge = Instantiate(edge, transform);
        edge.name = "edge_s2";
        edge.transform.localPosition = new Vector3((width + wallfatness + doorwidth) / 4f, -height / 2f);
        walls[south].part2 = edge;
        doors[south].create(neighbor);
    }

    void makeWestwallwithdoor(room neighbor)
    {
        Transform edge = Instantiate(pathlib.edge, transform);
        edge.gameObject.SetActive(true);
        edge.name = "edge_w1";
        edge.transform.localScale = new Vector3(wallfatness, (height + wallfatness - doorwidth) / 2f);
        edge.transform.localPosition = new Vector3(-width / 2f, (-height - wallfatness - doorwidth) / 4f);
        walls[west].part1 = edge;
        edge = Instantiate(edge, transform);
        edge.name = "edge_w2";
        edge.transform.localPosition = new Vector3(-width / 2f, (height + wallfatness + doorwidth) / 4f);
        walls[west].part2 = edge;
        doors[west].create(neighbor);
    }



    void makeNorthwallwithdoor()
    {
        Transform edge = Instantiate(pathlib.edge, transform);
        edge.gameObject.SetActive(true);
        edge.name = "edge_n1";
        edge.transform.localScale = new Vector3((width + wallfatness - doorwidth) / 2f, wallfatness);
        edge.transform.localPosition = new Vector3((-width - wallfatness - doorwidth) / 4f, height / 2f);
        walls[north].part1 = edge;
        edge = Instantiate(edge, transform);
        edge.name = "edge_n2";
        edge.transform.localPosition = new Vector3((width + wallfatness + doorwidth) / 4f, height / 2f);
        walls[north].part2 = edge;
        doors[north].create();
    }

    void makeEastwallwithdoor()
    {
        Transform edge = Instantiate(pathlib.edge, transform);
        edge.gameObject.SetActive(true);
        edge.name = "edge_e1";
        edge.transform.localScale = new Vector3(wallfatness, (height + wallfatness - doorwidth) / 2f);
        edge.transform.localPosition = new Vector3(width / 2f, (-height - wallfatness - doorwidth) / 4f);
        walls[east].part1 = edge;
        edge = Instantiate(edge, transform);
        edge.name = "edge_e2";
        edge.transform.localPosition = new Vector3(width / 2f, (height + wallfatness + doorwidth) / 4f);
        walls[east].part2 = edge;
        doors[east].create();
    }


    void makeSouthwallwithdoor()
    {
        Transform edge = Instantiate(pathlib.edge, transform);
        edge.gameObject.SetActive(true);
        edge.name = "edge_s1";
        edge.transform.localScale = new Vector3((width + wallfatness - doorwidth) / 2f, wallfatness);
        edge.transform.localPosition = new Vector3((-width - wallfatness - doorwidth) / 4f, -height / 2f);
        walls[south].part1 = edge;
        edge = Instantiate(edge, transform);
        edge.name = "edge_s2";
        edge.transform.localPosition = new Vector3((width + wallfatness + doorwidth) / 4f, -height / 2f);
        walls[south].part2 = edge;
        doors[south].create();
    }

    void makeWestwallwithdoor()
    {
        Transform edge = Instantiate(pathlib.edge, transform);
        edge.gameObject.SetActive(true);
        edge.name = "edge_w1";
        edge.transform.localScale = new Vector3(wallfatness, (height + wallfatness - doorwidth) / 2f);
        edge.transform.localPosition = new Vector3(-width / 2f, (-height - wallfatness - doorwidth) / 4f);
        walls[west].part1 = edge;
        edge = Instantiate(edge, transform);
        edge.name = "edge_w2";
        edge.transform.localPosition = new Vector3(-width / 2f, (height + wallfatness + doorwidth) / 4f);
        walls[west].part2 = edge;
        doors[west].create();
    }




    void makeNorthwallwithdoor(Transform edge)
    {
        if (edge == null)
        {
            edge = Instantiate(pathlib.edge, transform);
            edge.gameObject.SetActive(true);
        }
        edge.name = "edge_n1";
        edge.transform.localScale = new Vector3((width + wallfatness - doorwidth) / 2f, wallfatness);
        edge.transform.localPosition = new Vector3((-width - wallfatness - doorwidth) / 4f, height / 2f);
        walls[north].part1 = edge;
        edge = Instantiate(edge, transform);
        edge.name = "edge_n2";
        edge.transform.localPosition = new Vector3((width + wallfatness + doorwidth) / 4f, height / 2f);
        walls[north].part2 = edge;
        doors[north].create();
    }

    void makeEastwallwithdoor(Transform edge)
    {
        if (edge == null)
        {
            edge = Instantiate(pathlib.edge, transform);
            edge.gameObject.SetActive(true);
        }
        edge.name = "edge_e1";
        edge.transform.localScale = new Vector3(wallfatness, (height + wallfatness - doorwidth) / 2f);
        edge.transform.localPosition = new Vector3(width / 2f, (-height - wallfatness - doorwidth) / 4f);
        walls[east].part1 = edge;
        edge = Instantiate(edge, transform);
        edge.name = "edge_e2";
        edge.transform.localPosition = new Vector3(width / 2f, (height + wallfatness + doorwidth) / 4f);
        walls[east].part2 = edge;
        doors[east].create();
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
        walls[south].part1 = edge;
        edge = Instantiate(edge, transform);
        edge.name = "edge_s2";
        edge.transform.localPosition = new Vector3((width + wallfatness + doorwidth) / 4f, -height / 2f);
        walls[south].part2 = edge;
        doors[south].create();
    }

    void makeWestwallwithdoor(Transform edge)
    {
        if (edge == null)
        {
            edge = Instantiate(pathlib.edge, transform);
            edge.gameObject.SetActive(true);
        }
        edge.name = "edge_w1";
        edge.transform.localScale = new Vector3(wallfatness, (height + wallfatness - doorwidth) / 2f);
        edge.transform.localPosition = new Vector3(-width / 2f, (-height - wallfatness - doorwidth) / 4f);
        walls[west].part1 = edge;
        edge = Instantiate(edge, transform);
        edge.name = "edge_w2";
        edge.transform.localPosition = new Vector3(-width / 2f, (height + wallfatness + doorwidth) / 4f);
        walls[west].part2 = edge;
        doors[west].create();
    }



    void makeNorthwallnodoor()
    {
        if (!doors[north].exists && walls[north].part1 == null)
        {
            Transform edge = Instantiate(pathlib.edge, transform);
            edge.gameObject.SetActive(true);
            edge.name = "edge_n";
            edge.transform.localScale = new Vector3(width + wallfatness, wallfatness);
            edge.transform.localPosition = new Vector3(0f, height / 2f);
            walls[north].part1 = edge;
            doors[north].destroy();
        }
    }

    void makeEastwallnodoor()
    {
        if (walls[east].part1 == null)
        {
            Transform edge = Instantiate(pathlib.edge, transform);
            edge.gameObject.SetActive(true);
            edge.name = "edge_e";
            edge.transform.localScale = new Vector3(wallfatness, wallfatness + height);
            edge.transform.localPosition = new Vector3(width / 2f, 0f);
            walls[east].part1 = edge;
            doors[east].destroy();
        }
    }

    void makeSouthwallnodoor()
    {
        if (walls[south].part1 == null)
        {
            Transform edge = Instantiate(pathlib.edge, transform);
            edge.gameObject.SetActive(true);
            edge.name = "edge_s";
            edge.transform.localScale = new Vector3(width + wallfatness, wallfatness);
            edge.transform.localPosition = new Vector3(0f, -height / 2f);
            walls[south].part1 = edge;
            doors[south].destroy();
        }
    }

    void makeWestwallnodoor()
    {
        if (walls[west].part1 == null)
        {
            Transform edge = Instantiate(pathlib.edge, transform);
            edge.gameObject.SetActive(true);
            edge.name = "edge_w";
            edge.transform.localScale = new Vector3(wallfatness, wallfatness + height);
            edge.transform.localPosition = new Vector3(-width / 2f, 0f);
            walls[west].part1 = edge;
            doors[west].destroy();
        }
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
        walls[north].part1 = edge;
        doors[north].destroy();
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
        walls[east].part1 = edge;
        doors[east].destroy();
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
        walls[south].part1 = edge;
        doors[south].destroy();
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
        walls[west].part1 = edge;
        doors[west].destroy();
    }


    /*
        public void closeNorthDoor()
        {
            doors[0].doorvars.CommandClose();
        }
        public void closeEastDoor()
        {
            doors[1].doorvars.CommandClose();
        }
        public void closeSouthDoor()
        {
            doors[2].doorvars.CommandClose();
        }
        public void closeWestDoor()
        {
            doors[3].doorvars.CommandClose();
        }

        public void openNorthDoor()
        {
            doors[0].doorvars.CommandOpen();
        }
        public void openEastDoor()
        {
            doors[1].doorvars.CommandOpen();
        }
        public void openSouthDoor()
        {
            doors[2].doorvars.CommandOpen();
        }
        public void openWestDoor()
        {
            doors[3].doorvars.CommandOpen();
        }

        public void changeNorthDoorState()
        {
            if (doors[0].isOpen)
            {
                closeNorthDoor();
            }
            else
            {
                openNorthDoor();
            }
        }
        public void changeEastDoorState()
        {
            if (doors[1].isOpen)
            {
                closeEastDoor();
            }
            else
            {
                openEastDoor();
            }
        }
        public void changeSouthDoorState()
        {
            if (doors[2].isOpen)
            {
                closeSouthDoor();
            }
            else
            {
                openSouthDoor();
            }
        }
        public void changeWestDoorState()
        {
            if (doors[3].isOpen)
            {
                closeWestDoor();
            }
            else
            {
                openWestDoor();
            }
        }*/


    public void closeAllDoors()
    {
        closeNorthDoor();
        closeEastDoor();
        closeSouthDoor();
        closeWestDoor();
    }
    public void openAllDoors()
    {
        openNorthDoor();
        openEastDoor();
        openSouthDoor();
        openWestDoor();
    }


    public void closeNorthDoor()
    {
        if (doors[north].exists)
        {
            doors[north].doorvars.CommandClose();
            room neighbor = doors[north].neighbor;
            if (neighbor != null)
            {
                neighbor.doors[south].doorvars.CommandClose();
            }
        }
    }
    public void closeEastDoor()
    {
        if (doors[east].exists)
        {
            doors[east].doorvars.CommandClose();
            room neighbor = doors[east].neighbor;
            if (neighbor != null)
            {
                neighbor.doors[west].doorvars.CommandClose();
            }
        }
    }
    public void closeSouthDoor()
    {
        if (doors[south].exists)
        {
            doors[south].doorvars.CommandClose();
            room neighbor = doors[south].neighbor;
            if (neighbor != null)
            {
                neighbor.doors[north].doorvars.CommandClose();
            }
        }
    }
    public void closeWestDoor()
    {
        if (doors[west].exists)
        {
            doors[west].doorvars.CommandClose();
            room neighbor = doors[west].neighbor;
            if (neighbor != null)
            {
                neighbor.doors[east].doorvars.CommandClose();
            }
        }
    }

    public void openNorthDoor()
    {
        if (doors[north].exists)
        {
            doors[north].doorvars.CommandOpen();
            room neighbor = doors[north].neighbor;
            if (neighbor != null)
            {
                neighbor.doors[south].doorvars.CommandOpen();
            }
        }
    }
    public void openEastDoor()
    {
        if (doors[east].exists)
        {
            doors[east].doorvars.CommandOpen();
            room neighbor = doors[east].neighbor;
            if (neighbor != null)
            {
                neighbor.doors[west].doorvars.CommandOpen();
            }
        }
    }
    public void openSouthDoor()
    {
        if (doors[south].exists)
        {
            doors[south].doorvars.CommandOpen();
            room neighbor = doors[south].neighbor;
            if (neighbor != null)
            {
                neighbor.doors[north].doorvars.CommandOpen();
            }
        }
    }
    public void openWestDoor()
    {
        if (doors[west].exists)
        {
            doors[west].doorvars.CommandOpen();
            room neighbor = doors[west].neighbor;
            if (neighbor != null)
            {
                neighbor.doors[east].doorvars.CommandOpen();
            }
        }
    }



    public void closeDoor(int side)
    {
        if (doors[side].exists)
        {
            doors[side].doorvars.CommandClose();
            room neighbor = doors[side].neighbor;
            if (neighbor != null)
            {
                neighbor.doors[(side + 2) & 3].doorvars.CommandClose();
            }
        }
    }

    public void openDoor(int side)
    {
        if (doors[side].exists)
        {
            doors[side].doorvars.CommandOpen();
            room neighbor = doors[side].neighbor;
            if (neighbor != null)
            {
                neighbor.doors[(side + 2) & 3].doorvars.CommandOpen();
            }
        }
    }

    public void closeDoor(int[] sides)
    {
        for (int i = 0; i < sides.Length; i++)
        {
            closeDoor(sides[i]);
        }
    }

    public void openDoor(int[] sides)
    {
        for (int i = 0; i < sides.Length; i++)
        {
            openDoor(sides[i]);
        }
    }


    public void changeDoorState(int side)
    {
        if (doors[side].exists)
        {
            if (doors[side].isOpen)
            {
                doors[side].doorvars.CommandClose();
                room neighbor = doors[side].neighbor;
                if (neighbor != null)
                {
                    neighbor.doors[(side + 2) & 3].doorvars.CommandClose();
                }
            }
            else
            {
                doors[side].doorvars.CommandOpen();
                room neighbor = doors[side].neighbor;
                if (neighbor != null)
                {
                    neighbor.doors[(side + 2) & 3].doorvars.CommandOpen();
                }
            }
        }
    }

    public void changeDoorState(int side, bool opendoor)
    {
        if (doors[side].exists)
        {
            if (opendoor)
            {
                doors[side].doorvars.CommandOpen();
                room neighbor = doors[side].neighbor;
                if (neighbor != null)
                {
                    neighbor.doors[(side + 2) & 3].doorvars.CommandOpen();
                }
            }
            else
            {
                doors[side].doorvars.CommandClose();
                room neighbor = doors[side].neighbor;
                if (neighbor != null)
                {
                    neighbor.doors[(side + 2) & 3].doorvars.CommandClose();
                }
            }
        }
    }

    public void changeDoorState(int[] sides)
    {
        for (int i = 0; i < sides.Length; i++)
        {
            changeDoorState(sides[i]);
        }
    }

    public void changeDoorState(bool[] opendoors)
    {
        for (int i = 0; i < 4; i++)
        {
            changeDoorState(i, opendoors[i]);
        }
    }

   /* public void changeDoorState(short opendoors)
    {
        for (int i = 0; i < 4; i++)
        {
            changeDoorState(3 - i, (opendoors & (1 << i)) != 0);
        }
    }*/




    protected void OnDestroy()
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
        foreach (door door in doors)
        {
            if (door != null && door.exists)
            {
                if (door.neighbor != null)
                {
                    door.neighbor.changedoorexistancedestroydoor((door.side + 2) & 3);
                }
                Destroy(door.doorvars.gameObject);
            }
        }

    }



    public static room createroom(GameObject gmobject, Vector3 position, float w, float h, bool[] doorexistance, room[] neighbors)
    {
        room room;
        room = gmobject.AddComponent<room>();
        room.transform.position = position;
        room.createroom(w, h, doorexistance, neighbors);
        return room;
    }


    public static room createroom(GameObject gmobject, Vector3 position, float w, float h, bool[] doorexistance)
    {
        room room;
        room = gmobject.AddComponent<room>();
        room.transform.position = position;
        room.createroom(w, h, doorexistance);
        return room;
    }

    public static room createroom(Transform parent, Vector3 position, float w, float h, bool[] doorexistance, room[] neighbors)
    {
        room room = new GameObject("room").AddComponent<room>();
        room.transform.parent = parent;
        room.transform.position = position;
        room.createroom(w, h, doorexistance, neighbors);
        return room;
    }


    public static room createroom(Transform parent, Vector3 position, float w, float h, bool[] doorexistance)
    {
        room room = new GameObject("room").AddComponent<room>();
        room.transform.parent = parent;
        room.transform.position = position;
        room.createroom(w, h, doorexistance);
        return room;
    }

    public Rect rect
    {
        get
        {
            return new Rect(position, size);
        }
    }

    public Vector2 size
    {
        get
        {
            return new Vector2(width, height);
        }
    }

    public Vector3 position
    {
        get
        {
            return transform.position;
        }
    }


    public Vector3 northSideCentre
    {
        get
        {
            return transform.position + new Vector3(0f, height / 2f);
        }
    }
    public Vector3 eastSideCentre
    {
        get
        {
            return transform.position + new Vector3(width / 2f, 0f);
        }
    }
    public Vector3 southSideCentre
    {
        get
        {
            return transform.position - new Vector3(0f, height / 2f);
        }
    }
    public Vector3 westSideCentre
    {
        get
        {
            return transform.position - new Vector3(width / 2f, 0f);
        }
    }

    public Vector3 northSideInner
    {
        get
        {
            return transform.position + new Vector3(0f, (height - wallfatness) / 2f);
        }
    }
    public Vector3 eastSideInner
    {
        get
        {
            return transform.position + new Vector3((width - wallfatness) / 2f, 0f);
        }
    }
    public Vector3 southSideInner
    {
        get
        {
            return transform.position - new Vector3(0f, (height - wallfatness) / 2f);
        }
    }
    public Vector3 westSideInner
    {
        get
        {
            return transform.position - new Vector3((width - wallfatness) / 2f, 0f);
        }
    }

    public Vector3 northSideOuter
    {
        get
        {
            return transform.position + new Vector3(0f, (height + wallfatness) / 2f);
        }
    }
    public Vector3 eastSideOuter
    {
        get
        {
            return transform.position + new Vector3((width + wallfatness) / 2f, 0f);
        }
    }
    public Vector3 southSideOuter
    {
        get
        {
            return transform.position - new Vector3(0f, (height + wallfatness) / 2f);
        }
    }
    public Vector3 westSideOuter
    {
        get
        {
            return transform.position - new Vector3((width + wallfatness) / 2f, 0f);
        }
    }
    
    public const float inner = -1f;
    public const float centre = 0f;
    public const float outer = 1f;
    public Vector3 getSidePos(int side, float inner_outer)
    {
        Vector3 add;
        switch (side)
        {
            case north:
                add = new Vector3(0f, (height + inner_outer * wallfatness) / 2f);
                break;
            case east:
                add = new Vector3((width + inner_outer * wallfatness) / 2f, 0f);
                break;
            case south:
                add = new Vector3(0f, (height + inner_outer * wallfatness) / -2f);
                break;
            case west:
                add = new Vector3((width + inner_outer * wallfatness) / -2f, 0f);
                break;
            default:
                add = Vector3.zero;
                break;
        }
        return transform.position + add;
    }


    public Vector3 proportionalPosition(Vector2 xy)
    {
        return proportionalPosition(xy.x, xy.y);
    }

    public Vector3 proportionalPosition(float x, float y) // pos + x*(east-pos) + y*(north-pos)
    {
        return eastSideInner * x + northSideInner * y - position * (x + y - 1f);
    }

}

