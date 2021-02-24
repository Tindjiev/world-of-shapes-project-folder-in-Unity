using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityBase : MonoBehaviour
{
    private SkinManager _skin;
    public SkinManager Skin
    {
        get
        {
            if (_skin == null)
            {
                if (_skinTransform != null && _skinTransform.TryGetComponent(out _skin)) return _skin;
                if (MoveComponent != null)
                {
                    _skin = MoveComponent.GetComponentInChildren<SkinManager>();
                }
                else
                {
                    _skin = GetComponent<SkinManager>();
                }
            }
            return _skin;
        }

    }
    private Transform _skinTransform;
    public Transform SkinTransform => _skinTransform == null && Skin != null ? _skinTransform = _skin.transform : _skinTransform;

    public abstract bool CanAddToSeen(CollisionInfo target);

    protected void SetAsSkinTransform() => _skinTransform = transform;
    protected void SetAsSkinTransform(Transform skinTransform) => _skinTransform = skinTransform;

    private MoveComponent _moveComponent;
    public MoveComponent MoveComponent => _moveComponent == null ? _moveComponent = this.SearchComponent<MoveComponent>() : _moveComponent;

    public virtual Vector3 Position => MoveComponent.Position;

    [NonSerialized]
    private Team _team;
    [NonSerialized]
    private Transform _previousParent;
    public virtual Team Team
    {
        get
        {
            if (_team == null || transform.parent != _previousParent)
            {
                _previousParent = transform.parent;
                return _team = this.GetTeam();
            }
            return _team;
        }
    }

    public Color TeamColor => Team.TeamColor;
}

public abstract class EntityPartOfCharacter : EntityBase
{

    private BaseCharacterControl _holder;
    public BaseCharacterControl Holder => _holder == null ? _holder = this.GetCharacter() : _holder;

    [NonSerialized]
    private Team _team;
    [NonSerialized]
    private Team _previousTeam;
    public override Team Team
    {
        get
        {
            if (_team == null || Holder.Team != _previousTeam)
            {
                _previousTeam = Holder.Team;
                return _team = this.GetTeam();
            }
            return _team;
        }
    }

    public override bool CanAddToSeen(CollisionInfo target)
    {
        return Holder.CanAddToSeen(target);
    }

}

public abstract class BaseCharacterControl : EntityBase
{
    [SerializeField]
    private TargetPriority _targetPriority = TargetPriority.StandartPriority; //priorty of being targeted
    public TargetPriority TargetPriority => _targetPriority;

    [SerializeField]
    private DamageMode _damageMode = DamageMode.Standart; //can it be damaged
    public DamageMode DamageMode
    {
        get => _damageMode;
        protected set => _damageMode = value;
    }

    [SerializeField]
    private VisionBase _vision;
    public VisionBase Vision => _vision != null && _vision.gameObject.activeSelf ? _vision : _vision = GetComponentInChildren<VisionBase>();


    [Serializable]
    public class AggroInfo
    {
        [ReadOnlyOnInspectorDuringPlay]
        public AggroMode AggresiveMode = default;
        public bool CombineAggroModeAndCanTarget = false;
        public List<BaseCharacterControl> CanTarget = new List<BaseCharacterControl>();
        public BaseCharacterControl BaseTargetsOn = null;
        public Predicate<BaseCharacterControl> CheckIfCanBeTargetedFromTeam;

        public bool CheckCanTarget(BaseCharacterControl potentialTarget)
        {
            return CheckIfCanBeTargetedFromTeam(potentialTarget) || (CombineAggroModeAndCanTarget && CanTarget.Contains(potentialTarget, true));
        }
    }

    [SerializeField]
    private AggroInfo _infoOnAggresion = new AggroInfo();

    public AggroMode AggroMode
    {
        get => _infoOnAggresion.AggresiveMode;
        protected set => _infoOnAggresion.AggresiveMode = value;
    }

    private LifeComponent _life = null;
    public LifeComponent Life => _life == null ? _life = this.SearchComponent<LifeComponent>() : _life;

    protected Attack[] _attacks;

    [ReadOnlyOnInspector]
    public Vector3 DirectionVector, TargetPosition;
    [ReadOnlyOnInspector]
    public Transform TargetTransform;

