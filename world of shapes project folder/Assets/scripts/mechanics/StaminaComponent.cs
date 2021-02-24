using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaComponent : MonoBehaviour
{
    public EntityBase Holder { get; private set; }

    private const float _MAX_STAMINA = 100f;
    private float _staminaLevel = _MAX_STAMINA;
    public float Staminalevel
    {
        get => _staminaLevel > _MAX_STAMINA ? _staminaLevel = _MAX_STAMINA : _staminaLevel;
        private set => _staminaLevel = value;
    }

    private GUIStyle _staminaTextStyle;

    public float SecondsToFull => 10f + 1.1f * Holder.MoveComponent.CurrentSpeed;
    private float _increasePerFrame => Time.deltaTime * _MAX_STAMINA / SecondsToFull;

    private void Awake()
    {
        Holder = this.SearchComponent<EntityBase>();
    }

    protected void Start()
    {
        Staminalevel = _MAX_STAMINA;

        _staminaTextStyle = new GUIStyle();
        _staminaTextStyle.alignment = TextAnchor.UpperRight;
        _staminaTextStyle.fontSize = Screen.height * 5 / 100;
        _staminaTextStyle.normal.textColor = new Color(0.65f, 0f, 0.65f);
    }
	

	private void LateUpdate ()
    {
        if (SecondsToFull <= 0f)
        {
            _staminaLevel = _MAX_STAMINA;
        }
        else
        {
            _staminaLevel += _increasePerFrame;
        }
	}

    public void AddStamina(float toAdd) => _staminaLevel += toAdd;


    //private void OnGUI()
    //{
    //    if (Staminalevel > 0f)
    //    {
    //        GUI.Label(new Rect(Screen.width, Screen.height / 2f, 0, 0), string.Format("stamina: {0:0.} ", Staminalevel), _staminaTextStyle);
    //    }
    //    else
    //    {
    //        GUI.Label(new Rect(Screen.width, Screen.height / 2f, 0, 0), "stamina: 0", _staminaTextStyle);
    //    }
    //}



}
