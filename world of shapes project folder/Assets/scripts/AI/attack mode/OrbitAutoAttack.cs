using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitAutoAttack : AttackModeSingleTarget
{
    private OrbitAttack _orbitAttack;

    public float TimeToThrow
    {
        get => _throwTimer.TotalCooldown;
        set => _throwTimer.SetCounter(value);
    }
    [SerializeField]
    private Timer _throwTimer = new Timer();

    protected new void Awake()
    {
        base.Awake();
        _orbitAttack = this.SearchComponent<OrbitAttack>();
    }


    protected void Update()
    {
        if (AICharacter.CheckModeAttack() && (!_orbitAttack.enabled || (TimeToThrow >= 0f && _throwTimer.CheckIfTimePassed)))
        {
            _throwTimer.StartTimer();
            _orbitAttack.Activate(true);
        }
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(OrbitAutoAttack), true), UnityEditor.CanEditMultipleObjects]
    private class OrbitAutoAttackEditor : ExtendedEditor
    {
        protected override void OnInspectorGUIExtend(Object currentTarget)
        {
            var orbitAuto = (OrbitAutoAttack)currentTarget;
            DrawProperties();
            AddNewToList(UnityEditor.EditorGUILayout.FloatField(TidyUpString(nameof(TimeToThrow)), orbitAuto.TimeToThrow), orbitAuto.TimeToThrow);
        }

        protected override void ApplyChanges(Object currentTarget)
        {
            if (_changedList[0].Changed)
            {
                ((OrbitAutoAttack)currentTarget).TimeToThrow = (float)_changedList[0].Value;
            }
        }
    }
#endif

}
