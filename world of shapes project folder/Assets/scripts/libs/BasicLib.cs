using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

public static class BasicLib
{

    public static Vector3 GetPositionFromScreen(in Vector3 position)
    {
        Vector3 temp = Camera.main.ScreenToWorldPoint(position);
        temp.z = 0f;
        return temp;
    }

    public static Rect GetRectForGUI(in Vector3 position, float w, float h)  //translates position in world to position in screen
    {
        Vector3 temp = Camera.main.WorldToScreenPoint(position);
        return new Rect(temp.x, Screen.height - temp.y, w, h);
    }

    #region Instantiate

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

    public static GameObject InitializePlayer(float life, in Vector3 position, Transform team, Action death)
    {
        AddInEditor.CreatePlayer(team);
        ControlBase.PlayerGameObject.GetCharacter().AddAttacks();
        ControlBase.PlayerGameObject.SearchComponent<MoveComponent>().SetPosition(position);
        ControlBase.PlayerGameObject.SearchComponent<LifeComponent>().AddActionOnDeath(death);
        ControlBase.PlayerGameObject.SearchComponent<LifeComponent>().Health = life;
        return ControlBase.PlayerGameObject;
    }

    #endregion

}

public static class LayerNames
{
    public const int bodies = 8;
    public const int obstacles = 9;
    public const int imagbodies = 10;
    public const int attacks = 11;
    public const int vision = 12;
    public const int shields = 13;
    public const int eventObjects = 30;
    public const int playerTrigger = 31;

    public static int GetLayerMask(this int layer)
    {
        int layerMask = 0;
        for (int i = 0; i < 32; ++i)
        {
            if (!Physics2D.GetIgnoreLayerCollision(layer, i))
            {
                layerMask |= 1 << i;
            }
        }
        return layerMask;
    }
}

public static class MyRandomLib
{

    public static bool Rand50 => Random.Range(0, 2) == 0;



    //scale=4.4: ~10-90      9.2: ~1-99     1: ~37.75-62.25     0.8: ~40-60     0: 50-50     2.77: ~20-80
    public static float ExpRandom(float scale) => scale == 0.0 ? Random.value : Mathf.Log(Random.value * (Mathf.Exp(scale) - 1) + 1) / scale;
    public static float ExpRandom(float scale, float amplitude, float offset = 0f) => amplitude * (ExpRandom(scale) + offset);

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
        return BasicLib.InstantiatePrefabGmbjct(attack, holder.transform);
    }
    public static void AddAttacks(this BaseCharacterControl holder, params string[] attacks)
    {
        holder.AddAttacks((IEnumerable<string>)attacks);
    }
    public static void AddAttacks(this BaseCharacterControl holder, IEnumerable<string> attacks)
    {
        foreach (string attack in attacks)
        {
            BasicLib.InstantiatePrefabGmbjct(attack, holder.transform);
        }
    }
    public static void AddAttacks(this BaseCharacterControl holder)
    {
        if (holder.SearchComponent<PushAttack>() == null)
        {
            BasicLib.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<PushAttack>(), holder.transform);
        }
        if (holder.SearchComponent<SprayAttack>() == null)
        {
            BasicLib.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<SprayAttack>(), holder.transform);
        }
        if (holder.SearchComponent<Shield>() == null)
        {
            BasicLib.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<Shield>(), holder.transform);
        }
        if (holder.SearchComponent<DirectionChangeAttack>() == null)
        {
            BasicLib.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<DirectionChangeAttack>(), holder.transform);
        }
        if (holder.transform.SearchComponent<OrbitAttack>() == null)
        {
            BasicLib.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<OrbitAttack>(), holder.transform);
        }
        if (holder.transform.SearchComponent<FireAttack>() == null)
        {
            BasicLib.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<FireAttack>(), holder.transform);
        }
        if (holder.transform.SearchComponent<BallChargeAttack>() == null)
        {
            BasicLib.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<BallChargeAttack>(), holder.transform);
        }
        if (holder.transform.SearchComponent<OrbitClearAttack>() == null)
        {
            BasicLib.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<OrbitClearAttack>(), holder.transform);
        }
        if (holder.transform.SearchComponent<Boomerang>() == null)
        {
            BasicLib.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<Boomerang>(), holder.transform);
        }
        if (holder.transform.SearchComponent<MeleeChargeAttack>() == null)
        {
            BasicLib.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<MeleeChargeAttack>(), holder.transform);
        }
        if (holder.transform.SearchComponent<SummonAttack>() == null)
        {
            BasicLib.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<SummonAttack>(), holder.transform);
        }
        if (holder.transform.SearchComponent<MachineGun>() == null)
        {
            BasicLib.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<MachineGun>(), holder.transform);
        }
        if (holder.transform.SearchComponent<BasicHeal>() == null)
        {
            BasicLib.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<BasicHeal>(), holder.transform);
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
                if (holder.transform.SearchComponent<PushAttack>() == null)
                {
                    return BasicLib.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<PushAttack>(), holder.transform);
                }
                break;
            case fire:
                if (holder.transform.SearchComponent<FireAttack>() == null)
                {
                    return BasicLib.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<FireAttack>(), holder.transform);
                }
                break;
            case ball:
                if (holder.transform.SearchComponent<BallChargeAttack>() == null)
                {
                    return BasicLib.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<BallChargeAttack>(), holder.transform);
                }
                break;
            case spray:
                if (holder.transform.SearchComponent<SprayAttack>() == null)
                {
                    return BasicLib.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<SprayAttack>(), holder.transform);
                }
                break;
            case orbitClear:
                if (holder.transform.SearchComponent<OrbitClearAttack>() == null)
                {
                    return BasicLib.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<OrbitClearAttack>(), holder.transform);
                }
                break;
            case orbitAttack:
                if (holder.transform.SearchComponent<OrbitAttack>() == null)
                {
                    return BasicLib.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<OrbitAttack>(), holder.transform);
                }
                break;
            case Boomerang:
                if (holder.transform.SearchComponent<Boomerang>() == null)
                {
                    return BasicLib.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<Boomerang>(), holder.transform);
                }
                break;
            case directionChange:
                if (holder.transform.SearchComponent<DirectionChangeAttack>() == null)
                {
                    return BasicLib.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<DirectionChangeAttack>(), holder.transform);
                }
                break;
            case charge:
                if (holder.transform.SearchComponent<MeleeChargeAttack>() == null)
                {
                    return BasicLib.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<MeleeChargeAttack>(), holder.transform);
                }
                break;
            case shield:
                if (holder.transform.SearchComponent<Shield>() == null)
                {
                    return BasicLib.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<Shield>(), holder.transform);
                }
                break;
            case summoner:
                if (holder.transform.SearchComponent<SummonAttack>() == null)
                {
                    return BasicLib.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<SummonAttack>(), holder.transform);
                }
                break;
            case machine:
                if (holder.transform.SearchComponent<MachineGun>() == null)
                {
                    BasicLib.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<MachineGun>(), holder.transform);
                }
                break;
            case heal:
                if (holder.transform.SearchComponent<BasicHeal>() == null)
                {
                    BasicLib.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<BasicHeal>(), holder.transform);
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
                if (addToExisting || holder.SearchComponent<PushAttack>() == null)
                {
                    return BasicLib.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<PushAttack>(), holder.transform);
                }
                break;
            case fire:
                if (addToExisting || holder.transform.SearchComponent<FireAttack>() == null)
                {
                    return BasicLib.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<FireAttack>(), holder.transform);
                }
                break;
            case ball:
                if (addToExisting || holder.transform.SearchComponent<BallChargeAttack>() == null)
                {
                    return BasicLib.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<BallChargeAttack>(), holder.transform);
                }
                break;
            case spray:
                if (addToExisting || holder.transform.SearchComponent<SprayAttack>() == null)
                {
                    return BasicLib.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<SprayAttack>(), holder.transform);
                }
                break;
            case orbitClear:
                if (addToExisting || holder.transform.SearchComponent<OrbitClearAttack>() == null)
                {
                    return BasicLib.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<OrbitClearAttack>(), holder.transform);
                }
                break;
            case orbitAttack:
                if (addToExisting || holder.transform.SearchComponent<OrbitAttack>() == null)
                {
                    return BasicLib.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<OrbitAttack>(), holder.transform);
                }
                break;
            case Boomerang:
                if (addToExisting || holder.transform.SearchComponent<Boomerang>() == null)
                {
                    return BasicLib.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<Boomerang>(), holder.transform);
                }
                break;
            case directionChange:
                if (addToExisting || holder.transform.SearchComponent<DirectionChangeAttack>() == null)
                {
                    return BasicLib.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<DirectionChangeAttack>(), holder.transform);
                }
                break;
            case charge:
                if (addToExisting || holder.transform.SearchComponent<MeleeChargeAttack>() == null)
                {
                    return BasicLib.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<MeleeChargeAttack>(), holder.transform);
                }
                break;
            case shield:
                if (addToExisting || holder.transform.SearchComponent<Shield>() == null)
                {
                    return BasicLib.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<Shield>(), holder.transform);
                }
                break;
            case summoner:
                if (addToExisting || holder.transform.SearchComponent<SummonAttack>() == null)
                {
                    return BasicLib.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<SummonAttack>(), holder.transform);
                }
                break;
            case machine:
                if (addToExisting || holder.transform.SearchComponent<MachineGun>() == null)
                {
                    return BasicLib.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<MachineGun>(), holder.transform);
                }
                break;
            case heal:
                if (addToExisting || holder.transform.SearchComponent<BasicHeal>() == null)
                {
                    BasicLib.InstantiatePrefabGmbjct(Attack.BuildPathToWeapon<BasicHeal>(), holder.transform);
                }
                break;
        }
        return null;
    }
}


