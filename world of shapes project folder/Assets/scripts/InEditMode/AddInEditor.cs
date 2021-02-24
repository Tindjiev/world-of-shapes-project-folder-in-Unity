using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;
using Side = Room.Side;
[ExecuteInEditMode]
public class AddInEditor : MonoBehaviour
{
    [SerializeField]
    private string _input = "";
    public static string Input => EditorGameObject._input;

    private static AddInEditor _self;
    public static AddInEditor EditorGameObject
    {
        get => _self != null ? _self : _self = FindObjectOfType<AddInEditor>();
        private set => _self = value;
    }


#pragma warning disable CS0414
    [SerializeField]
    private GameObject
        _playerPrefab = null,
        _mob = null,
        _triangleCharger = null,
        _dialoguePrefab = null
        ;
#pragma warning restore CS0414 



#if UNITY_EDITOR
    private static void CreateUndoAndSelection(Object obj, string undoName)
    {
        Undo.RegisterCreatedObjectUndo(obj, undoName);
        Selection.objects = new Object[] { obj is GameObject gmbjct ? gmbjct : (obj is Component component ? component.gameObject : obj) };
    }

    [MenuItem("GameObject/Create Centered Empty", false, 0)]
    private static void CreateEmpty()
    {
        EditorLib.DoMenuItemFunctionOnce(() =>{
                GameObject empty = new GameObject("GameObject");
                CreateUndoAndSelection(empty, "delete empty object");
            });
    }

    [MenuItem("GameObject/Character/Player", false, 0)]
    public static void AddPlayer()
    {
        EditorLib.DoMenuItemFunctionOnce(() =>
        {
            if (FindObjectOfType<PlayerControlBattle>() != null)
            {
                throw new Exception("player already exists");
            }
            AddCharacter(CreatePlayer(ManageStuffInEditor.Team));
        });
    }



    [MenuItem("GameObject/Character/Npc", false, 0)]
    public static void AddNpc()
    {
        EditorLib.DoMenuItemFunctionOnce(() =>
        {
            AddCharacter(MobCreate.SimpleMob(Vector3.zero, ManageStuffInEditor.Team, 10f));
        });
    }



