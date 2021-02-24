using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventObjectKillMobs : EventObjectBaseClass
{
    [SerializeField]
    private BaseCharacterControl[] _mobs = new BaseCharacterControl[0];

    protected override bool CheckToTrigger()
    {
        for (int i = 0; i < _mobs.Length; i++)
        {
            if (!_mobs[i].IsDead()) return false;
        }
        return true;
    }

    public void AddMobs(params BaseCharacterControl[] newMobs)
    {
        MyLib.ExpandArray(ref _mobs, newMobs);
    }

    public void AddMobs(params GameObject[] newMobs)
    {
        BaseCharacterControl[] characters = new BaseCharacterControl[newMobs.Length];
        for(int i = 0; i < newMobs.Length; ++i)
        {
            characters[i] = newMobs[i].GetCharacter();
        }
        AddMobs(characters);
    }
    public void AddMobs(params Component[] newMobs)
    {
        BaseCharacterControl[] characters = new BaseCharacterControl[newMobs.Length];
        for (int i = 0; i < newMobs.Length; ++i)
        {
            characters[i] = newMobs[i].GetCharacter();
        }
        AddMobs(characters);
    }
}
