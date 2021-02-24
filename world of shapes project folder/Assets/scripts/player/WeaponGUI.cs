using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Slot = PlayerControlBattle.Slot;

public class WeaponGUI : MonoBehaviour
{

    private Vector2 _unSelectedBackgroundSize;
    private float _unSelectedCooldownwidth;
    private int _selectedSlot = -1;
    
    private PlayerControlBattle _player;

    private Dictionary<Attack, Sprite> _sprites = new Dictionary<Attack, Sprite>();

    protected void Setup()
    {
        WeaponSlotGUI slot = transform.GetChild(0).GetComponent<WeaponSlotGUI>();
        Image image = slot.BackgroundImage;
        image.color = MyColorLib.SetColorsAndA(_player.Team.TeamColor, image.color.a);
        Color invertedColor = _player.Team.TeamColor.InvertColor();
        slot.TextKeyComponent.color = invertedColor;
        slot.TextSlotNumberComponent.color = invertedColor;

        foreach (var attack in _player.GetComponentsInChildren<Attack>())
        {
            _sprites.Add(attack, Sprite.Create(attack.DefaultImage, new Rect(0f, 0f, attack.DefaultImage.width, attack.DefaultImage.height), new Vector2(0.5f, 0.5f)));
        }
    }

    private void LateUpdate()
    {
        if (_player == null)
        {
            _player = FindObjectOfType<PlayerControlBattle>();
            if (_player != null) Setup();
            return;
        }
        SetChildren(_player.Count());
        int i = 0;
        foreach (Slot slot in _player)
        {
            Attack currattack = slot.Attack;
            WeaponSlotGUI slotGUI = transform.GetChild(i).GetComponent<WeaponSlotGUI>();
            if (_player.SlotInputIndex == i && i != _selectedSlot)
            {
                if (_selectedSlot != -1)
                {
                    UnSelectSlot(transform.GetChild(_selectedSlot).GetComponent<WeaponSlotGUI>());
                }
                SelectSlot(slotGUI);
                _selectedSlot = i;
            }
            if (currattack != null)
            {
                if (!slotGUI.WeaponImageComponent.enabled)
                {
                    slotGUI.WeaponImageComponent.enabled = true;
                    slotGUI.CooldownTr.GetComponent<Image>().enabled = true;
                }

                if (slotGUI.WeaponImageComponent.sprite != _sprites[currattack])
                {
                    slotGUI.WeaponImageComponent.sprite = _sprites[currattack];
                }

                slotGUI.CooldownTr.sizeDelta = new Vector2(slotGUI.CooldownTr.sizeDelta.x, 60 * currattack.Cooldown.TimeRemainingRatio);
            }
            else if (slotGUI.WeaponImageComponent.enabled)
            {
                slotGUI.WeaponImageComponent.enabled = false;
                slotGUI.CooldownTr.GetComponent<Image>().enabled = false;
            }
            ++i;
        }
    }



