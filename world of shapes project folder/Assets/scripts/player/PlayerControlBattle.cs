using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class PlayerControlBattle : PlayerBaseControl, IEnumerable<PlayerControlBattle.Slot>
{
    private Slot[] _slots;
    private float _lastTimeChange = 0f;
    private bool _savedInputs = true;
    private const float _TIME_TO_SAVE = 5f;

    public int SlotInputIndex { get; private set; }

    public struct Slot
    {
        public KeyCode Key;
        public Attack Attack;

        public Slot(KeyCode key, Attack attack)
        {
            Key = key;
            Attack = attack;
        }

        public bool AttackInput() => Attack.Activate(Attack.InputFunction(Key));

        public string KeyName
        {
            get
            {
                if (Key == KeyCode.Mouse0)
                {
                    return "L-Click";
                }
                if (Key == KeyCode.Mouse1)
                {
                    return "R-Click";
                }
                return Key.ToString();
            }
        }

    }

    protected new void Awake()
    {
        base.Awake();
    }
    protected new void Start()
    {
        base.Start();
        ActionsSet();
    }

    protected new bool Update()
    {
        if (!base.Update()) return false;
        ChooseInputSlot();
        ChangeActionOfInputSlot();
        foreach (Slot slot in _slots)
        {
            if (slot.Attack != null) slot.AttackInput();
        }
        return true;
    }

    protected new void OnDestroy()
    {
        base.OnDestroy();
    }

    protected new void OnDisable()
    {
        base.OnDisable();
    }

    private void ActionsSet()
    {
        _attacks = this.SearchManyComponentsInOneLevelOfChildrenDepth<Attack>();

        try
        {
            if (LoadData()) return;
        }
        catch (System.Exception e)
        {
#if UNITY_EDITOR
            Debug.Break();
            Debug.Log(e);
#endif
        }

        KeyCode[] keys = new KeyCode[] { KeyCode.Mouse0, KeyCode.Mouse1, KeyCode.Space , KeyCode.Q, KeyCode.E, KeyCode.F };

        _slots = new Slot[keys.Length];
        for (int i = 0; i < _slots.Length && i < _attacks.Length; i++)
        {
            _slots[i].Attack = _attacks[i];
            _slots[i].Key = keys[i];
        }


    }

    private void ChooseInputSlot()
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i) || Input.GetKeyDown(KeyCode.Keypad1 + i))
            {
                SlotInputIndex = i;
                return;
            }
        }
    }

    private void ChangeActionOfInputSlot()
    {
        int input;
        if (!Input.GetKey(KeyCode.LeftControl) && (input = System.Math.Sign(Input.GetAxis("Mouse ScrollWheel"))) != 0)
        {
            int i;
            if (_slots[SlotInputIndex].Attack == null)
            {
                i = input > 0 ? -1 : _attacks.Length;
            }
            else 
            {
                i = System.Array.FindIndex(_attacks, x => x == _slots[SlotInputIndex].Attack);
            }

            if ((i += input) >= _attacks.Length || i <= -1)
            {
                _slots[SlotInputIndex].Attack = null;
            }
            else
            {
                _slots[SlotInputIndex].Attack = _attacks[i];
            }
            //while (true)
            //{
            //    if ((i += input) >= _attacks.Length || i <= -1)
            //    {
            //        _slots[InputIndex].Attack = null;
            //        break;
            //    }
            //    else if(System.Array.FindIndex(_slots, x => x.Attack == _attacks[i]) == -1)
            //    {
            //        _slots[InputIndex].Attack = _attacks[i];
            //        break;
            //    }
            //}


            _lastTimeChange = Time.time;
            _savedInputs = false;
        }
        if (!_savedInputs && Time.time - _lastTimeChange > _TIME_TO_SAVE)
        {
            SaveData();
        }
    }

    public void SaveData()
    {
        new SlotsData(this).Save();
        _savedInputs = true;
    }
    public bool LoadData()
    {
        SlotsData sd;
        if ((sd = SlotsData.Load(SlotsData.GetStringPath)) != null)
        {
            _slots = sd.GetData(this);
            return true;
        }
        return false;
    }

    public IEnumerator<Slot> GetEnumerator()
    {
        return ((IEnumerable<Slot>)_slots).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    [System.Serializable]
    public class SlotsData
    {
        public KeyCode[] keys;
        public int[] AttackIndexes;


        public static string GetStringPath => io.savespath + "inputs of weapons" + io.saveExtension;

        public SlotsData(PlayerControlBattle player)
        {
            keys = new KeyCode[player._slots.Length];
            AttackIndexes = new int[keys.Length];
            for (int i = 0; i < keys.Length; ++i)
            {
                keys[i] = player._slots[i].Key;
                AttackIndexes[i] = System.Array.FindIndex(player._attacks, x => x == player._slots[i].Attack);
            }
        }

        public Slot[] GetData(PlayerControlBattle player)
        {
            Slot[] slots = new Slot[keys.Length];
            for (int i = 0; i < keys.Length; ++i)
            {
                slots[i].Key = keys[i];
                slots[i].Attack = AttackIndexes[i] != -1 ? player._attacks[AttackIndexes[i]] : null;
            }
            return slots;
        }

        public static SlotsData Load(string path)
        {
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);
                if (stream.Length == 0)
                {
                    return null;
                }
                var temp = (SlotsData)formatter.Deserialize(stream);
                stream.Close();
                return temp;
            }
            return null;
        }
        public void Save()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(GetStringPath, FileMode.Create);

            formatter.Serialize(stream, this);
            stream.Close();
        }
    }

}



































