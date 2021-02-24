using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveComponent : MonoBehaviour
{
    public static bool ShowMovements
    {
        get => LocalPathFinder.ShowMovements;
        set => LocalPathFinder.ShowMovements = value;
    }

    public EntityBase Entity { get; private set; }
    private StaminaComponent _stamina;

    public Rigidbody2D ImageBody { get; private set; }

    [field: SerializeField]
    public float CurrentSpeed { get; private set; }

    [field: SerializeField]
    public bool HasFreeMovement { get; private set; } = true;
    public bool IsDashing { get; private set; } = false;

    //path variables, using LinkedList because with Queues you can't peek or remove the last element of the Queue
    private LinkedList<Vector3> _path;
    private LinkedList<Vector3> _targetPositions;
    private LinkedList<GameObject> _pathPoints;

    private int _n;
    private Vector3 _dashDirection;

    private float _forceMagnitudeDash
    {
        get
        {
            int nmax = (int)(_tempDashTime / Time.fixedDeltaTime);
            return 2f * _tempDashDistance / (nmax * (nmax - 1) * Time.fixedDeltaTime * Time.fixedDeltaTime); //|a|=2*d/(tend*(tend-Tframe)), where tend=nmax*Tframe  (t=nT)
        }
    }

    private float _tempDashDistance = 0f;
    private float _tempDashTime = 0f;

    public bool Stationary
    {
        get => _isStationary;
        set
        {
            _isStationary = value;
            enabled = !value;
            ImageBody.gameObject.SetActive(!value);
        }
    }

    [SerializeField, ReadOnlyOnInspectorDuringPlay]
    private bool _isStationary = false;
    public float BaseSpeed = 5f;
    public float DashDistance = 5f;
    public float DashTime = 0.25f;

    private Rigidbody2D _rigidBody;

    public Vector3 Position => transform.position;


    public Vector3 Velocity => _rigidBody.velocity;

    private StateMachine _SM;
    private PushedState _pushedState;
    private DashState _dashState;

    protected void Awake()
    {
        Entity = this.SearchComponent<EntityBase>();
        _stamina = this.SearchComponent<StaminaComponent>();
        _rigidBody = GetComponent<Rigidbody2D>();

        _path = new LinkedList<Vector3>();
        _targetPositions = new LinkedList<Vector3>();
        _pathPoints = new LinkedList<GameObject>();

        CurrentSpeed = BaseSpeed;

        _SM = new StateMachine();

        FreeMoveState s = new FreeMoveState(_SM, this);
        (_pushedState = new PushedState(_SM, this)).SetTargetStates(s);
        (_dashState = new DashState(_SM, this)).SetTargetStates(s);

        _SM.SetStartingState(s);
    }

    private void Start()
    {
        ImageBody = CreateImageBody();
        Stationary = _isStationary;
    }

    private void FixedUpdate()
    {
        _SM.LogicalFixedUpdate();
    }

    protected void OnEnable()
    {
        _SM.Initialize();
    }

    protected void OnDisable()
    {
        _SM.ClearState();
    }

    public void Push(in Vector3 x)
    {      // v += b*x - ( v dot (x/|x|) )*x/|x| , removing the sunistosa of velocity in the axis of vector x
        if (Stationary) return;
        _SM.ChangeState(_pushedState);
        float xMagnitude = x.magnitude;
        _rigidBody.AddForce((_rigidBody.drag * xMagnitude + 1.85f) / xMagnitude * x - Vector3.Dot(Velocity, x) / x.sqrMagnitude * x, ForceMode2D.Impulse);
    }
            //sunistosa is removed so if the body is going opposite of the push it doesn't cancel out, but also retain other movements in other directions
    public void Push(float magnitude, Vector3 direction)
    {      // v += b*magnitude*x - ( v dot direction )*direction , removinge the sunistosa of velocity in the axis of direction
        if (Stationary) return;
#if UNITY_EDITOR
        float temp = direction.sqrMagnitude;
        if (temp <= 0.999f || temp >= 1.001f)
        {
            Debug.Log("vector not exactly unit " + direction.magnitude, this);
            Debug.Break();
            direction.Normalize();
        }
#endif
        _SM.ChangeState(_pushedState);
        _rigidBody.AddForce((_rigidBody.drag * magnitude + 1.85f - Vector3.Dot(Velocity, direction)) * direction, ForceMode2D.Impulse);
    }

    public void Dash(in Vector3 direction)
    {
        if (Stationary) return;
        if (HasFreeMovement && (_stamina == null || _stamina.Staminalevel > 10f) && !IsDashing)
        {
            IsDashing = true;
            _rigidBody.velocity = default;
            if (_stamina != null) _stamina.AddStamina(-40f);
            SetUpDashTemps(direction, DashDistance, DashTime);
            _SM.ChangeState(_dashState);
        }
    }

    public void Dash(in Vector3 direction, float distance, float time)
    {
        if (Stationary) return;
        if (HasFreeMovement && (_stamina == null || _stamina.Staminalevel > 10f) && !IsDashing)
        {
            IsDashing = true;
            _rigidBody.velocity = default;
            if (_stamina != null) _stamina.AddStamina(-40f);
            SetUpDashTemps(direction, distance, time);
            _SM.ChangeState(_dashState);
        }
    }

    public void Dash(in Vector3 direction, float distance, float time, bool IgnoreStamina)
    {
        if (Stationary) return;
        if (HasFreeMovement && (IgnoreStamina || _stamina == null || _stamina.Staminalevel > 10f) && !IsDashing)
        {
            IsDashing = true;
            _rigidBody.velocity = default;
            if (!IgnoreStamina && _stamina != null) _stamina.AddStamina(-40f);
            SetUpDashTemps(direction, distance, time);
            _SM.ChangeState(_dashState);
        }
    }

    private void SetUpDashTemps(in Vector3 direction, float distance, float time)
    {
        _tempDashDistance = distance;
        _tempDashTime = time;
        _dashDirection = direction;
        _n = 1;
    }

    private bool FollowPath(in Vector3 nextpath)
    {
        if ((transform.position - nextpath).sqrMagnitude < (CurrentSpeed * Time.fixedDeltaTime * 1.5f).Sq())
        {
            _rigidBody.MovePosition(nextpath);
            return false;
        }
        _rigidBody.velocity = Followdt(nextpath, transform.position, CurrentSpeed);
        return true;
    }

    
    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void StartFromScratchNewEndpos(in Vector3 NewtargetPosition)
    {
        ClearPath();
        AddNewEndpos(NewtargetPosition);
    }

    #region AddNewEndPos
    public bool AddNewEndpos(in Vector3 NewTargetPosition)
    {
        var floor = Rules.Floor;
        if (floor == null)
        {
            _targetPositions.AddLast(NewTargetPosition);
            return true;
        }
        var currentRoom = floor.GetClosestRoomFromPosition(_targetPositions.Count != 0 ? _targetPositions.Last.Value : Position);
        var targetRoom = floor.GetClosestRoomFromPosition(NewTargetPosition);
#if UNITY_EDITOR
        if (targetRoom == null || currentRoom == null)
        {
            Debug.Log("target room or current room null " + targetRoom + currentRoom, this);
            return false;
        }
#endif
        if (currentRoom == targetRoom)
        {
            _targetPositions.AddLast(NewTargetPosition);
            return true;
        }
        else if (currentRoom.IsNeighboringWithOpenDoor(targetRoom))
        {
            FindPathBetweenNeighboringRooms(currentRoom, targetRoom, NewTargetPosition);
            return true;
        }
        return FindAndSetPathBetweenRooms(floor, currentRoom, targetRoom, NewTargetPosition);
    }

    public void AddNewEndpos(IEnumerable<Vector3> NewtargetPositions)
    {
        foreach (var position in NewtargetPositions)
        {
            AddNewEndpos(position);
        }
    }
    public void AddNewEndpos(params Vector3[] targetPositions) => AddNewEndpos((IEnumerable<Vector3>)targetPositions);
    #endregion

    #region PathBewteenRooms

    private void FindPathBetweenNeighboringRooms(Room currentRoom, Room targetRoom, in Vector3 targetPosition)
    {
        Vector3 currentPosition = Position;
        if (currentRoom[Room.Side.North] == targetRoom || currentRoom[Room.Side.South] == targetRoom)
        {
            var roomsPosX = currentRoom.Position.x;
            var currentPosX = currentPosition.x - roomsPosX;
            var targetPosX = targetPosition.x - roomsPosX;
            if (Mathf.Abs(currentPosX) > Room.DoorWidth / 2f &&
                    Mathf.Abs(targetPosX) > Room.DoorWidth / 2f &&
                    Mathf.Sign(currentPosX) == Mathf.Sign(targetPosX))
            {
                _targetPositions.AddLast(
                    new Vector3(currentPosX > 0f ? 0.45f : -0.45f * Room.DoorWidth + roomsPosX, (currentRoom.Position.y + targetRoom.Position.y) / 2f));
            }
        }
        else
        {
            var roomsPosY = currentRoom.Position.y;
            var currentPosY = currentPosition.y - roomsPosY;
            var targetPosY = targetPosition.y - roomsPosY;
            if (Mathf.Abs(currentPosY) > Room.DoorWidth / 2f &&
                    Mathf.Abs(targetPosY) > Room.DoorWidth / 2f &&
                    Mathf.Sign(currentPosY) == Mathf.Sign(targetPosY))
            {
                _targetPositions.AddLast(
                    new Vector3((currentRoom.Position.x + targetRoom.Position.x) / 2f, currentPosY > 0f ? 0.45f : -0.45f * Room.DoorWidth + roomsPosY));
            }
        }
        _targetPositions.AddLast(targetPosition);
    }

    private bool FindAndSetPathBetweenRooms(Floor floor, Room currentRoom, Room targetRoom, in Vector3 targetPosition)
    {
        if (floor.FindRoomPath(currentRoom, targetRoom, out LinkedList<Room> path))
        {
            var roomList = new List<Room>(path);
            for (int i = 1; i < roomList.Count; i++)
            {
                _targetPositions.AddLast(Room.PickPositionInRoomDoorSide(roomList[i - 1], roomList[i]));
            }
            _targetPositions.AddLast(targetPosition);
            return true;
        }
        return false;
    }

    #endregion

    public void ClearTargetPositionsOnly()
    {
        _targetPositions.Clear();
    }

    public void ClearPath()
    {
        if (_targetPositions != null)  _targetPositions.Clear();

        if (_path != null) _path.Clear();

        ClearPathPoints();

        if (ImageBody != null) ImageBody.transform.position = Position;
    }

    private void ClearPathPoints()
    {
        if (_pathPoints != null) while (_pathPoints.Count != 0) ClearFirstPathPoint();
    }
    private void ClearFirstPathPoint()
    {
        Destroy(_pathPoints.First.Value);
        _pathPoints.RemoveFirst();
    }

    public bool HasPath => _path.Count != 0;

    public void SetBaseSpeed() => CurrentSpeed = BaseSpeed;
    public void SetRatioToBaseSpeed(float ratio) => CurrentSpeed = ratio * BaseSpeed;


    private Rigidbody2D CreateImageBody()
    {
        BoxCollider2D coll = GetComponent<BoxCollider2D>();
        SkinManager skin = Entity.Skin;
        var skinMainRenderer = skin[0].Renderer;
        coll.size = new Vector2(skinMainRenderer.sprite.rect.width, skinMainRenderer.sprite.rect.height) / skinMainRenderer.sprite.pixelsPerUnit;
        GameObject gmbjct = BasicLib.InstantiatePrefabGmbjct("imaginaryBody", transform.parent).transform.GetChild(0).gameObject;
        gmbjct.AddComponent<LocalPathFinder>();
        SpriteRenderer rend = gmbjct.GetComponent<SpriteRenderer>();
        rend.sprite = MyColorLib.GetSpriteColored(rend.color, skin[0].OriginalImage);  // sprite color is prefabed to pinkish
        rend.color = Color.white; // sprite renderer color is set to white so it can stay unaffected
        gmbjct.GetComponent<BoxCollider2D>().size = new Vector2(rend.sprite.rect.width / 100f, rend.sprite.rect.height / 100f);
        return gmbjct.GetComponent<Rigidbody2D>();
    }

    #region Follow

    public static Vector2 Follow(in Vector3 target, in Vector3 pos, float movement)
    {
        float temp;
        Vector2 move = default;
        if (System.Math.Abs(temp = target.x - pos.x) > movement)
        {
            move.x = movement * System.Math.Sign(temp);
        }
        else
        {
            move.x = temp;
        }
        if (System.Math.Abs(temp = target.y - pos.y) > movement)
        {
            move.y = movement * System.Math.Sign(temp);
        }
        else
        {
            move.y = temp;
        }
        return move;
    }

    public static Vector2 Followdt(in Vector3 target, in Vector3 pos, float speed)
    {
        float temp;
        Vector2 move = default;
        bool moveXNormal = false, moveYNormal = false;
        if (System.Math.Abs(temp = target.x - pos.x) > speed * Time.fixedDeltaTime)
        {
            moveXNormal = true;
            move.x = temp > 0f ? speed : -speed;
        }
        else if (temp != 0f)
        {
            move.x = temp / Time.fixedDeltaTime;
        }
        if (System.Math.Abs(temp = target.y - pos.y) > speed * Time.fixedDeltaTime)
        {
            moveYNormal = true;
            move.y = temp > 0f ? speed : -speed;
        }
        else if (temp != 0f)
        {
            move.y = temp / Time.fixedDeltaTime;
        }
        if (moveXNormal && moveYNormal)
        {
            move.x *= MyMathlib.INVERSE_SQRT_OF_2;
            move.y *= MyMathlib.INVERSE_SQRT_OF_2;
        }
        return move;
    }

    #endregion


    #region States

    protected abstract class MoveComponentState : IState
    {
        protected StateMachine _SM;
        protected MoveComponent _move;
        protected MoveComponentState(StateMachine SM, MoveComponent move)
        {
            _SM = SM;
            _move = move;
        }

        public abstract void OnStateEnter();

        public void LogicalUpdate()
        {
            throw new System.NotImplementedException();
        }

        public abstract void LogicalFixedUpdate();

        public void LogicalLateUpdate()
        {
            throw new System.NotImplementedException();
        }

        public abstract void OnStateExit();
    }

    protected class FreeMoveState : MoveComponentState
    {
        public FreeMoveState(StateMachine SM, MoveComponent move) : base(SM, move)
        {
        }

        public override void OnStateEnter()
        {
        }

        public override void LogicalFixedUpdate()
        {
            if (!_move.HasFreeMovement || _move.ImageBody == null) return;
            if (_move.HasPath)
            {
                var path = _move._path;
                if (!_move.FollowPath(path.First.Value))
                {
                    path.RemoveFirst();
                    if (ShowMovements && _move._pathPoints.Count != 0) _move.ClearFirstPathPoint();
                }
            }
            else
            {
                _move.ClearPathPoints();
                if (_move.ImageBody != null) _move.ImageBody.MovePosition(_move.Position);
                _move._rigidBody.velocity = default;
            }
        }

        public override void OnStateExit()
        {
        }
    }

    protected class PushedState : MoveComponentState
    {
        private FreeMoveState _freeMoveState;
        public PushedState(StateMachine SM, MoveComponent move) : base(SM, move)
        {
        }

        public void SetTargetStates(FreeMoveState freeMoveState)
        {
            _freeMoveState = freeMoveState;
        }

        public override void OnStateEnter()
        {
            _move.HasFreeMovement = false;
        }

        public override void LogicalFixedUpdate()
        {
            if (_move._rigidBody.velocity.sqrMagnitude < 4f) _SM.ChangeState(_freeMoveState);
        }

        public override void OnStateExit()
        {
            _move.HasFreeMovement = true;
        }
    }

    protected class DashState : MoveComponentState
    {
        private FreeMoveState _freeMoveState;
        private float _tempDrag;
        public DashState(StateMachine SM, MoveComponent move) : base(SM, move)
        {
        }

        public void SetTargetStates(FreeMoveState freeMoveState)
        {
            _freeMoveState = freeMoveState;
        }

        public override void OnStateEnter()
        {
            _move.IsDashing = true;
            _tempDrag = _move._rigidBody.drag;
            _move._rigidBody.drag = 0f;
        }

        public override void LogicalFixedUpdate()
        {
            _move._rigidBody.AddForce(_move._forceMagnitudeDash * (Vector2)_move._dashDirection);
            if (++_move._n > _move._tempDashTime / Time.fixedDeltaTime)
            {
                _SM.ChangeState(_freeMoveState);
            }
        }

        public override void OnStateExit()
        {
            _move.IsDashing = false;
            _move._rigidBody.drag = _tempDrag;
        }
    }

    #endregion

    #region LocalPathFinder
    public class LocalPathFinder : MonoBehaviour
    {
        public static bool _showMovements = false;
        public static bool ShowMovements
        {
            get => _showMovements;
            set
            {
                if (_showMovements == value) return;
                if (value)
                {
                    _showMovements = true;
                    foreach (var imaginaryBody in FindObjectsOfType<LocalPathFinder>(true))
                    {
                        imaginaryBody.Renderer.enabled = true;
                        foreach (var position in imaginaryBody.MoveComponent._path) imaginaryBody.AddPathPoint(position);
                    }
                }
                else
                {
                    _showMovements = false;
                    foreach (var imaginaryBody in FindObjectsOfType<LocalPathFinder>(true))
                    {
                        imaginaryBody.Renderer.enabled = false;
                        imaginaryBody.MoveComponent.ClearPathPoints();
                    }
                }
            }
        }
        private static GameObject _pathGrid = null;


        public MoveComponent MoveComponent { get; private set; }
        public SpriteRenderer Renderer { get; private set; }
        public Rigidbody2D RigidBody { get; private set; }

        private bool _collided = false;
        private Vector3 _lastPosition;   //to check if it is not blocked by anything
        private Vector3 _lastTargetPosition;       //to check if there's a new target position

        private const float _STUCK_MAX_TIME = 0.35f;
        private readonly Timer _stuckTimer = new Timer(_STUCK_MAX_TIME);
        private Vector3 _stuckPosition = default;

        private StateMachine _SM;

        private void Awake()
        {
            MoveComponent = this.SearchComponent<MoveComponent>();
            RigidBody = GetComponent<Rigidbody2D>();
            (Renderer = GetComponent<SpriteRenderer>()).enabled = ShowMovements;
            transform.position = MoveComponent.Position;

            _SM = new StateMachine();

            InactiveState s1 = new InactiveState(_SM, this);
            MiddleState s2 = new MiddleState(_SM, this);
            ActiveState s3 = new ActiveState(_SM, this);

            s1.SetTargetStates(s2);
            s2.SetTargetStates(s3, s1);
            s3.SetTargetStates(s1, s2);

            _SM.SetStartingState(s1);
        }

        private void OnEnable()
        {
            _stuckTimer.StartTimer();
            _stuckPosition = MoveComponent.Position;
            _SM.Initialize();
        }



        private void FixedUpdate()
        {
            if (!MoveComponent.HasFreeMovement) return;
            if (_stuckTimer.CheckIfTimePassed)
            {
                if (_stuckPosition == MoveComponent.Position && MoveComponent.HasPath)
                {
                    if (MoveComponent._targetPositions.Count != 0)
                    {
                        MoveComponent.StartFromScratchNewEndpos(MoveComponent._targetPositions.Last.Value);
                    }
                    else
                    {
                        MoveComponent.StartFromScratchNewEndpos(MoveComponent._path.Last.Value);
                    }
                }
                _stuckTimer.StartTimer();
                _stuckPosition = MoveComponent.Position;
            }
            _SM.LogicalFixedUpdate();
        }

        #region AddRemoveCollider

        private void AddToPath(in Vector3 newPosition)
        {
            if (MoveComponent.ShowMovements) AddPathPoint(newPosition);
            MoveComponent._path.AddLast(newPosition);
        }
        private void AddPathPoint(in Vector3 newPosition)
        {
            if (_pathGrid == null) _pathGrid = BasicLib.InstantiatePrefabGmbjct("misc/path grid", "path grid", null);
            var temp = Instantiate(_pathGrid, newPosition, Quaternion.identity, transform.parent);
            MoveComponent._pathPoints.AddLast(temp);
            temp.SetActive(true);
        }
        private void RemoveLastFromPath()
        {
            if (MoveComponent._path.Count != 0) MoveComponent._path.RemoveLast();
            if (_showMovements && MoveComponent._pathPoints.Count != 0)
            {
                Destroy(MoveComponent._pathPoints.Last.Value);
                MoveComponent._pathPoints.RemoveLast();
            }
        }

        private void CheckCollidedAndRemove()
        {
            if (_collided)
            {
                _collided = false;
                RemoveLastFromPath();
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            _collided = true;
        }

        #endregion

        #region Steps

        private void Prepare()
        {
            MoveComponent.ClearPathPoints();
            MoveComponent._path.Clear();
            Vector2 temp = (Vector2)MoveComponent.Position + Follow(MoveComponent._targetPositions.First.Value, MoveComponent.Position, 1f);
            RigidBody.MovePosition(temp);
            AddToPath(temp);
            _lastTargetPosition = MoveComponent._targetPositions.First.Value;
        }

        private void DoOneStep()
        {
            CheckCollidedAndRemove();
            _lastPosition = transform.position;
            RigidBody.MovePosition(RigidBody.position + Follow(MoveComponent._targetPositions.First.Value, RigidBody.position, 1f));
            _lastTargetPosition = MoveComponent._targetPositions.First.Value;
        }

        private bool FindPath()
        {
            CheckCollidedAndRemove();
            if (transform.position == MoveComponent._targetPositions.First.Value)
            {
                AddToPath(transform.position);
                MoveComponent._targetPositions.RemoveFirst();
                if (MoveComponent._targetPositions.Count == 0) return true;
                _lastTargetPosition = MoveComponent._targetPositions.First.Value;
                _lastPosition = transform.position;
                RigidBody.MovePosition(RigidBody.position + Follow(MoveComponent._targetPositions.First.Value, RigidBody.position, 1f));
            }
            else if (_lastPosition == transform.position)
            {
                AddToPath(transform.position);
                MoveComponent._targetPositions.RemoveFirst();
                if (MoveComponent._targetPositions.Count == 0) return true;
                _lastTargetPosition = MoveComponent._targetPositions.First.Value;
            }
            else
            {
                AddToPath(transform.position);
                _lastPosition = transform.position;
                RigidBody.MovePosition(RigidBody.position + Follow(MoveComponent._targetPositions.First.Value, RigidBody.position, 1f));
            }
            return false;
        }

        #endregion

        #region States
        protected abstract class LocalPathFinderState : IState
        {
            protected StateMachine _SM;
            protected LocalPathFinder _pathFinder;
            protected LocalPathFinderState(StateMachine SM, LocalPathFinder pathFinder)
            {
                _SM = SM;
                _pathFinder = pathFinder;
            }

            public abstract void OnStateEnter();

            public void LogicalUpdate()
            {
                throw new System.NotImplementedException();
            }

            public abstract void LogicalFixedUpdate();

            public void LogicalLateUpdate()
            {
                throw new System.NotImplementedException();
            }

            public abstract void OnStateExit();
        }


        protected class InactiveState : LocalPathFinderState
        {
            private MiddleState _middleState;
            public InactiveState(StateMachine SM, LocalPathFinder pathFinder) : base(SM, pathFinder)
            {
            }

            public void SetTargetStates(MiddleState middleState)
            {
                _middleState = middleState;
            }

            public override void OnStateEnter()
            {
            }

            public override void LogicalFixedUpdate()
            {
                if (_pathFinder.MoveComponent._targetPositions.Count != 0) _SM.ChangeState(_middleState);
            }

            public override void OnStateExit()
            {
            }
        }
        protected class MiddleState : LocalPathFinderState
        {
            private InactiveState _inactiveState;
            private ActiveState _activeState;
            public MiddleState(StateMachine SM, LocalPathFinder pathFinder) : base(SM, pathFinder)
            {
            }

            public void SetTargetStates(ActiveState activeState, InactiveState inactiveState)
            {
                _activeState = activeState;
                _inactiveState = inactiveState;
            }

            public override void OnStateEnter()
            {
                _pathFinder.Prepare();
            }

            public override void LogicalFixedUpdate()
            {
                if (_pathFinder.MoveComponent._targetPositions.Count == 0 || _pathFinder._lastTargetPosition != _pathFinder.MoveComponent._targetPositions.First.Value)
                {
                    _SM.ChangeState(_inactiveState);
                }
                else
                {
                    _pathFinder.DoOneStep();
                    _SM.ChangeState(_activeState);
                }
            }

            public override void OnStateExit()
            {
            }
        }
        protected class ActiveState : LocalPathFinderState
        {
            private InactiveState _inactiveState;
            private MiddleState _middleState;

            public ActiveState(StateMachine SM, LocalPathFinder pathFinder) : base(SM, pathFinder)
            {
            }

            public void SetTargetStates(InactiveState inactiveState, MiddleState middleState)
            {
                _inactiveState = inactiveState;
                _middleState = middleState;
            }

            public override void OnStateEnter()
            {
            }

            public override void LogicalFixedUpdate()
            {
                if (_pathFinder.MoveComponent._targetPositions.Count == 0)
                {  //this is for if endpos is cleared from an outside factor
                    _pathFinder.MoveComponent.ClearPath();
                    _SM.ChangeState(_inactiveState);
                }
                else if (_pathFinder._lastTargetPosition != _pathFinder.MoveComponent._targetPositions.First.Value)
                {
                    _SM.ChangeState(_middleState);
                }
                else if (_pathFinder.FindPath())
                {
                    _SM.ChangeState(_inactiveState);
                }
            }

            public override void OnStateExit()
            {
            }
        }
        #endregion

    }
    #endregion
}
