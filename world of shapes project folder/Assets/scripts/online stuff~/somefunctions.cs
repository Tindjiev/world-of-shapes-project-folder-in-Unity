using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

public static class somefunctions
{

    public static Vector3 Follow(Vector3 target, Vector3 pos, float movement)
    {
        float temp;
        Vector3 move = Vector3.zero;
        if (System.Math.Abs(temp = target.x - pos.x) > movement)
        {
            move.x = movement * System.Math.Sign(temp);
        }
        else
        {
            move.x = temp;
        }
        if (System.Math.Abs(temp = target.y - pos.y) > movement)
        {
            move.y = movement * System.Math.Sign(temp);
        }
        else
        {
            move.y = temp;
        }
        return move;
    }

    public static Vector3 Followdt(Vector3 target, Vector3 pos, float speed)
    {
        float temp;
        Vector3 move = Vector3.zero;
        if (System.Math.Abs(temp = target.x - pos.x) > speed * Time.fixedDeltaTime)
        {
            move.x = speed * System.Math.Sign(temp);
        }
        else
        {
            move.x = temp / Time.fixedDeltaTime;
        }
        if (System.Math.Abs(temp = target.y - pos.y) > speed * Time.fixedDeltaTime)
        {
            move.y = speed * System.Math.Sign(temp);
        }
        else
        {
            move.y = temp / Time.fixedDeltaTime;
        }
        if (move.sqrMagnitude != speed * speed)
        {
            move *= (float)mathlib.invsqrtof2;
        }
        return move;
    }

    /*public static Vector3 Follow(Vector3 target, Vector3 pos, float howclose, float movement)
    {
        float temp;
        Vector3 move = Vector3.zero;
        if (System.Math.Abs(temp = target.x - pos.x) > howclose)
        {
            move.x = movement * System.Math.Sign(temp);
        }
        if (System.Math.Abs(temp = target.y - pos.y) > howclose)
        {
            move.y = movement * System.Math.Sign(temp);
        }
        return move;
    }*/

    /*    public static Vector2 swingcharge(BaseCharacterControl vars, ref bool firsttime, Vector2 fakepos, float howfarhop, float howhop, ref float forhop)
        {
            Vector2 movement = new Vector2(0, 0);
            if (firsttime)
            {
                firsttime = false;
                vars.anglevector = getunitvector( - fakepos);
            }
            forhop += howhop;
            movement = forhop * vars.anglevector;
            if (forhop > howfarhop)
            {
                firsttime = true;
                vars.tfswing = false;
                forhop = 0;
            }

            return movement;
        }*/

    public static bool distancesqtfbigger(Vector2 v1_sub_v2, float dx, float dy)
    {
        return System.Math.Abs(v1_sub_v2.x) > dx || System.Math.Abs(v1_sub_v2.y) > dy;
    }

    public static bool distancesqtfsmaller(Vector2 v1_sub_v2, float dx, float dy)
    {
        return System.Math.Abs(v1_sub_v2.x) < dx || System.Math.Abs(v1_sub_v2.y) < dy;
    }

    /*  public Vector2 orbitaround(Vector2 centre, Vector2 pos, float r, float v)
      {
          double angle = angletotargetrad(pos-centre) + v / r;
          Debug.Log(angle* 57.295779513082320876798154814105);
          pos.x = (float)System.Math.Cos(angle);
          pos.y = (float)System.Math.Sin(angle);
          return r*pos;
      }*/

    public static Vector3 orbitaround(Vector3 centre, double angle, float r)
    {
        return mathlib.polarvectrad(r, angle) + centre;
    }

    public static GameObject[] clonestuff(int num, GameObject original, Transform parent)
    {
        GameObject[] clones;
        int i;
        clones = new GameObject[num];
        for (i = 0; i < num; i++)
        {
            clones[i] = MonoBehaviour.Instantiate(original, parent);
        }
        return clones;
    }

    /*  public static void orbitshield(bool condition, ref GameObject[] orbits, ref int num, Transform body, ref double angle, ref float radius, ref float velocity, Vector3 movement, ref int cd)
      {
          int i;
          if (condition)
          {
              radius = 0.1f;
              velocity = 0.05f;
              angle = cd = 0;
              for (i = 0; i < num; i++)
              {
                  orbits[i].transform.position = new Vector3((float)System.Math.Cos(i * 2 * System.Math.PI / num), (float)System.Math.Sin(i * 2 * System.Math.PI / num), 0) + body.position;
                  orbits[i].SetActive(true);
              }
              body.parent.GetChild(3).gameObject.SetActive(false);
          }
          angle += velocity;
          if (radius < 1.5f)
              radius += 0.3f;
          for (i = 0; i < num; i++)
          {
              if (orbits[i] != null)
                  orbits[i].transform.position = orbitaround(body.position, angle + i * 2 * System.Math.PI / num, radius);
          }
      }*/

   /* public static Transform checkforothers(Transform[] groups, Vector3 pos, double dist, bool checkfolo)
    {
        double min = dist *= dist, tempdist;
        Transform mintr = null;
        int i, j, len;
        for (j = 0; j < groups.Length; j++)
        {
            len = groups[j].childCount;
            for (i = 0; i < len; i++)
            {
                if (groups[j].GetChild(i).GetChild(0).position != pos && groups[j].GetChild(i).gameObject.activeSelf == true && (tempdist = (groups[j].GetChild(i).GetChild(0).position - pos).sqrMagnitude) < dist)
                {
                    if (min > tempdist && !(checkfolo && checkfollow(pos, groups[j].GetChild(i).GetChild(0), 10)))
                    {
                        min = tempdist;
                        mintr = groups[j].GetChild(i).GetChild(0);
                    }
                }
            }
        }
        return mintr;
    }*/

    /*public static bool checkfollow(Vector3 pos, Transform targ, int num)
    {
        BaseCharacterControl targvars;
        for (int i = 0; i < num && targ != null; i++)
        {
            if ((targvars = targ.parent.GetChild(1).GetComponent<BaseCharacterControl>()).folotr != null && targvars.folotr.position == pos)
            {
                return true;
            }
            else
            {
                targ = targvars.folotr;
            }
        }
        return false;
    }*/

    public static Transform getteam(Transform team)
    {
        while (team != null && team.parent != null)
        {
            if (team.parent.parent == null)
            {
                return team;
            }
            team = team.parent;
        }
        return null;
    }

 /*   public static bool validatecollision(Collider2D collision, Transform transform)
    {
        return collision.GetComponent<aboutcollisions>().damagable == true && (getteam(transform.parent) != getteam(collision.transform.parent));
    }*/

    public static Vector3 getposfromscreen(Vector3 pos)
    {
        Vector3 temp = Camera.main.ScreenToWorldPoint(pos);
        temp.z = 0f;
        return temp;
    }


    public static Rect getrectforGUI(Vector3 position, float w, float h)  //translates pos in world to pos in screen
    {
        Vector3 temp = Camera.main.WorldToScreenPoint(position);
        return new Rect(temp.x, Screen.height - temp.y, w, h);
    }

    /*public static Vector3 grabmove(BaseCharacterControl vars, Transform body)
    {
        if ((vars.targtr.position - body.position).sqrMagnitude < 2)
        {
            vars.control = vars.control0;
        }
        if (20f / body.parent.GetComponentInChildren<lifescript>().life > 2.8f)
        {
            return 2.8f * (vars.targtr.position - body.position).normalized;
        }
        else
        {
            return 20f / body.parent.GetComponentInChildren<lifescript>().life * (vars.targtr.position - body.position).normalized;
        }
    }*/

    /*public static Vector3 pushedmove(BaseCharacterControl vars, Vector3 distancevect)
    {
        if (distancevect.sqrMagnitude < 2)
        {
            vars.control = vars.control0;
        }
        return 0.1f * distancevect;
    }*/

    public static float exprandom(double scale, double amplitude, double offset)
    {               //scale=4.4: ~10-90      9.2: ~1-99     1: ~37.75-62.25     0.8: ~40-60     0: 50-50     2.77: ~20-80
        if (scale == 0.0)
            return (float)(amplitude * (offset + Random.value));
        else
            return (float)(amplitude * (System.Math.Log((System.Math.Exp(scale) - 1) * Random.value + 1) / scale + offset));
    }

    public static Vector3 wanderrandom(Vector3 centre_sub_pos, double scale1, double scale2, double amplitude1, double amplitude2, double offset1, double offset2)
    {
        return new Vector3(exprandom(scale1 * centre_sub_pos.x, amplitude1, offset1), exprandom(scale2 * centre_sub_pos.y, amplitude2, offset2), 0);
    }

    public static Vector3 followaround(Vector3 centre_sub_pos)
    {
        double dist = centre_sub_pos.magnitude;
        float th, r;
        if (dist > 2.6)
        {
            th = exprandom(-0.8 * dist, System.Math.PI, 0.0);
            r = exprandom(-2.0 * th, 10.0, 0.0);
            return mathlib.mulcmplxvect(centre_sub_pos / (float)dist, mathlib.polarvectrad(r, th * mathlib.evenneg_oddpos((int)(r * 1597.0))));
        }
        else
        {
            th = exprandom(0.8 * dist, System.Math.PI, 0.0);
            r = exprandom(2.0 * th, 10.0, 0.0);
            return mathlib.mulcmplxvect(centre_sub_pos / (float)dist, mathlib.polarvectrad(r, th * mathlib.evenneg_oddpos((int)(r * 1597.0))));
        }
    }

    public static double anglediff(double rad1, double rad2)
    {
        return System.Math.Abs(stabiliseradpp(rad1) - stabiliseradpp(rad2));
    }

    public static double stabiliseradpp(double rad)
    {
        if (rad > 0)
        {
            while (rad > System.Math.PI)
            {
                rad -= mathlib.TAU;
            }
        }
        else
        {
            while (rad < -System.Math.PI)
            {
                rad += mathlib.TAU;
            }
        }
        return rad;
    }

    public static double stabiliserad02p(double rad)
    {
        if (rad > 0)
        {
            while (rad > mathlib.TAU)
            {
                rad -= mathlib.TAU;
            }
        }
        else
        {
            while (rad < 0)
            {
                rad += mathlib.TAU;
            }
        }
        return rad;
    }

    public static int anglesign(double f1, double f2)
    {
        int i;
        f1 = stabiliseradpp(f1)- stabiliseradpp(f2);
        if((i = System.Math.Sign(System.Math.Abs(f1) - System.Math.PI) * System.Math.Sign(f1)) == 0)
        {
            i++;
        }
        return i;
    }


    public static Transform InstantiatePrefabTr(string path, string newname, Transform parent)
    {
        GameObject temp = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/" + path), parent);
        temp.name = newname;
        return temp.transform;
    }

    public static Transform InstantiatePrefabTr(string path, Transform parent)
    {
        GameObject temp = Resources.Load<GameObject>("Prefabs/" + path);
        string name = temp.name;
        temp = MonoBehaviour.Instantiate(temp, parent);
        temp.name = name;
        return temp.transform;
    }

    public static GameObject InstantiatePrefabGmbjct(string path, string newname, Transform parent)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            GameObject temp = (GameObject)PrefabUtility.InstantiatePrefab(Resources.Load<GameObject>("Prefabs/" + path), parent);
            temp.name = newname;
            return temp;
        }
        else
        {
            GameObject temp = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/" + path), parent);
            temp.name = newname;
            return temp;
        }
#else
        GameObject temp = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/" + path), parent);
        temp.name = newname;
        return temp;
#endif
        /*
        GameObject temp = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/" + path), parent);
        temp.name = newname;
        return temp;
        */
    }

    public static GameObject InstantiatePrefabGmbjct(string path, Transform parent)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            GameObject temp = Resources.Load<GameObject>("Prefabs/" + path);
            string name = temp.name;
            temp = (GameObject)PrefabUtility.InstantiatePrefab(temp, parent);
            temp.name = name;
            return temp;
        }
        else
        {
            GameObject temp = Resources.Load<GameObject>("Prefabs/" + path);
            string name = temp.name;
            temp = MonoBehaviour.Instantiate(temp, parent);
            temp.name = name;
            return temp;
        }
#else
        GameObject temp = Resources.Load<GameObject>("Prefabs/" + path);
        string name = temp.name;
        temp = MonoBehaviour.Instantiate(temp, parent);
        temp.name = name;
        return temp;
#endif
        /*
        GameObject temp = Resources.Load<GameObject>("Prefabs/" + path);
        string name = temp.name;
        temp = MonoBehaviour.Instantiate(temp, parent);
        temp.name = name;
        return temp;
        */
    }

    public static T MyInstantiatePrefab<T>(T original, string newname, Transform parent) where T : Object
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            T temp = (T)PrefabUtility.InstantiatePrefab(original, parent);
            temp.name = newname;
            return temp;
        }
        else
        {
            T temp = MonoBehaviour.Instantiate(original, parent);
            temp.name = newname;
            return temp;
        }
#else
        T temp = MonoBehaviour.Instantiate(original, parent);
        temp.name = newname;
        return temp;