    private void SetChildren(int num)
    {
        if (num == transform.childCount)
        {
            return;
        }
        if (num > transform.childCount)
        {
            for (int i = transform.childCount; i < num; i++)
            {
                BasicLib.MyInstantiate(transform.GetChild(0), transform);
            }
        }
        else
        {
            for (int i = transform.childCount; i > num; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
        int index = 0;
        foreach (Slot slot in _player)
        {
            var slotGUI = transform.GetChild(index).GetComponent<WeaponSlotGUI>();
            slotGUI.TextKeyComponent.text = slot.KeyName;
            slotGUI.TextSlotNumberComponent.text = (index + 1).ToString();
            ++index;
        }
    }

    private void SelectSlot(WeaponSlotGUI slot)
    {
        _unSelectedBackgroundSize = slot.BackgroundTr.sizeDelta;
        slot.BackgroundTr.sizeDelta = _unSelectedBackgroundSize * 1.1f;

        _unSelectedCooldownwidth = slot.CooldownTr.sizeDelta.x;
        slot.CooldownTr.sizeDelta = new Vector2(_unSelectedCooldownwidth * 1.1f, slot.CooldownTr.sizeDelta.y);
    }
    
    private void UnSelectSlot(WeaponSlotGUI slot)
    {
        slot.BackgroundTr.sizeDelta = _unSelectedBackgroundSize;
        slot.CooldownTr.sizeDelta = new Vector2(_unSelectedCooldownwidth, slot.CooldownTr.sizeDelta.y);
    }

}

/*

        keyStyle = new GUIStyle();
        keyStyle.fontStyle = FontStyle.Bold;
        keyStyle.fontSize = 15;
//public int offsettest = 100;
void OnGUI()
{
    /*int tempint = keyStyle.fontSize;
    keyStyle.fontSize = 10;
    GUI.DrawTexture(new Rect(Screen.width / 3, Screen.height / 3 + offsettest, 100, 10), background);
    GUI.Label(new Rect(Screen.width / 3, Screen.height / 3, 0f, 0f), "test", keyStyle);
    keyStyle.fontSize = tempint;*
    if (camerascript.showPlayerInterface && playervars != null && playervars.atins != null)
    {
        int numberofslots = playervars.atins.Length;
        for (int i = 0; i < numberofslots; i++)
        {
            Attack currattack = null;
            bool selectedSlot = playervars.inputIndex == i;
            if (playervars.atins[i].attackIndex != -1)
            {
                //Debug.Log(playervars.atins[i].attackIndex);
                currattack = playervars.attacks[playervars.atins[i].attackIndex];
            }
            float time = 0f;
            if (currattack != null)
            {
                time = currattack.cooldown.timeRemaining;
            }
            Rect backgroundrect = new Rect(Screen.width / 2f + (i + 0.5f - numberofslots / 2f) * (SlotWidth + 20), 0, SlotWidth, SlotHeight);
            Rect attackimagerect = new Rect(backgroundrect.x, backgroundrect.y + backgroundrect.height * 0.5f, backgroundrect.width * 0.8f, backgroundrect.height * 0.8f);
            if (selectedSlot)
            {
                attackimagerect.y += backgroundrect.height * 0.1f;
                backgroundrect.height *= 1.2f;
                attackimagerect.width *= 1.1f;
                attackimagerect.height *= 1.1f;
            }

            GUI.DrawTexture(backgroundrect.rectangleCentre(0f, -1f), background); //background


            keyStyle.alignment = TextAnchor.UpperLeft;
            GUI.Label(backgroundrect.rectangleCentre(-0.1f, -1f), (i + 1).ToString(), keyStyle);  //slot number

            string keystring = playervars.atins[i].actioninput.ToString();
            keyStyle.alignment = TextAnchor.UpperCenter;
            GUI.DrawTexture(new Rect(backgroundrect.position.x, backgroundrect.position.y + backgroundrect.height, backgroundrect.size.x, keyStyle.fontSize /*keyStyle.CalcSize(new GUIContent(keystring)).y*).rectangleCentre(0f, -1f), background); //added background for inputkey
            GUI.Label(new Rect(new Vector2(backgroundrect.x, backgroundrect.y + backgroundrect.height), Vector2.zero), keystring, keyStyle); //input key

            if (currattack != null)
            {
                GUI.DrawTexture(attackimagerect.rectangleCentre(0f, 0f), currattack.defaultImage); //weapon image
                if (time > 0f)
                {
                    Rect newrectemp = new Rect(backgroundrect.x, backgroundrect.y + backgroundrect.height, backgroundrect.width, backgroundrect.height * currattack.cooldown.timeRemainingRatio);
                    GUI.DrawTexture(newrectemp.rectangleCentre(0f, 1f), blackbox); //darkened slot due to cooldown
                    //Debug.Log(rectangleCentre(newrectemp, 0f, 1f).y+ rectangleCentre(newrectemp, 0f, 1f).y);
                    keyStyle.alignment = TextAnchor.LowerCenter;
                    GUI.Label(new Rect(new Vector2(backgroundrect.x, backgroundrect.y + backgroundrect.height), Vector2.zero), string.Format("{0:00.00}", time), keyStyle); //cooldown
                }
            }
        }

        GUI.Label(new Rect(new Vector2(Screen.width >> 2, Screen.height >> 2), Vector2.zero), "YOUR HEALTH: " + playervars.getvars<lifescript>().life.ToString(), keyStyle);
    }
}
*/

/*  if (playervars.inputIndex % numberofslots == i)
  {
      GUI.DrawTexture(new Rect(Screen.width / 2f + (i - numberofslots / 2f) * (mw + 20), 10, mw* 1.2f, mh* 1.2f), background);
      GUI.Label(new Rect(Screen.width / 2f + (i - numberofslots / 2f) * (mw + 20), 30, mw* 1.2f, mh* 1.2f), KeyToString(playervars.atins[i].actioninput.key), keyStyle);
  }
  else
  {
      GUI.DrawTexture(new Rect(Screen.width / 2f + (i - 2.5f) * (mw + 20), 10, mw, mh), background);
      GUI.Label(new Rect(Screen.width / 2f + (i - 2.5f) * (mw + 20), 30, mw, mh), KeyToString(playervars.atins[i].actioninput.key), keyStyle);
  }*/