public static class MyInputs
{

    public static void InputsOn()
    {
        if (ControlBase.PlayerGameObject == null) return;
        PlayerBaseControl.ShouldBeActive = true;
    }
    public static void InputsOff()
    {
        if (ControlBase.PlayerGameObject == null) return;
        PlayerBaseControl.ShouldBeActive = false;
    }
    public static void InputsOnOff(bool onOff)
    {
        if (ControlBase.PlayerGameObject == null) return;
        PlayerBaseControl.ShouldBeActive = onOff;
    }


    public static int GetNumberPressed(MyLib.boolfunctionKeycode inputFunction)
    {
        for (int i = 0; i < 10; i++)
        {
            if (inputFunction(KeyCode.Alpha0 + i) || inputFunction(KeyCode.Keypad0 + i))
            {
                return i;
            }
        }
        return -1;
    }
    public static int GetNumberPressed() => GetNumberPressed(Input.GetKey);

    public static Vector2 GetMousePositionOnscreen
    {
        get
        {
            Vector2 mousePos = Input.mousePosition;
            mousePos.y = Screen.height - mousePos.y;
            return mousePos;
        }
    }

    public static Vector3 GetMousePositionFromscreen
    {
        get
        {
            if (MouseComponent.Mouse != null) return MouseComponent.Mouse.position;
            Vector3 tempPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            tempPosition.z = 0f;
            return tempPosition;
        }
    }

    public static bool CheckIfAnyKeyIsPressed(params KeyCode[] keys)
    {
        for (int i = keys.Length - 1; i >= 0; --i)
        {
            if (Input.GetKey(keys[i])) return true;
        }
        return false;
    }
    public static bool CheckIfAllKeysArePressed(params KeyCode[] keys)
    {
        for (int i = keys.Length - 1; i >= 0; --i)
        {
            if (!Input.GetKey(keys[i])) return false;
        }
        return true;
    }

}

public static class GroupKeysMethodExtensions
{

    public static bool CheckIfAnyKeyIsPressed(this KeyCode[] keys)
    {
        for (int i = keys.Length - 1; i >= 0; --i)
        {
            if (Input.GetKey(keys[i])) return true;
        }
        return false;
    }
    public static bool CheckIfAllKeysArePressed(this KeyCode[] keys)
    {
        for (int i = keys.Length - 1; i >= 0; --i)
        {
            if (!Input.GetKey(keys[i])) return false;
        }
        return true;
    }

    public static bool CheckIfAnyKeyIsPressed(this IEnumerable<KeyCode> keys)
    {
        foreach (var key in keys)
        {
            if (Input.GetKey(key)) return true;
        }
        return false;
    }
    public static bool CheckIfAllKeysArePressed(this IEnumerable<KeyCode> keys)
    {
        foreach (var key in keys)
        {
            if (!Input.GetKey(key)) return false;
        }
        return true;
    }
}


[Serializable]
public struct InputStruct
{
    public KeyCode[] MainKey;       //at least 1 MainKey must be pressed
    public ArrayKeys[] SecondaryKeys; //after a mainkey is pressed, all of secondary keys (one of each group) must be pressed

    public int TimesNeeded;

    private Func<KeyCode, bool> _inputFunction;