    public MyLib.MyArrayList<Attack> AttackedBy { get; set; } = new MyLib.MyArrayList<Attack>().SetHowMuchSpaceToAdd(10);
    public MyLib.MyArrayList<EntityBase> Attacked { get; set; } = new MyLib.MyArrayList<EntityBase>().SetHowMuchSpaceToAdd(10);

    public struct AttackAndDamage
    {
        public Attack Attack { get; private set; }
        public float Damage { get; private set; }
        public AttackAndDamage(Attack attack, float damage)
        {
            Attack = attack;
            Damage = damage;
        }

        public void addDamage(float addedDamage)
        {
            Damage += addedDamage;
        }
    }
    public struct DamagedByInfo
    {
        public BaseCharacterControl Character;
        public MyLib.MyArrayList<AttackAndDamage> Attacks;

        public DamagedByInfo(BaseCharacterControl character)
        {
            Character = character;
            Attacks = new MyLib.MyArrayList<AttackAndDamage>();
        }
        public DamagedByInfo(Attack attack, float damage) : this(attack.Holder)
        {
            Attacks = new MyLib.MyArrayList<AttackAndDamage> { new AttackAndDamage(attack, damage) };
        }
        public DamagedByInfo(AttackAndDamage attackAndDamage) : this(attackAndDamage.Attack.Holder)
        {
            Attacks = new MyLib.MyArrayList<AttackAndDamage> { attackAndDamage };
        }

        public void Add(Attack attack, float damage)
        {
            int index = Attacks.FindIndex(x => x.Attack == attack);
            if (index == -1)
            {
                Attacks.Add(new AttackAndDamage(attack, damage));
            }
            else
            {
                Attacks.Array[index].addDamage(damage);
            }
        }

    }

    public List<DamagedByInfo> DamagedBy = new List<DamagedByInfo>();
    public void AddDamagedBy(Attack attack, float addedDamage)
    {
        int index = DamagedBy.FindIndex(x => x.Character == attack.Holder);
        if (index == -1)
        {
            DamagedBy.Add(new DamagedByInfo(attack, addedDamage));
        }
        else
        {
            DamagedBy[index].Add(attack, addedDamage);
        }
    }

    public float DamageGot
    {
        get
        {
            float sum = 0f;
            DamagedBy.ForEach(x => x.Attacks.ForEach(xx => sum += xx.Damage));
            return sum;
        }
    }

    protected void Awake()
    {
        //MoveComponent.gameObject.layer = colorlib.getlayerfromteam(somefunctions.getteam(transform));
        _attacks = this.SearchManyComponentsInOneLevelOfChildrenDepth<Attack>();
        foreach(Attack attack in _attacks)
        {
            attack.gameObject.SetActive(true);
        }
        SetAggroMode(_infoOnAggresion.AggresiveMode);
        DamagedBy.Clear();
    }

    public override bool CanAddToSeen(CollisionInfo target)
    {
        return CanTarget(target.Entity as BaseCharacterControl);
    }

    public void SetAggroMode(AggroMode mode, BaseCharacterControl FollowCanTargetOf = null)
    {
        switch (_infoOnAggresion.AggresiveMode = mode)
        {
            case AggroMode.AttackEnemyTeams:
                _infoOnAggresion.CheckIfCanBeTargetedFromTeam = CheckIfCanBeTargetedTargetEnemyTeams;
                break;
            case AggroMode.AttackNonAlliedTeams:
                _infoOnAggresion.CheckIfCanBeTargetedFromTeam = CheckIfCanBeTargetedTargetNonAlliedTeams;
                break;
            case AggroMode.AttackEverything:
                _infoOnAggresion.CheckIfCanBeTargetedFromTeam = CheckIfCanBeTargetedTargetEverything;
                break;
            case AggroMode.FollowCanTargetOfTeam:
                _infoOnAggresion.CheckIfCanBeTargetedFromTeam = Team.CheckIfCanBeTargeted;
                break;
            case AggroMode.Handpicked:
                _infoOnAggresion.CheckIfCanBeTargetedFromTeam = (x) => false;
                break;
            case AggroMode.FollowCanTargetFromAnother:
                if (_infoOnAggresion != null)
                {
                    _infoOnAggresion.BaseTargetsOn = FollowCanTargetOf;
                    _infoOnAggresion.CheckIfCanBeTargetedFromTeam = CheckIfCanBeTargetedBasedOnAnother;
                }
                else
                {
                    SetAggroMode(default);
                }
                break;
        }
    }

