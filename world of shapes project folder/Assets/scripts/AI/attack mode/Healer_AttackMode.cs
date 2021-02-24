using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer_AttackMode : AttackModeClass
{
    private BaseCharacterControl _currentTargetField;
    private BaseCharacterControl _currentTarget
    {
        get => _currentTargetField;
        set => AICharacter.TargetTransform = (_currentTargetField = value) != null ? value.MoveComponent.transform : null;
    }
    private Vector3 _semiLastTargetPos;

    private const float _TIME_TO_CHECK_HEAL_CHANGE_AGAIN = 0.35f;
    private Coroutine _coroutineCheckToChangeHealTarget;

    protected new void Awake()
    {
        base.Awake();
        if (AICharacter.AggroMode != AggroMode.Handpicked)
        {
            AICharacter.SetAggroMode(AggroMode.AttackEverything);
        }
    }

    protected void Start()
    {
        Currentattack = this.SearchComponent<BasicHeal>();
    }

    public override void OnStateEnter()
    {
        _coroutineCheckToChangeHealTarget = this.DoActionAndRepeat(() => _currentTarget = CheckToChangeTarget(), _TIME_TO_CHECK_HEAL_CHANGE_AGAIN);
        AICharacter.MoveComponent.SetBaseSpeed();
    }

    public override void LogicalUpdate()
    {
        if (_currentTarget == null) return;
        Vector3 targetPos = _currentTarget.Position;
        Currentattack.Activate(true);

        if ((targetPos - AICharacter.Position).sqrMagnitude < Currentattack.Reach.Sq() * (0.5f * 0.5f))
        {
            AICharacter.MoveComponent.ClearPath();
        }
        else if (!AICharacter.MoveComponent.HasPath || (targetPos - _semiLastTargetPos).sqrMagnitude > AICharacter.MoveComponent.CurrentSpeed.Sq())
        {
            _semiLastTargetPos = targetPos;
            AICharacter.MoveComponent.StartFromScratchNewEndpos(targetPos);
        }
    }

    public override void OnStateExit()
    {
        StopCoroutine(_coroutineCheckToChangeHealTarget);
    }

    public override bool CheckToActivate()
    {
        foreach (var character in (IEnumerable<BaseCharacterControl>)AICharacter.Vision)
        {
            if (character.Life.MissingHealth && AICharacter.IsAlliedWith(character)) return true;
        }
        return false;
    }

    public override bool CheckToChange()
    {
        if(_currentTarget.IsDead() || !_currentTarget.Life.MissingHealth)
        {
            _currentTarget = CheckToChangeTarget();
            if (_currentTarget == null) return true;
        }
        return false;
    }

    private BaseCharacterControl CheckToChangeTarget()
    {
        float minHealth = float.MaxValue;
        BaseCharacterControl characterToHeal = null;
        foreach (var character in (IEnumerable<BaseCharacterControl>)AICharacter.Vision)
        {
            if (character.Life.MissingHealth && minHealth > character.Life.Health && AICharacter.IsAlliedWith(character))
            {
                minHealth = character.Life.Health;
                characterToHeal = character;
            }
        }
        return characterToHeal;
    }

}