    private enum InputFunctionEnum
    {
        GetKey,
        GetKeyDown,
        GetKeyUp,
    }
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
    }

    private int _timesLeft;
    private float _lastTime;
    private const float _TIME_DIFFERENCE = 0.3f;

    public bool CheckInput()
    {
        if (CheckInputKey())
        {
            if (TimesNeeded <= 1) //if it is a one-time press (or less by mistake) then don't bother to check other stuff
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
#if UNITY_EDITOR
                    if (_timesLeft != 1)
                    {
                        Debug.Break();
                        Debug.Log(_timesLeft + " (times_left != 1)");
                    }
#endif
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
            if (!SecondaryKeys[i].Keys.CheckIfAnyKeyIsPressed())
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
            if (SecondaryKeys[i].Keys == null || SecondaryKeys[i].Keys.Length == 0) continue;
            temp += KeyToString(SecondaryKeys[i].Keys[0]) + " + ";
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
        SecondaryKeys = new ArrayKeys[0];
        TimesNeeded = 1;
        _timesLeft = TimesNeeded;
        _lastTime = 0f;
    }

    #region Constructors
    public InputStruct(Func<KeyCode, bool> function, params KeyCode[] keys) : this(function)
    {
        MainKey = keys;
    }
    public InputStruct(Func<KeyCode, bool> function, KeyCode[] mainkeys, KeyCode[] secondaryKeys) : this(function, mainkeys)
    {
        SecondaryKeys = new ArrayKeys[secondaryKeys.Length];
        for(int i = 0; i < SecondaryKeys.Length; ++i)
        {
            SecondaryKeys[i] = new ArrayKeys(secondaryKeys[i]);
        }
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

    public InputStruct(Func<KeyCode, bool> function, KeyCode[] mainkeys, KeyCode[][] secondaryKeys) : this(function, mainkeys)
    {
        SecondaryKeys = new ArrayKeys[secondaryKeys.Length];
        for(int i = 0; i < SecondaryKeys.Length; ++i)
        {
            SecondaryKeys[i] = new ArrayKeys(secondaryKeys[i]);
        }
    }
    public InputStruct(Func<KeyCode, bool> function, KeyCode[] mainkeys, KeyCode[][] secondaryKeys, int times) : this(function, mainkeys, secondaryKeys)
    {
        TimesNeeded = times;
    }

    public InputStruct(Func<KeyCode, bool> function, KeyCode mainkey, KeyCode[][] secondaryKeys) : this(function, new KeyCode[] { mainkey }, secondaryKeys)
    { }
    public InputStruct(Func<KeyCode, bool> function, KeyCode mainkey, KeyCode[][] secondaryKeys, int times) : this(function, new KeyCode[] { mainkey }, secondaryKeys, times)
    { }
    #endregion

    [Serializable]
    public struct ArrayKeys //this struct is delcared, so i can serialize array of arrays of keys
    {
        public KeyCode[] Keys;
        public ArrayKeys(params KeyCode[] keys)
        {
            Keys = keys;
        }

        public static implicit operator KeyCode[](ArrayKeys arrayKeys) => arrayKeys.Keys;
    }
}


public static class MyColorLib
{

    public static Transform[] teams;
    public const int green = 0, blue = 1, red = 2, yellow = 3;
    public readonly static Color BASE_COLOR_TO_REPLACE = new Color32(52, 52, 52, 255); //+1 of actual
    public readonly static Color BASE_COLOR_TO_REPLACE_TRUE = new Color32(51, 51, 51, 255);

    public static Color SetColorsAndA(Color newColor, float a)
    {
        return new Color(newColor.r, newColor.g, newColor.b, a);
    }


    [System.Serializable]
    public struct ColoredSprite
    {
        public Sprite sprite;
        public Color color;

        public ColoredSprite(Sprite newSprite, Color newColor)
        {
            sprite = newSprite;
            color = newColor;
        }
    }

    [System.Serializable]
    public class ColoredSpriteList
    {
        public string OriginalName;
        public List<ColoredSprite> ColoredSprites;

        public Sprite GetColoredSprite(Color color)
        {
            foreach (ColoredSprite colsprite in ColoredSprites)
            {
                if (colsprite.color == color) return colsprite.sprite;
            }
            return null;
        }


        public ColoredSpriteList(string name)
        {
            OriginalName = name;
            ColoredSprites = new List<ColoredSprite>();
        }

        public ColoredSpriteList(Sprite sprite, Color color, Color colortochange)
        {
            OriginalName = sprite.name;
            Sprite newsprite = ReplaceColor(color, sprite, colortochange);
            ColoredSprites = new List<ColoredSprite>();
            ColoredSprites.Add(new ColoredSprite(newsprite, color));
        }

        public ColoredSpriteList(Sprite sprite, string spritename, Color color, Color colortochange)
        {
            OriginalName = spritename;
            Sprite newsprite = ReplaceColor(color, sprite, colortochange);
            ColoredSprites = new List<ColoredSprite>();
            ColoredSprites.Add(new ColoredSprite(newsprite, color));
        }

        public ColoredSpriteList(string name, ColoredSprite newsprite)
        {
            OriginalName = name;
            ColoredSprites = new List<ColoredSprite>();
            ColoredSprites.Add(newsprite);
        }

        public ColoredSpriteList(string name, List<ColoredSprite> coloredspriteslist)
        {
            OriginalName = name;
            ColoredSprites = new List<ColoredSprite>(coloredspriteslist);
        }

    }

    private static List<ColoredSpriteList> _coloredSprites;
    private static List<Texture2D> _generatedTextures;

    public static List<ColoredSpriteList> ColoredSprites
    {
        get => _coloredSprites != null ? _coloredSprites : _coloredSprites = new List<ColoredSpriteList>();
        set => _coloredSprites = value;
    }
    public static List<Texture2D> GeneratedTextures
    {
        get => _generatedTextures != null ? _generatedTextures : _generatedTextures = new List<Texture2D>();
        set => _generatedTextures = value;
    }

    public static Sprite GetSpriteColored(Color newColor, Sprite sprite)
    {
        return GetSpriteColored(newColor, sprite, MyColorLib.BASE_COLOR_TO_REPLACE);
    }

    public static Sprite GetSpriteColored(Color newColor, Sprite sprite, Color colorToChange)
    {
        if (colorToChange == MyColorLib.BASE_COLOR_TO_REPLACE_TRUE) colorToChange = MyColorLib.BASE_COLOR_TO_REPLACE;
        ColoredSprites.ForEach(x => x.ColoredSprites.RemoveAll(y => y.sprite == null));
        ColoredSprites.RemoveAll(x => x.ColoredSprites.Count == 0);
        int index1 = ColoredSprites.FindIndex(x => x.OriginalName == sprite.name);
        if (index1 == -1) //first time registering this sprite
        {
            ColoredSprites.Add(new ColoredSpriteList(sprite.name, new ColoredSprite(sprite, colorToChange)));
            if (newColor == colorToChange) return sprite;
            Sprite newsprite = ReplaceColor(newColor, sprite, colorToChange);
            ColoredSprites[ColoredSprites.Count - 1].ColoredSprites.Add(new ColoredSprite(newsprite, newColor));
            return newsprite;
        }
        else
        {
            int index2 = ColoredSprites[index1].ColoredSprites.FindIndex(x => x.color == newColor);
            if (index2 == -1) //first time registering this color of the sprite
            {
                if (ColoredSprites[index1].ColoredSprites.Count == 0)
                {
                    Sprite newsprite = ReplaceColor(newColor, sprite, colorToChange);
                    ColoredSprites[index1].ColoredSprites.Add(new ColoredSprite(newsprite, newColor));
                    return newsprite;
                }
                else
                {
                    Sprite newsprite = ReplaceColor(newColor, sprite, colorToChange);
                    ColoredSprites[index1].ColoredSprites.Add(new ColoredSprite(newsprite, newColor));
                    return newsprite;
                }
            }
            else
            {
                return ColoredSprites[index1].ColoredSprites[index2].sprite;
            }
        }
    }


    private static Sprite ReplaceColor(Color newcolor, Sprite sprite, Color colortochange)
    {
        Sprite newsprite;
        Texture2D texture = SpriteToNewTexture2D(sprite);
        var textcolor = texture.GetPixels();
        int i, len = textcolor.Length;
        for (i = 0; i < len; ++i)
        {
            if (textcolor[i].a != 0f)
            {
                Vector3 div = MyDivideColors(textcolor[i], colortochange);
                if (div.x == div.y && div.y == div.z && div.z <= 1f && div.z != 0f)
                {
                    textcolor[i] = new Color(newcolor.r * div.z, newcolor.g * div.z, newcolor.b * div.z, newcolor.a);
                }
            }
        }
        texture.SetPixels(textcolor);
        texture.Apply();
        newsprite = Sprite.Create(texture,
            new Rect(0f, 0f, texture.width, texture.height),
            new Vector2(sprite.pivot.x / texture.width, sprite.pivot.y / texture.height),
            sprite.pixelsPerUnit);
        newsprite.name = sprite.name;
        return newsprite;
    }

    public static Texture2D SpriteToNewTexture2D(Sprite sprite)
    {
        Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
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
    }

    public static Texture2D RectangleTextureSetColor(this Texture2D texture, Color color)
    {
        texture.SetPixel(0, 0, color);
        texture.Apply();
        return texture;
    }

    private static Vector3 MyDivideColors(Color c1, Color c2)
    {
        Vector3 div;
        if (c2.r == 0f)
        {
            if (c1.r == 0f) div.x = 1f;
            else div.x = float.PositiveInfinity;
        }
        else
        {
            div.x = c1.r / c2.r;
        }
        if (c2.g == 0f)
        {
            if (c1.g == 0f) div.y = 1f;
            else div.y = float.PositiveInfinity;
        }
        else
        {
            div.y = c1.g / c2.g;
        }
        if (c2.b == 0f)
        {
            if (c1.b == 0f) div.z = 1f;
            else div.z = float.PositiveInfinity;
        }
        else
        {
            div.z = c1.b / c2.b;
        }
        return div;
    }

    public static Color MyMultiplyToColor(Color c, float mult)
    {
        float r = c.r * mult;
        float g = c.g * mult;
        float b = c.b * mult;
        if (r > 1f) r = 1f;
        if (g > 1f) g = 1f;
        if (b > 1f) b = 1f;
        return new Color(r, g, b, c.a);
    }

    public struct HSL
    {
        private float _h;
        public float H
        {
            get => _h;
            set
            {
                if ((_h = value) > 1f)
                {
                    _h %= 1f;
                }
                else if (_h < 0f)
                {
                    _h = 1f - ((-_h) % 1f);
                }
            }
        }
        public float S;
        public float L;
        public float A;

        public HSL(float h, float s, float l)
        {
            _h = 0f;
            S = s;
            L = l;
            A = 1f;
            H = h;
        }

        public HSL(float h, float s, float l, float a)
        {
            _h = 0f;
            S = s;
            L = l;
            A = a;
            H = h;
        }

        public Color RGB => HSLtoRGB(_h, S, L, A);

        public static bool operator ==(HSL color1, HSL color2) => color1.H == color2.H && color1.S == color2.S && color1.L == color2.L;
        public static bool operator !=(HSL color1, HSL color2) => !(color1 == color2);


        public bool Equals(HSL other) => this == other;

        public override bool Equals(object obj) => !ReferenceEquals(null, obj) && obj is HSL hsl && Equals(hsl);

        public override int GetHashCode() => 155696781;

        public HSL GetInvertedColor => new HSL(H + 0.5f, S, 1f - L, A);
        public Color GetInvertedColorRGB => HSLtoRGB(H + 0.5f, S, 1f - L, A);

    }

    public static Color InvertColor(this Color color)
    {
        return color.ToHSL().GetInvertedColor.RGB;
    }

    #region RGBandHSLconversions
    public static HSL ToHSL(this Color rgb)
    {
        return RGBtoHSL(rgb.r, rgb.g, rgb.b, rgb.a);
    }
    public static Color HSLtoRGB(float h, float s, float l, float a = 1f)
    {
        float hDegrees = h * 360f;
        float c = (1 - System.Math.Abs(2f * l - 1f)) * s;
        float x = c * (1 - System.Math.Abs((hDegrees / 60f % 2f) - 1f));
        float m = l - c / 2f;

        if (hDegrees < 60f)
        {
            return new Color(c + m, x + m, m, a);
        }
        else if (hDegrees < 120f)
        {
            return new Color(x + m, c + m, m, a);
        }
        else if (hDegrees < 180f)
        {
            return new Color(m, c + m, x + m, a);
        }
        else if (hDegrees < 240f)
        {
            return new Color(m, x + m, c + m, a);
        }
        else if (hDegrees < 300f)
        {
            return new Color(x + m, m, c + m, a);
        }
        else // 300 <= hDegrees < 360
        {
            return new Color(c + m, m, x + m, a);
        }
    }
    public static HSL RGBtoHSL(float r, float g, float b, float a = 1f)
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
                    float shift = 0 / 60;       // R / (360 / hex sides)
                    if (segment < 0)
                    {          // hue > 180, full rotation
                        shift = 360 / 60;         // R / (360 / hex sides)
                    }
                    h = segment + shift;
                    break;
                case 2:
                    segment = (b - r) / c;
                    shift = 120 / 60;     // G / (360 / hex sides)
                    h = segment + shift;
                    break;
                case 3:
                    segment = (r - g) / c;
                    shift = 240 / 60;     // B / (360 / hex sides)
                    h = segment + shift;
                    break;
            }
        }
        return new HSL(h * (60f / 360f), s, l, a);
    }
    #endregion

}

