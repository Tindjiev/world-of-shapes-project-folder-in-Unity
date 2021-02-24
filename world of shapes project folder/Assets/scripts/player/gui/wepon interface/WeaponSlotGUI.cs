using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponSlotGUI : MonoBehaviour
{
    public RectTransform BackgroundTr;
    public RectTransform CooldownTr;
    public RectTransform WeaponImageTr;
    public RectTransform TextKeyTr;
    public RectTransform TextSlotNumberTr;

    public Image BackgroundImage { get; private set; }
    public Image WeaponImageComponent { get; private set; }

    public TextMeshProUGUI TextKeyComponent { get; private set; }
    public TextMeshProUGUI TextSlotNumberComponent { get; private set; }

    private void Awake()
    {
        BackgroundImage = BackgroundTr.GetComponent<Image>();
        WeaponImageComponent = WeaponImageTr.GetComponent<Image>();
        TextKeyComponent = TextKeyTr.GetComponent<TextMeshProUGUI>();
        TextSlotNumberComponent = TextSlotNumberTr.GetComponent<TextMeshProUGUI>();
    }
}
