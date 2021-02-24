using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputWASD : MonoBehaviour
{
    [SerializeField]
    private PlayerMovementInputsSO _inputsSO;

    private MoveComponent _moveComponent;
    private bool _gotInputLastFrame = false, _sprintedLastFrame = false;

    public float SprintMultiplier = 1.8f;


    protected void Start()
    {
        _moveComponent = this.SearchComponent<MoveComponent>();
    }


    private void LateUpdate()
    {
        Vector3 inputPosition = GetInputWASD();
        if (inputPosition != _moveComponent.Position)
        {
            _moveComponent.ClearTargetPositionsOnly();
            _moveComponent.AddNewEndpos(inputPosition);
            _gotInputLastFrame = true;
        }
        else if (_gotInputLastFrame)
        {
            _moveComponent.ClearPath();
            _gotInputLastFrame = false;
        }
    }


    private Vector3 GetInputWASD()
    {
        if (!_moveComponent.HasFreeMovement)
        {
            return _moveComponent.Position;
        }
        Vector3 endpos = _moveComponent.Position;
        IfSprint();
        if (_inputsSO.InputUp.CheckInput())
        {
            endpos.y += 1f;
        }
        if (_inputsSO.InputDown.CheckInput())
        {
            endpos.y -= 1f;
        }
        if (_inputsSO.InputRight.CheckInput())
        {
            endpos.x += 1f;
        }
        if (_inputsSO.InputLeft.CheckInput())
        {
            endpos.x -= 1f;
        }
        return endpos;
    }

    private void IfSprint()
    {
        if (SprintMultiplier != 1f && _inputsSO.InputSprint.CheckInput())
        {
            _moveComponent.SetRatioToBaseSpeed(SprintMultiplier);
            _sprintedLastFrame = true;
        }
        else
        {
            if (_sprintedLastFrame)
            {
                _moveComponent.SetBaseSpeed();
            }
            _sprintedLastFrame = false;
        }
    }

}