public static class Rules
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
    }
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
    */
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


public static class MyMathlib
{

    public const float PI = (float)System.Math.PI;
    public const float TAU = (float)(2.0 * PI);
    public const float RAD_TO_DEG = (float)(180.0 / System.Math.PI);
    public const float DEG_TO_RAD = (float)(System.Math.PI / 180.0);
    public const float SQRT_OF_2 = 1.4142135623730950488f;
    public const float INVERSE_SQRT_OF_2 = 0.7071067811865475244f;

    public static int Sq(this int n) => n * n;
    public static float Sq(this float n) => n * n;
    public static double Sq(this double n) => n * n;
    public static decimal Sq(this decimal n) => n * n;

    #region vectorOperations

    public static Vector3 PolarVectorRad(float magnitude, float rad)
    {
        return new Vector3(magnitude * Mathf.Cos(rad), magnitude * Mathf.Sin(rad));
    }

    public static Vector3 PolarVectorRad(float rad)
    {
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad));
    }

    public static Vector3 PolarVectorRad(float x, float y, float rad)
    {
        return new Vector3(x * Mathf.Cos(rad), y * Mathf.Sin(rad));
    }

    public static Vector3 PolarVectorDeg(float magnitude, float degrees)
    {
        return new Vector3(magnitude * Mathf.Cos(degrees *= DEG_TO_RAD), magnitude * Mathf.Sin(degrees));
    }

    public static Vector3 PolarVectorDeg(float degrees)
    {
        return new Vector3(Mathf.Cos(degrees *= DEG_TO_RAD), Mathf.Sin(degrees));
    }
    public static Vector3 PolarVectorDeg(float x, float y, float degrees)
    {
        return new Vector3(x * Mathf.Cos(degrees *= DEG_TO_RAD), y * Mathf.Sin(degrees));
    }
    
    public static Vector2 PolarVector2Rad(float magnitude, float rad)
    {
        return new Vector2(magnitude * Mathf.Cos(rad), magnitude * Mathf.Sin(rad));
    }

    public static Vector2 PolarVector2Rad(float rad)
    {
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }

    public static Vector2 PolarVector2Rad(float x, float y, float rad)
    {
        return new Vector2(x * Mathf.Cos(rad), y * Mathf.Sin(rad));
    }

    public static Vector2 PolarVector2Deg(float magnitude, float degrees)
    {
        return new Vector2(magnitude * Mathf.Cos(degrees *= DEG_TO_RAD), magnitude * Mathf.Sin(degrees));
    }

    public static Vector2 PolarVector2Deg(float degrees)
    {
        return new Vector2(Mathf.Cos(degrees *= DEG_TO_RAD), Mathf.Sin(degrees));
    }
    public static Vector2 PolarVector2Deg(float x, float y, float degrees)
    {
        return new Vector2(x * Mathf.Cos(degrees *= DEG_TO_RAD), y * Mathf.Sin(degrees));
    }

    public static Vector3 Conjugatevect(in Vector3 vect)
    {
        return new Vector3(vect.x, -vect.y);
    }

    public static Vector3 AbsElementWise(in Vector3 vect)
    {
        return new Vector3(System.Math.Abs(vect.x), System.Math.Abs(vect.y));
    }

    public static Vector3 MulElementWise(in Vector3 vect1, in Vector3 vect2)
    {
        return new Vector3(vect1.x * vect2.x, vect1.y * vect2.y);
    }
    public static Vector3 MulElementWise(in Vector3 vect1, float x, float y)
    {
        return new Vector3(vect1.x * x, vect1.y * y);
    }
    public static Vector3 MulElementWise(float x1, float y1, float x2, float y2)
    {
        return new Vector3(x1 * x2, y1 * y2);
    }

    public static Vector3 MultiplyComplex(in Vector3 vect1, in Vector3 vect2)
    {                                                                       //vect1*vect2
        return new Vector3(vect1.x * vect2.x - vect1.y * vect2.y, vect1.y * vect2.x + vect1.x * vect2.y);
    }
    public static Vector3 MultiplyComplex(float x, float y, in Vector3 vect2)
    {                                                                       //vect1*vect2
        return new Vector3(x * vect2.x - y * vect2.y, y * vect2.x + x * vect2.y);
    }
    public static Vector3 MultiplyComplex(float x1, float y1, float x2, float y2)
    {                                                                       //vect1*vect2
        return new Vector3(x1 * x2 - y1 * y2, y1 * x2 + x1 * y2);
    }

    public static Vector3 MultiplyComplexConjugate(in Vector3 vect1, in Vector3 vect2)
    {                                                                       //vect1*conj(vect2)
        return new Vector3(vect1.x * vect2.x + vect1.y * vect2.y, vect1.y * vect2.x - vect1.x * vect2.y, 0f);
    }

    public static Vector3 Rotate90(Vector3 vect_to_rotate)
    {                                                                                      //rotate a vector by 90 degrees
        var tempY = vect_to_rotate.x;                             //vect_to_rotate = new Vector3(-vect_to_rotate.y, vect_to_rotate.x);
        vect_to_rotate.x = -vect_to_rotate.y;
        vect_to_rotate.y = tempY;
        return vect_to_rotate;
    }

    public static Vector3 Rotate90(Vector3 vect_to_rotate, in Vector3 centre)
    {                                                                                      //rotate a vector to a centre by 90 degrees
        var tempY = centre.y - centre.x + vect_to_rotate.x;       //vect_to_rotate = Rotate90(vect_to_rotate - centre) + centre;
        vect_to_rotate.x = centre.y + centre.x - vect_to_rotate.y;
        vect_to_rotate.y = tempY;
        return vect_to_rotate;
    }

    public static Vector3 RotateDegrees(Vector3 vect_to_rotate, float degrees, in Vector3 centre)
    {                                                                                      //rotate a vector to a centre by angle in degrees
        return MultiplyComplex(vect_to_rotate - centre, PolarVectorDeg(degrees)) + centre;
    }

    public static Vector3 RotateDegrees(Vector3 vect_to_rotate, float degrees)
    {                                                                                      //rotate a vector by angle in degrees
        return MultiplyComplex(vect_to_rotate, PolarVectorDeg(degrees));
    }

    public static Vector3 RotateRadians(Vector3 vect_to_rotate, float rad, in Vector3 centre)
    {                                                                                      //rotate a vector to a centre by angle in rad
        return MultiplyComplex(vect_to_rotate - centre, PolarVectorRad(rad)) + centre;
    }

    public static Vector3 RotateRadians(Vector3 vect_to_rotate, float rad)
    {                                                                                      //rotate a vector by angle in rad
        return MultiplyComplex(vect_to_rotate, PolarVectorRad(rad));
    }

    public static Vector3 RotateVector(Vector3 pos, in Vector3 rotate, in Vector3 centre)
    {                                                                                      //rotate a vector to a centre by angle of a vector
        return MultiplyComplex(pos - centre, rotate) + centre;
    }

    public static Vector3 RotateVector(Vector3 pos, in Vector3 rotate)
    {                                                                                      //rotate a vector by angle of a vector
        return MultiplyComplex(pos, rotate);
    }

    public static void Rotate90(this ref Vector3 vect_to_rotate)
    {                                                                                      //rotate a vector by 90 degrees
        var tempY = vect_to_rotate.x;                             //vect_to_rotate = new Vector3(-vect_to_rotate.y, vect_to_rotate.x);
        vect_to_rotate.x = -vect_to_rotate.y;
        vect_to_rotate.y = tempY;
    }

    public static void Rotate90(this ref Vector3 vect_to_rotate, in Vector3 centre)
    {                                                                                      //rotate a vector to a centre by 90 degrees
        var tempY = centre.y - centre.x + vect_to_rotate.x;       //vect_to_rotate = Rotate90(vect_to_rotate - centre) + centre;
        vect_to_rotate.x = centre.y + centre.x - vect_to_rotate.y;
        vect_to_rotate.y = tempY;
    }

    public static void RotateDegrees(this ref Vector3 pos, float degrees, in Vector3 centre)
    {
        pos = MultiplyComplex(pos - centre, PolarVectorDeg(degrees)) + centre;
    }

    public static void RotateDegrees(this ref Vector3 pos, float degrees)
    {
        pos = MultiplyComplex(pos, PolarVectorDeg(degrees));
    }

    public static void RotateRadians(this ref Vector3 pos, float rad, in Vector3 centre)
    {
        pos = MultiplyComplex(pos - centre, PolarVectorRad(rad)) + centre;
    }

    public static void RotateRadians(this ref Vector3 pos, float rad)
    {
        pos = MultiplyComplex(pos, PolarVectorRad(rad));
    }
    public static float AnlgeDegrees(this in Vector3 vector)
    {
        return Mathf.Atan2(vector.y, vector.x) * RAD_TO_DEG;
    }

    public static float AnlgeDegrees(this in Vector2 vector)
    {
        return Mathf.Atan2(vector.y, vector.x) * RAD_TO_DEG;
    }

    public static float AngleRadians(this in Vector3 vector)
    {
        return Mathf.Atan2(vector.y, vector.x);
    }

    public static float AngleRadians(this in Vector2 vector)
    {
        return Mathf.Atan2(vector.y, vector.x);
    }

    public static Vector3 ProjectOn(this Vector3 vectorToProject, in Vector3 projectOnVector)
    {
        return Vector3.Dot(vectorToProject, projectOnVector) / projectOnVector.sqrMagnitude * projectOnVector;
    }
    public static Vector3 ProjectOnNormalisedVector(this Vector3 vectorToProject, in Vector3 projectOnVector)
    {
        return Vector3.Dot(vectorToProject, projectOnVector) * projectOnVector;
    }

    public static Vector2 ProjectOn(this Vector2 vectorToProject, Vector2 projectOnVector)
    {
        return Vector2.Dot(vectorToProject, projectOnVector) / projectOnVector.sqrMagnitude * projectOnVector;
    }
    public static Vector2 ProjectOnNormalisedVector(this Vector2 vectorToProject, Vector2 projectOnVector)
    {
        return Vector2.Dot(vectorToProject, projectOnVector) * projectOnVector;
    }