#endif
    }

    public static T MyInstantiate<T>(T original, string newname, Transform parent) where T : Object
    {
        T temp = MonoBehaviour.Instantiate(original, parent);
        temp.name = newname;
        return temp;
    }
    public static T MyInstantiate<T>(T original, Transform parent) where T : Object
    {
        return MyInstantiate(original, original.name, parent);
    }

    public static GameObject InitializePlayer(float life, Vector3 position, Transform team, mylib.voidfunction death)
    {
        //if (globalvariables.playergameobject == null)
        //{
        AddInEditor.CreatePlayer(team);
        globalvariables.playergameobject.getvars<BaseCharacterControl>().AddAttacks();
        //}
        /*else if (!globalvariables.playergameobject.activeSelf)
        {
            globalvariables.playergameobject.SetActive(true);
        }*/
        globalvariables.playergameobject.getvars<MoveComponent>().SetPosition(position);
        globalvariables.playergameobject.getvars<lifescript>().death = death;
        globalvariables.playergameobject.getvars<lifescript>().life = life;
        return globalvariables.playergameobject;
    }


    /*

    public static GameObject createbody(Transform parent)
    {
        SpriteRenderer sprite;
        Rigidbody2D rb;
        BoxCollider2D coll;
        aboutcollisions ac;
        GameObject gmbjct = new GameObject();
        gmbjct.name = "body";
        gmbjct.transform.parent = parent;
        ac = gmbjct.AddComponent<aboutcollisions>();
        ac.damagable = true;
        ac.forrange = true;
        ac.blockable = true;
        ac.grabbable = 1;
        sprite = gmbjct.AddComponent<SpriteRenderer>();
        sprite.sprite = images.bodyimage.sprite;
        sprite.sortingOrder = 2;
        rb = gmbjct.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
        coll = gmbjct.AddComponent<BoxCollider2D>();
        coll.size = new Vector2(sprite.sprite.rect.width / 100f, sprite.sprite.rect.height / 100f);
        return gmbjct;
    }

    public static GameObject createlifebar(Transform parent)
    {
        GameObject gmbjct = new GameObject();
        SpriteRenderer sprite;
        gmbjct.transform.parent = parent;
        gmbjct.name = "lifebar";
        gmbjct.AddComponent<lifescript>();
        sprite = gmbjct.AddComponent<SpriteRenderer>();
        sprite.sprite = images.lifebarimage.sprite;
        sprite.sortingOrder = 10;
        return gmbjct;
    }

    public static GameObject createsword(Transform parent)
    {
        SpriteRenderer sprite;
        GameObject gmbjct = new GameObject();
        BoxCollider2D coll;
        AudioSource audio;
        gmbjct.transform.parent = parent;
        gmbjct.transform.localScale = new Vector3(3f, 4f);
        gmbjct.name = "sword";
        gmbjct.AddComponent<aboutcollisions>();
        sprite = gmbjct.AddComponent<SpriteRenderer>();
        sprite.sprite = images.swordimage.sprite;
        sprite.sortingOrder = 1;
        coll = gmbjct.AddComponent<BoxCollider2D>();
        coll.size = new Vector2(sprite.sprite.rect.width / 100f, sprite.sprite.rect.height / 100f);
        coll.isTrigger = true;
        coll.enabled = false;
        gmbjct.AddComponent<swingsword>();
        audio = gmbjct.AddComponent<AudioSource>();
        audio.clip = audiolib.swing;
        audio.playOnAwake = false;
        audio.dopplerLevel = 0;
        audio.spatialBlend = 1;
        audio.maxDistance = 20;
        audio.rolloffMode = AudioRolloffMode.Linear;
  //      audio.SetCustomCurve(AudioSourceCurveType.CustomRolloff, new AnimationCurve(new Keyframe[2] { new Keyframe(0, 1), new Keyframe(8, 0) }));
        return gmbjct;
    }

    public static GameObject createorbit(Transform parent)
    {
        SpriteRenderer sprite;
        BoxCollider2D coll;
        GameObject gmbjct = new GameObject();
        gmbjct.name = "orbit";
        gmbjct.transform.parent = parent;
        gmbjct.AddComponent<aboutcollisions>();
        sprite = gmbjct.AddComponent<SpriteRenderer>();
        sprite.sprite = images.orbitimage.sprite;
        sprite.sortingOrder = 1;
        coll = gmbjct.AddComponent<BoxCollider2D>();
        coll.size = new Vector2(sprite.sprite.rect.width / 100f, sprite.sprite.rect.height / 100f);
        coll.isTrigger = true;
        gmbjct.AddComponent<orbitcollision>();
        return gmbjct;
    }

    public static GameObject createballcharge(Transform parent)
    {
        SpriteRenderer sprite;
        CircleCollider2D coll;
        GameObject gmbjct = new GameObject();
        gmbjct.name = "ballcharge";
        gmbjct.transform.parent = parent;
        gmbjct.AddComponent<aboutcollisions>();
        sprite = gmbjct.AddComponent<SpriteRenderer>();
        sprite.sprite = images.ballimage.sprite;
        sprite.sortingOrder = 1;
        coll = gmbjct.AddComponent<CircleCollider2D>();
        coll.radius = sprite.sprite.rect.width / 200f;
        coll.isTrigger = true;
        coll.enabled = false;
        gmbjct.AddComponent<ballcharge>();
        return gmbjct;
    }

    public static GameObject createspray(Transform parent)
    {
        SpriteRenderer sprite;
        BoxCollider2D coll;
        GameObject gmbjct = new GameObject();
        gmbjct.name = "spray";
        gmbjct.transform.parent = parent;
        gmbjct.transform.localScale = new Vector3(3f, 3f);
        gmbjct.AddComponent<aboutcollisions>();
        sprite = gmbjct.AddComponent<SpriteRenderer>();
        sprite.sprite = images.sprayimage.sprite;
        sprite.sortingOrder = 1;
        coll = gmbjct.AddComponent<BoxCollider2D>();
        coll.size = new Vector2(0.3f, 0.13f);
        coll.offset = new Vector2(0.01f, 0f);
        coll.isTrigger = true;
        coll.enabled = false;
        gmbjct.AddComponent<spray>();
        return gmbjct;
    }

    public static GameObject creategrab(Transform parent)
    {
        SpriteRenderer sprite;
        PolygonCollider2D coll;
        LineRenderer line;
        Vector2[] points;
        GameObject gmbjct = new GameObject();
        gmbjct.name = "grab";
        gmbjct.transform.parent = parent;
        gmbjct.AddComponent<aboutcollisions>();
        sprite = gmbjct.AddComponent<SpriteRenderer>();
        sprite.sprite = images.grabimage.sprite;
        sprite.sortingOrder = 1;
        coll = gmbjct.AddComponent<PolygonCollider2D>();
        coll.isTrigger = true;
        coll.enabled = false;
        points = new Vector2[6];
        points[0] = new Vector2(-0.1f, 0.18f);
        points[1] = new Vector2(-0.31f, 0.06f);
        points[2] = new Vector2(-0.05f, -0.18f);
        points[3] = new Vector2(0.31f, -0.18f);
        points[4] = new Vector2(0.31f, -0.1f);
        points[5] = new Vector2(0.15f, 0.115f);
        coll.SetPath(0, points);
        gmbjct.AddComponent<grab>();
        line = gmbjct.AddComponent<LineRenderer>();
        line.sortingOrder = 1;
        line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        line.receiveShadows = false;
        line.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        line.startWidth = line.endWidth = 0.25f;
        return gmbjct;
    }

    public static GameObject createorbitclear(Transform parent)
    {
        SpriteRenderer sprite;
        PolygonCollider2D coll;
        Vector2[] points;
        GameObject gmbjct = new GameObject();
        gmbjct.name = "orbitclear";
        gmbjct.transform.parent = parent;
        gmbjct.AddComponent<orbitclear>();
        sprite = gmbjct.AddComponent<SpriteRenderer>();
        sprite.sprite = images.orbitclearimage.sprite;
        sprite.sortingOrder = 1;
        (coll = gmbjct.AddComponent<PolygonCollider2D>()).enabled = false;
        coll.isTrigger = true;

        return gmbjct;
    }

    public static GameObject createplayer(Transform parent)
    {
        GameObject gmbjct = new GameObject();
        BaseCharacterControl tempvars;
        Transform tr = gmbjct.transform;
        tr.parent = parent;
        gmbjct.name = "PLAYER";
        tempvars = gmbjct.AddComponent<BaseCharacterControl>();
        gmbjct.AddComponent<wepinterface>();
        createbody(tr).AddComponent<playerscript>();
        tempvars.countbody = 0;
        createlifebar(tr).AddComponent<lifesaver>();
        tempvars.countlifebar = 1;
        createsword(tr);
        tempvars.countsword = 2;
        createballcharge(tr);
        tempvars.countballcharge = 3;
        creategrab(tr);
        tempvars.countgrab = 4;
        createorbitclear(tr);
        tempvars.countorbitclear = 5;
        return gmbjct;
    }

    public static GameObject createswinger(Transform parent)
    {
        GameObject gmbjct = new GameObject();
        BaseCharacterControl tempvars;
        gmbjct.transform.parent = parent;
        gmbjct.name = "SWINGER";
        tempvars = gmbjct.AddComponent<BaseCharacterControl>();
        createbody(gmbjct.transform).AddComponent<swinger>();
        tempvars.countbody = 0;
        createlifebar(gmbjct.transform);
        tempvars.countlifebar = 1;
        createsword(gmbjct.transform);
        tempvars.countsword = 2;
        return gmbjct;
    }

    public static GameObject createbarrel(Transform parent)
    {
        GameObject gmbjct = new GameObject();
        BaseCharacterControl tempvars;
        gmbjct.transform.parent = parent;
        gmbjct.name = "BARREL";
        tempvars = gmbjct.AddComponent<BaseCharacterControl>();
        createbody(gmbjct.transform).AddComponent<heavyone>();
        tempvars.countbody = 0;
        createlifebar(gmbjct.transform);
        tempvars.countlifebar = 1;
        createballcharge(gmbjct.transform);
        tempvars.countballcharge = 2;
        return gmbjct;
    }

    public static GameObject createsprayer(Transform parent)
    {
        GameObject gmbjct = new GameObject();
        BaseCharacterControl tempvars;
        gmbjct.transform.parent = parent;
        gmbjct.name = "SPRAYER";
        tempvars = gmbjct.AddComponent<BaseCharacterControl>();
        createbody(gmbjct.transform).AddComponent<sprayer>();
        tempvars.countbody = 0;
        createlifebar(gmbjct.transform);
        tempvars.countlifebar = 1;
        return gmbjct;
    }

    public static GameObject createrock(Transform parent)
    {
        aboutcollisions ac;
        GameObject gmbjct = new GameObject(), child;
        CircleCollider2D coll;
        gmbjct.transform.parent = parent;
        gmbjct.name = "rock";
        ac = gmbjct.AddComponent<aboutcollisions>();
        ac.grabbable = 2;
        gmbjct.AddComponent<statics>();
        gmbjct.AddComponent<SpriteRenderer>().sprite = images.rocks[Random.Range(0, 3)].sprite;
        gmbjct.AddComponent<rockbuffscript>();
        child = new GameObject();
        child.transform.parent = gmbjct.transform;
        gmbjct.name = "rangechecker";
        child.AddComponent<checkinrange>();
        coll = child.AddComponent<CircleCollider2D>();
        coll.radius = 10f;
        coll.isTrigger = true;
        return gmbjct;
    }

    public static GameObject createborder(Transform parent)
    {
        aboutcollisions ac;
        SpriteRenderer sprite;
        GameObject gmbjct = new GameObject();
        gmbjct.transform.parent = parent;
        gmbjct.name = "edge";
        ac = gmbjct.AddComponent<aboutcollisions>();
        ac.grabbable = 2;
        (sprite = gmbjct.AddComponent<SpriteRenderer>()).sprite = images.edgeimage.sprite;
        sprite.sortingOrder = 8;
        gmbjct.AddComponent<statics>();
        return gmbjct;
    }

    public static GameObject createnexus(Transform parent)
    {
        aboutcollisions ac;
        Rigidbody2D rb;
        SpriteRenderer sprite;
        GameObject gmbjct = new GameObject();
        gmbjct.name = "nexus";
        gmbjct.transform.parent = parent;
        ac = gmbjct.AddComponent<aboutcollisions>();
        ac.damagable = true;
        ac.grabbable = 2;
        (sprite = gmbjct.AddComponent<SpriteRenderer>()).sprite = images.nexusimage.sprite;
        sprite.sortingOrder = 8;
        gmbjct.AddComponent<statics>();
        gmbjct.AddComponent<nexusscript>();
        rb = gmbjct.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
        return gmbjct;
    }

    */



    public static void startingstuff(Transform tr, BaseCharacterControl vars, float startinglife)
    {
        SpriteRenderer rend = tr.GetComponent<SpriteRenderer>();
        //tr.parent.GetComponentInChildren<lifescript>().life = startinglife;
        rend.sprite = colorlib.GetSpriteColored(vars.Teamcolor, rend.sprite, colorlib.BASE_COLOR_TO_REPLACE);
    }




}


public static class WeaponsStaticClass
{

    public const int push = 0, fire = 1, ball = 2, spray = 3, orbitClear = 4;
    public const int orbitAttack = 5, Boomerang = 6, directionChange = 7, charge = 8, shield = 9, summoner = 10, machine = 11, heal = 12;

    private static readonly string[] _weaponNamePaths = new string[] {
        Attack.BuildPathToWeapon<PushAttack>(),
        Attack.BuildPathToWeapon<SprayAttack>(),
        Attack.BuildPathToWeapon<Shield>(),
        Attack.BuildPathToWeapon<DirectionChangeAttack>(),
        Attack.BuildPathToWeapon<OrbitAttack>(),
        Attack.BuildPathToWeapon<FireAttack>(),
        Attack.BuildPathToWeapon<BallChargeAttack>(),
        Attack.BuildPathToWeapon<OrbitClearAttack>(),
        Attack.BuildPathToWeapon<Boomerang>(),
        Attack.BuildPathToWeapon<MeleeChargeAttack>(),
        Attack.BuildPathToWeapon<SummonAttack>(),
        Attack.BuildPathToWeapon<MachineGun>(),
        Attack.BuildPathToWeapon<BasicHeal>(),
    };

    public static IEnumerable<string> WeaponNames => _weaponNamePaths;

public static GameObject AddAttacks(this BaseCharacterControl holder, string attack)
    {
        return somefunctions.InstantiatePrefabGmbjct(attack, holder.transform);
    }
    public static void AddAttacks(this BaseCharacterControl holder, params string[] attacks)
    {
        holder.AddAttacks((IEnumerable<string>)attacks);
    }
    public static void AddAttacks(this BaseCharacterControl holder, IEnumerable<string> attacks)
    {
        foreach (string attack in attacks)
        {
            somefunctions.InstantiatePrefabGmbjct(attack, holder.transform);
        }
    }
    public static void AddAttacks(this BaseCharacterControl holder)
    {
        if (holder.getvars<PushAttack>() == null)
        {
            somefunctions.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<PushAttack>(), holder.transform);
        }
        if (holder.getvars<SprayAttack>() == null)
        {
            somefunctions.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<SprayAttack>(), holder.transform);
        }
        if (holder.getvars<Shield>() == null)
        {
            somefunctions.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<Shield>(), holder.transform);
        }
        if (holder.getvars<DirectionChangeAttack>() == null)
        {
            somefunctions.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<DirectionChangeAttack>(), holder.transform);
        }
        if (holder.transform.getvars<OrbitAttack>() == null)
        {
            somefunctions.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<OrbitAttack>(), holder.transform);
        }
        if (holder.transform.getvars<FireAttack>() == null)
        {
            somefunctions.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<FireAttack>(), holder.transform);
        }
        if (holder.transform.getvars<BallChargeAttack>() == null)
        {
            somefunctions.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<BallChargeAttack>(), holder.transform);
        }
        if (holder.transform.getvars<OrbitClearAttack>() == null)
        {
            somefunctions.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<OrbitClearAttack>(), holder.transform);
        }
        if (holder.transform.getvars<Boomerang>() == null)
        {
            somefunctions.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<Boomerang>(), holder.transform);
        }
        if (holder.transform.getvars<MeleeChargeAttack>() == null)
        {
            somefunctions.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<MeleeChargeAttack>(), holder.transform);
        }
        if (holder.transform.getvars<SummonAttack>() == null)
        {
            somefunctions.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<SummonAttack>(), holder.transform);
        }
        if (holder.transform.getvars<MachineGun>() == null)
        {
            somefunctions.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<MachineGun>(), holder.transform);
        }
        if (holder.transform.getvars<BasicHeal>() == null)
        {
            somefunctions.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<BasicHeal>(), holder.transform);
        }
    }

    public static void AddAttacks(this BaseCharacterControl holder, int[] include)
    {
        if (include == null)
        {
            AddAttacks(holder);
        }
        else
        {
            for (int i = 0; i < include.Length; i++)
            {
                AddAttacks(holder, include[i]);
            }
        }
    }
    public static void AddAttacks(this BaseCharacterControl holder, int[] include, bool addToExisting)
    {
        if (include == null)
        {
            AddAttacks(holder);
        }
        else
        {
            for (int i = 0; i < include.Length; i++)
            {
                AddAttacks(holder, include[i], addToExisting);
            }
        }
    }


    public static GameObject AddAttacks(this BaseCharacterControl holder, int include)
    {
        switch (include)
        {
            case push:
                if (holder.transform.getvars<PushAttack>() == null)
                {
                    return somefunctions.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<PushAttack>(), holder.transform);
                }
                break;
            case fire:
                if (holder.transform.getvars<FireAttack>() == null)
                {
                    return somefunctions.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<FireAttack>(), holder.transform);
                }
                break;
            case ball:
                if (holder.transform.getvars<BallChargeAttack>() == null)
                {
                    return somefunctions.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<BallChargeAttack>(), holder.transform);
                }
                break;
            case spray:
                if (holder.transform.getvars<SprayAttack>() == null)
                {
                    return somefunctions.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<SprayAttack>(), holder.transform);
                }
                break;
            case orbitClear:
                if (holder.transform.getvars<OrbitClearAttack>() == null)
                {
                    return somefunctions.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<OrbitClearAttack>(), holder.transform);
                }
                break;
            case orbitAttack:
                if (holder.transform.getvars<OrbitAttack>() == null)
                {
                    return somefunctions.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<OrbitAttack>(), holder.transform);
                }
                break;
            case Boomerang:
                if (holder.transform.getvars<Boomerang>() == null)
                {
                    return somefunctions.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<Boomerang>(), holder.transform);
                }
                break;
            case directionChange:
                if (holder.transform.getvars<DirectionChangeAttack>() == null)
                {
                    return somefunctions.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<DirectionChangeAttack>(), holder.transform);
                }
                break;
            case charge:
                if (holder.transform.getvars<MeleeChargeAttack>() == null)
                {
                    return somefunctions.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<MeleeChargeAttack>(), holder.transform);
                }
                break;
            case shield:
                if (holder.transform.getvars<Shield>() == null)
                {
                    return somefunctions.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<Shield>(), holder.transform);
                }
                break;
            case summoner:
                if (holder.transform.getvars<SummonAttack>() == null)
                {
                    return somefunctions.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<SummonAttack>(), holder.transform);
                }
                break;
            case machine:
                if (holder.transform.getvars<MachineGun>() == null)
                {
                    somefunctions.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<MachineGun>(), holder.transform);
                }
                break;
            case heal:
                if (holder.transform.getvars<BasicHeal>() == null)
                {
                    somefunctions.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<BasicHeal>(), holder.transform);
                }
                break;
        }
        return null;
    }
    public static GameObject AddAttacks(this BaseCharacterControl holder, int include, bool addToExisting)
    {
        switch (include)
        {
            case push:
                if (addToExisting || holder.getvars<PushAttack>() == null)
                {
                    return somefunctions.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<PushAttack>(), holder.transform);
                }
                break;
            case fire:
                if (addToExisting || holder.transform.getvars<FireAttack>() == null)
                {
                    return somefunctions.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<FireAttack>(), holder.transform);
                }
                break;
            case ball:
                if (addToExisting || holder.transform.getvars<BallChargeAttack>() == null)
                {
                    return somefunctions.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<BallChargeAttack>(), holder.transform);
                }
                break;
            case spray:
                if (addToExisting || holder.transform.getvars<SprayAttack>() == null)
                {
                    return somefunctions.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<SprayAttack>(), holder.transform);
                }
                break;
            case orbitClear:
                if (addToExisting || holder.transform.getvars<OrbitClearAttack>() == null)
                {
                    return somefunctions.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<OrbitClearAttack>(), holder.transform);
                }
                break;
            case orbitAttack:
                if (addToExisting || holder.transform.getvars<OrbitAttack>() == null)
                {
                    return somefunctions.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<OrbitAttack>(), holder.transform);
                }
                break;
            case Boomerang:
                if (addToExisting || holder.transform.getvars<Boomerang>() == null)
                {
                    return somefunctions.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<Boomerang>(), holder.transform);
                }
                break;
            case directionChange:
                if (addToExisting || holder.transform.getvars<DirectionChangeAttack>() == null)
                {
                    return somefunctions.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<DirectionChangeAttack>(), holder.transform);
                }
                break;
            case charge:
                if (addToExisting || holder.transform.getvars<MeleeChargeAttack>() == null)
                {
                    return somefunctions.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<MeleeChargeAttack>(), holder.transform);
                }
                break;
            case shield:
                if (addToExisting || holder.transform.getvars<Shield>() == null)
                {
                    return somefunctions.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<Shield>(), holder.transform);
                }
                break;
            case summoner:
                if (addToExisting || holder.transform.getvars<SummonAttack>() == null)
                {
                    return somefunctions.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<SummonAttack>(), holder.transform);
                }
                break;
            case machine:
                if (addToExisting || holder.transform.getvars<MachineGun>() == null)
                {
                    return somefunctions.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<MachineGun>(), holder.transform);
                }
                break;
            case heal:
                if (addToExisting || holder.transform.getvars<BasicHeal>() == null)
                {
                    somefunctions.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<BasicHeal>(), holder.transform);
                }
                break;
        }
        return null;
    }
}


