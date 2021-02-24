using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAround_ChillMode : ChillModeClass
{

    [SerializeField]
    private Transform _centreTransform = null;
    
    [SerializeField]
    private bool _setFixedCentrePosition = false;
    [SerializeField]
    private Vector3 _stationaryCentrePosition = default;

    private bool _choseSationaryCentreAfterTransformNull = false;
    public Vector3 CentrePosition
    {
        get
        {
            if (_centreTransform != null && _centreTransform.gameObject.activeInHierarchy)
            {
                _choseSationaryCentreAfterTransformNull = false;
                return _centreTransform.position;
            }
            else if(!_setFixedCentrePosition && !_choseSationaryCentreAfterTransformNull)
            {
                _choseSationaryCentreAfterTransformNull = true;
                return _stationaryCentrePosition = AICharacter.Position;
            }
            else
            {
                return _stationaryCentrePosition;
            }
        }
        set
        {
            _setFixedCentrePosition = true;
            _stationaryCentrePosition = value;
        }
    }

    private const float _TIME_TO_RESET = 7f;
    private Timer _lastCheckTime = new Timer(_TIME_TO_RESET);
    private Vector3 _lastTargetPosition;

    private IState _startingState;

    protected new void Awake()
    {
        base.Awake();

        _SM = new StateMachine();

        NearTargetState s1 = new NearTargetState(_SM, this);
        NearStillTargetState s2 = new NearStillTargetState(_SM, this);
        FarFromTargetState s3 = new FarFromTargetState(_SM, this);

        s1.SetTargetStates(s3, s2);
        s2.SetTargetStates(s1);
        s3.SetTargetStates(s1);

        _SM.SetStartingState(_startingState = s3);
    }

    public override void LogicalUpdate()
    {
        base.LogicalUpdate();
        _lastTargetPosition = CentrePosition;
        if (_lastCheckTime.CheckIfTimePassed) _SM.ChangeState(_startingState);
    }

    public void ClearStationaryPosition()
    {
        _choseSationaryCentreAfterTransformNull = false;
        _setFixedCentrePosition = false;
    }

    public static Vector3 FollowAround(in Vector3 target, in Vector3 currentPosition, float maxDistanceToWalk)
    {
        Vector3 target_sub_pos = target - currentPosition;
        float th = MyRandomLib.ExpRandom(((3f * 3f * 3f * 3f) - target_sub_pos.sqrMagnitude.Sq()) / 2254.32f, MyMathlib.PI);
        float r = MyRandomLib.ExpRandom(-th / 1.13415f, maxDistanceToWalk);
        return currentPosition + r * MyMathlib.RotateRadians(target_sub_pos.normalized, MyRandomLib.Rand50 ? th : -th);
    }

    #region SetCentreTarget
    public Transform SetCentreTarget(MoveComponent centreMove)
    {
        if (centreMove == null) return _centreTransform = null;
        return _centreTransform = centreMove.transform;
    }
    public Transform SetCentreTarget(BaseCharacterControl centreCharacter)
    {
        if (centreCharacter == null) return _centreTransform = null;
        return _centreTransform = centreCharacter.MoveComponent.transform;
    }
    public Transform SetCentreTarget(Transform centreTransform, bool findMoveComponent = false)
    {
        if (centreTransform == null) return _centreTransform = null;
        if (!findMoveComponent) return _centreTransform = centreTransform;
        var move = centreTransform.SearchComponent<MoveComponent>();
        return _centreTransform = move == null ? move.transform : centreTransform;
    }
    public Transform SetCentreTarget<T>(T centreComponent, bool findMoveComponent = false) where T : Component
    {
        return SetCentreTarget(centreComponent.transform, findMoveComponent);
    }
    public Transform SetCentreTarget(GameObject centreGameObject, bool findMoveComponent = false)
    {
        return SetCentreTarget(centreGameObject.transform, findMoveComponent);
    }
    #endregion

    protected abstract class AIChillModeMoverAroundState : AIModeState
    {
        protected readonly MoveAround_ChillMode _chillMode;

        protected AIChillModeMoverAroundState(StateMachine AISM, MoveAround_ChillMode chillMode) : base(AISM)
        {
            _chillMode = chillMode;
        }

    }

    protected class NearStillTargetState : AIChillModeMoverAroundState
    {
        protected NearTargetState _nearTargetState;

        public NearStillTargetState(StateMachine AISM, MoveAround_ChillMode vars) : base(AISM, vars)
        {
        }


        public void SetTargetStates(NearTargetState nearTargetState)
        {
            _nearTargetState = nearTargetState;
        }

        public override void OnStateEnter()
        {

        }

        public override void LogicalUpdate()
        {
            AI aiCharacter = _chillMode.AICharacter;
            Vector3 TemptargetPosition = _chillMode.CentrePosition;

            if (!aiCharacter.MoveComponent.HasPath)
            {
                aiCharacter.MoveComponent.StartFromScratchNewEndpos(FollowAround(TemptargetPosition, aiCharacter.MoveComponent.Position, 3f));
                aiCharacter.MoveComponent.SetRatioToBaseSpeed(1 / 5f);
                _chillMode._lastCheckTime.StartTimer();
            }
            if (TemptargetPosition != _chillMode._lastTargetPosition) //if target is not still anymore
            {
                _SM.ChangeState(_nearTargetState);
            }
        }

        public override void OnStateExit()
        {

        }
    }

    protected class NearTargetState : AIChillModeMoverAroundState
    {

        protected FarFromTargetState _farFromTargetState;
        protected NearStillTargetState _nearTargetStillState;

        public NearTargetState(StateMachine AISM, MoveAround_ChillMode vars) : base(AISM, vars)
        {
        }

        public void SetTargetStates(FarFromTargetState farFromTargetState, NearStillTargetState nearTargetStillState)
        {
            _farFromTargetState = farFromTargetState;
            _nearTargetStillState = nearTargetStillState;
        }


        public override void OnStateEnter()
        {

        }

        public override void LogicalUpdate()
        {
            AI aiCharacter = _chillMode.AICharacter;
            Vector3 TemptargetPosition = _chillMode.CentrePosition;
            Vector3 target_sub_pos = TemptargetPosition - aiCharacter.MoveComponent.Position;
            float distsqr = target_sub_pos.sqrMagnitude;

            if (!aiCharacter.MoveComponent.HasPath) //if doesn't have a path
            {
                aiCharacter.MoveComponent.StartFromScratchNewEndpos(FollowAround(TemptargetPosition, aiCharacter.MoveComponent.Position, 10f));
                if (distsqr > 7f * 7f)
                {
                    aiCharacter.MoveComponent.SetRatioToBaseSpeed(1.6f);
                }
                else
                {
                    aiCharacter.MoveComponent.SetBaseSpeed();
                }
                _chillMode._lastCheckTime.StartTimer();
            }
            if (distsqr > 12f * 12f) //if target is far
            {
                _SM.ChangeState(_farFromTargetState);
            }
            else if (TemptargetPosition == _chillMode._lastTargetPosition)
            {
                _SM.ChangeState(_nearTargetStillState);
            }
        }

        public override void OnStateExit()
        {

        }

    }

    protected class FarFromTargetState : AIChillModeMoverAroundState
    {

        private NearTargetState _nearTargetState;

        public FarFromTargetState(StateMachine AISM, MoveAround_ChillMode vars) : base(AISM, vars)
        {
        }

        public void SetTargetStates(NearTargetState nearTargetState)
        {
            _nearTargetState = nearTargetState;
        }


        public override void OnStateEnter()
        {
            _chillMode.AICharacter.MoveComponent.ClearPath();
        }

        public override void LogicalUpdate()
        {
            AI aiCharacter = _chillMode.AICharacter;
            Vector3 TemptargetPosition = _chillMode.CentrePosition;
            float distSq = (TemptargetPosition - aiCharacter.MoveComponent.Position).sqrMagnitude;
            if (!aiCharacter.MoveComponent.HasPath)
            {
                aiCharacter.MoveComponent.StartFromScratchNewEndpos(TemptargetPosition + MyMathlib.PolarVectorRad(10f * Random.value, MyMathlib.TAU * Random.value));
                aiCharacter.MoveComponent.SetRatioToBaseSpeed(2f);
                _chillMode._lastCheckTime.StartTimer();
            }
            if (distSq < 64f)
            {
                _SM.ChangeState(_nearTargetState);
            }
        }

        public override void OnStateExit()
        {

        }
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(MoveAround_ChillMode), true), UnityEditor.CanEditMultipleObjects]
    private class MoveAroundChillModeEditor : ExtendedEditor
    {
        private MoveAround_ChillMode _moveAroundChillMode;

        private void OnEnable()
        {
            _moveAroundChillMode = (MoveAround_ChillMode)target;
        }

        protected override void OnInspectorGUIExtend(Object currentTarget)
        {
            DrawPropertiesExcept(nameof(_setFixedCentrePosition), nameof(_stationaryCentrePosition), nameof(_centreTransform));
            DrawCentreTarget();
            DrawCentrePositionAndOption();
        }

        private void DrawCentreTarget()
        {
            var transformTemp = (Transform)UnityEditor.EditorGUILayout.ObjectField(TidyUpString(nameof(_centreTransform)), _moveAroundChillMode._centreTransform, typeof(Transform), true);
            if (transformTemp != _moveAroundChillMode._centreTransform)
            {
                if (transformTemp != null)
                {
                    var characterTemp = transformTemp.GetCharacter();
                    if (characterTemp != null)
                    {
                        _changedList.Add(new ValueAndChanged(characterTemp.MoveComponent.transform, true));
                    }
                    else
                    {
                        _changedList.Add(new ValueAndChanged(transformTemp, true));
                    }
                }
                else
                {
                    _changedList.Add(new ValueAndChanged(null, true));
                }
            }
            else
            {
                _changedList.Add(new ValueAndChanged(null, false));
            }
        }

        private static bool _buttonPressed = false;
        private void DrawCentrePositionAndOption()
        {
            var boolProperty = serializedObject.FindProperty(nameof(_setFixedCentrePosition));
            UnityEditor.EditorGUILayout.PropertyField(boolProperty);
            if (boolProperty.boolValue)
            {
                DrawProperty(nameof(_stationaryCentrePosition));
                _buttonPressed = false;
                if (GUILayout.Button("Set Current Position"))
                {
                    _buttonPressed = true;
                }
            }
        }

        protected override void ApplyChanges(Object currentTarget)
        {
            if (_changedList[0].Changed)
            {
                ((MoveAround_ChillMode)currentTarget)._centreTransform = (Transform)_changedList[0].Value;
            }
            if (_buttonPressed)
            {
                var MoveArroundMode = (MoveAround_ChillMode)currentTarget;
                MoveArroundMode._stationaryCentrePosition = MoveArroundMode.SearchComponent<AI>().Position;
            }
        }

    }
#endif

}