#endregion


    //if n is even then return -1, else if its odd then return 1
    public static int EvenNegativeAndOddPositive(this int n) => ((n & 1) << 1) - 1;  // (n%2)*2 - 1


    //if n is even then return 1, else if its odd then return -1
    public static int EvenPositiveAndOddNegative(int n) => 1 - ((n & 1) << 1); // 1 - (n%2)*2

    public static bool IsEven(this int number) => (number & 1) == 0;
    public static bool IsOdd(this int number) => (number & 1) != 0;

    public static bool IsInsideSquare(float x, float y, float edgeSize)
    {
        return Mathf.Abs(x + y) + Mathf.Abs(x - y) < edgeSize;
    }
    public static bool IsInsideSquare(in Vector2 xy, float edgeSize)
    {
        return IsInsideSquare(xy.x, xy.y, edgeSize);
    }
    public static bool IsInsideSquare(in Vector3 xy, float edgeSize)
    {
        return IsInsideSquare(xy.x, xy.y, edgeSize);
    }
    public static bool IsInsideSquare(float x, float y, float edgeSize, float xcentre, float ycentre)
    {
        return IsInsideSquare(x - xcentre, y - ycentre, edgeSize);
    }
    public static bool IsInsideSquare(in Vector2 xy, float edgeSize, in Vector2 centre)
    {
        return IsInsideSquare(xy.x - centre.x, xy.y - centre.y, edgeSize);
    }
    public static bool IsInsideSquare(in Vector3 xy, float edgeSize, in Vector3 centre)
    {
        return IsInsideSquare(xy.x - centre.x, xy.y - centre.y, edgeSize);
    }


    public static Rect RectangleCentre(this Rect RectWithImaginedCentre, float xcentre, float ycentre)
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
        if (xcentre == -1f && ycentre == -1f) return RectWithImaginedCentre;
        return new Rect(RectWithImaginedCentre.x - (xcentre + 1f) * RectWithImaginedCentre.width / 2f , RectWithImaginedCentre.y - (ycentre + 1f) * RectWithImaginedCentre.height / 2f , RectWithImaginedCentre.width + 0.5f, RectWithImaginedCentre.height+0.5f);
    }
    public static Rect RectangleCentre(this Rect RectWithImaginedCentre, TextAnchor alignment)
    { 
        float xcentre = ((int)alignment % 3) - 1f;
        float ycentre = ((int)alignment / 3) - 1f;
        return RectWithImaginedCentre.RectangleCentre(xcentre, ycentre);
    }
    public static Rect RectangleCentre(this Rect RectWithImaginedCentre, GUIStyle style)
    {
        return RectWithImaginedCentre.RectangleCentre(style.alignment);
    }

    public static int BitCount(this uint n)
    {
        int count = 0;
        for (; n != 0; n &= n - 1, ++count) ;
        return count;
    }

    public static int BitCount(this ulong n)
    {
        int count = 0;
        for (; n != 0; n &= n - 1, ++count) ;
        return count;
    }
}