public static class Inputs
{
    [Serializable]
    public struct InputStruct
    {
        public KeyCode[] MainKey;       //at least 1 MainKey must be pressed
        public KeyCode[] SecondaryKeys; //after a mainkey is pressed all secondaries keys must be pressed
        private Func<KeyCode, bool> _inputFunction;
        [SerializeField, ReadOnlyOnInspectorDuringPlay]
        private InputFunctionEnum _inputFunctionEnum;
        public Func<KeyCode, bool> InputFunction
        {
            get
            {
                if (_inputFunction == null)
                {
                    switch (_inputFunctionEnum)
                    {
                        case InputFunctionEnum.GetKey:
                            return _inputFunction = Input.GetKey;
                        case InputFunctionEnum.GetKeyDown:
                            return _inputFunction = Input.GetKeyDown;
                        case InputFunctionEnum.GetKeyUp:
                            return _inputFunction = Input.GetKeyUp;
                    }
                }
                return _inputFunction;
            }
            set
            {
                _inputFunction = value;
            }
        }
        public int TimesNeeded;
        private int _timesLeft;
        private float _lastTime;
        private const float _TIME_DIFFERENCE = 0.3f;

        //private string tostring;
        private enum InputFunctionEnum
        {
            GetKey,
            GetKeyDown,
            GetKeyUp,
        }
        public bool CheckInput()
        {
            if (CheckInputKey())
            {
                if (TimesNeeded == 1) //if it is a one-time press then don't bother to check other stuff
                {
                    return true;
                }
                //               Debug.Log(Time.time - LastTime);
                if (_timesLeft == TimesNeeded) // this case checks if it's the first time pressed in a sequence
                {
                    _timesLeft--;
                    _lastTime = Time.time;
                    return false;
                }
                else if (Time.time - _lastTime < _TIME_DIFFERENCE) // if it's not the first time then check if it within the allowed time differnce of the previous press
                {
                    if (_timesLeft <= 1)  // if it is on time and only 1 time was left then its the last of the queue so next time its pressed it should be treated as first time, thus _timesLeft=TimesNeeded
                    {                      // ideally its times left==1, but just in case...
                        if (_timesLeft != 1)
                        {
                            Debug.Break();
                            Debug.Log(_timesLeft + " times_left != 1");
                        }
                        _timesLeft = TimesNeeded;
                        _lastTime = Time.time;
                        return true;
                    }
                    else  //if times left is greater than 1 then move on normally by subtracting 1 from times left
                    {
                        _timesLeft--;
                        _lastTime = Time.time;
                        return false;
                    }
                }
                else //fail to press again withing time means a new queue has started thus times_left is 1 lower than times needed
                {
                    _timesLeft = TimesNeeded - 1;
                    _lastTime = Time.time;
                    return false;
                }
            }
            return false;
        }

