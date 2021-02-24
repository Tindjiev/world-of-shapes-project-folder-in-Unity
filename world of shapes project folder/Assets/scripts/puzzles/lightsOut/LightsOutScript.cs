using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsOutScript : MonoBehaviour
{
    public static bool ActiveDdebug = false;
    public static InputStruct Escape = new InputStruct(Input.GetKeyDown, KeyCode.Escape);

    private LinkedList<Uinf> _rows;
    private LinkedList<int> _rowsRearrangement = new LinkedList<int>();
    public Uinf B { get; private set; }
    public bool Solved { get; private set; } = false;
    private MyLib.MyArrayList<Uinf> _rowsTransposedAndGaussInfo = new MyLib.MyArrayList<Uinf>();


    public class Square
    {
        private const float _ADD_TO_EDGE = 0.5f;
        public static Vector2 Offset = new Vector2(1 / 3f, 1 / 3f);
        public static float EdgeSizeRatio;

        public readonly Color WrongColor, CorrectColor;
        public readonly int i, j;
        public readonly Texture2D Texture;

        public bool IsCorrect { get; private set; } = false;

        public static float EdgeSize
        {
            get => EdgeSizeRatio * Screen.width + _ADD_TO_EDGE;
            set => EdgeSizeRatio = value / Screen.width;
        }
        public Vector2 PositionOnScreen =>
            new Vector2(
                (EdgeSize - _ADD_TO_EDGE) * (j + 0.5f) + Offset.x * Screen.width,
                (EdgeSize - _ADD_TO_EDGE) * (i + 0.5f) + Offset.y * Screen.height);

        public Square(Color wrongColor, Color correctColor, int i, int j)
        {
            Texture = new Texture2D(1, 1);
            Texture.SetPixel(0, 0, WrongColor = wrongColor);
            Texture.Apply();
            CorrectColor = correctColor;
            this.i = i;
            this.j = j;
        }

        ~Square()
        {
            Destroy(Texture);
#if UNITY_EDITOR
            Debug.Log("entered Deconstructor");
#endif
        }

        public void ChangeState()
        {
            if (IsCorrect)
            {
                Texture.SetPixel(0, 0, WrongColor);
                Texture.Apply();
                IsCorrect = false;
            }
            else
            {
                Texture.SetPixel(0, 0, CorrectColor);
                Texture.Apply();
                IsCorrect = true;
            }
        }
    }

    public Square[,] BoardArray;
    [System.NonSerialized]
    public Graph<Square> BoardGraph;

    [SerializeField]
    private int _rowsNumber, _columnsNumber;

    [SerializeField]
    private Color _wrongColor, _correctColor;


    private GUIStyle _textStyle;

    protected void Awake()
    {
        enabled = false;
        _textStyle = new GUIStyle();
        _textStyle.alignment = TextAnchor.MiddleCenter;
        _textStyle.normal.textColor = Color.white;
    }
    protected void Start()
    {
        if (BoardGraph == null)
        {
            CreateBoard(_rowsNumber, _columnsNumber, _wrongColor, _correctColor);
        }
        var stackPath = BoardGraph.FindPath(BoardGraph.Root, BoardGraph.LastAddedNode);
        while (stackPath.Count != 0)
        {
            stackPath.Pop().Value.ChangeState();
        }
        MakeCurrentStateUinf();

    }

    private void OnEnable()
    {
        if (Square.EdgeSizeRatio == 0f) Square.EdgeSizeRatio = 60f / 1920f;
    }

    public void CreateBoard(int rows, int columns, Color wrongColor, Color correctColor)
    {
        BoardArray = new Square[rows, columns];
        for (int i = 0; i < BoardArray.GetLength(0); i++)
        {
            for (int j = 0; j < BoardArray.GetLength(1); j++)
            {
                BoardArray[i, j] = new Square(wrongColor, correctColor, i, j);
                BoardArray[i, j].ChangeState();
            }
        }
        BoardGraph = new Graph<Square>(BoardArray);//, true);
        MakeRowsAndTriangulate();
    }

    private void Update()
    {
        Square selectedsq = InputChooseBoard(Input.GetKeyDown(KeyCode.Mouse0));
        if (selectedsq != null)
        {
            ChangeSquareAndNeighbors(selectedsq);
            //printSolution();
        }
        Square selectedsqdebug = InputChooseBoard(ActiveDdebug && Input.GetKeyDown(KeyCode.Mouse1));
        if (selectedsqdebug != null)
        {
            selectedsqdebug.ChangeState();
            MakeCurrentStateUinf();
            //printSolution();
        }
        if (Escape.CheckInput())
        {
            Close();
        }
    }

    private void OnGUI()
    {
        _textStyle.fontSize = Screen.width * 2 / 110;
        GUI.Label(new Rect(Screen.width >> 1, BoardArray[0, 0].PositionOnScreen.y - Screen.width * 200f / 1920f, 0f, 0f), string.Format("Press " + Escape + " to exit"), _textStyle);
        for (int i = 0; i < BoardArray.GetLength(0); i++)
        {
            for (int j = 0; j < BoardArray.GetLength(1); j++)
            {
                GUI.DrawTexture(new Rect(BoardArray[i, j].PositionOnScreen, new Vector2(Square.EdgeSize, Square.EdgeSize)).RectangleCentre(0f, 0f), BoardArray[i, j].Texture);
            }
        }
    }

    private Square InputChooseBoard(bool input)
    {
        if (!input) return null;
        Vector2 mousepos = MyInputs.GetMousePositionOnscreen;
        for (int i = 0; i < BoardArray.GetLength(0); i++)
        {
            for (int j = 0; j < BoardArray.GetLength(1); j++)
            {
                if (MyMathlib.IsInsideSquare(mousepos, Square.EdgeSize, BoardArray[i, j].PositionOnScreen))
                {
                    return BoardArray[i, j];
                }
            }
        }
        //foreach (square sq in boardgraph)
        //{
        //    if (mathlib.isInsideSquare(mousepos, square.edgeSize, sq.positionOnScreen))
        //    {
        //        return sq;
        //    }
        //}
        return null;
    }

    public void ChangeSquareAndNeighbors(Square centreSquare)
    {
        if (centreSquare == null) return;
        centreSquare.ChangeState();
        var node = BoardGraph.FindNode(centreSquare);
        if (node == null)
        {
#if UNITY_EDITOR
            Debug.Log("square wasn't found in graph", this);
            Debug.Break();
#endif
            return;
        }
        foreach (var neighborSquare in node)
        {
            neighborSquare.ChangeState();
        }
        MakeCurrentStateUinf();
    }

    private static List<GameObject> _gameObjects = new List<GameObject>();
    private static Transform _parent = null;
    public void Open()
    {
        this.DoActionInNextFrame(OpenTrue);
    }
    public void OpenWithSolver()
    {
        this.DoActionInNextFrame(OpenWithSolverTrue);
    }
    public void Open(bool withSolver)
    {
        if (withSolver)
        {
            Open();
        }
        else
        {
            OpenWithSolver();
        }
    }

    private void OpenTrue()
    {
        _parent = transform.parent;
        transform.parent = null;
        foreach (GameObject go in FindObjectsOfType<GameObject>())
        {
            if (go.transform.parent == null && go != gameObject && go != Camera.main.gameObject)
            {
                go.SetActive(false);
                _gameObjects.Add(go);
            }
        }
        enabled = true;
    }
    private void OpenWithSolverTrue()
    {
        OpenTrue();
        gameObject.AddComponent<LightsOutSolver>().enabled = true;
    }
    public void Close()
    {
        foreach (GameObject go in _gameObjects)
        {
            if (go != null) go.SetActive(true);
        }
        _gameObjects.Clear();
        transform.parent = _parent;
        enabled = false;

        Solved = true;
        foreach(var square in BoardGraph)
        {
            if(!square.IsCorrect)
            {
                Solved = false;
                break;
            }
        }
    }

    //A*x = B
    //A is the changes after clicking a square
    public static LinkedList<Uinf> MakeARows(Graph<Square> boardGraph, Square[,] boardArray)
    {
        int size = boardGraph.Size;
        LinkedList<Uinf> Rows = new LinkedList<Uinf>();
        bool[][] bools = new bool[size][];
        foreach (var squareNode in (IEnumerable<Graph<Square>.Node>)boardGraph)
        {
            int i = boardArray.Search(squareNode.Value);
            bools[i] = new bool[size];
            bools[i][i] = true;
            foreach (Square neighbor in squareNode)
            {
                bools[i][boardArray.Search(neighbor)] = true;
            }
        }
        for(int i = 0; i < bools.Length; ++i)
        {
            Rows.AddLast(new Uinf(bools[i]));
        }
        return Rows;
    }

    private void MakeRowsAndTriangulate()
    {
        _rows = MakeARows(BoardGraph, BoardArray).TriangulateGauss(_rowsRearrangement, _rowsTransposedAndGaussInfo);
    }

    //A*x=B
    //B is the current state of the board each row in a line
    private void MakeCurrentStateUinf()
    {
        B = new Uinf(BoardArray.Length);
        int i = 0;
        for (var Node = _rowsRearrangement.First.Next; Node != null; Node = Node.Next, i++)
        {
            if (!BoardArray[Node.Value / BoardArray.GetLength(0), Node.Value % BoardArray.GetLength(1)].IsCorrect)
            {
                B.SetBit(i);
            }
        }
    }

    public Uinf GetSolution()
    {
        return B.TriangulateGauss(_rowsTransposedAndGaussInfo).SolveForAxEQb(_rowsTransposedAndGaussInfo);
    }
    private void PrintSolution()
    {
        //Debug.Log(L.ArrayToString());
        //Debug.Log(BaseLib.SolveForAxeqbFast(AA, L).ArrayToString());
        Debug.Log(GetSolution());
        //Debug.Log(BaseLib.SolveForAxeqbFast(AA, L).ArrayToString() == L.TriangulateGauss(RowsRearrangement, RowsTransposedAndGaussInfo).SolveForAxEQb(RowsTransposedAndGaussInfo).ToString());
    }

}