public struct HMS
{
    private int _hours;
    private int _minutes;
    private float _seconds;

    public int Hours => _hours;

    public int Minutes => _minutes;

    public float Seconds => _seconds;

    public HMS(int h, int m, float s)
    {
        _hours = h;
        _minutes = m;
        _seconds = s;

        Normalize();
    }

    public static HMS ConvertFromSeconds(float givenseconds)
    {
        HMS time = new HMS(0, 0, givenseconds);
        time.Normalize();
        return time;
    }

    public static HMS ConvertFromMinutes(float givenminutes)
    {
        int minutes = (int)givenminutes;
        HMS time = new HMS(0, minutes, (givenminutes - minutes) * 60f);
        time.Normalize();
        return time;
    }

    public static HMS ConvertFromHours(float givenhours)
    {
        int hours = (int)givenhours;
        HMS time = ConvertFromMinutes((givenhours - hours) * 60f);
        time._hours = hours;
        time.Normalize();
        return time;
    }

    public void Normalize()
    {
        int addednextunits = (int)(_seconds / 60f);
        _seconds -= addednextunits * 60f;
        _minutes += addednextunits;

        addednextunits = (int)(_minutes / 60f);
        _minutes -= addednextunits * 60;
        _hours += addednextunits;
    }

    public static HMS operator +(HMS clock1, HMS clock2)
    {
        HMS result = new HMS(clock1._hours + clock2._hours, clock1._minutes + clock2._minutes, clock1._seconds + clock2._seconds);
        result.Normalize();
        return result;
    }

