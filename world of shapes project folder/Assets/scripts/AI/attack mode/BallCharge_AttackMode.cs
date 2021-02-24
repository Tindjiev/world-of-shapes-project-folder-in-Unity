using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCharge_AttackMode : AttackFromDistance_AttackMode
{
    private BallChargeAttack _ballAttack;

    public float MaxSizeRatioToShoot = 1f;
    public float MinLifeToshoot = 2.5f;

    protected new void Awake()
    {
        base.Awake();
        _ballAttack = this.SearchComponent<BallChargeAttack>();
    }

    public override void LogicalUpdate()
    {
        base.LogicalUpdate();
        if (!_ballAttack.Thrown &&
                (_ballAttack.transform.localScale.x >= _ballAttack.MaxSize * MaxSizeRatioToShoot || AICharacter.Life.Health < MinLifeToshoot))
        {
            _ballAttack.Activate(false);
        }
    }

    public override void SetDirectionOfCharacter()
    {
        if (Target.MoveComponent.Velocity == default)
        {
            base.SetDirectionOfCharacter();
        }
        else
        {
            AICharacter.DirectionVector = AICharacter.PredictDirection(Target.Position, _ballAttack.Speed, Target.MoveComponent.Velocity);
        }
    }

}
