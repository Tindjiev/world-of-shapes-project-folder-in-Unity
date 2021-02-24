using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBaseControl : BaseCharacterControl
{

    protected MouseSelect _mouseSelect;
    private InputWASD _wasdInputs;

    [SerializeField]
    private InputStruct _inputJump;



    private static bool _shouldBeActive = true;
    public static bool ShouldBeActive
    {
        get => _shouldBeActive;
        set
        {
            if (value)
            {
                _shouldBeActive = true;
                if (ControlBase.PlayerGameObject != null)
                {
                    ControlBase.PlayerGameObject.SearchComponent<PlayerBaseControl>().enabled = true;
                }
                var temp = FindObjectOfType<WeaponGUI>(true);
                if (temp != null) temp.gameObject.SetActive(true);
            }
            else
            {
                _shouldBeActive = false;
                var temp = FindObjectOfType<WeaponGUI>(true);
                if (temp != null) temp.gameObject.SetActive(false);
            }
        }
    }



    protected new void Awake()
    {
        base.Awake();
        ShouldBeActive = true;
        _mouseSelect = MouseComponent.Mouse.GetComponent<MouseSelect>();
    }

    protected void OnDestroy()
    {

    }

    protected new void Start()
    {
        base.Start();
        if (_wasdInputs == null)
        {
            if ((_wasdInputs = this.SearchComponent<InputWASD>()) == null)
            {
                _wasdInputs = gameObject.AddComponent<InputWASD>();
            }
        }
    }

    protected bool Update()
    {
        if (!ShouldBeActive)
        {
            enabled = false;
            return false;
        }
        TargetTransform = _mouseSelect.GetCollidedWith;
        CheckTojump();
        if (!_wasdInputs.enabled) _wasdInputs.enabled = true;
        return true;
    }

    private void LateUpdate()
    {
        TargetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        TargetPosition.z = 0f;
        DirectionVector = (TargetPosition - MoveComponent.Position).normalized;
    }

    protected void OnDisable()
    {
        if (!enabled && _wasdInputs != null)
        {
            _wasdInputs.enabled = false;
        }
        MoveComponent.ClearPath();
    }


    private bool CheckTojump()
    {
        if (_inputJump.CheckInput() && MoveComponent.Velocity != default)
        {
            MoveComponent.Dash(MoveComponent.Velocity.normalized);
            return true;
        }
        return false;
    }

    public void PlayerDeath()
    {
        SceneChanger.SwitchToSetScene();
        gameObject.SetActive(false);
    }
}



