    public override string ToString()
    {
        return string.Format("{0}:{1:00}:{2:00.00}", _hours, _minutes, _seconds);
    }

    public string ToStringAvailable()
    {
        string temp = "";
        if (_hours != 0)
        {
            temp += _hours.ToString() + "h ";
        }
        if (_minutes != 0)
        {
            temp += _minutes.ToString() + "m ";
        }
        temp += string.Format("{0:0.00}", _seconds) + "s";
        return temp;
    }

}

[Serializable]
public class Timer
{

    public static float _timePaused = 0f;

    private static float _actualTime => Time.time - _timePaused;

    [NonSerialized]
    private float _timeMoment = float.MinValue;

    [field: SerializeField]
    public float TotalCooldown { get; private set; }
    public Timer()
    {
        TotalCooldown = 0f;
    }

    public Timer(float TimeToCount)
    {
        TotalCooldown = TimeToCount;
    }

    public void StartTimer()
    {
        _timeMoment = _actualTime;
    }

    public void StartTimer(float delay)
    {
        _timeMoment = _actualTime + delay;
    }

    public void StartTimerWithTempCounter(float tempCounter)
    {
        _timeMoment = _actualTime + tempCounter - TotalCooldown;
    }

    public void StartTimerAndSetCounter(float newCounter)
    {
        TotalCooldown = newCounter;
        _timeMoment = _actualTime;
    }

    public void StartTimerAndSetCounter(float newCounter, float delay)
    {
        TotalCooldown = newCounter;
        _timeMoment = _actualTime + delay;
    }

    public void Delay(float delay)
    {
        _timeMoment -= delay;
    }

    public void SetCounter(float timeToCount)
    {
        TotalCooldown = timeToCount;
    }

    public void ReduceTimeRemaining(float SecondsToReduce)
    {
        _timeMoment -= SecondsToReduce;
    }

    public bool CheckIfTimePassed => _actualTime - _timeMoment > TotalCooldown;

    public bool CheckIfTimeNotPassed => _actualTime - _timeMoment < TotalCooldown;

    public float TimePassed => _actualTime - _timeMoment;

    public float TimeRemaining
    {
        get
        {
            float timeRemaining = TotalCooldown - _actualTime + _timeMoment;
            return timeRemaining < 0f ? 0f : timeRemaining;
        }
    }

