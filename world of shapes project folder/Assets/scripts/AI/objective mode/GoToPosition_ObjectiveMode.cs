using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToPosition_ObjectiveMode : ObjectiveModeClass
{

    [SerializeField]
    private Vector3 _targetPosition;
    
    protected new void Awake()
    {
        base.Awake();
    }

    public override void LogicalUpdate()
    {
        if (!AICharacter.MoveComponent.HasPath)
        {
        }
        /*
        if (target == null)
        {
            Room currentroom = rules.floor.GetClosestRoomFromPosition(Aivars.Position);
            if (currentroom == targetRoom)
            {
                Aivars.StationaryTargetosition_ChillMode = targetRoom.Position;
                Aivars.SetModeChill();
                return;
            }
            if (!Aivars.MoveComponent.HasPath)
            {
                searchpath(currentroom, targetRoom);
            }
        }
        else
        {
            Room currentRoom = rules.floor.GetClosestRoomFromPosition(Aivars.Position);
            if (currentRoom == _targetInRoom)
            {
                Aivars.Target_AttackMode = target.getvars<move>();
                Aivars.SetModeAttack();
                return;
            }
            var targetInRoomTemp = rules.floor.GetClosestRoomFromPosition(target.position);
            if (targetInRoomTemp != _targetInRoom)
            {
                _targetInRoom = targetInRoomTemp;
                searchpath(currentRoom, _targetInRoom);
            }
            else if (!Aivars.MoveComponent.HasPath)
            {
                searchpath(currentRoom, _targetInRoom);
            }
        }
        */
    }


    public override bool CheckToActivate()
    {
        throw new System.NotImplementedException();
    }

    public override bool CheckToChange()
    {
        throw new System.NotImplementedException();
    }

}