    public bool CanTarget(BaseCharacterControl potentialTarget)
    {
        if (potentialTarget == null) potentialTarget.IsDead();
        return potentialTarget.TargetPriority != TargetPriority.Untargetable && _infoOnAggresion.CheckCanTarget(potentialTarget);
    }

    protected bool CheckIfCanBeTargetedTargetEnemyTeams(BaseCharacterControl potentialTarget)
    {
        return Team.EnemyTeams.Contains(potentialTarget.Team);
    }
    protected bool CheckIfCanBeTargetedTargetNonAlliedTeams(BaseCharacterControl potentialTarget)
    {
        return !Team.AlliedTeams.Contains(potentialTarget.Team);
    }
    protected bool CheckIfCanBeTargetedTargetAnyOtherTeam(BaseCharacterControl potentialTarget)
    {
        return Team != potentialTarget.Team;
    }
    protected bool CheckIfCanBeTargetedTargetEverything(BaseCharacterControl potentialTarget)
    {
        return this != potentialTarget;
    }
    protected bool CheckIfCanBeTargetedBasedOnAnother(BaseCharacterControl potentialTarget)
    {
        if (_infoOnAggresion.BaseTargetsOn != null)
        {
            return _infoOnAggresion.BaseTargetsOn.CanTarget(potentialTarget);
        }
        else
        {
            SetAggroMode(default);
            return CanTarget(potentialTarget);
        }
    }

    public bool CanDamage(BaseCharacterControl potentialTarget)
    {
        return !IsAlliedWith(potentialTarget) || CanTarget(potentialTarget);
    }

    public bool IsAlliedWith(BaseCharacterControl character)
    {
        return Team.AlliedTeams.Contains(character.Team);
    }
    public bool IsEnemiesWith(BaseCharacterControl character)
    {
        return Team.EnemyTeams.Contains(character.Team);
    }
    public bool IsNeutralWith(BaseCharacterControl character)
    {
        return Team.NeutralTeams.Contains(character.Team);
    }

    protected void Start()
    {
    }

    [System.Serializable]
    public class CharacterData : DataClassBase
    {
        public float Health;
        public float MaxHealth;
        public Vector3 Position;
    }

    public static bool IsDead(BaseCharacterControl character)
    {
        return character == null || !character.gameObject.activeInHierarchy || character.Life.Health <= 0f;
    }


    public static BaseCharacterControl GetClosestOfSeenTargets(BaseCharacterControl character)
    {
        BaseCharacterControl TargetToReturn = null;
        float minDistSq = float.MaxValue;
        foreach (var target in (IEnumerable<BaseCharacterControl>)character.Vision)
        {
            float temp = (target.Position - character.Position).sqrMagnitude;
            if (minDistSq > temp && character.CanTarget(target))
            {
                minDistSq = temp;
                TargetToReturn = target;
            }
        }
        return TargetToReturn;
    }

    public static BaseCharacterControl GetClosestOfSeenAllies(BaseCharacterControl character)
    {
        BaseCharacterControl TargetToReturn = null;
        float minDistSq = float.MaxValue;
        foreach (var target in (IEnumerable<BaseCharacterControl>)character.Vision)
        {
            float temp = (target.Position - character.Position).sqrMagnitude;
            if (minDistSq > temp && character.IsAlliedWith(target))
            {
                minDistSq = temp;
                TargetToReturn = target;
            }
        }
        return TargetToReturn;
    }


#if UNITY_EDITOR

    [UnityEditor.CustomEditor(typeof(BaseCharacterControl), true), UnityEditor.CanEditMultipleObjects]
    private class CharacterEditor : ExtendedEditor
    {
        private static bool _showAddWeapons = false;