    [MenuItem("GameObject/Character/Misc/Set triangle chargers on a room/Don't match vision of Room", false, 0)]
    private static void SetupTriangleChargersOnRoom()
    {
        SetupTriangleChargersOnRoom(false);
    }
    [MenuItem("GameObject/Character/Misc/Set triangle chargers on a room/Match vision of Room", false, 0)]
    private static void SetupTriangleChargersOnRoomNoMatch()
    {
        SetupTriangleChargersOnRoom(true);
    }
    private static void SetupTriangleChargersOnRoom(bool matchRoom = true) //example n8e12
    {
        Room room = ManageStuffInEditor.GetRoomFromBothSelections(false);
        string str = Input;
        if (!System.Text.RegularExpressions.Regex.IsMatch(str.ToLower().Trim(), @"([nesw][1-9][0-9]*)+"))
        {
            throw new Exception("Invalid input string");
        }
        List<Side> sides = new List<Side>();
        List<int> nums = new List<int>();
        for (int i = 0; i < str.Length;)
        {
            switch (str[i])
            {
                case 'n':
                    if (sides.Contains(Side.North)) throw new Exception("Invalid input string; north appears twice");
                    sides.Add(Side.North);
                    break;
                case 'e':
                    if (sides.Contains(Side.East)) throw new Exception("Invalid input string; east appears twice");
                    sides.Add(Side.East);
                    break;
                case 's':
                    if (sides.Contains(Side.South)) throw new Exception("Invalid input string; south appears twice");
                    sides.Add(Side.South);
                    break;
                case 'w':
                    if (sides.Contains(Side.West)) throw new Exception("Invalid input string; west appears twice");
                    sides.Add(Side.West);
                    break;
            }
            if (sides.Count > 4) throw new Exception("Invalid input string; more than 4 sides"); //i think it will never be the case, but just to be sure
            string strNum = "";
            for (++i; i < str.Length && char.IsDigit(str[i]); ++i)
            {
                strNum += str[i];
            }
            nums.Add(int.Parse(strNum));
        }

        /*
        string sr = "";
        for(int i = 0; i < sides.Count; ++i)
        {
            sr += sides[i] + " " + nums[i] + " ";
        }
        Debug.Log(sr);
        */

        Object[] temp;
        GameObject[] tempGO;
        Selection.objects = temp = tempGO = SetChargersTrapOnSide<VisionOthers>(room, sides.ToArray(), nums.ToArray(), ManageStuffInEditor.Team);
        if (temp.Length == 0) return;
        Undo.RegisterCreatedObjectUndo(temp[0], "Undo triangle chargers");
        int undoID = Undo.GetCurrentGroup();
        for (int i = 1; i < temp.Length; ++i)
        {
            Undo.RegisterCreatedObjectUndo(temp[i], "Undo triangle chargers");
            Undo.CollapseUndoOperations(undoID);
        }

        foreach (var thing in tempGO)
        {
            thing.GetComponentInChildren<VisionOthers>().AddVision(room.GetComponentInChildren<VisionOfRoom>(true), matchRoom);
        }

    }
    public static GameObject[] SetChargersTrapOnSide<Vision>(Room room, Side[] Sides, int[] nums, Transform team) where Vision : VisionOfMob
    {
        List<GameObject> chargers = new List<GameObject>();
        for (int i = 0; i < Sides.Length; i++)
        {
            int chargersNum = nums[i];
            Vector3 centrepos = room.getSidePos(Sides[i], Room.inner);
            if (Sides[i] == Side.North || Sides[i] == Side.South)
            {
                for (int j = 0; j < chargersNum; j++)
                {
                    GameObject charger;
                    charger = AddInEditor.MobCreate.Charger(centrepos + new Vector3((j + 0.5f - (chargersNum / 2f)) * room.WidthInner * 0.9f / chargersNum, 0f), team, 5f, 10f, 30f);
                    charger.ReplaceVision<Vision>();
                    //addtocandoAndtoOthers(charger.transform);
                    chargers.Add(charger);
                }
            }
            else
            {
                for (int j = 0; j < chargersNum; j++)
                {
                    GameObject charger;
                    charger = AddInEditor.MobCreate.Charger(centrepos + new Vector3(0f, (j + 0.5f - (chargersNum / 2f)) * room.HeightInner * 0.9f / chargersNum), team, 5f, 10f, 30f);
                    charger.ReplaceVision<Vision>();
                    //addtocandoAndtoOthers(charger.transform);
                    chargers.Add(charger);
                }
            }
        }
        return chargers.ToArray();
    }

    public static GameObject[] SetChargersTrapOnSide<Vision>(Room room, Side[] Sides, int num, Transform team) where Vision : VisionOfMob
    {
        int[] nums = new int[Sides.Length];
        for (int i = 0; i < nums.Length; ++i) nums[i] = num;
        return SetChargersTrapOnSide<Vision>(room, Sides, nums, team);
    }