    public float TimeRemainingRatio
    {
        get
        {
            if (TotalCooldown == 0f) return 0f;
            float ratio = TimeRemaining / TotalCooldown;
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

    public override string ToString()
    {
        return "Timer with cooldown: " + TotalCooldown + ", with time remaining: " + TimeRemaining;
    }

}


public static class MyLib
{

    public delegate void voidfunction();
    public delegate void voidfunctionTransform(Transform tr);
    public delegate void voidfunctionGameObject(GameObject gmbjct);
    public delegate bool boolfunction();
    public delegate bool boolfunctionbool(bool tf);
    public delegate bool boolfunctioninput(InputStruct input);
    public delegate bool boolfunctionKeycode(KeyCode input);
    public delegate Transform Transformfunction();
    public delegate GameObject GameObjectfunction();
    public delegate GameObject GameObjectfunctionTransform(Transform tr);

    public static void DoNothing()
    {

    }
    public static bool BoolTrueDoNothing() => true;
    public static bool BoolFalseDoNothing() => false;

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
    public static void ExpandArray<T>(ref T[] MainArray, T[] ArrayToAdd)
    {
        if (MainArray == null)
        {
            MainArray = new T[ArrayToAdd.Length];
            for(int i = MainArray.Length - 1; i >= 0; --i)
            {
                MainArray[i] = ArrayToAdd[i];
            }
            return;
        }
        if (ArrayToAdd == null || ArrayToAdd.Length == 0)
        {
            return;
        }
        int mainLen = MainArray.Length;
        System.Array.Resize(ref MainArray, MainArray.Length + ArrayToAdd.Length);
        for (int i = 0; i < ArrayToAdd.Length; ++i)
        {
            MainArray[i + mainLen] = ArrayToAdd[i];
        }
    }
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
    public static int ArrayRemoveAt<T>(ref T[] Array, int i)
    {
        if (Array == null)
        {
            return -1;
        }
        int iToReturn = i;
        for (i++; i < Array.Length; i++)
        {
            Array[i - 1] = Array[i];
        }
        System.Array.Resize(ref Array, Array.Length - 1);
        return iToReturn;
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
    public static T SearchAndRemove<T>(this List<T> cantarget, System.Predicate<T> ConditionToRemove, System.Predicate<T> ConditionToReturn) where T : class
    {
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


    public static T PickRandom<T>(this T[] array)
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

    public static bool CheckOutOfRange<T>(this T[] array, int i)
    {
        return i < 0 || i >= array.Length;
    }
    public static bool CheckOutOfRange<T>(this T[,] array, int i, int j)
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

    public static LinkedListNode<T> PushToLast<T>(this LinkedList<T> List, LinkedListNode<T> NodeToBePushed)
    {
        LinkedListNode<T> temp = NodeToBePushed.Next;
        List.Remove(NodeToBePushed);
        List.AddLast(NodeToBePushed);
        return temp;
    }
    public static LinkedListNode<T> GetNodeAt<T>(this LinkedList<T> List, int i)
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

    #region TypeComparasions

    public static bool IsTheExactType(this object obj, Type type) => obj != null ? obj.GetType() == type : false;
    public static bool IsTheExactType(this object thisObject, object obj) => obj != null ? thisObject.IsTheExactType(obj.GetType()) : false;
    public static bool IsTheExactType<T>(this Type type) => type == typeof(T);
    public static bool IsTheExactType<T>(this object obj) => obj.IsTheExactType(typeof(T));

    public static bool IsSubClassOf(this object obj, Type type) => obj != null ? obj.GetType().IsSubclassOf(type) : false;
    public static bool IsSubClassOf(this object thisObject, object obj) => obj != null ? thisObject.IsSubClassOf(obj.GetType()) : false;
    public static bool IsSubClassOf<T>(this Type type) => type.IsSubclassOf(typeof(T));
    public static bool IsSubClassOf<T>(this object obj) => obj.IsSubClassOf(typeof(T));

    public static bool IsExactTypeOrSubClassOf(this object obj, Type type) => obj.IsTheExactType(type) || obj.IsSubClassOf(type);
    public static bool IsExactTypeOrSubClassOf(this object thisObject, object obj) => thisObject.IsTheExactType(obj) || thisObject.IsSubClassOf(obj);
    public static bool IsExactTypeOrSubClassOf<T>(this Type type) => type.IsExactTypeOrSubClassOf(typeof(T)); //for object is not need because it's the "is" keyword
    public static bool IsExactTypeOrSubClassOf(this Type thisType, Type type) => thisType == type || thisType.IsSubclassOf(type);

    #endregion

    public static string GetCurrentTimeDate()
    {
        return System.DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
    }


}

public static class PrintLib
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

    public static string BoolToString(this bool value) => value ? "1" : "0";

}


public static class SearchComponentExtensions
{
    private static GameObject _nullGameObject = null;

    public static T SearchComponent<T>(this Component component) where T : Component
    {
        if (component == null) return null;
        for (Transform tr = component.transform; tr != null; tr = tr.parent)
        {
            if (tr.TryGetComponent(out T tempComponent))
            {
                return tempComponent;
            }
            else if (tr.TryGetComponent(out BaseCharacterControl _))
            {
                if(tr.TryGetComponent(out tempComponent))
                {
                    return tempComponent;
                }
                return tr.GetComponentInChildren<T>(true);
            }
        }
        var temp = (_nullGameObject == null ? _nullGameObject = GameObject.Find("null") : _nullGameObject);
        return temp != null ? temp.GetComponent<T>() : null;
    }

    public static Transform SearchComponentTransform<T>(this Component component) where T : Component
    {
        if (component == null) return null;
        for (Transform tr = component.transform; tr != null; tr = tr.parent)
        {
            if (tr.TryGetComponent(out T _))
            {
                return tr;
            }
            else if (tr.TryGetComponent(out BaseCharacterControl _))
            {
                if (tr.TryGetComponent(out T tempComponent))
                {
                    return tempComponent.transform;
                }
                return (tempComponent = tr.GetComponentInChildren<T>(true)) != null ? tempComponent.transform : null;
            }
        }
        return null;
    }

    public static T[] SearchManyComponentsInOneLevelOfChildrenDepth<T>(this Component component) where T : Component
    {
        if (component == null) return null;
        T[] componentsFound = new T[0];
        for (Transform tr = component.transform; true; tr = tr.parent)
        {
            T[] tempComponentArray = tr.GetComponentsInDirectChildren<T>();
            if (tempComponentArray.Length != 0)
            {
                MyLib.ExpandArray(ref componentsFound, tempComponentArray);
            }
            if (tr.parent == null || tr.TryGetComponent(out BaseCharacterControl _))
            {
                if ((tempComponentArray = tr.GetComponents<T>()).Length != 0)
                {
                    MyLib.ExpandArray(ref componentsFound, tempComponentArray);
                }
                return componentsFound;
            }
        }
    }
    public static T[] GetComponentsInDirectChildren<T>(this Component component) where T : Component
    {
        if (component == null) return null;
        List<T> components = new List<T>();
        for (int i = 0; i < component.transform.childCount; ++i)
        {
            if (component.transform.GetChild(i).TryGetComponent(out T tempComponent)) components.Add(tempComponent);
        }
        return components.ToArray();
    }
    public static T GetComponentInDirectChildren<T>(this Component component) where T : Component
    {
        if (component == null) return null;
        for (int i = 0; i < component.transform.childCount; ++i)
        {
            if (component.transform.GetChild(i).TryGetComponent(out T tempComponent)) return tempComponent;
        }
        return null;
    }

    public static T SearchComponent<T>(this GameObject gameObject) where T : Component => gameObject != null ? gameObject.transform.SearchComponent<T>() : null;
    public static Transform SearchComponentTransform<T>(this GameObject gameObject) where T : Component => gameObject != null ? gameObject.transform.SearchComponentTransform<T>() : null;
    public static T[] GetComponentsInDirectChildren<T>(this GameObject gameObject) where T : Component => gameObject != null ? gameObject.transform.GetComponentsInDirectChildren<T>() : null;

    public static Team GetTeam(this Component component)
    {
        for (Transform teamTransform = component.transform; teamTransform != null; teamTransform = teamTransform.parent)
        {
            if (teamTransform.TryGetComponent(out Team team))
            {
                return team;
            }
        }
        var temp = (_nullGameObject == null ? _nullGameObject = GameObject.Find("null") : _nullGameObject);
        return temp != null ? temp.GetComponent<Team>() : null;
    }

    public static Team GetTeam(this GameObject gameObject) => gameObject.transform.GetTeam();

    public static BaseCharacterControl GetCharacter(this Component component)
    {
        for (Transform teamTransform = component.transform; teamTransform != null; teamTransform = teamTransform.parent)
        {
            if (teamTransform.TryGetComponent(out BaseCharacterControl character))
            {
                return character;
            }
        }
        return null;
    }

    public static BaseCharacterControl GetCharacter(this GameObject gameObject) => gameObject.transform.GetCharacter();
}



public static class ComponentExtensions
{
    public static void Disable(this Behaviour Behaviour) => Behaviour.enabled = false;
    public static void Enable(this Behaviour Behaviour) => Behaviour.enabled = true;
    public static void Disable(this Renderer Renderer) => Renderer.enabled = false;
    public static void Enable(this Renderer Renderer) => Renderer.enabled = true;
}

public static class GameObjectExtensions
{
    public static void SetActiveTrue(this GameObject gameObject) => gameObject.SetActive(true);
    public static void SetActiveFalse(this GameObject gameObject) => gameObject.SetActive(false);
}





