        private bool CheckInputKey()
        {
            if (SecondaryKeys.Length == 0)
            {
                for (int i = 0; i < MainKey.Length; i++)
                {
                    if (InputFunction(MainKey[i]))
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                bool activated = false;
                for (int i = 0; i < MainKey.Length; i++)
                {
                    if (InputFunction(MainKey[i]))
                    {
                        activated = true;
                    }
                }
                if (!activated)
                {
                    return false;
                }
            }
            for (int i = 0; i < SecondaryKeys.Length; i++)
            {
                if (!Input.GetKey(SecondaryKeys[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public override string ToString()
        {
            string temp = "";
            for (int i = 0; i < SecondaryKeys.Length; i++)
            {
                temp += KeyToString(SecondaryKeys[i]) + " + ";
            }
            if (MainKey.Length != 0)
            {
                temp += KeyToString(MainKey[0]);
            }
            return temp;
        }

        static string KeyToString(KeyCode key)
        {
            if (KeyCode.Alpha0 <= key && key <= KeyCode.Alpha9)
            {
                return ((int)key - (int)KeyCode.Alpha0).ToString();
            }
            else if (KeyCode.Mouse0 <= key && key <= KeyCode.Mouse2)
            {
                switch (key)
                {
                    case KeyCode.Mouse0:
                        return "LClick";
                    case KeyCode.Mouse1:
                        return "RClick";
                    case KeyCode.Mouse2:
                        return "MClick";
                    default:
                        return "UknClick";
                }
                /*if (key == KeyCode.Mouse0)
                {
                    return "LClick";
                }
                else if (key == KeyCode.Mouse1)
                {
                    return "RClick";
                }
                else
                {
                    return "MClick";
                }*/
            }
            else if (key == KeyCode.Return || key == KeyCode.KeypadEnter)
            {
                return "Enter";
            }
            else if (key == KeyCode.LeftControl || key == KeyCode.RightControl)
            {
                return "Ctrl";
            }
            else if (key == KeyCode.LeftAlt || key == KeyCode.RightAlt)
            {
                return "Alt";
            }
            else
            {
                return key.ToString();
            }
        }

        public InputStruct(Func<KeyCode, bool> function)
        {
            _inputFunction = function;
            if (function == Input.GetKey)
            {
                _inputFunctionEnum = InputFunctionEnum.GetKey;
            }
            else if (function == Input.GetKeyDown)
            {
                _inputFunctionEnum = InputFunctionEnum.GetKeyDown;
            }
            else if (function == Input.GetKeyUp)
            {
                _inputFunctionEnum = InputFunctionEnum.GetKeyUp;
            }
            else
            {
                _inputFunctionEnum = default;
            }
            MainKey = new KeyCode[0];
            SecondaryKeys = new KeyCode[0];
            TimesNeeded = 1;
            _timesLeft = TimesNeeded;
            _lastTime = 0f;
        }

        public InputStruct(Func<KeyCode, bool> function, KeyCode[] keys) : this(function)
        {
            MainKey = keys;
        }
        public InputStruct(Func<KeyCode, bool> function, KeyCode[] mainkeys, KeyCode[] secondaryKeys) : this(function, mainkeys)
        {
            SecondaryKeys = secondaryKeys;
        }
        public InputStruct(Func<KeyCode, bool> function, KeyCode[] mainkeys, KeyCode[] secondaryKeys, int times) : this(function, mainkeys, secondaryKeys)
        {
            TimesNeeded = times;
        }

        public InputStruct(KeyCode key) : this(Input.GetKey, new KeyCode[] { key })
        { }
        public InputStruct(Func<KeyCode, bool> function, KeyCode key) : this(function, new KeyCode[] { key })
        { }
        public InputStruct(Func<KeyCode, bool> function, KeyCode key, int times) : this(function, new KeyCode[] { key }, times)
        { }
        public InputStruct(Func<KeyCode, bool> function, KeyCode[] keys, int times) : this(function, keys, new KeyCode[0], times)
        { }
        public InputStruct(Func<KeyCode, bool> function, KeyCode mainkey, KeyCode[] secondaryKeys) : this(function, new KeyCode[] { mainkey }, secondaryKeys)
        { }
        public InputStruct(Func<KeyCode, bool> function, KeyCode mainkey, KeyCode[] secondaryKeys, int times) : this(function, new KeyCode[] { mainkey }, secondaryKeys, times)
        { }
    }

    public static void InputsOn()
    {
        if (globalvariables.playergameobject == null)
        {
            return;
        }
        BasePlayerControl.shouldBeActive = true;
        //globalvariables.playergameobject.getvars<playercontrols>().enabled = true;
    }
    public static void InputsOff()
    {
        if (globalvariables.playergameobject == null)
        {
            return;
        }
        BasePlayerControl.shouldBeActive = false;
        //globalvariables.playergameobject.getvars<playercontrols>().enabled = false;
    }
    public static void InputsOnOff(bool on)
    {
        if (globalvariables.playergameobject == null)
        {
            return;
        }
        globalvariables.playergameobject.getvars<playercontrols>().enabled = on;
    }


    public static int getNumberPressed(mylib.boolfunctionKeycode getkey)
    {
        for (int i = 0; i < 10; i++)
        {
            if (getkey(KeyCode.Alpha0 + i) || getkey(KeyCode.Keypad0 + i))
            {
                return i;
            }
        }
        return -1;
    }

    public static mylib.boolfunctionKeycode domouseinputsfunctions(int button,int down_up)
    {
        switch (System.Math.Sign(down_up))
        {
            case -1:
                switch (button)
                {
                    case 0:
                        return LeftClickDown;
                    case 1:
                        return RightClickDown;
                    case 2:
                        return MiddleClickDown;
                }
                break;
            case 0:
                switch (button)
                {
                    case 0:
                        return LeftClick;
                    case 1:
                        return RightClick;
                    case 2:
                        return MiddleClick;
                }
                break;
            case 1:
                switch (button)
                {
                    case 0:
                        return LeftClickUp;
                    case 1:
                        return RightClickUp;
                    case 2:
                        return MiddleClickUp;
                }
                break;
        }
        return null;
    }

    public static bool LeftClickDown()
    {
        return Input.GetMouseButtonDown(0);
    }

    public static bool RightClickDown()
    {
        return Input.GetMouseButtonDown(1);
    }

    public static bool MiddleClickDown()
    {
        return Input.GetMouseButtonDown(2);
    }

    
    public static bool LeftClick()
    {
        return Input.GetMouseButton(0);
    }

    public static bool RightClick()
    {
        return Input.GetMouseButton(1);
    }

    public static bool MiddleClick()
    {
        return Input.GetMouseButton(2);
    }

    
    public static bool LeftClickUp()
    {
        return Input.GetMouseButtonUp(0);
    }

    public static bool RightClickUp()
    {
        return Input.GetMouseButtonUp(1);
    }

    public static bool MiddleClickUp()
    {
        return Input.GetMouseButtonUp(2);
    }

    
    public static bool LeftClickDown(KeyCode key)
    {
        return Input.GetMouseButtonDown(0);
    }

    public static bool RightClickDown(KeyCode key)
    {
        return Input.GetMouseButtonDown(1);
    }

    public static bool MiddleClickDown(KeyCode key)
    {
        return Input.GetMouseButtonDown(2);
    }


    public static bool LeftClick(KeyCode key)
    {
        return Input.GetMouseButton(0);
    }

    public static bool RightClick(KeyCode key)
    {
        return Input.GetMouseButton(1);
    }

    public static bool MiddleClick(KeyCode key)
    {
        return Input.GetMouseButton(2);
    }


    public static bool LeftClickUp(KeyCode key)
    {
        return Input.GetMouseButtonUp(0);
    }

    public static bool RightClickUp(KeyCode key)
    {
        return Input.GetMouseButtonUp(1);
    }

    public static bool MiddleClickUp(KeyCode key)
    {
        return Input.GetMouseButtonUp(2);
    }


    public static Vector2 getmouseposOnscreen
    {
        get
        {
            Vector2 mousepos = Input.mousePosition;
            mousepos.y = Screen.height - mousepos.y;
            return mousepos;
        }
    }

    public static Vector3 getmouseposfromscreen
    {
        get
        {
            if (globalvariables.mouse != null)
            {
                return globalvariables.mouse.position;
            }
            else
            {
                Vector3 temp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                temp.z = 0f;
                return temp;
            }
        }
    }

}

public static class imagelib
{

    /*
    //private static string _pathToGeneratedTextures => Application.persistentDataPath + "/Resources/Generated Textures/";
    //private static int a = 0;
    private static Texture2D NewTexture2DFromTexture2D(Texture2D texture)
    {

        
        var bytes = texture.EncodeToPNG();
        var file = new System.IO.File.Open(_pathToGeneratedTextures + texture.name + a++, FileMode.Create);
        var binary = new System.IO.BinaryWriter(file);
        binary.Write(bytes);
        file.Close();
        //System.IO.File.WriteAllBytes(_pathToGeneratedTextures + texture.name + a++, texture.EncodeToPNG());
        return Resources.Load<Texture2D>(_pathToGeneratedTextures + texture.name + a++,);
    }
    */

    public static List<Texture2D> GeneratedTextures;

    static imagelib()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            colorlib.ColoredSprites = new List<colorlib.coloredspritelist>();
            GeneratedTextures = new List<Texture2D>();
        }
#else
        colorlib.coloredsprites = new List<colorlib.coloredspritelist>();
        GeneratedTextures = new List<Texture2D>();
#endif
    }

    public static Texture2D SpriteToNewTexture2D(Sprite sprite)
    {
        /* Debug.Log(sprite.rect.width);
         Debug.Log(sprite.textureRect.width);*/
        Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
        //return NewTexture2DFromTexture2D(sprite.texture);
        if (GeneratedTextures == null)
        {
            GeneratedTextures = new List<Texture2D>();
        }
        GeneratedTextures.Add(newText);
        newText.SetPixels(sprite.texture.GetPixels((int)System.Math.Ceiling(sprite.rect.x),
                                                     (int)System.Math.Ceiling(sprite.rect.y),
                                                     (int)System.Math.Ceiling(sprite.rect.width),
                                                     (int)System.Math.Ceiling(sprite.rect.height)));
        newText.Apply();
        return newText;
        
        /* try
         {
             if (sprite.rect.width != sprite.texture.width)
             {
                 Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
                 Color[] colors = newText.GetPixels();
                 Color[] newColors = sprite.texture.GetPixels((int)System.Math.Ceiling(sprite.textureRect.x),
                                                              (int)System.Math.Ceiling(sprite.textureRect.y),
                                                              (int)System.Math.Ceiling(sprite.textureRect.width),
                                                              (int)System.Math.Ceiling(sprite.textureRect.height));
                 newText.SetPixels(newColors);
                 newText.Apply();
                 return newText;
             }
             else
                 return sprite.texture;
         }
         catch
         {
             return sprite.texture;
         }*/
    }


    public static images.image getimage(Sprite pic)
    {
        images.image im;
        im.sprite = pic;
        im.texture = SpriteToNewTexture2D(pic);
        return im;
    }

    public static images.image getimage(Texture2D pic)
    {
        images.image im;
        im.texture = pic;
        im.sprite = Sprite.Create(pic, new Rect(0f, 0f, pic.width, pic.height), new Vector2(0.5f, 0.5f));
        return im;
    }

    public static images.image getimage(Texture2D pic, Vector3 pivot)
    {
        images.image im;
        im.texture = pic;
        im.sprite = Sprite.Create(pic, new Rect(0f, 0f, pic.width, pic.height), pivot);
        return im;
    }
    public static Texture2D RectangleTextureSetColor(this Texture2D texture, Color color)
    {
        texture.SetPixel(0, 0, color);
        texture.Apply();
        return texture;
    }



}



public static class colorlib
{

    public static Transform[] teams;// = new Transform[] { GameObject.Find("green").transform, GameObject.Find("blue").transform, GameObject.Find("red").transform, GameObject.Find("yellow").transform };
    public const int green = 0, blue = 1, red = 2, yellow = 3;
    public readonly static Color BASE_COLOR_TO_REPLACE = new Color32(52, 52, 52, 255); //+1 of actual
    public readonly static Color BASE_COLOR_TO_REPLACE_TRUE = new Color32(51, 51, 51, 255);

    public static Color setColorsAndA(Color newColor, float a)
    {
        return new Color(newColor.r, newColor.g, newColor.b, a);
    }


    [System.Serializable]
    public struct coloredsprite
    {
        public Sprite sprite;
        public Color color;

        public coloredsprite(Sprite newsprite, Color newcolor)
        {
            sprite = newsprite;
            color = newcolor;
        }
    }

    [System.Serializable]
    public struct coloredspritelist
    {
        public string originalname;
        public List<coloredsprite> coloredsprites;

        public Sprite getcoloredsprite(Color color)
        {
            foreach (coloredsprite colsprite in coloredsprites)
            {
                if (colsprite.color == color)
                {
                    return colsprite.sprite;
                }
            }
            return null;
        }


        public coloredspritelist(string name)
        {
            originalname = name;
            coloredsprites = new List<coloredsprite>();
        }

        public coloredspritelist(Sprite sprite, Color color, Color colortochange)
        {
            originalname = sprite.name;
            Sprite newsprite = ReplaceColor(color, sprite, colortochange);
            coloredsprites = new List<coloredsprite>();
            coloredsprites.Add(new coloredsprite(newsprite, color));
        }

        public coloredspritelist(Sprite sprite, string spritename, Color color, Color colortochange)
        {
            originalname = spritename;
            Sprite newsprite = ReplaceColor(color, sprite, colortochange);
            coloredsprites = new List<coloredsprite>();
            coloredsprites.Add(new coloredsprite(newsprite, color));
        }

        public coloredspritelist(string name, coloredsprite newsprite)
        {
            originalname = name;
            coloredsprites = new List<coloredsprite>();
            coloredsprites.Add(newsprite);
        }

        public coloredspritelist(string name, List<coloredsprite> coloredspriteslist)
        {
            originalname = name;
            coloredsprites = new List<coloredsprite>(coloredspriteslist);
        }

    }

    public static List<coloredspritelist> ColoredSprites = null;


    public static Sprite GetSpriteColored(Color newColor, Sprite sprite)
    {
        return GetSpriteColored(newColor, sprite, colorlib.BASE_COLOR_TO_REPLACE);
    }

    public static Sprite GetSpriteColored(Color newColor, Sprite sprite, Color colorToChange)
    {
        if (colorToChange == colorlib.BASE_COLOR_TO_REPLACE_TRUE) colorToChange = colorlib.BASE_COLOR_TO_REPLACE;
        ColoredSprites.ForEach(x => x.coloredsprites.RemoveAll(y => y.sprite == null));
        ColoredSprites.RemoveAll(x => x.coloredsprites.Count == 0);
        int index1 = ColoredSprites.FindIndex(x => x.originalname == sprite.name);
        if (index1 == -1) //first time registering this sprite
        {
            //Debug.Log(0 + sprite.name + newColor + colorToChange);
            ColoredSprites.Add(new coloredspritelist(sprite.name, new coloredsprite(sprite, colorToChange)));
            if (newColor == colorToChange) return sprite;
            Sprite newsprite = ReplaceColor(newColor, sprite, colorToChange);
            ColoredSprites[ColoredSprites.Count - 1].coloredsprites.Add(new coloredsprite(newsprite, newColor));
            return newsprite;
        }
        else
        {
            int index2 = ColoredSprites[index1].coloredsprites.FindIndex(x => x.color == newColor);
            if (index2 == -1) //first time registering this color of the sprite
            {
                if (ColoredSprites[index1].coloredsprites.Count == 0)
                {
                    //Debug.Log(1 + sprite.name + newColor);
                    Sprite newsprite = ReplaceColor(newColor, sprite, colorToChange);
                    ColoredSprites[index1].coloredsprites.Add(new coloredsprite(newsprite, newColor));
                    return newsprite;
                }
                else
                {
                    //Debug.Log(2 + sprite.name + newColor);
                    Sprite newsprite = ReplaceColor(newColor, sprite, colorToChange);
                    ColoredSprites[index1].coloredsprites.Add(new coloredsprite(newsprite, newColor));
                    return newsprite;
                }
            }
            else
            {
                //Debug.Log(3 + sprite.name + newColor);
                return ColoredSprites[index1].coloredsprites[index2].sprite;
            }
        }
    }


    private static Sprite ReplaceColor(Color newcolor, Sprite sprite, Color colortochange)
    {
        Sprite newsprite;
        Texture2D texture = imagelib.SpriteToNewTexture2D(sprite);
        var textcolor = texture.GetPixels();
        int i, len = texture.height * texture.width;
        for (i = 0; i < len; i++)
        {
            if (textcolor[i].a != 0f)
            {
                Vector3 div = divideColors(textcolor[i], colortochange);
                //if(!float.IsInfinity(div.x))
                //Debug.Log(div + " " + (div.x == div.y && div.y == div.z && div.z <= 1f && div.z != 0f));
                if (div.x == div.y && div.y == div.z && div.z <= 1f && div.z != 0f)
                {
                    textcolor[i] = new Color(newcolor.r * div.z, newcolor.g * div.z, newcolor.b * div.z, newcolor.a);
                }
            }
        }
        texture.SetPixels(textcolor);
        texture.Apply();
        newsprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(sprite.pivot.x / texture.width, sprite.pivot.y / texture.height));
        newsprite.name = sprite.name;
        //   MonoBehaviour.Destroy(texture);
        return newsprite;
    }
    /*
    public static Sprite ReplaceColor(Color newcolor, Texture2D texture, Color colortochange)
    {
        Color32[] textcolor = texture.GetPixels32();
        int i, len = texture.height * texture.width;
        for (i = 0; i < len; i++)
        {
            if (textcolor[i] == colortochange)
            {
                textcolor[i] = newcolor;
            }
        }
        texture.SetPixels32(textcolor);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
    */

    /*  public static Color getteamcolor(Transform parent)
      {
          teamvariables vars;
          while (parent != null)
          {
              if (parent.parent == null)
              {
                  if ((vars = parent.GetComponent<teamvariables>()) != null)
                  {
                      return vars.teamcolor;
                  }
                  else
                  {
                      return colorstuff.colortochange;
                  }
              }
              parent = parent.parent;
          }
          return colorstuff.colortochange;
      }*/

    public static Sprite ReplaceSpriteColor(Sprite currentsprite, Sprite[] newsprites, Color newcolor, Color colortochange)
    {
        int i = mylib.GetNumberAtEnd(currentsprite.name);
        if (i == -1)
        {
            Debug.Log("no number at the end of sprite name");
            Debug.Log(currentsprite);
            return currentsprite;
        }
        if (i >= newsprites.Length)
        {
            Sprite[] temparray = new Sprite[i + 1];
            int j;
            for (j = 0; j < newsprites.Length; j++)
            {
                temparray[j] = newsprites[j];
            }
            for (; j < i; j++)
            {
                temparray[j] = null;
            }
            temparray[i] = ReplaceColor(newcolor, currentsprite, colortochange);
            newsprites = new Sprite[++i];
            for (j = 0; j < i; j++)
            {
                newsprites[j] = temparray[j];
            }
        }
        else if (newsprites[i] == null)
        {
            newsprites[i] = ReplaceColor(newcolor, currentsprite, colortochange);
        }
        return newsprites[i];
    }

    static Vector3 divideColors(Color c1, Color c2)
    {
        Vector3 div;
        if(c2.r == 0f)
        {
            if (c1.r == 0f)
            {
                div.x = 1f;
            }
            else
            {
                div.x = float.PositiveInfinity;
            }
        }
        else
        {
            div.x = c1.r / c2.r;
        }
        if (c2.g == 0f)
        {
            if (c1.g == 0f)
            {
                div.y = 1f;
            }
            else
            {
                div.y = float.PositiveInfinity;
            }
        }
        else
        {
            div.y = c1.g / c2.g;
        }
        if (c2.b == 0f)
        {
            if (c1.b == 0f)
            {
                div.z = 1f;
            }
            else
            {
                div.z = float.PositiveInfinity;
            }
        }
        else
        {
            div.z = c1.b / c2.b;
        }
        return div;
    }

    public static Color multiplyToColor(Color c, float mult)
    {
        float r = c.r * mult;
        float g = c.g * mult;
        float b = c.b * mult;
        if (r > 1f)
        {
            r = 1f;
        }
        if (g > 1f)
        {
            g = 1f;
        }
        if (b > 1f)
        {
            b = 1f;
        }
        return new Color(r, g, b, c.a);
    }



    public static int getlayerfromteam(Transform team)
    {
        return layernames.bodies;
    }
  /*      for (int i = 0; i < teams.Length; i++)
        {
            if (teams[i] == team)
            {
                return layernames.greenlayer + i;
            }
        }
        return layernames.bodies;
    }*/
    /* if (team == red.transform)
     {
         return layernames.redlayer;
     }
     else if (team == blue.transform)
     {
         return layernames.bluelayer;
     }
     else if (team == green.transform)
     {
         return layernames.greenlayer;
     }
     else if (team == yellow.transform)
     {
         return layernames.yellowlayer;
     }*/


    public struct HSL
    {
        private float privh;
        public float h
        {
            get
            {
                return privh;
            }
            set
            {
                privh = value;
                while (privh > 1f)
                {
                    privh -= 1f;
                }
                while (privh < 0f)
                {
                    privh += 1f;
                }
            }
        }
        public float s;
        public float l;
        public float a;

        public HSL(float h, float s, float l)
        {
            privh = h;
            this.s = s;
            this.l = l;
            this.a = 1f;
            this.h = privh;
        }

        public HSL(float h, float s, float l, float a)
        {
            privh = h;
            this.s = s;
            this.l = l;
            this.a = a;
            this.h = privh;
        }
        /*public HSL(Color rgb)
        {
            this = rgb.HSL();
        }*/

        public Color RGB
        {
            get
            {
                float htemp = h * 360f;
                float c = (1 - System.Math.Abs(2 * l - 1)) * s;
                float x = c * (1 - System.Math.Abs((htemp / 60f % 2f) - 1f));
                float m = l - c / 2;

                if (htemp < 60)
                {
                    return new Color(c + m, x + m, m, a);
                }
                else if (htemp < 120)
                {
                    return new Color(x + m, c + m, m, a);
                }
                else if (htemp < 180)
                {
                    return new Color(m, c + m, x + m, a);
                }
                else if (htemp < 240)
                {
                    return new Color(m, x + m, c + m, a);
                }
                else if (htemp < 300)
                {
                    return new Color(x + m, m, c + m, a);
                }
                else
                {
                    return new Color(c + m, m, x + m, a);
                }
            }
        }

        public static bool operator ==(HSL color1, HSL color2)
        {
            return color1.h == color2.h && color1.s == color2.s && color1.l == color2.l;
        }
        public static bool operator !=(HSL color1, HSL color2)
        {
            return !(color1 == color2);
            //return color1.h != color2.h || color1.s != color2.s || color1.l != color2.l;
        }


        public bool Equals(HSL other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is HSL && Equals((HSL)obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }

    public static HSL toHSL(this Color rgb)
    {
        return RGBtoHSL(rgb.r, rgb.g, rgb.b, rgb.a);
    }

    public static HSL RGBtoHSL(float r, float g, float b, float a)
    {
        if (r == 0f && g == 0f && b == 0f)
        {
            return new HSL(0, 0, 0, a);
        }
        if(r == 1f && g == 1f && b == 1f)
        {
            return new HSL(0, 0, 1, a);
        }
        float max = r, min = r;
        short maxindex = 1;
        float h, s, l;
        float c;
        if (g > max)
        {
            max = g;
            maxindex = 2;
        }
        else if (g < min)
        {
            min = g;
        }
        if (b > max)
        {
            max = b;
            maxindex = 3;
        }
        else if (b < min)
        {
            min = b;
        }
        l = (min + max) / 2f;
        c = max - min;

        h = 0f;
        if (c == 0f)
        {
            s = 0f;
        }
        else
        {
            s = c / (1 - System.Math.Abs(2 * l - 1));
            switch (maxindex)
            {
                case 1:
                    float segment = (g - b) / c;
                    float shift = 0 / 60;       // R° / (360° / hex sides)
                    if (segment < 0)
                    {          // hue > 180, full rotation
                        shift = 360 / 60;         // R° / (360° / hex sides)
                    }
                    h = segment + shift;
                    break;
                case 2:
                    segment = (b - r) / c;
                    shift = 120 / 60;     // G° / (360° / hex sides)
                    h = segment + shift;
                    break;
                case 3:
                    segment = (r - g) / c;
                    shift = 240 / 60;     // B° / (360° / hex sides)
                    h = segment + shift;
                    break;
            }
        }
        return new HSL(h * (60f / 360f), s, l, a);
    }

    public static HSL invertColor(HSL color)
    {
        color.h += 0.5f;
        color.l = 1f - color.l;
        return color;
    }
    public static Color invertColor(Color color)
    {
        return invertColor(color.toHSL()).RGB;
    }
    public static Color getInvertedColor(this Color color)
    {
        return invertColor(color);
    }


}

public static class audiolib
{

    public static AudioClip dm, wololo, punch, death, grabthrow, grabbed, grabreturn, swing;

}


public static class animfunctions
{
   /* public static void SetSpritesToClip(AnimationClip clip, Sprite[] sprites, float duration)
    {

        EditorCurveBinding spriteBinding = new EditorCurveBinding();
        spriteBinding.type = typeof(SpriteRenderer);
        spriteBinding.path = "";
        spriteBinding.propertyName = "m_Sprite";

        ObjectReferenceKeyframe[] spriteKeyFrames = new ObjectReferenceKeyframe[sprites.Length];
        for (int i = 0; i < (sprites.Length); i++)
        {
            spriteKeyFrames[i] = new ObjectReferenceKeyframe();
            spriteKeyFrames[i].time = i * duration / sprites.Length;
            spriteKeyFrames[i].value = sprites[i];
        }

        AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, spriteKeyFrames);
    }*/



}


public static class rules
{
    private static Floor _floor;
    public static Floor Floor
    {
        get => _floor != null ? _floor : _floor = Object.FindObjectOfType<Floor>();
        set => _floor = value;
    }



    /*public static bool cando<T>(LinkedList<T> candolist, T target) where T : Component
    {
        return candolist.Contains(target) && target.gameObject.activeInHierarchy == true;
    }*/
    public static bool cando<T>(this LinkedList<T> candolist, T target) where T : Component
    {
        for (LinkedListNode<T> curr = candolist.First; curr != null;)
        {
            if (curr.Value == target && target.gameObject.activeInHierarchy)
            {
                return true;
            }
            if (curr.Value == null)
            {
                LinkedListNode<T> toremove = curr;
                curr = curr.Next;
                candolist.Remove(toremove);
                continue;
            }
            curr = curr.Next;
        }
        return false;
    }
    public static bool cando<T>(this LinkedList<T> candolist, T target, bool DoNotCheckActive) where T : Component
    {
        for (LinkedListNode<T> curr = candolist.First; curr != null;)
        {
            if (curr.Value == target && (DoNotCheckActive || target.gameObject.activeInHierarchy))
            {
                return true;
            }
            if (curr.Value == null)
            {
                LinkedListNode<T> toremove = curr;
                curr = curr.Next;
                candolist.Remove(toremove);
                continue;
            }
            curr = curr.Next;
        }
        return false;
    }
    public static bool Contains<T>(this LinkedList<T> candolist, System.Predicate<T> condition)
    {
        for (LinkedListNode<T> curr = candolist.First; curr != null; curr = curr.Next)
        {
            if (condition(curr.Value))
            {
                return true;
            }
        }
        return false;
    }
    public static bool Contains<T>(this LinkedList<T> list, System.Predicate<T> conditionToFind, System.Predicate<T> conditionToRemove)
    {
        for (LinkedListNode<T> curr = list.First; curr != null;)
        {
            if (conditionToRemove(curr.Value))
            {
                LinkedListNode<T> toRemove = curr;
                curr = curr.Next;
                list.Remove(toRemove);
                continue;
            }
            else if (conditionToFind(curr.Value))
            {
                return true;
            }
            curr = curr.Next;
        }
        return false;
    }
    public static bool Remove<T>(this LinkedList<T> list, System.Predicate<T> conditionToRemove)
    {
        for (LinkedListNode<T> curr = list.First; curr != null;)
        {
            if (conditionToRemove(curr.Value))
            {
                LinkedListNode<T> toRemove = curr;
                curr = curr.Next;
                list.Remove(toRemove);
                continue;
            }
            curr = curr.Next;
        }
        return false;
    }
    public static bool Remove<T>(this LinkedList<T> list, System.Predicate<T> conditionToRemove, Action<T> RemoveAction)
    {
        for (LinkedListNode<T> curr = list.First; curr != null;)
        {
            if (conditionToRemove(curr.Value))
            {
                LinkedListNode<T> toRemove = curr;
                curr = curr.Next;
                RemoveAction(toRemove.Value);
                list.Remove(toRemove);
                continue;
            }
            curr = curr.Next;
        }
        return false;
    }
    public static LinkedListNode<T> Find<T>(this LinkedList<T> list, System.Predicate<T> condition)
    {
        for (LinkedListNode<T> curr = list.First; curr != null; curr = curr.Next)
        {
            if (condition(curr.Value))
            {
                return curr;
            }
        }
        return null;
    }
    public static LinkedListNode<T> Find<T>(this LinkedList<T> list, System.Predicate<T> conditionToFind, System.Predicate<T> conditionToRemove)
    {
        for (LinkedListNode<T> curr = list.First; curr != null;)
        {
            if (conditionToFind(curr.Value))
            {
                return curr;
            }
            if (conditionToRemove(curr.Value))
            {
                LinkedListNode<T> toremove = curr;
                curr = curr.Next;
                list.Remove(toremove);
                continue;
            }
            curr = curr.Next;
        }
        return null;
    }

    public static int FindIndex<T>(this LinkedList<T> list, System.Predicate<T> condition)
    {
        int index = 0;
        for (LinkedListNode<T> curr = list.First; curr != null; curr = curr.Next, index++)
        {
            if (condition(curr.Value))
            {
                return index;
            }
        }
        return -1;
    }
    public static int FindIndex<T>(this LinkedList<T> list, System.Predicate<T> conditionToFind, System.Predicate<T> conditionToRemove)
    {
        int index = 0;
        for (LinkedListNode<T> curr = list.First; curr != null; index++)
        {
            if (conditionToFind(curr.Value))
            {
                return index;
            }
            if (conditionToRemove(curr.Value))
            {
                LinkedListNode<T> toremove = curr;
                curr = curr.Next;
                list.Remove(toremove);
                continue;
            }
            curr = curr.Next;
        }
        return -1;
    }

    public static bool collisiondamage(Attack attacker, CollisionInfo collparameters, float damage)
    {
        if (collparameters == null || !collparameters.damagable || ((BaseCharacterControl)collparameters.vars).DamageMode == DamageMode.Invulnerable)
        {
            return false;
        }
        BaseCharacterControl collvars = collparameters.vars as BaseCharacterControl;
        if(collvars != null)
        if (attacker.vars.CanDamage(collvars))
        //if (attacker.vars.CanDamage((BaseCharacterControl)collparameters.vars))
        {
            lifescript colllifevars = collparameters.lifevars;
            if (colllifevars != null)
            {
                if (damage != 0f && colllifevars.life > 0f)
                {
                    if (attacker.vars.DamageMode == DamageMode.Undamageable)
                    {
                        colllifevars.addDamage(attacker, 0f);
                    }
                    else
                    {
                        colllifevars.addDamage(attacker, damage);
                    }
                }
                return true;
            }
            else
            {
                Debug.Log("no colllifevars part1", collparameters);
                Debug.Log("no colllifevars part2", attacker);
                Debug.Break();
                return false;
            }
        }
        return false;
    }


    public static bool CheckToBlockAttack(this Projectile attacker, CollisionInfo collparameters)
    {
        if (collparameters == null || !collparameters.stopattacks)
        {
            return false;
        }
        BaseCharacterControl attackervars = attacker.getAttack().vars;
        BaseCharacterControl collvars = collparameters.vars as BaseCharacterControl;
        if (collvars == null || collvars.CanDamage(attackervars))
        {
            attacker.blocked();
            return true;
        }
        return false;
    }

    /*
    public static void addToCandoAndToOthers<T>(T[] enemies, Transform ToAdd, bool ToAddcantarget, bool enemiescantarget) where T : Component
    {
        if (ToAddcantarget)
        {
            BaseCharacterControl vars = ToAdd.getvars<BaseCharacterControl>();
            addToCando(new LinkedList<Transform>[] { vars.cantarget, vars.candamage }, enemies);
        }
        else
        {
            addToCando(ToAdd.getvars<BaseCharacterControl>().candamage, enemies);
        }
        addToOthersCando(enemies, ToAdd.getvarsTR<move>(), enemiescantarget);
    }

    public static void addToCandoAndToOthers(GameObject[] enemies, Transform ToAdd, bool ToAddcantarget, bool enemiescantarget)
    {
        if (ToAddcantarget)
        {
            BaseCharacterControl vars = ToAdd.getvars<BaseCharacterControl>();
            addToCando(new LinkedList<Transform>[] { vars.cantarget, vars.candamage }, enemies);
        }
        else
        {
            addToCando(ToAdd.getvars<BaseCharacterControl>().candamage, enemies);
        }
        addToOthersCando(enemies, ToAdd.getvarsTR<move>(), enemiescantarget);
    }

    public static void addToCandoAndToOthers(Transform enemyteam, Transform ToAdd, bool ToAddcantarget, bool enemiescantarget)
    {
        if (ToAddcantarget)
        {
            BaseCharacterControl vars = ToAdd.getvars<BaseCharacterControl>();
            addToCando(new LinkedList<Transform>[] { vars.cantarget, vars.candamage }, enemyteam);
        }
        else
        {
            addToCando(ToAdd.getvars<BaseCharacterControl>().candamage, enemyteam);
        }
        addToOthersCando(enemyteam, ToAdd.getvarsTR<move>(), enemiescantarget);
    }




    public static void addToCando(LinkedList<Transform>[] candolists, Transform team)
    {
        for (int i = 0; i < team.childCount; i++)
        {
            foreach (LinkedList<Transform> candolist in candolists)
            {
                candolist.AddLast(team.GetChild(i).getvarsTR<move>());
            }
        }
    }

    public static void addToCando<T>(LinkedList<Transform>[] candolists, T[] enemies) where T : Component
    {
        
        foreach (T enemy in enemies)
        {
            foreach (LinkedList<Transform> candolist in candolists)
            {
                candolist.AddLast(enemy.getvarsTR<move>());
            }
        }
    }

    public static void addToCando(LinkedList<Transform>[] candolists, GameObject[] enemies)
    {

        foreach (GameObject enemy in enemies)
        {
            foreach (LinkedList<Transform> candolist in candolists)
            {
                candolist.AddLast(enemy.getvarsTR<move>());
            }
        }
    }

    public static void addToCando(LinkedList<Transform> candolist, Transform team)
    {
        for (int i = 0; i < team.childCount; i++)
        {
            candolist.AddLast(team.GetChild(i).getvarsTR<move>());
        }
    }

    public static void addToCando<T>(LinkedList<Transform> candolist, T[] enemies) where T : Component
    {
        
        foreach (T enemy in enemies)
        {
            candolist.AddLast(enemy.getvarsTR<move>());
        }
    }

    public static void addToCando(LinkedList<Transform> candolist, GameObject[] enemies)
    {

        foreach (GameObject enemy in enemies)
        {
            candolist.AddLast(enemy.getvarsTR<move>());
        }
    }




    public static void addToOthersCando<T>(T[] enemies, Transform ToAdd, bool cantarget) where T : Component
    {
        if (cantarget)
        {
            foreach (T enemy in enemies)
            {
                if (enemy != null)
                {
                    BaseCharacterControl vars = enemy.getvars<BaseCharacterControl>();
                    vars.candamage.AddLast(ToAdd);
                    vars.cantarget.AddLast(ToAdd);
                }
            }
        }
        else
        {
            foreach (T enemy in enemies)
            {
                if (enemy != null)
                {
                    enemy.getvars<BaseCharacterControl>().candamage.AddLast(ToAdd);
                }
            }
        }
    }
    public static void addToOthersCando(GameObject[] enemies, Transform ToAdd, bool cantarget)
    {
        if (cantarget)
        {
            foreach (GameObject enemy in enemies)
            {
                if (enemy != null)
                {
                    BaseCharacterControl vars = enemy.getvars<BaseCharacterControl>();
                    vars.candamage.AddLast(ToAdd);
                    vars.cantarget.AddLast(ToAdd);
                }
            }
        }
        else
        {
            foreach (GameObject enemy in enemies)
            {
                if (enemy != null)
                {
                    enemy.getvars<BaseCharacterControl>().candamage.AddLast(ToAdd);
                }
            }
        }
    }

    public static void addToOthersCando(Transform enemyteam, Transform ToAdd, bool cantarget)
    {
        if (cantarget)
        {
            for (int i = 0; i < enemyteam.childCount; i++)
            {
                BaseCharacterControl vars = enemyteam.GetChild(i).getvars<BaseCharacterControl>();
                vars.candamage.AddLast(ToAdd);
                vars.cantarget.AddLast(ToAdd);
            }
        }
        else
        {
            for (int i = 0; i < enemyteam.childCount; i++)
            {
                enemyteam.GetChild(i).getvars<BaseCharacterControl>().candamage.AddLast(ToAdd);
            }
        }
    }

    */
}


public static class mathlib
{

    public const double PI = System.Math.PI;
    public const double TAU = 2.0 * PI;
    public const double radtodeg = 180.0 / PI;
    public const double degtorad = PI / 180.0;
    public const double sqrtof2 = 1.4142135623730950488;
    public const double invsqrtof2 = 0.7071067811865475244;

    public static int sq(this int number)
    {
        return number * number;
    }
    public static float sq(this float number)
    {
        return number * number;
    }
    public static double sq(this double number)
    {
        return number * number;
    }
    public static decimal sq(this decimal number)
    {
        return number * number;
    }


    public static Vector3 polarvectrad(float magnitude, double rad)
    {
        return magnitude * new Vector3((float)System.Math.Cos(rad), (float)System.Math.Sin(rad), 0f);
    }

    public static Vector3 polarvectrad(double rad)
    {
        return new Vector3((float)System.Math.Cos(rad), (float)System.Math.Sin(rad), 0f);
    }

    public static Vector3 polarvectrad(float x, float y, double rad)
    {
        return new Vector3(x * (float)System.Math.Cos(rad), y * (float)System.Math.Sin(rad), 0f);
    }

    public static Vector3 polarvectdeg(float magnitude, double degrees)
    {
        return magnitude * new Vector3((float)System.Math.Cos(degrees *= degtorad), (float)System.Math.Sin(degrees), 0f);
    }

    public static Vector3 polarvectdeg(double degrees)
    {
        return new Vector3((float)System.Math.Cos(degrees *= degtorad), (float)System.Math.Sin(degrees), 0f);
    }
    public static Vector3 polarvectdeg(float x, float y, double degrees)
    {
        return new Vector3(x * (float)System.Math.Cos(degrees *= degtorad), y * (float)System.Math.Sin(degrees), 0f);
    }

    public static Vector3 Conjugatevect(Vector3 vect)
    {
        return new Vector3(vect.x, -vect.y);
    }

    public static Vector3 AbsElementWise(Vector3 vect)
    {
        return new Vector3(System.Math.Abs(vect.x), System.Math.Abs(vect.y));
    }

    public static Vector3 MulElementWise(Vector3 vect1,Vector3 vect2)
    {
        return new Vector3(vect1.x * vect2.x, vect1.y * vect2.y);
    }
    public static Vector3 MulElementWise(Vector3 vect1, float x, float y)
    {
        return new Vector3(vect1.x * x, vect1.y * y);
    }
    public static Vector3 MulElementWise(float x1, float y1, float x2, float y2)
    {
        return new Vector3(x1 * x2, y1 * y2);
    }

    public static Vector3 mulcmplxvect(Vector3 vect1, Vector3 vect2)
    {                                                                       //vect1*vect2
        return new Vector3(vect1.x * vect2.x - vect1.y * vect2.y, vect1.y * vect2.x + vect1.x * vect2.y);
    }
    public static Vector3 mulcmplxvect(float x, float y, Vector3 vect2)
    {                                                                       //vect1*vect2
        return new Vector3(x * vect2.x - y * vect2.y, y * vect2.x + x * vect2.y);
    }
    public static Vector3 mulcmplxvect(float x1, float y1, float x2, float y2)
    {                                                                       //vect1*vect2
        return new Vector3(x1 * x2 - y1 * y2, y1 * x2 + x1 * y2);
    }

    public static Vector3 mulcmplxvectconj(Vector3 vect1, Vector3 vect2)
    {                                                                       //vect1*conj(vect2)
        return new Vector3(vect1.x * vect2.x + vect1.y * vect2.y, vect1.y * vect2.x - vect1.x * vect2.y, 0f);
    }

    public static Vector3 rotate90(Vector3 vect_to_rotate, Vector3 centre)
    {                                                                                      //rotate a vector to a centre by 90 degrees
        return rotate90(vect_to_rotate - centre) + centre;
    }

    public static Vector3 rotate90(Vector3 vect_to_rotate)
    {                                                                                      //rotate a vector by 90 degrees
        return new Vector3(-vect_to_rotate.y, vect_to_rotate.x);
    }

    public static Vector3 rotatedeg(Vector3 vect_to_rotate, double degrees, Vector3 centre)
    {                                                                                      //rotate a vector to a centre by angle in degrees
        return mulcmplxvect(vect_to_rotate - centre, polarvectdeg(1f, degrees)) + centre;
    }

    public static Vector3 rotatedeg(Vector3 vect_to_rotate, double degrees)
    {                                                                                      //rotate a vector by angle in degrees
        return mulcmplxvect(vect_to_rotate, polarvectdeg(1f, degrees));
    }

    public static Vector3 rotaterad(Vector3 vect_to_rotate, double rad, Vector3 centre)
    {                                                                                      //rotate a vector to a centre by angle in rad
        return mulcmplxvect(vect_to_rotate - centre, polarvectrad(1f, rad)) + centre;
    }

    public static Vector3 rotaterad(Vector3 vect_to_rotate, double rad)
    {                                                                                      //rotate a vector by angle in rad
        return mulcmplxvect(vect_to_rotate, polarvectrad(1f, rad));
    }

    public static Vector3 rotatevector(Vector3 pos, Vector3 rotate, Vector3 centre)
    {                                                                                      //rotate a vector to a centre by angle of a vector
        return mulcmplxvect(pos - centre, rotate) + centre;
    }

    public static Vector3 rotatevector(Vector3 pos, Vector3 rotate)
    {                                                                                      //rotate a vector by angle of a vector
        return mulcmplxvect(pos, rotate);
    }

    public static void refrotatedeg(ref Vector3 pos, double degrees, Vector3 centre)
    {
        pos = mulcmplxvect(pos - centre, polarvectdeg(1f, degrees)) + centre;
    }

    public static void refrotatedeg(ref Vector3 pos, double degrees)
    {
        pos = mulcmplxvect(pos, polarvectdeg(1f, degrees));
    }

    public static void refrotaterad(ref Vector3 pos, double rad, Vector3 centre)
    {
        pos = mulcmplxvect(pos - centre, polarvectrad(1f, rad)) + centre;
    }

    public static void refrotaterad(ref Vector3 pos, double rad)
    {
        pos = mulcmplxvect(pos, polarvectrad(1f, rad));
    }
    
    /* public static double distance(Vector2 v1_sub_v2)
     {
         return System.Math.Sqrt(v1_sub_v2.x * v1_sub_v2.x + v1_sub_v2.y * v1_sub_v2.y);
     }

     public static double distance2(Vector2 v1_sub_v2)
     {
         return v1_sub_v2.x * v1_sub_v2.x + v1_sub_v2.y * v1_sub_v2.y;
     }*/

    public static double anglevectordeg(Vector2 vector)
    {
        return System.Math.Atan2(vector.y, vector.x) * radtodeg;
    }

    public static double anglevectorrad(Vector2 vector)
    {
        return System.Math.Atan2(vector.y, vector.x);
    }

    /*  public static Vector3 getunitvector(Vector3 vect)
      {
          if (vect != Vector3.zero)
          {
              vect /= vect.magnitude;
          }
          return vect;
      }*/

    public static Vector3 getunitvector2(Vector3 vect)
    {
        if (vect == Vector3.zero)
        {
            return new Vector3(1f, 0f, 0f);
        }
        else
        {
            return vect / vect.magnitude;
        }
    }

    /*   public static float dotproduct(Vector3 v1,Vector3 v2)
       {
           return v1.x * v2.x + v1.y * v2.y;
       }*/

    public static Vector3 projectOn(this Vector3 toproject, Vector3 onproject)
    {
        return Vector3.Dot(toproject, onproject) / onproject.sqrMagnitude * onproject;
    }
    public static Vector3 projectOnNormed(this Vector3 toproject, Vector3 onproject)
    {
        return Vector3.Dot(toproject, onproject) * onproject;
    }

    public static Vector2 projectOn(this Vector2 toproject, Vector2 onproject)
    {
        return Vector2.Dot(toproject, onproject) / onproject.sqrMagnitude * onproject;
    }
    public static Vector2 projectOnNormed(this Vector2 toproject, Vector2 onproject)
    {
        return Vector2.Dot(toproject, onproject) * onproject;
    }

    public static int evenneg_oddpos(int n) //if n is even then return -1, else if its odd then return 1
    {
        return ((n & 1) << 1) - 1;  // (n%2)*2 - 1
    }

    public static int evenpos_oddneg(int n) //if n is even then return 1, else if its odd then return -1
    {
        return 1 - ((n & 1) << 1); // 1 - (n%2)*2
    }
    public static bool isEven(this int number)
    {
        return (number & 1) == 0;
    }
    public static bool isOdd(this int number)
    {
        return (number & 1) == 1;
    }

    public static bool isInsideSquare(float x, float y, float edgeSize)
    {
        return System.Math.Abs(x + y) + System.Math.Abs(x - y) < edgeSize;
    }
    public static bool isInsideSquare(Vector2 xy, float edgeSize)
    {
        return isInsideSquare(xy.x, xy.y, edgeSize);
    }
    public static bool isInsideSquare(Vector3 xy, float edgeSize)
    {
        return isInsideSquare(xy.x, xy.y, edgeSize);
    }
    public static bool isInsideSquare(float x, float y, float edgeSize,float xcentre,float ycentre)
    {
        x -= xcentre;
        y -= ycentre;
        return isInsideSquare(x, y, edgeSize);
    }
    public static bool isInsideSquare(Vector2 xy, float edgeSize, Vector2 centre)
    {
        xy -= centre;
        return isInsideSquare(xy.x, xy.y, edgeSize);
    }
    public static bool isInsideSquare(Vector3 xy, float edgeSize, Vector3 centre)
    {
        xy -= centre;
        return isInsideSquare(xy.x, xy.y, edgeSize);
    }


    public static Rect rectangleCentre(this Rect RectWithImaginedCentre, float xcentre, float ycentre)
    /* xcentre,ycentre : centre relative to rectangle
     * 
     *    -1     -1    : upper left (default/unchanged)
     *    -1      0    : y-middle left
     *    -1      1    : lower left
     *     0     -1    : upper x-middle
     *     0      0    : y-middle  x-middle
     *     0      1    : lower  x-middle
     *     1     -1    : upper right
     *     1      0    : y-middle  right
     *     1      1    : lower  right
      */
    {
        /*switch (xcentre)
        {
            case -1:
                switch (ycentre)
                {
                    case -1:
                        return imaginaryCentre;
                    case 0:
                        return new Rect(imaginaryCentre.x, imaginaryCentre.y - imaginaryCentre.height / 2f, imaginaryCentre.width, imaginaryCentre.height);
                    case 1:
                        return new Rect(imaginaryCentre.x, imaginaryCentre.y + imaginaryCentre.height / 2f, imaginaryCentre.width, imaginaryCentre.height);
                }
                break;
        }*/

        if (xcentre == -1f && ycentre == -1f)
        {
            return RectWithImaginedCentre;
        }
        return new Rect(RectWithImaginedCentre.x - (xcentre + 1f) * RectWithImaginedCentre.width / 2f , RectWithImaginedCentre.y - (ycentre + 1f) * RectWithImaginedCentre.height / 2f , RectWithImaginedCentre.width + 0.5f, RectWithImaginedCentre.height+0.5f);
    }
    public static Rect rectangleCentre(this Rect RectWithImaginedCentre, TextAnchor alignment)
    { 
        float xcentre = ((int)alignment % 3) - 1f;
        float ycentre = ((int)alignment / 3) - 1f;
        return RectWithImaginedCentre.rectangleCentre(xcentre, ycentre);
    }
    public static Rect rectangleCentre(this Rect RectWithImaginedCentre, GUIStyle style)
    {
        return RectWithImaginedCentre.rectangleCentre(style.alignment);
    }

    public static int bitCount(this uint n)
    {
        int count = 0;
        for (; n != 0; n &= n - 1, ++count) ;
        return count;
    }

    public static int bitCount(this ulong n)
    {
        int count = 0;
        for (; n != 0; n &= n - 1, ++count) ;
        return count;
    }
}

public static class timelib
{


    public struct hms
    {
        int hours;
        int minutes;
        float seconds;

        public int h
        {
            get
            {
                return hours;
            }
        }

        public int m
        {
            get
            {
                return minutes;
            }
        }

        public float s
        {
            get
            {
                return seconds;
            }
        }

        public hms(int h, int m, float s)
        {
            hours = h;
            minutes = m;
            seconds = s;

            normalize();
        }

        public static hms ConvertFromSeconds(float givenseconds)
        {
            hms time = new hms(0, 0, givenseconds);
            time.normalize();
            return time;
        }

        public static hms ConvertFromMinutes(float givenminutes)
        {
            int minutes = (int)givenminutes;
            hms time = new hms(0, minutes, (givenminutes - minutes) * 60f);
            time.normalize();
            return time;
        }

        public static hms ConvertFromHours(float givenhours)
        {
            int hours = (int)givenhours;
            hms time = ConvertFromMinutes((givenhours - hours) * 60f);
            time.hours = hours;
            time.normalize();
            return time;
        }

        public void normalize()
        {
            int addednextunits = (int)(seconds / 60f);
            seconds -= addednextunits * 60f;
            minutes += addednextunits;

            addednextunits = (int)(minutes / 60f);
            minutes -= addednextunits * 60;
            hours += addednextunits;
        }

        void addtonext(ref int nextunits, ref int units, int ratio)
        {
            int addednextunits = (int)(units / (float)ratio);
            units -= addednextunits * ratio;
            nextunits += addednextunits;
        }

        public static hms operator +(hms clock1, hms clock2)
        {
            hms result = new hms(clock1.hours + clock2.hours, clock1.minutes + clock2.minutes, clock1.seconds + clock2.seconds);
            result.normalize();
            return result;
        }

        public override string ToString()
        {
            return string.Format("{0}:{1:00}:{2:00.00}", hours, minutes, seconds);
        }

        public string ToStringAvail()
        {
            string temp = "";
            if (hours != 0)
            {
                temp += hours.ToString() + "h ";
            }
            if (minutes != 0)
            {
                temp += minutes.ToString() + "m ";
            }
            temp += string.Format("{0:0.00}", seconds) + "s";
            return temp;
        }

    }

    [Serializable]
    public class timer
    {

        public static float timePaused = 0f;

        static float actualTime
        {
            get
            {
                return Time.time - timePaused;
            }
        }
        [NonSerialized]
        float timeMoment = float.MinValue;
        [field: SerializeField]
        public float TotalCooldown { get; private set; }
        public timer()
        {
            TotalCooldown = 0f;
        }

        public timer(float TimeToCount)
        {
            TotalCooldown = TimeToCount;
        }

        public void startTimer()
        {
            timeMoment = actualTime;
        }

        public void startTimer(float delay)
        {
            timeMoment = actualTime + delay;
        }

        public void startTimerWithTempCounter(float tempCounter)
        {
            timeMoment = actualTime + tempCounter - TotalCooldown;
        }

        public void startTimerAndSetCounter(float newCounter)
        {
            TotalCooldown = newCounter;
            timeMoment = actualTime;
        }

        public void startTimerAndSetCounter(float newCounter, float delay)
        {
            TotalCooldown = newCounter;
            timeMoment = actualTime + delay;
        }

        public void Delay(float delay)
        {
            timeMoment -= delay;
        }

        public void setCounter(float TimeToCount)
        {
            this.TotalCooldown = TimeToCount;
        }

        public void reduceTimeRemaining(float SecondsToReduce)
        {
            this.timeMoment -= SecondsToReduce;
        }

        public bool checkIfTimePassed()
        {
            return actualTime - timeMoment > TotalCooldown;
        }

        public bool checkIfTimeNotPassed()
        {
            return actualTime - timeMoment < TotalCooldown;
        }

        public float timePassed
        {
            get
            {
                return actualTime - timeMoment;
            }
        }

        public float timeRemaining
        {
            get
            {
                float time_remaining = TotalCooldown - actualTime + timeMoment;
                if (time_remaining < 0f)
                {
                    return 0f;
                }
                else
                {
                    return time_remaining;
                }
            }
        }

        public float timeRemainingRatio
        {
            get
            {
                if (TotalCooldown == 0f)
                {
                    return 0f;
                }
                float ratio = timeRemaining / TotalCooldown;
                if (ratio > 1f)
                {
                    return 1f;
                }
                else if (ratio < 0f)
                {
                    return 0f;
                }
                else
                {
                    return ratio;
                }
            }
        }


    }
    /*

    public static void resetTimer(ref float timemoment)
    {
        timemoment = Time.time;
    }
    public static void resetTimer(ref float timemoment, float delay)
    {
        timemoment = Time.time - delay;
    }

    public static float TimePassed(float timemoment)
    {
        return Time.time - timemoment;
    }

    public static void TimeRemaining()
    {

    }

    public static bool checkTimerWithin(float timemoment, float time_difference)
    {
        return Time.time - timemoment < time_difference;
    }

    public static bool checkTimerOutside(float timemoment, float time_difference)
    {
        return Time.time - timemoment > time_difference;
    }*/

    public static string GetCurrentTimeDAte()
    {
        return System.DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
    }

}

public static class mylib
{

    public delegate void voidfunction();
    public delegate void voidfunctionTransform(Transform tr);
    public delegate void voidfunctionGameObject(GameObject gmbjct);
    public delegate bool boolfunction();
    public delegate bool boolfunctionbool(bool tf);
    public delegate bool boolfunctioninput(Inputs.InputStruct input);
    public delegate bool boolfunctionKeycode(KeyCode input);
    public delegate Transform Transformfunction();
    public delegate GameObject GameObjectfunction();
    public delegate GameObject GameObjectfunctionTransform(Transform tr);

    public static void donothing()
    {

    }
    public static bool booltruedonothing()
    {
        return true;
    }
    public static bool boolfalsedonothing()
    {
        return false;
    }

    public static int GetNumberAtEnd(string s)
    {
        string temps = "";
        if (!char.IsDigit(s[s.Length - 1]))
        {
            return -1;
        }
        for (int i = s.Length - 1; i > -1; i--)
        {
            if (char.IsDigit(s[i]))
            {
                temps = s[i].ToString() + temps;
            }
            else
            {
                return int.Parse(temps);
            }
        }
        return int.Parse(temps);
    }


    /*public static Type[] ChangeArraySize<Type>(Type[] array, int newsize)
    {
        if (array == null || array.Length == 0)
        {
            return new Type[newsize];
        }
        Type[] newarray;
        newarray = new Type[newsize];
        if (array.Length > newsize)
        {
            for (int i = 0; i < newsize; i++)
            {
                newarray[i] = array[i];
            }
        }
        else
        {
            for (int i = 0; i < array.Length; i++)
            {
                newarray[i] = array[i];
            }
        }
        return newarray;
    }*/

    /*public static void ChangeArraySize<Type>(ref Type[] array, int newsize)
    {
        if (array == null || array.Length == 0)
        {
            array = new Type[newsize];
            return;
        }
        Type[] newarray;
        newarray = new Type[newsize];
        if (array.Length > newsize)
        {
            for (int i = 0; i < newsize; i++)
            {
                newarray[i] = array[i];
            }
        }
        else
        {
            for (int i = 0; i < array.Length; i++)
            {
                newarray[i] = array[i];
            }
        }
        array = newarray;
    }*/


    /*public static Type[] ExpandArray<Type>(Type[] MainArray, Type[] ArrayToAdd)
    {
        if (MainArray == null)
        {
            MainArray = new Type[0];
        }
        if (ArrayToAdd == null)
        {
            ArrayToAdd = new Type[0];
        }
        Type[] newarray = ChangeArraySize(MainArray, MainArray.Length + ArrayToAdd.Length);
        for (int i = 0; i < ArrayToAdd.Length; i++)
        {
            newarray[i + MainArray.Length] = ArrayToAdd[i];
        }
        return newarray;
    }*/


    public static void ExpandArray<Type>(ref Type[] Array, Type ToAdd)
    {
        if (Array == null)
        {
            Array = new Type[] { ToAdd };
            return;
        }
        int mainlen = Array.Length;
        System.Array.Resize(ref Array, mainlen + 1);
        Array[mainlen] = ToAdd;
    }
    public static void ExpandArray<Type>(ref Type[] MainArray, Type[] ArrayToAdd)
    {
        if (MainArray == null)
        {
            MainArray = new Type[0];
        }
        if (ArrayToAdd == null)
        {
            ArrayToAdd = new Type[0];
        }
        int mainlen = MainArray.Length;
        System.Array.Resize(ref MainArray, MainArray.Length + ArrayToAdd.Length);
        for (int i = 0; i < ArrayToAdd.Length; i++)
        {
            MainArray[i + mainlen] = ArrayToAdd[i];
        }
    }

    /*public static Type[] ArrayRemoveOne<Type>(Type[] Array, Type ToRemove)
    {
        if (Array == null)
        {
            return null;
        }
        int i;
        for (i = 0; i < Array.Length; i++)
        {
            if(EqualityComparer<Type>.Default.Equals(ToRemove, Array[i]))
            {
                break;
            }
        }
        for(i++; i < Array.Length;i++)
        {
            Array[i - 1] = Array[i];
        }
        return ChangeArraySize(Array, Array.Length - 1);
    }*/
    public static int ArrayRemoveOne<T>(ref T[] array, T ToRemove)
    {
        if (array == null)
        {
            return -1;
        }
        int i;
        for (i = 0; i < array.Length; ++i)
        {
            if (EqualityComparer<T>.Default.Equals(ToRemove, array[i]))
            {
                break;
            }
        }
        if (i == array.Length)
        {
            return -1;
        }
        int ireturn = ++i;
        for (; i < array.Length; ++i)
        {
            array[i - 1] = array[i];
        }
        System.Array.Resize(ref array, array.Length - 1);
        return ireturn;
    }

    public static int ArrayRemoveOne<T>(ref T[] array, Predicate<T> condition)
    {
        if (array == null)
        {
            return -1;
        }
        int i;
        for (i = 0; i < array.Length; i++)
        {
            if (condition(array[i]))
            {
                break;
            }
        }
        if (i == array.Length)
        {
            return -1;
        }
        int ireturn = i++;
        for (; i < array.Length; ++i)
        {
            array[i - 1] = array[i];
        }
        System.Array.Resize(ref array, array.Length - 1);
        return ireturn;
    }
    public static int ArrayRemoveAt<Type>(ref Type[] Array, int i)
    {
        if (Array == null)
        {
            return -1;
        }
        int ireturn = i;
        for (i++; i < Array.Length; i++)
        {
            Array[i - 1] = Array[i];
        }
        System.Array.Resize(ref Array, Array.Length - 1);
        return ireturn;
    }

    public static int ArrayRemoveDuplicates<T>(ref T[] array)
    {
        if (array == null)
        {
            return 0;
        }
        int numberOfDuplicates = 0;
        for (int i = 0; i < array.Length - numberOfDuplicates; ++i)
        {
            for (int j = i + 1; j < array.Length - numberOfDuplicates; ++j)
            {
                if (EqualityComparer<T>.Default.Equals(array[i], array[j]))
                {
                    numberOfDuplicates++;
                    for (; j < array.Length - numberOfDuplicates - 1; ++j)
                    {
                        array[j] = array[j + 1];
                    }
                    break;
                }
            }
        }
        System.Array.Resize(ref array, array.Length - numberOfDuplicates);
        return numberOfDuplicates;
    }
    public static void ArrayInsert<T>(ref T[] array, T newElement, int index)
    {
        if (array == null) return;
        System.Array.Resize(ref array, array.Length + 1);
        for (int i = array.Length - 1; i > index; --i)
        {
            array[i] = array[i - 1];
        }
        array[index] = newElement;
    }
    public static void ArrayAdd<T>(ref T[] array, T newElement)
    {
        if (array == null) return;
        System.Array.Resize(ref array, array.Length + 1);
        array[array.Length - 1] = newElement;
    }
    public static void SwapValues<T>(this T[] array, int index1, int index2)
    {
        if (index1 == index2)
        {
            return;
        }
        var temp = array[index1];
        array[index1] = array[index2];
        array[index2] = temp;
    }


    /*
    static T searchList<T>(List<T> cantarget, System.Predicate<T> ConditionToReturn) where T : class
    {
        List<T> candidates = new List<T>();
        for (int i = cantarget.Count - 1; i > -1; i--)
        {
            if (ConditionToReturn(cantarget[i]))
            {
                return cantarget[i];
            }
        }
        return null;
    }*/

    static T searchList<T>(this List<T> cantarget, System.Predicate<T> ConditionToRemove, System.Predicate<T> ConditionToReturn) where T : class
    {
        //List<T> candidates = new List<T>();
        for (int i = cantarget.Count - 1; i > -1; i--)
        {
            if (ConditionToRemove(cantarget[i]))
            {
                cantarget.RemoveAt(i);
            }
            else if (ConditionToReturn(cantarget[i]))
            {
                return cantarget[i];
            }
        }
        return null;
    }

    public static int FindIndex<T>(this T[] array, T toFind)
    {
        for(int i = 0; i < array.Length; ++i)
        {
            if (EqualityComparer<T>.Default.Equals(toFind, array[i]))
            {
                return i;
            }
        }
        return -1;
    }
    public static int FindIndex<T>(this T[] array, Predicate<T> condition)
    {
        for (int i = 0; i < array.Length; ++i)
        {
            if (condition(array[i]))
            {
                return i;
            }
        }
        return -1;
    }
    public static int Search<T>(this T[,] array, T toFind)
    {
        for (int i = 0; i < array.GetLength(0); ++i)
        {
            for (int j = 0; j < array.GetLength(1); ++j)
            {
                if (EqualityComparer<T>.Default.Equals(toFind, array[i, j]))
                {
                    return i * array.GetLength(1) + j;
                }
            }
        }
        return -1;
    }
    public static void Search<T>(this T[,] array, T toFind, out int iOut, out int jOut)
    {
        for (int i = 0; i < array.GetLength(0); ++i)
        {
            for (int j = 0; j < array.GetLength(1); ++j)
            {
                if (EqualityComparer<T>.Default.Equals(toFind, array[i, j]))
                {
                    iOut = i;
                    jOut = j;
                    return;
                }
            }
        }
        iOut = -1;
        jOut = -1;
    }


    public static T PickRandom<T>(T[] array)
    {
        return array[Random.Range(0, array.Length - 1)];
    }

    public static int CountInArray<T>(T[] array, System.Predicate<T> conditionToCount)
    {
        int n = 0;
        for (int i = 0; i < array.Length; ++i)
        {
            if (conditionToCount(array[i]))
            {
                ++n;
            }
        }
        return n;
    }

    /*public static T[] CleanArray<T>(T[] array, System.Predicate<T> conditionToRemove)
    {
        T[] temp = new T[array.Length];
        int j = 0;
        for (int i = 0; i < array.Length; i++)
        {
            if (!conditionToRemove(array[i]))
            {
                temp[j++] = array[i];
            }
        }
        return ChangeArraySize(temp, j);
    }*/
    public static void CleanArray<T>(ref T[] array, System.Predicate<T> conditionToRemove)
    {
        int j = 0;
        for (int i = 0; i < array.Length; i++)
        {
            if (!conditionToRemove(array[i]))
            {
                array[j++] = array[i];
            }
        }
        System.Array.Resize(ref array, j);
        /*int i = 0;
        for (; i < array.Length; i++)
        {
            if (!conditionToRemove(array[i]))
            {
                break;
            }
        }
        if (i == array.Length)
        {
            return;
        }
        int j = i++;
        for (; i < array.Length; i++)
        {
            if (!conditionToRemove(array[i]))
            {
                array[j++] = array[i];
            }
        }
        System.Array.Resize(ref array, j);*/
    }

    public static bool checkOutOfRange<T>(this T[] array, int i)
    {
        return i < 0 || i >= array.Length;
    }
    public static bool checkOutOfRange<T>(this T[,] array, int i, int j)
    {
        return i < 0 || i >= array.GetLength(0) || j < 0 || j >= array.GetLength(1);
    }

    public static string Reverse(this string s)
    {
        string news = "";
        for (int i = s.Length - 1; i > -1; i--)
        {
            news += s[i];
        }
        return news;
    }

    public static LinkedListNode<T> pushToLast<T>(this LinkedList<T> List, LinkedListNode<T> NodeToBePushed)
    {
        LinkedListNode<T> temp = NodeToBePushed.Next;
        List.Remove(NodeToBePushed);
        List.AddLast(NodeToBePushed);
        return temp;
    }
    public static LinkedListNode<T> getNodeAt<T>(this LinkedList<T> List, int i)
    {
        int ii = 0;
        for (LinkedListNode<T> Node = List.First; Node != null; Node = Node.Next, ++ii)
        {
            if (ii == i)
            {
                return Node;
            }
        }
        return null;
    }

    public static bool Contains<T>(this List<T> list, T toFind, bool removeNulls) where T : class
    {
        if (removeNulls)
        {
            for (int i = list.Count - 1; i > -1; --i)
            {
                if (list[i] == null)
                {
                    list.RemoveAt(i);
                }
                else if (list[i] == toFind)
                {
                    return true;
                }
            }
            return false;
        }
        else
        {
            return list.Contains(toFind);
        }
    }

    [Serializable]
    public class ReadOnlyList<T> : IEnumerable<T>
    {
        [SerializeField, ReadOnlyOnInspector]
        private IList<T> _list;
        public T this[int index] => _list[index];

        public int Count => _list.Count;

        public ReadOnlyList(IList<T> list)
        {
            _list = list;
        }

        public static implicit operator ReadOnlyList<T>(List<T> list) => new ReadOnlyList<T>(list);
        public static implicit operator ReadOnlyList<T>(T[] array) => new ReadOnlyList<T>(array);

        public bool Contains(T element)
        {
            return _list.Contains(element);
        }
        public bool Contains(System.Predicate<T> condition)
        {
            for (int i = _list.Count; i > -1; --i)
            {
                if (condition(_list[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    [System.Serializable]
    public class MyArrayList<T> : IList<T>
    {
        public int Count { get; private set; }
        private int _toAdd;
        public int Capacity
        {
            get => _array.Length;
            set => System.Array.Resize(ref _array, value);
        }
        public int FreeSlots
        {
            get
            {
                return Capacity - Count;
            }
        }
        public bool HasFreeSlots
        {
            get
            {
                return Count != Capacity;
            }
        }
        [SerializeField]
        private T[] _array;
        public T[] Array => _array;

        public T this[int index]
        {
            get => _array[index];
            set => _array[index] = value;
        }

        public T LastElement
        {
            get
            {
                return _array[Count - 1];
            }
            set
            {
                _array[Count - 1] = value;
            }
        }

        public bool IsReadOnly => false;

        public MyArrayList()
        {
            _array = new T[0];
            Count = 0;
            _toAdd = 1;
        }
        /*public MyArrayList(int toAdd = 1)
        {
            this.array = new T[0];
            this.Count = 0;
            this.toAdd = toAdd;
        }*/
        public MyArrayList(int capacity, int toAdd = 1)
        {
            _array = new T[capacity];
            Count = 0;
            _toAdd = toAdd;
        }
        public MyArrayList(T[] array, int toAdd = 1)
        {
            _array = array;
            Count = array.Length;
            _toAdd = toAdd;
        }

        public MyArrayList<T> SetHowMuchSpaceToAdd(int toAdd)
        {
            _toAdd = toAdd < 1 ? 1 : toAdd;
            return this;
        }

        public void copyFromArray(T[] array)
        {
            for (Count = 0; Count < array.Length && Count < _array.Length; ++Count)
            {
                _array[Count] = array[Count];
            }
        }

        public void Add(T value)
        {
            if (Count == Capacity)
            {
                Capacity += _toAdd;
            }
            _array[Count++] = value;
        }
        public void RemoveLast()
        {
            if (Count > 0)
            {
                Count--;
            }
        }

        public void Clear()
        {
            Count = 0;
        }
        public void Clear(int newCapacity)
        {
            Clear();
            if (_array.Length != newCapacity)
            {
                _array = new T[newCapacity];
            }
        }

        public void ForEach(System.Action<T> action)
        {
            for(int i = 0; i < Count; ++i)
            {
                action(_array[i]);
            }
        }
        public int FindIndex(System.Predicate<T> condition)
        {
            for (int i = 0; i < Count; i++)
            {
                if (condition(_array[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for(int i = 0; i < Count; ++i)
            {
                yield return _array[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _array.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            for(int i = Count - 1; i > -1; --i)
            {
                if(EqualityComparer<T>.Default.Equals(item, _array[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        public void Insert(int index, T item)
        {
            Add(LastElement);
            for (int i = Count - 2; i > index; --i)
            {
                _array[i] = _array[i - 1];
            }
            _array[index] = item;
        }

        public void RemoveAt(int index)
        {
            for(int i = index + 1; i < Count; ++i)
            {
                _array[i - 1] = _array[i];
            }
            --Count;
        }

        public bool Contains(T item)
        {
            for (int i = Count - 1; i > -1; --i)
            {
                if (EqualityComparer<T>.Default.Equals(item, _array[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            for(int i = 0; i < Count; ++i)
            {
                array[i + arrayIndex] = _array[i];
            }
        }

        public bool Remove(T item)
        {
            for (int i = Count - 1; i > -1; --i)
            {
                if (EqualityComparer<T>.Default.Equals(item, _array[i]))
                {
                    RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
    }

    public static LinkedListNode<T> Remove<T>(this LinkedListNode<T> Node)
    {
        LinkedListNode<T> Next = Node.Next;
        Node.List.Remove(Node);
        return Next;
    }



    public static T castSafe<T>(this Object toCast) where T : Object
    {
        try
        {
            return (T)toCast;
        }
        catch (System.InvalidCastException)
        {
            return null;
        }
    }

    public static bool isType<T>(this object toCast)
    {
        return toCast is T;
        /*
        try
        {
            T temp = (T)toCast;
            return true;
        }
        catch (System.InvalidCastException)
        {
            return false;
        }
        */
    }

    public static bool IsTheSameType(this object obj1, object obj2)
    {
        var a = obj1.GetType();
        var b = obj2.GetType();
        if (a == null || b == null) return false;
        return a == b;
    }

    public static bool IsTypeOrSubClassOf(this Type thisType, Type type) => thisType == type || thisType.IsSubclassOf(type);


}

public static class printlib
{
    public static string ArrayToString<T>(this T[] Array) where T : unmanaged
    {
        string s;
        if (Array.Length != 0)
        {
            s = Array[0].ToString();
        }
        else
        {
            return "";
        }
        for (int i = 1; i < Array.Length; i++)
        {
            s += " , " + Array[i].ToString();
        }
        return s;
    }
    public static string ArrayToString<T>(this T[,] Array) where T : unmanaged
    {
        string s = "";
        for (int i = 0; i < Array.GetLength(0); i++)
        {
            s += Array[i, 0].ToString();
            for (int j = 1; j < Array.GetLength(1); j++)
            {
                s += " , " + Array[i, j].ToString();
            }
            s += "\n";
        }
        return s;
    }

    public static string ArrayToString(this bool[] Array)
    {
        if (Array == null)
        {
            return "null";
        }
        string s = "";
        for (int i = 0; i < Array.Length; i++)
        {
            s += Array[i].BoolToString();
        }
        return s;
    }
    public static string ArrayToString(this bool[,] Array)
    {
        if (Array == null)
        {
            return "null";
        }
        string s = "";
        for (int i = 0; i < Array.GetLength(0); i++)
        {
            for (int j = 0; j < Array.GetLength(1); j++)
            {
                s += Array[i, j].BoolToString();
            }
            s += "\n";
        }
        return s;
    }

    public static string BoolToString(this bool tf)
    {
        if (tf)
        {
            return "1";
        }
        else
        {
            return "0";
        }
    }

}

public static class pathlib
{
    public static Transform edge = null;

    static List<Room> visited = new List<Room>();

    struct roomdist
    {
        public Room room;
        public Room comefrom;
        public float cost_plus_h;
        public float cost;

        public roomdist(Room room, Room comefrom, float cost, float h)
        {
            this.room = room;
            this.comefrom = comefrom;
            this.cost = cost;
            this.cost_plus_h = cost + h;
        }
    }

    static List<roomdist> next = new List<roomdist>();
    static List<Room> path = new List<Room>();

    static int Argfindmindistance(List<roomdist> list)
    {
        float mindistance = list[0].cost_plus_h;
        int index = 0;
        int len = list.Count;
        for (int i = 1; i < len; i++)
        {
            if (mindistance > list[i].cost_plus_h)
            {
                mindistance = list[i].cost_plus_h;
                index = i;
            }
        }
        //Debug.Log(mindistance);
        return index;
    }

    public static bool FindRoomPath(Room start, Room end, out LinkedList<Room> roompath)
    {
        roompath = new LinkedList<Room>();
        if (start == null || end == null)
        {
            return false;
        }

        int starti = -1;
        for(int i = 0; i < path.Count; i++)
        {
            if (start == path[i])
            {
                starti = i;
                break;
            }
        }
        if (starti != -1)
        {
            int endi = -1;
            roompath.Clear();
            roompath.AddLast(path[starti]);
            for (int i = starti + 1; i < path.Count; i++)
            {
                if (!roomsareneighbors(path[i], path[i - 1]))
                {
                    break;
                }
                roompath.AddLast(path[i]);
                if (end == path[i])
                {
                    endi = i;
                    break;
                }
            }
            if (endi != -1)
            {
                //Debug.Log("already found");
                return true;
            }
        }

        visited.Clear();
        next.Clear();
        path.Clear();

        Astar(start, end, 0f);
        path.Reverse(); // in the Astar the path is produced as a stack meaning the startroom is at the end of the list, so it must reversed

        visited.Clear();
        next.Clear();

        //Debug.Log("found new");

        roompath = new LinkedList<Room>(path);

        return path.Count != 0;
    }

    public static void FindRoomPath(Room start, Room end, bool debug)
    {
        if (start == null || end == null)
        {
            return;
        }
        visited.Clear();
        next.Clear();
        path.Clear();
        if (debug)
        {
            AstarDebug(start, end, 0f);
            path.Reverse(); // in the Astar the path is produced as a stack meaning the startroom is at the end of the list, so i reverse it
        }
        else
        {
            Astar(start, end, 0f);
            path.Reverse(); // in the Astar the path is produced as a stack meaning the startroom is at the end of the list, so i reverse it
        }
        visited.Clear();
        next.Clear();
        //Debug.Log("hmmm");
    }
    /*
    public static void buildPositionPath(LinkedList<room> roompath, Queue<Vector3> posQueue)
    {
        posQueue.Clear();
        if (roompath.Count > 1)
        {
            for (LinkedListNode<room> curr = roompath.First.Next; curr != null; curr = curr.Next)
            {
                posQueue.Enqueue(PickPositionInRoom(curr.Previous.Value, curr.Value));
            }
            posQueue.Enqueue(roompath.Last.Value.position);
        }
        else if (roompath.Count == 1)
        {
            posQueue.Enqueue(roompath.Last.Value.position);
        }
    }
      */

    public static Vector3 PickPositionInRoom(Room CurrentRoom, Room NextRoom)
    {
        float rng = somefunctions.exprandom(0.0, Room.DoorWidth / 2f, -0.5);
        if (CurrentRoom[0] == NextRoom)
        {
            return new Vector3(CurrentRoom.Position.x + rng, CurrentRoom.Position.y + (CurrentRoom.Height / 2f + Room.WallFatness));
        }
        else if (CurrentRoom[1] == NextRoom)
        {
            return new Vector3(CurrentRoom.Position.x + (CurrentRoom.Width / 2f + Room.WallFatness), CurrentRoom.Position.y + rng);
        }
        else if (CurrentRoom[2] == NextRoom)
        {
            return new Vector3(CurrentRoom.Position.x + rng, CurrentRoom.Position.y - (CurrentRoom.Height / 2f + Room.WallFatness));
        }
        else if (CurrentRoom[3] == NextRoom)
        {
            return new Vector3(CurrentRoom.Position.x - (CurrentRoom.Width / 2f + Room.WallFatness), CurrentRoom.Position.y + rng);
        }
        else
        {
            return CurrentRoom.Position;
        }
    }

    public static Room Astar(Room curr, Room goal, float d)
    {
        if (curr == goal)
        {
            addtopath(goal);
            return goal;
        }
        else if (roomsareneighbors(curr, goal))
        {
            addtopath(goal);
            addtopath(curr);
            return goal;
        }
        visited.Add(curr);
        addtonext(curr, goal, d);
        if (next.Count == 0)
        {
            return null;
        }
        int index = Argfindmindistance(next);
        roomdist nextrd = next[index];
        next.RemoveAt(index);
        return endAstar(curr, goal, nextrd, Astar(nextrd.room, goal, nextrd.cost));
    }

    static void addtonext(Room curr, Room goal, float d)
    {
        float rng = Random.value;
        foreach(Door door in (IEnumerable<Door>)curr)
        {
            if (door.ActuallyOpen && !visited.Contains(door.Neighbor))
            {
                if (ReplaceInNext(door.Neighbor, curr, d + distance(curr, door.Neighbor), distance(goal, door.Neighbor)))
                {
                    int rngadd = 2 * (int)(rng / 0.75f);
                    if (((int)door.Side & 1) == 0)
                    {
                        Room tempNeighbor = door.Neighbor[1 + rngadd];
                        if (door.Neighbor.CheckIfDoorActuallyOpen((Room.Side)(1 + rngadd)) && !visited.Contains(tempNeighbor))
                        {
                            ReplaceInNext(tempNeighbor, curr, d + distance(curr, tempNeighbor), distance(goal, tempNeighbor));
                        }
                        tempNeighbor = door.Neighbor[3 - rngadd];
                        if (door.Neighbor.CheckIfDoorActuallyOpen((Room.Side)(3 - rngadd)) && !visited.Contains(tempNeighbor))
                        {
                            ReplaceInNext(tempNeighbor, curr, d + distance(curr, tempNeighbor), distance(goal, tempNeighbor));
                        }
                    }
                    else
                    {
                        Room tempNeighbor = door.Neighbor[0 + rngadd];
                        if (door.Neighbor.CheckIfDoorActuallyOpen((Room.Side)(0 + rngadd)) && !visited.Contains(tempNeighbor))
                        {
                            ReplaceInNext(tempNeighbor, curr, d + distance(curr, tempNeighbor), distance(goal, tempNeighbor));
                        }
                        tempNeighbor = door.Neighbor[2 - rngadd];
                        if (door.Neighbor.CheckIfDoorActuallyOpen((Room.Side)(2 - rngadd)) && !visited.Contains(tempNeighbor))
                        {
                            ReplaceInNext(tempNeighbor, curr, d + distance(curr, tempNeighbor), distance(goal, tempNeighbor));
                        }
                    }
                }
            }
        }
        /*
        if (rng > 0.5f)
        {
            for (int i = 0; i < 4; i++)
            {
                room.door door = curr.doors[i];
                if (door.actuallyOpen && !visited.Contains(door.neighbor))
                {
                    if (ReplaceInNext(door.neighbor, curr, d + distance(curr, door.neighbor), distance(goal, door.neighbor)))
                    {
                        int rngadd = 2 * (int)(rng / 0.75f);
                        if ((i & 1) == 0)
                        {
                            door = door.neighbor.doors[1 + rngadd];
                            if (door.actuallyOpen && !visited.Contains(door.neighbor))
                            {
                                ReplaceInNext(door.neighbor, curr, d + distance(curr, door.neighbor), distance(goal, door.neighbor));
                            }
                            door = curr.doors[i].neighbor.doors[3 - rngadd];
                            if (door.actuallyOpen && !visited.Contains(door.neighbor))
                            {
                                ReplaceInNext(door.neighbor, curr, d + distance(curr, door.neighbor), distance(goal, door.neighbor));
                            }
                        }
                        else
                        {
                            door = door.neighbor.doors[0 + rngadd];
                            if (door.actuallyOpen && !visited.Contains(door.neighbor))
                            {
                                ReplaceInNext(door.neighbor, curr, d + distance(curr, door.neighbor), distance(goal, door.neighbor));
                            }
                            door = curr.doors[i].neighbor.doors[2 - rngadd];
                            if (door.actuallyOpen && !visited.Contains(door.neighbor))
                            {
                                ReplaceInNext(door.neighbor, curr, d + distance(curr, door.neighbor), distance(goal, door.neighbor));
                            }
                        }
                    }
                }
            }
        }
        else
        {
            for (int i = 3; i != 0; i--)
            {
                room.door door = curr.doors[i];
                if (door.actuallyOpen && !visited.Contains(door.neighbor))
                {
                    if (ReplaceInNext(door.neighbor, curr, d + distance(curr, door.neighbor), distance(goal, door.neighbor)))
                    {
                        int rngadd = 2 * (int)(rng / 0.251f);
                        if ((i & 1) == 0)
                        {
                            door = door.neighbor.doors[1 + rngadd];
                            if (door.actuallyOpen && !visited.Contains(door.neighbor))
                            {
                                ReplaceInNext(door.neighbor, curr, d + distance(curr, door.neighbor), distance(goal, door.neighbor));
                            }
                            door = curr.doors[i].neighbor.doors[3 - rngadd];
                            if (door.actuallyOpen && !visited.Contains(door.neighbor))
                            {
                                ReplaceInNext(door.neighbor, curr, d + distance(curr, door.neighbor), distance(goal, door.neighbor));
                            }
                        }
                        else
                        {
                            door = door.neighbor.doors[0 + rngadd];
                            if (door.actuallyOpen && !visited.Contains(door.neighbor))
                            {
                                ReplaceInNext(door.neighbor, curr, d + distance(curr, door.neighbor), distance(goal, door.neighbor));
                            }
                            door = curr.doors[i].neighbor.doors[2 - rngadd];
                            if (door.actuallyOpen && !visited.Contains(door.neighbor))
                            {
                                ReplaceInNext(door.neighbor, curr, d + distance(curr, door.neighbor), distance(goal, door.neighbor));
                            }
                        }
                    }
                }
            }
        }
        */
    }


    static Room endAstar(Room curr, Room goal, roomdist nextrd, Room roomreturned)
    {
        if (roomreturned != null)
        {
            if (roomreturned == goal) //this means that nextrd.room is on path
            {
                if (nextrd.comefrom == curr) //curr being nextrd.comefrom means that curr must be also on path
                {
                    fillcorners(curr, nextrd.room, goal, false);
                    addtopath(curr);
                    return goal;
                }
                else
                {
                    return nextrd.comefrom; //return the next room of the path that we must find
                }
            }
            else if (roomreturned == curr) //if temprd.room is not path then we need to check if curr is on path else we return the room we are looking for
            {
                fillcorners(curr, path[path.Count - 1], goal, false);
                addtopath(curr);
                return goal;
            }
            else
            {
                return roomreturned;   //keep looking for the next room of the path
            }
        }
        return null;
    }



    public static Room AstarDebug(Room curr, Room goal, float d)
    {
        if (curr == goal)
        {
            addtopath(goal);
            colorpath(goal);
            return goal;
        }
        else if (roomsareneighbors(curr, goal))
        {
            addtopath(goal);
            colorpath(goal);
            addtopath(curr);
            colorpath(curr);
            return goal;
        }
        visited.Add(curr);
        colorvisited(curr);
        //Debug.Log("current", curr);
        addtonextDebug(curr, goal, d);
        if (next.Count == 0)
        {
            return null;
        }
        int index = Argfindmindistance(next);
        roomdist nextrd = next[index];
        next.RemoveAt(index);
        return endAstarDebug(curr, goal, nextrd, AstarDebug(nextrd.room, goal, nextrd.cost));
    }

    static void addtonextDebug(Room curr, Room goal, float d)
    {
        float rng = Random.value;
        foreach (Door door in (IEnumerable<Door>)curr)
        {
            if (door.ActuallyOpen && !visited.Contains(door.Neighbor))
            {
                if (ReplaceInNext(door.Neighbor, curr, d + distance(curr, door.Neighbor), distance(goal, door.Neighbor)))
                {
                    colornext(door.Neighbor);
                    int rngadd = 2 * (int)(rng / 0.75f);
                    if (((int)door.Side & 1) == 0)
                    {
                        Room tempNeighbor = door.Neighbor[1 + rngadd];
                        if (door.Neighbor.CheckIfDoorActuallyOpen((Room.Side)(1 + rngadd)) && !visited.Contains(tempNeighbor))
                        {
                            if (ReplaceInNext(tempNeighbor, curr, d + distance(curr, tempNeighbor), distance(goal, tempNeighbor)))
                                colornext(door.Neighbor);
                        }
                        tempNeighbor = door.Neighbor[3 - rngadd];
                        if (door.Neighbor.CheckIfDoorActuallyOpen((Room.Side)(3 - rngadd)) && !visited.Contains(tempNeighbor))
                        {
                            if (ReplaceInNext(tempNeighbor, curr, d + distance(curr, tempNeighbor), distance(goal, tempNeighbor)))
                                colornext(door.Neighbor);
                        }
                    }
                    else
                    {
                        Room tempNeighbor = door.Neighbor[0 + rngadd];
                        if (door.Neighbor.CheckIfDoorActuallyOpen((Room.Side)(0 + rngadd)) && !visited.Contains(tempNeighbor))
                        {
                            if (ReplaceInNext(tempNeighbor, curr, d + distance(curr, tempNeighbor), distance(goal, tempNeighbor)))
                                colornext(door.Neighbor);
                        }
                        tempNeighbor = door.Neighbor[2 - rngadd];
                        if (door.Neighbor.CheckIfDoorActuallyOpen((Room.Side)(2 - rngadd)) && !visited.Contains(tempNeighbor))
                        {
                            if (ReplaceInNext(tempNeighbor, curr, d + distance(curr, tempNeighbor), distance(goal, tempNeighbor)))
                                colornext(door.Neighbor);
                        }
                    }
                }
            }
        }
        /*
        if (rng > 0.5f)
        {
            for (int i = 0; i < 4; i++)
            {
                room.door door = curr.doors[i];
                if (door.actuallyOpen && !visited.Contains(door.neighbor) /*&& !nextContains(door.neighbor)*)
                {
                    if (ReplaceInNext(door.neighbor, curr, d + distance(curr, door.neighbor), distance(goal, door.neighbor)))
                    {
                        colornext(door.neighbor);
                        int rngadd = 2 * (int)(rng / 0.75f);
                        if ((i & 1) == 0)
                        {
                            door = door.neighbor.doors[1 + rngadd];
                            if (door.actuallyOpen && !visited.Contains(door.neighbor) /*&& !nextContains(door.neighbor)*)
                            {
                                if (ReplaceInNext(door.neighbor, curr, d + distance(curr, door.neighbor), distance(goal, door.neighbor)))
                                {
                                    colornext(door.neighbor);
                                }
                                //next.Add(new roomdist(door.neighbor, curr, d + distance(curr, door.neighbor) + distance(goal, door.neighbor)));
                            }
                            door = curr.doors[i].neighbor.doors[3 - rngadd];
                            if (door.actuallyOpen && !visited.Contains(door.neighbor)/* && !nextContains(door.neighbor)*)
                            {
                                if (ReplaceInNext(door.neighbor, curr, d + distance(curr, door.neighbor), distance(goal, door.neighbor)))
                                {
                                    colornext(door.neighbor);
                                }
                            }
                        }
                        else
                        {
                            door = door.neighbor.doors[0 + rngadd];
                            if (door.actuallyOpen && !visited.Contains(door.neighbor) /*&& !nextContains(door.neighbor)*)
                            {
                                if (ReplaceInNext(door.neighbor, curr, d + distance(curr, door.neighbor), distance(goal, door.neighbor)))
                                {
                                    colornext(door.neighbor);
                                }
                            }
                            door = curr.doors[i].neighbor.doors[2 - rngadd];
                            if (door.actuallyOpen && !visited.Contains(door.neighbor) /*&& !nextContains(door.neighbor)*)
                            {
                                if (ReplaceInNext(door.neighbor, curr, d + distance(curr, door.neighbor), distance(goal, door.neighbor)))
                                {
                                    colornext(door.neighbor);
                                }
                            }
                        }
                    }
                }
            }
        }
        else
        {
            for (int i = 3; i != 0; i--)
            {
                room.door door = curr.doors[i];
                if (door.actuallyOpen && !visited.Contains(door.neighbor) /*&& !nextContains(door.neighbor)*)
                {
                    if (ReplaceInNext(door.neighbor, curr, d + distance(curr, door.neighbor), distance(goal, door.neighbor)))
                    {
                        int rngadd = 2 * (int)(rng / 0.251f);
                        colornext(door.neighbor);
                        if ((i & 1) == 0)
                        {
                            door = door.neighbor.doors[1 + rngadd];
                            if (door.actuallyOpen && !visited.Contains(door.neighbor) /*&& !nextContains(door.neighbor)*)
                            {
                                if (ReplaceInNext(door.neighbor, curr, d + distance(curr, door.neighbor), distance(goal, door.neighbor)))
                                {
                                    colornext(door.neighbor);
                                }
                                //next.Add(new roomdist(door.neighbor, curr, d + distance(curr, door.neighbor) + distance(goal, door.neighbor)));
                            }
                            door = curr.doors[i].neighbor.doors[3 - rngadd];
                            if (door.actuallyOpen && !visited.Contains(door.neighbor)/* && !nextContains(door.neighbor)*)
                            {
                                if (ReplaceInNext(door.neighbor, curr, d + distance(curr, door.neighbor), distance(goal, door.neighbor)))
                                {
                                    colornext(door.neighbor);
                                }
                            }
                        }
                        else
                        {
                            door = door.neighbor.doors[0 + rngadd];
                            if (door.actuallyOpen && !visited.Contains(door.neighbor) /*&& !nextContains(door.neighbor)*)
                            {
                                if (ReplaceInNext(door.neighbor, curr, d + distance(curr, door.neighbor), distance(goal, door.neighbor)))
                                {
                                    colornext(door.neighbor);
                                }
                            }
                            door = curr.doors[i].neighbor.doors[2 - rngadd];
                            if (door.actuallyOpen && !visited.Contains(door.neighbor) /*&& !nextContains(door.neighbor)*)
                            {
                                if (ReplaceInNext(door.neighbor, curr, d + distance(curr, door.neighbor), distance(goal, door.neighbor)))
                                {
                                    colornext(door.neighbor);
                                }
                            }
                        }
                    }
                }
            }
        }
        */
    }

    static Room endAstarDebug(Room curr, Room goal, roomdist nextrd, Room roomreturned)
    {
        if (roomreturned != null)
        {
            if (roomreturned == goal) //this means that nextrd.room is on path
            {
                if (nextrd.comefrom == curr) //curr being nextrd.comefrom means that curr must be also on path
                {
                    fillcorners(curr, nextrd.room, goal, true);
                    addtopath(curr);
                    colorpath(curr);
                    return goal;
                }
                else
                {
                    return nextrd.comefrom; //return the next room of the path that we must find
                }
            }
            else if (roomreturned == curr) //if temprd.room is not path then we need to check if curr is on path else we return the room we are looking for
            {
                fillcorners(curr, path[path.Count - 1], goal, true);
                addtopath(curr);
                colorpath(curr);
                return goal;
            }
            else
            {
                return roomreturned;   //keep looking for the next room of the path
            }
        }
        return null;
    }


    public static float distance(Room r1, Room r2)
    {
        return (r1.transform.position - r2.transform.position).magnitude;
        /*  float sqrt2;
          float dx = System.Math.Abs(r1.transform.position.x - r2.transform.position.x);
          float dy = System.Math.Abs(r1.transform.position.y - r2.transform.position.y);
          if (dx == 0)
          {
              sqrt2 = 1f;
          }
          else if (dx > dy)
          {
              sqrt2 = (float)(mathlib.sqrtof2 - 1.0) * dy / dx + 1f;
          }
          else
          {
              sqrt2 = (float)(mathlib.sqrtof2 - 1.0) * dx / dy + 1f;
          }
          return (r1.transform.position - r2.transform.position).magnitude * sqrt2;*/

        /* float dx = System.Math.Abs(r1.transform.position.x - r2.transform.position.x);
         float dy = System.Math.Abs(r1.transform.position.y - r2.transform.position.y);
         float min = dy;
         if (dx < dy)
         {
             min = dx;
         }
         return dx + dy + ((float)mathlib.sqrtof2 - 2) * min;*/
    }

    public static void addtopath(Room r)
    {
        path.Add(r);
    }



    public static bool roomsareneighbors(Room r1, Room r2)
    {
        if (r1 == null || r2 == null)
        {
            return false;
        }
        foreach (Door door in (IEnumerable<Door>)r1)
        {
            if (door.ActuallyOpen && door.Neighbor == r2)
            {
                return true;
            }
        }
        return false;
    }

    static bool nextContains(Room rd)
    {
        foreach (roomdist rdtemp in next)
        {
            if (rdtemp.room == rd)
            {
                return true;
            }
        }
        return false;
    }

    static bool ReplaceInNext(Room r, Room cm, float cost, float h)
    {
        int i = 0;
        float newdistance = cost + h;
        foreach (roomdist rdtemp in next)
        {
            if (rdtemp.room == r)
            {
                if (rdtemp.cost_plus_h > newdistance)       //if it is alraedy in next replace it if it has smaller distance, if it's not then just add it
                {
                    next.RemoveAt(i);
                    next.Add(new roomdist(r, cm, cost, h));
                    return true;
                }
                return false;
            }
            i++;
        }
        next.Add(new roomdist(r, cm, cost, h));
        return true;
    }

    static void fillcorners(Room curr, Room nextroom, Room goal, bool debug)
    {
        if (roomsareneighbors(curr, nextroom))
        {
            return;
        }
        float mindist = float.MaxValue;
        Room closestroom = null;
        foreach(Door door in (IEnumerable<Door>)curr)
        //for (int i = 0; i < 4; i++)
        {
            if (door.ActuallyOpen && roomsareneighbors(door.Neighbor, nextroom))
            {
                float tempdist = (door.Neighbor.transform.position - goal.transform.position).sqrMagnitude;
                if (tempdist < mindist)
                {
                    mindist = tempdist;
                    closestroom = door.Neighbor;
                }
            }
        }
        if (closestroom != null)
        {
            addtopath(closestroom);
            if (debug)
                closestroom.Color = new Color(0f, 0.9f, 0f);
        }
        /*
        if (Random.value > 0.5f)
        {
            for (int i = 0; i < 4; i++)
            {
                if (curr.doors[i].actuallyOpen && roomsareneighbors(curr.doors[i].neighbor, nextroom))
                {
                    float tempdist = (curr.doors[i].neighbor.transform.position - goal.transform.position).sqrMagnitude;
                    if (tempdist < mindist)
                    {
                        mindist = tempdist;
                        closestroom = curr.doors[i].neighbor;
                    }
                }
            }
            if (closestroom != null)
            {
                addtopath(closestroom);
                if (debug)
                    closestroom.color = new Color(0f, 0.9f, 0f);
            }
            return;
        }
        else
        {
            for (int i = 3; i != 0; i--)
            {
                if (curr.doors[i].actuallyOpen && roomsareneighbors(curr.doors[i].neighbor, nextroom))
                {
                    float tempdist = (curr.doors[i].neighbor.transform.position - goal.transform.position).sqrMagnitude;
                    if (tempdist < mindist)
                    {
                        mindist = tempdist;
                        closestroom = curr.doors[i].neighbor;
                    }
                }
            }
            if (closestroom != null)
            {
                addtopath(closestroom);
                if (debug)
                    closestroom.color = new Color(0f, 0.9f, 0f);
            }
            return;
        }
        */
    }

    static void colorpath(Room r)
    {
        r.Color = Color.green;
    }


    static void colorvisited(Room r)
    {
        r.Color = Color.red;
    }

    static void colornext(Room r)
    {
        r.Color = Color.yellow;
    }


    public static Room.Side findside(Vector3 currpos_sub_centre, float heigh_div_width)
    {
        if (currpos_sub_centre.y > heigh_div_width * currpos_sub_centre.x)
        {
            if (currpos_sub_centre.y > -heigh_div_width * currpos_sub_centre.x)
            {
                return Room.Side.North;
            }
            else
            {
                return Room.Side.West;
            }
        }
        else
        {
            if (currpos_sub_centre.y > -heigh_div_width * currpos_sub_centre.x)
            {
                return Room.Side.East;
            }
            else
            {
                return Room.Side.South;
            }
        }
    }


    public static teamvariables getvarsteam(this Component component)
    {
        Transform team = component.transform;
        teamvariables tempvars;
        while (team != null)
        {
            if ((tempvars = team.GetComponent<teamvariables>()) != null)
            {
                return tempvars;
            }
            team = team.parent;
        }
        return globalvariables.nullobject.GetComponent<teamvariables>();
    }
}




public static class GetVars
{
    public static T GetvarsInChildren<T>(Transform tr) where T : Component
    {
        T tempvars;
        int len = tr.childCount;
        for (int i = 0; i < len; i++)
        {
            if ((tempvars = GetvarsInChildren<T>(tr.GetChild(i))) != null)
            {
                return tempvars;
            }
        }
        return null;
    }

    public static T GetOtherChildComponent<T>(Transform tr) where T : Component
    {
        if (tr.parent != null)
        {
            return tr.parent.GetComponentInChildren<T>();
        }
        return null;
    }

    public static T[] GetComponentsInDirectChildren<T>(this Component component) where T : Component
    {
        if (component == null)
        {
            return null;
        }
        List<T> components = new List<T>();
        for (int i = 0; i < component.transform.childCount; ++i)
        {
            T TEMPcomponent = component.transform.GetChild(i).GetComponent<T>();
            if (TEMPcomponent != null)
                components.Add(TEMPcomponent);
        }

        return components.ToArray();
    }


    public static T getvars<T>(this Component component) where T : Component
    {
        if (component == null)
        {
            return null;
        }
        Transform tr = component.transform;
        while (tr != null)
        {
            T tempvars;
            if ((tempvars = tr.GetComponent<T>()) != null)
            {
                return tempvars;
            }
            else if (tr.GetComponent<BaseCharacterControl>() != null)
            {
                return tr.GetComponentInChildren<T>(true);
            }
            else if (tr.parent != null && (tempvars = tr.parent.GetComponent<T>()) != null)
            {
                return tempvars;
            }
            tr = tr.parent;
        }
        return globalvariables.nullobject.GetComponent<T>();
    }


    public static Transform getvarsTR<T>(this Component component) where T : Component
    {
        if (component == null)
        {
            return null;
        }
        Transform tr = component.transform;
        while (tr != null)
        {
            T tempvars;
            if (tr.GetComponent<T>() != null)
            {
                return tr;
            }
            else if (tr.GetComponent<BaseCharacterControl>() != null)
            {
                if ((tempvars = tr.GetComponentInChildren<T>(true)) != null)
                {
                    return tempvars.transform;
                }
                return null;
            }
            else if (tr.parent != null && tr.parent.GetComponent<T>() != null)
            {
                return tr.parent;
            }
            tr = tr.parent;
        }
        //return component.transform;
        return null;
    }


    public static T[] getmanyvars<T>(this Component component) where T : Component
    {
        if (component == null)
        {
            return null;
        }
        T[] tempvars;
        T[] varsfound = new T[0];
        Transform tr = component.transform;
        while (tr != null)
        {
            if ((tempvars = tr.GetComponents<T>()).Length != 0)
            {
                mylib.ExpandArray(ref varsfound, tempvars);
            }
            if (tr.GetComponent<teamvariables>() == null && (tempvars = tr.GetComponentsInDirectChildren<T>()).Length != 0)
            {
                mylib.ExpandArray(ref varsfound, tempvars);
            }
            tr = tr.parent;
        }
        return varsfound;
    }




    public static T[] GetComponentsInDirectChildren<T>(this GameObject gameObject) where T : Component
    {
        if (gameObject == null)
        {
            return null;
        }
        List<T> components = new List<T>();
        for (int i = 0; i < gameObject.transform.childCount; ++i)
        {
            T component = gameObject.transform.GetChild(i).GetComponent<T>();
            if (component != null)
                components.Add(component);
        }

        return components.ToArray();
    }


    public static teamvariables getvarsteam(this GameObject gameObject)
    {
        return gameObject.transform.getvarsteam();
    }

    public static T getvars<T>(this GameObject gameObject) where T : Component
    {
        if (gameObject == null)
        {
            return null;
        }
        return gameObject.transform.getvars<T>();
    }

    public static Transform getvarsTR<T>(this GameObject gameObject) where T : Component
    {
        if (gameObject == null)
        {
            return null;
        }
        return gameObject.transform.getvarsTR<T>();
    }

    public static T[] getmanyvars<T>(this GameObject gameObject) where T : Component
    {
        if (gameObject == null)
        {
            return null;
        }
        return gameObject.transform.getmanyvars<T>();
    }




}



public static class ComponentExtensions
{


    public static void disable(this Behaviour Behaviour)
    {
        Behaviour.enabled = false;
    }
    public static void enable(this Behaviour Behaviour)
    {
        Behaviour.enabled = true;
    }
    public static void disable(this Renderer Renderer)
    {
        Renderer.enabled = false;
    }
    public static void enable(this Renderer Renderer)
    {
        Renderer.enabled = true;
    }

}

public static class GameObjectExtensions
{

    public static void SetActivetrue(this GameObject gameObject)
    {
        gameObject.SetActive(true);
    }
    public static void SetActivefalse(this GameObject gameObject)
    {
        gameObject.SetActive(false);
    }


}



public static class StackExtensions
{
    public static Stack<T> Clone<T>(this Stack<T> original)
    {
        T[] arr = new T[original.Count];
        original.CopyTo(arr, 0);
        System.Array.Reverse(arr);
        return new Stack<T>(arr);
    }
}





public static class Create
{
    const string chargerPath = "mobs/charger";

    public static GameObject charger(Transform parent)
    {
        GameObject gmtemp = somefunctions.InstantiatePrefabGmbjct(chargerPath, parent);
        ChargingAI chargvars = gmtemp.getvars<ChargingAI>();
        /*deathAnimation_Base deathvars = gmtemp.getvars<deathAnimation_Base>();
        if(deathvars != null)
        {
            Object.Destroy(deathvars);
        }*/
        return gmtemp;
    }


}
