    [MenuItem("GameObject/Add Dialogue", false, 0)]
    public static void AddDialogue()
    {
        EditorLib.DoMenuItemFunctionOnce(() => {
            var character = ManageStuffInEditor.GetCharacterFromSelection();
            if (character == null)
            {
                throw new Exception("no character selected");
            }
            CreateUndoAndSelection(InstantiatePrefab(EditorGameObject._dialoguePrefab, character.MoveComponent.transform), "Undo chat object");
        });
    }
    [MenuItem("GameObject/Event Objects/Player position/Only Component", false, 0)]
    public static void AddEventObjectPlayerPosition()
    {
        AddEventObjectComponent<EventObjectPlayerPosition>();
    }
    [MenuItem("GameObject/Event Objects/Kill mobs", false, 0)]
    public static void AddEventObjectKillMobs()
    {
        AddEventObjectComponent<EventObjectKillMobs>();
    }
    [MenuItem("GameObject/Event Objects/Dialogue end", false, 0)]
    public static void AddEventObjectDialogueEnd()
    {
        AddEventObjectComponent<EventObjectDialogueEnd>();
    }
    [MenuItem("GameObject/Event Objects/LightsOut", false, 0)]
    public static void AddEventObjectLightsOut()
    {
        AddEventObjectComponent<EventObjectLightsOut>();
    }
    [MenuItem("GameObject/Event Objects/Composite", false, 0)]
    public static void AddEventObjectComposite()
    {
        AddEventObjectComponent<EventObjectComposite>();
    }
    [MenuItem("GameObject/Event Objects/Generic condition", false, 0)]
    public static void AddEventObjectGeneric()
    {
        AddEventObjectComponent<EventObjectCustomCondition>();
    }
    [MenuItem("GameObject/Event Objects/Null", false, 0)]
    public static void AddEventObjectNull()
    {
        AddEventObjectComponent<EventObjectNull>();
    }
    [MenuItem("GameObject/Event Objects/Player position/Create GameObject with Collider", false, 0)]
    public static void AddEventObjectPlayerPositionGameObject()
    {
        EditorLib.DoMenuItemFunctionOnce(() => {
            CreateUndoAndSelection(EventObjectPlayerPosition.AddAsGameObject(Selection.objects.Length == 1 && Selection.objects[0] is GameObject gmbjct ? gmbjct.transform : null),"Undo event object player position");
        });
    }


    public static void AddCharacter(GameObject character)
    {
        Room tempRoom = null;
        try
        {
            tempRoom = ManageStuffInEditor.GetRoomFromBothSelections();
        }
        catch (EditorExceptions.RoomSelectionException) { }
        CreateUndoAndSelection(character, "delete " + character.name);
        try
        {
            ManageStuffInEditor.SetObjectsToRoom(tempRoom);
        }
        catch (EditorExceptions.RoomSelectionException) { }
    }
    public static void AddEventObjectComponent<T>() where T : EventObjectBaseClass
    {
        var selection = Selection.objects;
        if (selection.Length == 0)
        {
            throw new EditorExceptions.NothingSelected();
        }
        else if (selection.Length > 1)
        {
            throw new EditorExceptions.MoreThanOneSelected();
        }
        var gameObjectTemp = selection[0] as GameObject;
        EditorLib.DoMenuItemFunctionOnce(() => {
            CreateUndoAndSelection(gameObjectTemp.AddEventComponent<T>(), "Undo event component");
        });
    }
#endif

    public static GameObject CreatePlayer(Transform team = null)
    {
        if (team == null) team = ManageStuffInEditor.Team;
        if (ControlBase.PlayerGameObject != null) return ControlBase.PlayerGameObject;
        if (EditorGameObject == null)
        {
            ControlBase.PlayerGameObject = InstantiatePrefab(Resources.Load<GameObject>("Prefabs/player"), team);
        }
        else
        {
            ControlBase.PlayerGameObject = InstantiatePrefab(EditorGameObject._playerPrefab, team);
        }
        ControlBase.PlayerGameObject.GetComponent<PlayerControlBattle>().AddAttacks();
        return ControlBase.PlayerGameObject;
    }

    public static class MobCreate
    {
        public static GameObject Generic(MyLib.GameObjectfunction createtype)
        {
            return createtype();
        }

        public static GameObject Generic(MyLib.GameObjectfunction createtype, Action<Transform> candos)
        {
            GameObject gmbjct = createtype();

            candos(gmbjct.transform);

            return gmbjct;
        }

        public static GameObject Generic(MyLib.GameObjectfunction createtype, Action<GameObject> misc)
        {
            GameObject gmbjct = createtype();

            misc(gmbjct);

            return gmbjct;
        }