        protected override void OnInspectorGUIExtend(UnityEngine.Object currentTarget)
        {
            var aggroInfo = ((BaseCharacterControl)currentTarget)._infoOnAggresion;
            DrawPropertiesExcept(nameof(_infoOnAggresion));
            DrawField(serializedObject.FindProperty(nameof(_infoOnAggresion)).FindPropertyRelative(nameof(AggroInfo.AggresiveMode)));
            switch (aggroInfo.AggresiveMode)
            {
                case AggroMode.Handpicked:
                    ShowCanTargetOnInspector();
                    break;
                case AggroMode.FollowCanTargetFromAnother:
                    DrawProperty(nameof(AggroInfo.BaseTargetsOn));
                    ShowCombineToggleAndShowCanIfEnabled();
                    break;
                default:
                    ShowCombineToggleAndShowCanIfEnabled();
                    break;
            }
            AddWeaponsButtons();
        }

        protected override void ApplyChanges(UnityEngine.Object currentTarget)
        {
        }

        private void ShowCombineToggleAndShowCanIfEnabled()
        {
            var combineAggroModeAndCanTargetProperty = serializedObject.FindProperty(nameof(_infoOnAggresion)).FindPropertyRelative(nameof(AggroInfo.CombineAggroModeAndCanTarget));
            DrawField(combineAggroModeAndCanTargetProperty);
            if (combineAggroModeAndCanTargetProperty.boolValue)
            {
                ShowCanTargetOnInspector();
            }
        }

        private void ShowCanTargetOnInspector()
        {
            DrawField(serializedObject.FindProperty(nameof(_infoOnAggresion)).FindPropertyRelative(nameof(AggroInfo.CanTarget)));
        }

        private void AddWeaponsButtons()
        {
            _showAddWeapons = UnityEditor.EditorGUILayout.Foldout(_showAddWeapons, "Add Weapons");
            if (!_showAddWeapons) return;
            foreach (string weapon in WeaponsStaticClass.WeaponNames)
            {
                string weaponName = weapon.Substring(weapon.IndexOf('/') + 1);
                if (GUILayout.Button("Add " + TidyUpString(weaponName)))
                {
                    var tempObjects = new List<UnityEngine.Object>();
                    foreach (var characterObject in targets)
                    {
                        var character = (BaseCharacterControl)characterObject;
                        if (weaponName != typeof(OrbitAttack).Name && character is AI)
                        {
                            Attack attackAlreadyHave = character.GetComponentInChildren<Attack>();
                            if (attackAlreadyHave != null)
                            {
                                if (attackAlreadyHave.GetType().Name == weaponName) continue;
                                if (character is AI)
                                {
                                    DestroyImmediate(attackAlreadyHave.gameObject);
                                }
                            }
                        }
                        var attackGameObject = character.AddAttacks(weapon);
                        tempObjects.Add(attackGameObject);
                        try
                        {
                            if (character is AI)
                            {
                                attackGameObject.GetComponent<Attack>().SetUpAI();
                            }
                        }
                        catch (NotImplementedException e)
                        {
                            Debug.LogError(e);
                        }
                    }
                    UnityEditor.Selection.objects = tempObjects.ToArray();
                }
            }
            if (GUILayout.Button("Clear Weapons"))
            {
                DestroyOtherWeapons();
            }
        }

        private void DestroyOtherWeapons()
        {
            foreach (var characterObject in targets)
            {
                var character = (BaseCharacterControl)characterObject;
                foreach (var attack in character.GetComponentsInChildren<Attack>())
                {
                    DestroyImmediate(attack.gameObject);
                }
                foreach (var aiMode in character.GetComponents<AIModeClass>())
                {
                    if (!(aiMode is ChillModeClass))
                    {
                        DestroyImmediate(aiMode);
                    }
                }
            }
        }
    }
#endif
}

public static class CharacterExtensionMethods
{
    public static bool IsDead(this BaseCharacterControl character)
    {
        return BaseCharacterControl.IsDead(character);
    }
}

public enum TargetPriority
{
    StandartPriority = 0,
    LowPriority,
    HighPriority,
    Untargetable,
}

public enum AggroMode
{
    AttackEnemyTeams = 0,
    AttackNonAlliedTeams,
    AttackAnyOtherTeam,
    AttackEverything,
    FollowCanTargetOfTeam,
    Handpicked,
    FollowCanTargetFromAnother,
}

public enum DamageMode
{
    Standart = 0,
    Undamageable,
    Invulnerable,
}