        public static GameObject Generic(MyLib.GameObjectfunction createtype, Action<GameObject> misc, Action<Transform> candos)
        {
            GameObject gmbjct = createtype();

            misc(gmbjct);

            candos(gmbjct.transform);

            return gmbjct;
        }

        public static GameObject[] Generic(int num, MyLib.GameObjectfunction createtype)
        {
            GameObject[] gmbjcts = new GameObject[num];
            for (int i = 0; i < num; i++)
            {
                gmbjcts[i] = createtype();
            }
            return gmbjcts;
        }
        public static GameObject[] Generic(int num, MyLib.GameObjectfunction createtype, Action<GameObject> misc)
        {
            GameObject[] gmbjcts = new GameObject[num];
            for (int i = 0; i < num; i++)
            {
                misc(gmbjcts[i] = createtype());
            }
            return gmbjcts;
        }
        public static GameObject[] Generic(int num, MyLib.GameObjectfunction createtype, Action<Transform> candos)
        {
            GameObject[] gmbjcts = new GameObject[num];
            for (int i = 0; i < num; i++)
            {
                candos((gmbjcts[i] = createtype()).transform);
            }
            return gmbjcts;
        }

        public static GameObject[] Generic(int num, MyLib.GameObjectfunction createtype, Action<GameObject> misc, Action<Transform> candos)
        {
            GameObject[] gmbjcts = new GameObject[num];
            for (int i = 0; i < num; i++)
            {
                candos((gmbjcts[i] = createtype()).transform);
                misc(gmbjcts[i]);
            }
            return gmbjcts;
        }

        public static GameObject[] Generic(MyLib.GameObjectfunction[] createtype)
        {
            GameObject[] gmbjcts = new GameObject[createtype.Length];
            for (int i = 0; i < gmbjcts.Length; i++)
            {
                gmbjcts[i] = createtype[i]();
            }
            return gmbjcts;
        }

        public static GameObject[] Generic(MyLib.GameObjectfunction[] createtype, Action<Transform> candos)
        {
            GameObject[] gmbjcts = new GameObject[createtype.Length];
            for (int i = 0; i < gmbjcts.Length; i++)
            {
                candos((gmbjcts[i] = createtype[i]()).transform);
            }
            return gmbjcts;
        }

        public static GameObject[] Generic(MyLib.GameObjectfunction[] createtype, Action<GameObject> misc)
        {
            GameObject[] gmbjcts = new GameObject[createtype.Length];
            for (int i = 0; i < gmbjcts.Length; i++)
            {
                misc(gmbjcts[i] = createtype[i]());
            }
            return gmbjcts;
        }

        public static GameObject[] Generic(MyLib.GameObjectfunction[] createtype, Action<GameObject> misc, Action<Transform> candos)
        {
            GameObject[] gmbjcts = new GameObject[createtype.Length];
            for (int i = 0; i < gmbjcts.Length; i++)
            {
                candos((gmbjcts[i] = createtype[i]()).transform);
                misc(gmbjcts[i]);
            }
            return gmbjcts;
        }

        public static GameObject[] Generic(MyLib.GameObjectfunction[] createtype, Action<GameObject>[] misc)
        {
            GameObject[] gmbjcts = new GameObject[createtype.Length];
            for (int i = 0; i < gmbjcts.Length; i++)
            {
                if (i < misc.Length)
                {
                    misc[i](gmbjcts[i] = createtype[i]());
                }
                else
                {
                    gmbjcts[i] = createtype[i]();
                }
            }
            return gmbjcts;
        }

        public static GameObject[] Generic(MyLib.GameObjectfunction[] createtype, Action<GameObject>[] misc, Action<Transform> candos)
        {
            GameObject[] gmbjcts = new GameObject[createtype.Length];
            for (int i = 0; i < gmbjcts.Length; i++)
            {
                candos((gmbjcts[i] = createtype[i]()).transform);
                if (i < misc.Length)
                {
                    misc[i](gmbjcts[i]);
                }
            }
            return gmbjcts;
        }


        public static GameObject SimpleMob(Vector3 position, Transform team, float health)
        {
            GameObject gmbjct;
            if (EditorGameObject == null)
            {
                gmbjct = InstantiatePrefab(Resources.Load<GameObject>("Prefabs/mobs/npc"), team);
            }
            else
            {
                gmbjct = InstantiatePrefab(EditorGameObject._mob, team);
            }
            AI aivars = gmbjct.SearchComponent<AI>();
            LifeComponent lifevars = aivars.Life;
            MoveComponent MoveComponent = aivars.MoveComponent;

            MoveComponent.SetPosition(position);
            lifevars.Health = health;

            return gmbjct;
        }

        public static GameObject sprayer(Vector3 position, Transform team, float health)
        {
            GameObject gmbjct = SimpleMob(position, team, health);
            AI aivars = gmbjct.SearchComponent<AI>();

            aivars.AddAttacks(WeaponsStaticClass.spray);
            aivars.AssignModeScript<AttackFromDistance_AttackMode>();
            //  sprayattack sprayvars = gmbjct.getvars<sprayattack>();

            return gmbjct;
        }


        public static GameObject ballthrower(Vector3 position, Transform team, float health)
        {
            GameObject gmbjct = SimpleMob(position, team, health);
            AI aivars = gmbjct.SearchComponent<AI>();


            aivars.AddAttacks(WeaponsStaticClass.ball);
            aivars.AssignModeScript<BallCharge_AttackMode>();
            //ballcharge ballvars = gmbjct.getvars<ballcharge>();

            return gmbjct;
        }

        public static GameObject sniper(Vector3 position, Transform team, float health)
        {
            GameObject gmbjct = SimpleMob(position, team, health);
            AI aivars = gmbjct.SearchComponent<AI>();


            aivars.AddAttacks(WeaponsStaticClass.ball);
            BallChargeAttack ball = gmbjct.SearchComponent<BallChargeAttack>();
            ball.CanPassThroughBodies = false;
            ball.Reach = 30f;
            ball.MaxSize = 1f;
            ball.Speed = 60f;
            aivars.AssignModeScript<BallCharge_AttackMode>();

            return gmbjct;
        }


        public static GameObject flameThrower(Vector3 position, Transform team, float health)
        {
            GameObject gmbjct = SimpleMob(position, team, health);
            AI aivars = gmbjct.SearchComponent<AI>();


            aivars.AddAttacks(WeaponsStaticClass.fire);
            aivars.AssignModeScript<AttackFromDistance_AttackMode>().ratioTooClosesq = 0.5f.Sq();

            return gmbjct;
        }

        public static GameObject shielder(Vector3 position, Transform team, float health)
        {
            GameObject gmbjct = SimpleMob(position, team, health);
            AI aivars = gmbjct.SearchComponent<AI>();
            aivars.AddAttacks(WeaponsStaticClass.shield).SearchComponent<Shield>().SetUpAI();


            return gmbjct;
        }




        public static GameObject pusher(Vector3 position, Transform team, float health)
        {
            GameObject gmbjct = SimpleMob(position, team, health);
            AI aivars = gmbjct.SearchComponent<AI>();

            aivars.AddAttacks(WeaponsStaticClass.push);
            aivars.AssignModeScript<AttackFromDistance_AttackMode>();
            //  PushAttack pushvars = gmbjct.getvars<PushAttack>();

            return gmbjct;
        }

        public static GameObject meleemob(Vector3 position, Transform team, float health)
        {
            GameObject gmbjct = SimpleMob(position, team, health);
            AI aivars = gmbjct.SearchComponent<AI>();

            aivars.AddAttacks(WeaponsStaticClass.charge);
            aivars.AssignModeScript<AttackFromDistance_AttackMode>();
            //  PushAttack pushvars = gmbjct.getvars<PushAttack>();

            return gmbjct;
        }

        public static GameObject Charger(Vector3 position, Transform team, float health, float damage, float speed)
        {
            TriangleChargerAI chargevars = InstantiatePrefab(EditorGameObject._triangleCharger, team).SearchComponent<TriangleChargerAI>();
            LifeComponent lifevars = chargevars.SearchComponent<LifeComponent>();
            lifevars.Health = health;
            MoveComponent MoveComponent = chargevars.SearchComponent<MoveComponent>();
            chargevars.Damage = damage;
            chargevars.MaxSpeed = speed;

            MoveComponent.SetPosition(position);
            //chargevars.transform.position = position;
            return chargevars.gameObject;
        }
        public static GameObject Charger(Transform team)
        {
            if (EditorGameObject == null)
            {
                return InstantiatePrefab(Resources.Load<GameObject>("Prefabs/mobs/charger"), team);
            }
            else
            {
                return InstantiatePrefab(EditorGameObject._triangleCharger, team);
            }
        }

        public static OrbitAttack AddOrbitAttack(GameObject gameObject, bool addAnotherOne, float numberOfParticles, float damage, float speed, float Reach, float anglespeedDEG, float maxradius, float particleSize, float throwtime)
        {
            if (addAnotherOne || gameObject.SearchComponent<OrbitAttack>() == null)
            {
                GameObject orbitattackgmbjct = gameObject.GetCharacter().AddAttacks(WeaponsStaticClass.orbitAttack, addAnotherOne);
                orbitattackgmbjct.AddComponent<OrbitAutoAttack>().TimeToThrow = throwtime;
                OrbitAttack orbitvars = orbitattackgmbjct.SearchComponent<OrbitAttack>();
                if (numberOfParticles >= 0)
                {
                    orbitvars.NumberOfParticles = (int)numberOfParticles;
                }
                else
                {
                    orbitvars.NumberOfParticles = (int)(-numberOfParticles * orbitvars.NumberOfParticles);
                }
                if (damage >= 0f)
                {
                    orbitvars.SetDamage(damage);
                }
                else
                {
                    orbitvars.SetDamage(-damage * orbitvars.Damage);
                }
                if (speed >= 0f)
                {
                    orbitvars.Speed = speed;
                }
                else
                {
                    orbitvars.Speed *= -speed;
                }
                if (Reach >= 0f)
                {
                    orbitvars.Reach = Reach;
                }
                else
                {
                    orbitvars.Reach *= -Reach;
                }
                if (anglespeedDEG >= 0f)
                {
                    orbitvars.AngularSpeedDegrees = anglespeedDEG;
                }
                else
                {
                    orbitvars.AngularSpeedDegrees *= -anglespeedDEG;
                }
                if (maxradius >= 0f)
                {
                    orbitvars.MaxRadius = maxradius;
                }
                else
                {
                    orbitvars.MaxRadius *= -maxradius;
                }
                if (particleSize >= 0f)
                {
                    orbitvars.ParticleSize = particleSize;
                }
                else
                {
                    orbitvars.ParticleSize *= -particleSize;
                }
                return orbitvars;
            }
            return null;
        }
    }

    public static GameObject InstantiatePrefab(GameObject prefab, Transform parent = null)
    {
#if UNITY_EDITOR
        GameObject gmbjct;
        if (!Application.isPlaying)
        {
            gmbjct = PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;
        }
        else
        {
            gmbjct = BasicLib.MyInstantiate(prefab, parent);
        }
#else
        GameObject gmbjct = Instantiate(prefab, parent);
#endif
        return gmbjct;
    }

}

#if UNITY_EDITOR
public static class EditorLib
{
    private const double TIME_DIFFERENCE = 0.001;
    private static double _time = -TIME_DIFFERENCE;
    public static void DoMenuItemFunctionOnce(Action function)
    {
        if (EditorApplication.timeSinceStartup - _time > TIME_DIFFERENCE)
        {
            _time = EditorApplication.timeSinceStartup;
            function();

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        }
    }
}
#endif




























