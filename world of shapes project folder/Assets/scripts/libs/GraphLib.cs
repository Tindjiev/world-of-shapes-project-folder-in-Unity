using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Graph<T> : IEnumerable<T>, IEnumerable<Graph<T>.Node>, ICollection<T>, ISerializationCallbackReceiver
{

    [field: SerializeField, ReadOnlyOnInspector]
    public int Size { get; protected set; }

    [SerializeField, HideInInspector]
    private int _removedNodes = 0;

    [field: SerializeField, ReadOnlyOnInspector]
    public bool CostedConnections { get; protected set; } = false;

    [field: NonSerialized]
    public Node Root { get; private set; }

    [field: NonSerialized]
    public Node LastAddedNode { get; private set; }

    public bool RootIsNull => Root == null || Root.Graph == null;


    public T RootValue => Root.Value;


    public int Count => Size;

    public bool IsReadOnly => false;

    #region Constructors
    public Graph()
    {

    }

    public Graph(T[] Array)
    {
        MakeWithArrayOneWayLink(Array);
    }
    public Graph(T[] Array, bool TwoWayLink)
    {
        if (TwoWayLink)
        {
            MakeWithArrayTwoWayLink(Array);
        }
        else
        {
           MakeWithArrayOneWayLink(Array);
        }
    }
    public Graph(T[,] Array)
    {
        MakeWithArray(Array);
    }
    public Graph(T[,] Array, bool diagonalNeighbors)
    {
        if (!diagonalNeighbors)
        {
            MakeWithArray(Array);
        }
        else
        {
            MakeWithArrayDiag(Array);
        }
    }

    public Graph(T[][] Arrays, int[,] linkInfo)
    {
        MakeWithArraysTree(Arrays, linkInfo);
    }
    #endregion

    #region Arrays

    public T[] getArray1D()
    {
        MyLib.MyArrayList<T> tempList = new MyLib.MyArrayList<T>(Size);
        DFS((x) => tempList.Add(x.Value));
        return tempList.Array;
    }
    public T[,] getArray2D()
    {
        int ilen = 0;
        for (Node Node = Root; Node != null || Node.NumberOfNeighbors < 1; Node = Node[0], ilen++) ;
        int jlen = Size / ilen;
        T[,] array = new T[ilen, jlen];
        int i, j = 0;
        for (Node Node = Root; Node != null || Node.NumberOfNeighbors < 1 || j < jlen; Node = Node[Node.NumberOfNeighbors - 1], j++)
        {
            i = 0;
            for (Node Node2 = Node; i<ilen; Node2 = Node2[0], i++)
            {
                array[i, j] = Node2.Value;
            }
        }
        return array;
    }

    Node[] MakeWithArrayOneWayLink(T[] Array)
    {
        int len = Array.Length;
        Node[] NodeArray = new Node[Array.Length];
        NodeArray[0] = new Node(this, Array[0]);
        for (int i = 1; i < len; i++)
        {
            NodeArray[i - 1].AddNeighbor(NodeArray[i] = new Node(this, Array[i]));
        }
        return NodeArray;
    }
    Node[] MakeWithArrayOneWayLink(T[] Array, Node BranchFrom)
    {
        Node[] Nodes = MakeWithArrayOneWayLink(Array);
        BranchFrom.AddNeighbor(Nodes[0]);
        return Nodes;
    }
    void MakeWithArrayTwoWayLink(T[] Array)
    {
        int len = Array.Length;
        Node LastNode = new Node(this, Array[0]);
        for (int i = 1; i < len; i++)
        {
            Node NewNode = new Node(this, Array[i]);
            LastNode.AddNeighborTwoWay(NewNode);
            LastNode = NewNode;
        }
    }
    void MakeWithArray(T[,] Array)
    {
        int ilen = Array.GetLength(0);
        int jlen = Array.GetLength(1);
        Node[,] NodesArray = new Node[ilen, jlen];


        if (Array[0, 0] != null) NodesArray[0, 0] = new Node(this, Array[0, 0]);
        for (int j = 1; j < jlen; ++j)
        {
            if (Array[0, j] != null) NodesArray[0, j] = new Node(this, Array[0, j], NodesArray[0, j - 1]);
        }
        for (int i = 1; i < ilen; ++i)
        {
            if (Array[i, 0] != null) NodesArray[i, 0] = new Node(this, Array[i, 0], NodesArray[i - 1, 0]);
            for (int j = 1; j < jlen; ++j)
            {
                if (Array[i, j] != null) NodesArray[i, j] = new Node(this, Array[i, j], new Node[] { NodesArray[i, j - 1], NodesArray[i - 1, j] });
            }
        }
    }
    void MakeWithArrayDiag(T[,] Array)
    {
        int ilen = Array.GetLength(0);
        int jlenm1 = Array.GetLength(1);
        Node[,] NodesArray = new Node[ilen, jlenm1];

        if (Array[0, 0] != null) NodesArray[0, 0] = new Node(this, Array[0, 0]);
        for (int j = 1; j < jlenm1; ++j)
        {
            if (Array[0, j] != null) NodesArray[0, j] = new Node(this, Array[0, j], NodesArray[0, j - 1]);
        }
        --jlenm1;
        for (int i = 1; i < ilen; ++i)
        {
            if (Array[i, 0] != null)
                NodesArray[i, 0] = new Node(this, Array[i, 0],
                new float[] { MyMathlib.SQRT_OF_2 },
                 NodesArray[i - 1, 1], NodesArray[i - 1, 0]);
            for (int j = 1; j < jlenm1; ++j)
            {
                if (Array[i, j] != null)
                    NodesArray[i, j] = new Node(this, Array[i, j],
                    new float[] { MyMathlib.SQRT_OF_2, MyMathlib.SQRT_OF_2 },
                    NodesArray[i - 1, j - 1], NodesArray[i - 1, j + 1], NodesArray[i - 1, j], NodesArray[i, j - 1]);
            }
            if (Array[i, jlenm1] != null)
                NodesArray[i, jlenm1] = new Node(this, Array[i, jlenm1],
                new float[] { MyMathlib.SQRT_OF_2 },
                NodesArray[i - 1, jlenm1 - 1], NodesArray[i - 1, jlenm1], NodesArray[i, jlenm1 - 1]);
        }
    }

    private void MakeWithArraysTree(T[][] Arrays, int[,] info)
    {
        Node[][] NodeArrays = new Node[Arrays.Length][];
        NodeArrays[0] = MakeWithArrayOneWayLink(Arrays[0]);
        switch (info.GetLength(1))
        {
            case 2:
                for (int i = 1; i < Arrays.Length; i++)
                {
                    NodeArrays[i] = MakeWithArrayOneWayLink(Arrays[i], NodeArrays[info[i, 0]][info[i, 1]]);
                }
                break;
            case 4:
                Node temp;
                if (info[0, 2] != -1)
                {
                    if(info[0,3] != -1)
                    {
                        temp = NodeArrays[info[0, 2]][info[0, 3]];
                    }
                    else
                    {
                        temp = NodeArrays[info[0, 2]][NodeArrays[info[0, 2]].Length - 1];
                    }
                    NodeArrays[0][NodeArrays[0].Length - 1].AddNeighbor(temp);
                }
                for (int i = 1; i < Arrays.Length; i++)
                {
                    NodeArrays[i] = MakeWithArrayOneWayLink(Arrays[i], NodeArrays[info[i, 0]][info[i, 1]]);
                    if (info[i, 2] != -1)
                    {
                        if (info[i, 3] != -1)
                        {
                            temp = NodeArrays[info[i, 2]][info[i, 3]];
                        }
                        else
                        {
                            temp = NodeArrays[info[i, 2]][NodeArrays[info[i, 2]].Length - 1];
                        }
                        NodeArrays[i][NodeArrays[i].Length - 1].AddNeighbor(temp);
                    }
                }
                break;
        }
    }

    #endregion

    #region Iterators

    public IEnumerator<T> GetEnumerator()
    {
        if (Size == 0) yield break;
        ResetVisitedNodes();
        var enumerator = FoReach(Root);
        while (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
    }
    
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    IEnumerator<Node> IEnumerable<Node>.GetEnumerator()
    {
        if (Size == 0) yield break;
        ResetVisitedNodes();
        var enumerator = ForEachNode(Root);
        while (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
    }

    private IEnumerator<T> FoReach(Node CurrentNode)
    {
        if (CurrentNode == null)
        {
            yield break;
        }
        yield return CurrentNode.Value;
        LabelVisitedNode(CurrentNode);
        for (int i = 0; i < CurrentNode.NumberOfNeighbors; i++)
        {
            Node tempnode = CurrentNode[i];
            if (!VisitedNode(tempnode))
            {
                IEnumerator<T> enumerator = FoReach(tempnode);
                while (enumerator.MoveNext())
                {
                    yield return enumerator.Current;
                }
            }
        }
    }
    private IEnumerator<Node> ForEachNode(Node CurrentNode)
    {
        if (CurrentNode == null) yield break;
        yield return CurrentNode;
        LabelVisitedNode(CurrentNode);
        for (int i = 0; i < CurrentNode.NumberOfNeighbors; i++)
        {
            var tempnode = CurrentNode[i];
            if (!VisitedNode(tempnode))
            {
                var enumerator = ForEachNode(tempnode);
                while (enumerator.MoveNext())
                {
                    yield return enumerator.Current;
                }
            }
        }
    }

    public Stack<Node> FindPath(Node start, T target)
    {
        if (CostedConnections)
        {
            return UniCostSearchPath(start, target);
        }
        else
        {
            return BFS(start, target);
        }
    }

    #endregion

    [SerializeField, HideInInspector]
    private Node[] _nodesSerialized = null;

    [SerializeField, HideInInspector]
    private int _rootID = -1, _lastAddedNodeID = -1;

    public void OnBeforeSerialize()
    {
        _nodesSerialized = new Node[Size];
        if (Size == 0) return;
        _rootID = Root.ID;
        if (LastAddedNode != null) _lastAddedNodeID = LastAddedNode.ID;
        int i = -1;
        foreach (var node in (IEnumerable<Node>)this)
        {
            _nodesSerialized[++i] = node;
        }
        if (LastAddedNode == null) _lastAddedNodeID = _nodesSerialized[i].ID;
    }

    public void OnAfterDeserialize()
    {
        Root = Array.Find(_nodesSerialized, x => x.ID == _rootID);
        LastAddedNode = Array.Find(_nodesSerialized, x => x.ID == _lastAddedNodeID);
        for (int i = 0; i < _nodesSerialized.Length; ++i)
        {
            _nodesSerialized[i].SetGraph(this);
            _nodesSerialized[i].TrueOnAfterDeserialize();
        }
    }

    #region PathFind

    public Stack<Node> FindPath(Node start, Node target)
    {
        return FindPath(start, target.Value);
    }
    public Stack<Node> FindPath(T start, T target)
    {
        Node startNode = FindNode(start);
        if (startNode == null)
        {
            return null;
        }
        return FindPath(startNode, target);
    }
    public Stack<Node> FindPath(T start, Node target)
    {
        Node startNode = FindNode(start);
        if (startNode == null)
        {
            return null;
        }
        return FindPath(startNode, target.Value);
    }

    private class InfoNode
    {
        public static LinkedList<InfoNode> Q = new LinkedList<InfoNode>();
        public Node node;
        public InfoNode prev;
        public float dist;
        public InfoNode[] Neighbors = null;

        public InfoNode(Node node)
        {
            this.node = node;
            this.prev = null;
            this.dist = float.MaxValue;
            Q.AddLast(this);

        }

        public bool IsInQ()
        {
            return Q.Contains(this);
        }

        public static bool IsOver()
        {
            return Q.Count == 0;
        }
        
        public static InfoNode FindInQ(Node node)
        {
            LinkedListNode<InfoNode> temp = Q.Find((x) => x.node == node);
            if (temp == null)
            {
                return null;
            }
            else
            {
                return temp.Value;
            }
        }
        public static LinkedListNode<InfoNode> UniformCostFIndMinInQ()
        {
            float min = float.MaxValue;
            LinkedListNode<InfoNode> minNode = null;
            LinkedListNode<InfoNode> curr;
            for (curr = Q.First; curr != null; curr = curr.Next)
            {
                if (min > curr.Value.dist)
                {
                    min = curr.Value.dist;
                    minNode = curr;
                }
            }
            return minNode;
        }
    }
    Stack<Node> UniCostSearchPath(Node start, T target)
    {
        InfoNode.Q = new LinkedList<InfoNode>();
        DFS(start, (Node node) => InfoNode.Q.AddLast(new InfoNode(node)));
        InfoNode source = InfoNode.Q.First.Value;
        InfoNode targetInfoNode = null;
        source.dist = 0f;

        while (!InfoNode.IsOver())
        {
            LinkedListNode<InfoNode> u = InfoNode.UniformCostFIndMinInQ();
            if (EqualityComparer<T>.Default.Equals(u.Value.node.Value, target))
            {
                targetInfoNode = u.Value;
                break;
            }
            InfoNode.Q.Remove(u);
            Node uNode = u.Value.node;
            int len = uNode.NumberOfNeighbors;
            for (int i = 0; i < len; i++)
            {
                InfoNode v = InfoNode.FindInQ(uNode[i]);
                if (v != null)
                {
                    float alt = u.Value.dist + uNode.DistanceToNeighbor(i);
                    if (alt < v.dist)
                    {
                        v.dist = alt;
                        v.prev = u.Value;
                    }
                }
            }
        }
        if (targetInfoNode == null)
        {
            return null;
        }
        Stack<Node> stack = new Stack<Node>();
        while (targetInfoNode.prev != null)
        {
            stack.Push(targetInfoNode.node);
            targetInfoNode = targetInfoNode.prev;
        }
        return stack;
    }

    #endregion

    public bool Remove(T ToRemove)
    {
        return Remove(FindNode(ToRemove));
    }


    public bool Remove(Node ToRemove)
    {
        if (ToRemove != null)
        {
            
            return ToRemove.RemoveFromGraph();
        }
        return false;
    }

    public Node FindNode(T ToFind)
    {
        return DFS(ToFind);
    }

    #region visitedNodes

    private static Uinf _visitedNodesFlags;
    private static void LabelVisitedNode(Node node)
    {
        _visitedNodesFlags[node.ID] = true;
    }
    private static bool VisitedNode(Node node)
    {
        return _visitedNodesFlags[node.ID];
    }
    private void ResetVisitedNodes()
    {
        _visitedNodesFlags = new Uinf(Size + _removedNodes);
    }

    #endregion

    #region DFS

    protected Node DFS(T ToFind)
    {
        ResetVisitedNodes();
        return DFSrecurse(Root, ToFind);
    }
    protected Node DFSrecurse(Node CurrentNode, T ToFind)
    {
        if (CurrentNode == null) return null;
        else if (EqualityComparer<T>.Default.Equals(CurrentNode.Value, ToFind)) return CurrentNode;
        LabelVisitedNode(CurrentNode);
        for (int i = 0; i < CurrentNode.NumberOfNeighbors; i++)
        {
            if (!VisitedNode(CurrentNode[i]))
            {
                Node TempFind = DFSrecurse(CurrentNode[i], ToFind);
                if (TempFind != null) return TempFind;
            }
        }
        return null;
    }

    void DFS(Action<Node> action)
    {
        ResetVisitedNodes();
        DFSrecurse(Root, action);
    }
    void DFS(Node start, Action<Node> action)
    {
        ResetVisitedNodes();
        DFSrecurse(start, action);
    }
    void DFSrecurse(Node CurrentNode, Action<Node> action)
    {
        if (CurrentNode == null) return;
        action(CurrentNode);
        LabelVisitedNode(CurrentNode);
        for (int i = 0; i < CurrentNode.NumberOfNeighbors; i++)
        {
            Node tempnode = CurrentNode[i];
            if (!VisitedNode(tempnode))
            {
                DFSrecurse(tempnode, action);
            }
        }
    }
    private void DFSrecurse(Node CurrentNode, Action<Node> action, Predicate<Node> conditionForAction)
    {
        if (CurrentNode == null) return;
        if (conditionForAction(CurrentNode))
        {
            action(CurrentNode);
        }
        LabelVisitedNode(CurrentNode);
        for (int i = 0; i < CurrentNode.NumberOfNeighbors; i++)
        {
            Node tempnode = CurrentNode[i];
            if (!VisitedNode(tempnode))
            {
                DFSrecurse(tempnode, action, conditionForAction);
            }
        }
        return;
    }
    private bool DFSrecurseFind(Node CurrentNode, Action<Node> action, Predicate<Node> conditionForAction)
    {
        if (CurrentNode == null) return false;
        if (conditionForAction(CurrentNode))
        {
            action(CurrentNode);
            return true;
        }
        LabelVisitedNode(CurrentNode);
        for (int i = 0; i < CurrentNode.NumberOfNeighbors; i++)
        {
            Node tempnode = CurrentNode[i];
            if (!VisitedNode(tempnode))
            {
                if (DFSrecurseFind(tempnode, action, conditionForAction))
                {
                    return true;
                }
            }
        }
        return false;
    }

    #endregion

    public void ForEach(Action<Node> action)
    {
        DFS(action);
    }
    public void ForEach(Action<Node> action, Predicate<Node> conditionForAction)
    {
        ResetVisitedNodes();
        DFSrecurse(Root, action, conditionForAction);
    }

    #region BFS

    private Stack<Node> BFS(Node start, T ToFind)
    {
        ResetVisitedNodes();
        Node[] prevs = new Node[Size];
        int[] prevsPointers = new int[Size];
        int returnedIndex = BFSrecusre(start, ToFind, new Queue<Node>(), new Queue<int>(), -1, prevs, prevsPointers, 0);
        Stack<Node> path = new Stack<Node>();
        while (returnedIndex > -1)
        {
            path.Push(prevs[returnedIndex]);
            returnedIndex = prevsPointers[returnedIndex];
        }
        return path;
    }
    private int BFSrecusre(Node currentNode, T ToFind, Queue<Node> PendingToVisit, Queue<int> indexOfPendingNodes, int indexOfCurrentNode, Node[] prev, int[] prevsPointers, int currentIndexToArrays)
    {
        if (currentNode == null)
        {
            return -2;
        }
        else if (EqualityComparer<T>.Default.Equals(currentNode.Value, ToFind))
        {
            prev[currentIndexToArrays] = currentNode;
            prevsPointers[currentIndexToArrays] = indexOfCurrentNode;
            return currentIndexToArrays;
        }
        LabelVisitedNode(currentNode);
        for (int i = 0; i < currentNode.NumberOfNeighbors; i++)
        {
            if (!VisitedNode(currentNode[i]) && !PendingToVisit.Contains(currentNode[i]))
            {
                prev[currentIndexToArrays] = currentNode;
                prevsPointers[currentIndexToArrays] = indexOfCurrentNode;
                PendingToVisit.Enqueue(currentNode[i]);
                indexOfPendingNodes.Enqueue(currentIndexToArrays++);
            }
        }
        if (PendingToVisit.Count == 0)
        {
            return -2;
        }
        int returnedValue = BFSrecusre(PendingToVisit.Dequeue(), ToFind, PendingToVisit, indexOfPendingNodes, indexOfPendingNodes.Dequeue(), prev, prevsPointers, currentIndexToArrays);
        if (returnedValue != -2)
        {
            return returnedValue;
        }
        return -2;
    }

    #endregion


    public void Add(T item)
    {
        new Node(this, item, LastAddedNode);
    }
    public void Add(T item, Node Neighbor)
    {
        new Node(this, item, Neighbor);
    }
    public void Add(T item, params Node[] Neighbors)
    {
        new Node(this, item, Neighbors);
    }

    public void Clear()
    {
        if (Size == 0) return;
        Node[] nodes = new Node[Size];
        int i = -1;
        foreach (Node node in (IEnumerable<Node>)this)
        {
            nodes[++i] = node;
        }
        foreach(Node node in nodes)
        {
            if (node != null) node.RemoveFromGraph();
        }
        for (int j = 0; j < _nodesSerialized.Length; ++j)
        {
            _nodesSerialized[j] = null;
        }
        ResetVisitedNodes();
        _removedNodes = 0;
    }

    public bool Contains(T item)
    {
        return FindNode(item) != null;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        foreach(T item in this)
        {
            array[arrayIndex++] = item;
        }
    }

    [Serializable]
    public class Node : IEnumerable<T>, IEnumerable<Node> , ISerializationCallbackReceiver
    {
        [field: NonSerialized]
        public Graph<T> Graph { get; private set; }

        public void SetGraph(Graph<T> graph) => Graph = graph;

        [SerializeField, ReadOnlyOnInspector]
        private int _id = -1;
        public int ID => _id;

        public T Value;

        public bool IsRoot => this == Graph.Root;

        public void SetAsRoot() => Graph.Root = this;

        [SerializeField, HideInInspector]
        protected NeighborAndDistance[] _neighbors;

        [NonSerialized]
        protected Node[] _cannotVisitNeighbors; //this means Nodes that have this Node as neighbor but not the other way around, used for deleting Nodes

        public Node this[int index] => _neighbors[index].Neighbor;

        public int NumberOfNeighbors => _neighbors.Length;
        public bool HasNeighbors => _neighbors.Length != 0;

        public float DistanceToNeighbor(int index) => _neighbors[index].Distance;

        public bool IsNeighboringTo(Node node) => System.Array.FindIndex(_neighbors, (x) => x.Neighbor == node) != -1;


        #region Constructors

        protected Node(Graph<T> graph)
        {
            SetGraph(graph);
            _id = Graph._removedNodes + Graph.Size++;
        }

        protected Node(Graph<T> graph, T value) : this(graph)
        {
            if (graph.RootIsNull)
            {
                graph.Root = this;
            }
            graph.LastAddedNode = this;
            Value = value;
            _neighbors = new NeighborAndDistance[0];
            _cannotVisitNeighbors = new Node[0];
        }
        public Node(Graph<T> graph, T value, Node neighbor) : this(graph, value)
        {
            if (neighbor != null)
            {
                neighbor.AddNeighborTwoWay(this);
            }
        }
        public Node(Graph<T> graph, T value, bool twoWayNeighbor, Node neighbor) : this(graph, value)
        {
            if (neighbor != null)
            {
                if (twoWayNeighbor)
                {
                    neighbor.AddNeighborTwoWay(this);
                }
                else
                {
                    neighbor.AddNeighbor(this);
                }
            }
        }
        public Node(Graph<T> graph, T value, float dist, Node neighbor) : this(graph, value)
        {
            if (dist != 1f)
            {
                if (neighbor != null)
                {
                    neighbor.AddNeighborTwoWay(this, dist);
                }
            }
            else
            {
                if (neighbor != null)
                {
                    neighbor.AddNeighborTwoWay(this);
                }
            }
        }
        public Node(Graph<T> graph, T value, params Node[] neighbors) : this(graph, value)
        {
            for (int i = 0; i < neighbors.Length; i++)
            {
                if (neighbors[i] != null)
                {
                    neighbors[i].AddNeighborTwoWay(this);
                }
            }
        }
        public Node(Graph<T> graph, T value, bool twoWayNeighbor, params Node[] neighbors) : this(graph, value)
        {
            if (twoWayNeighbor)
            {
                for (int i = 0; i < neighbors.Length; i++)
                {
                    if (neighbors[i] != null) neighbors[i].AddNeighborTwoWay(this);
                }
            }
            else
            {
                for (int i = 0; i < neighbors.Length; i++)
                {
                    if (neighbors[i] != null) neighbors[i].AddNeighbor(this);
                }
            }
        }
        public Node(Graph<T> graph, T value, float dist, params Node[] neighbors) : this(graph, value)
        {
            for (int i = 0; i < neighbors.Length; i++)
            {
                if (neighbors[i] != null)
                {
                    neighbors[i].AddNeighborTwoWay(this, dist);
                }
            }
        }
        public Node(Graph<T> graph, T value, float[] dists, params Node[] neighbors) : this(graph, value)
        {
            for (int i = 0; i < neighbors.Length; i++)
            {
                if (neighbors[i] != null)
                {
                    if (i < dists.Length && dists[i] != 1f)
                    {
                        neighbors[i].AddNeighborTwoWay(this, dists[i]);
                    }
                    else
                    {
                        neighbors[i].AddNeighborTwoWay(this);
                    }
                }
            }
        }

        #endregion

        #region AddNeighbor
        private bool CanBeNeighbor(Node newNeighbor) => this != newNeighbor && MyLib.FindIndex(_neighbors, x => x.Neighbor == newNeighbor) == -1;

        public bool AddNeighbor(Node newNeighbor, float dist = 1f)
        {
            if (!CanBeNeighbor(newNeighbor)) return false;
            if (dist != 1f)
            {
                MyLib.ExpandArray(ref _neighbors, new NeighborAndDistance(newNeighbor, dist));
                Graph.CostedConnections = true;
            }
            else
            {
                MyLib.ExpandArray(ref _neighbors, new NeighborAndDistance(newNeighbor, 1f));
            }
            MyLib.ArrayRemoveOne(ref _cannotVisitNeighbors, newNeighbor);
            if (!newNeighbor.IsNeighboringTo(this)) MyLib.ExpandArray(ref newNeighbor._cannotVisitNeighbors, this);
            return true;
        }

        private bool AddNeighbor(Node newNeighbor, bool addCannotVisit, float dist = 1f)
        {
            if (addCannotVisit) return AddNeighbor(newNeighbor, dist);
            if (!CanBeNeighbor(newNeighbor)) return false;
            if (dist != 1f)
            {
                MyLib.ExpandArray(ref _neighbors, new NeighborAndDistance(newNeighbor, dist));
                Graph.CostedConnections = true;
            }
            else
            {
                MyLib.ExpandArray(ref _neighbors, new NeighborAndDistance(newNeighbor, 1f));
            }
            MyLib.ArrayRemoveOne(ref _cannotVisitNeighbors, newNeighbor);
            return true;
        }
        public bool AddNeighborTwoWay(Node newNeighbor, float dist = 1f)
        {// put OR instead of AND because it should be considered a success if one manages, meaning it was the case of one being already neighbor but not the other way around
               //in case of being the same Node both will return false
            return AddNeighbor(newNeighbor, false, dist) | newNeighbor.AddNeighbor(this, false, dist); //put | instead of || because i want both of them to be executed
        }
        public bool AddNeighborTwoWay(Node newNeighbor, float distToNewNeighbor, float distFromNewNeighbor)
        {
            return AddNeighbor(newNeighbor, false, distToNewNeighbor) | newNeighbor.AddNeighbor(this, false, distFromNewNeighbor);
        }
        public void RemoveNeighbor(Node exNeighbor)
        {
            MyLib.ArrayRemoveOne(ref _neighbors, x => x.Neighbor == exNeighbor);
            MyLib.ArrayRemoveOne(ref exNeighbor._cannotVisitNeighbors, this);
        }

        #endregion

        public bool RemoveFromGraph()
        {
            if (IsRoot)
            {
                Graph.Root = HasNeighbors ? this[0] : (_cannotVisitNeighbors.Length > 0 ? _cannotVisitNeighbors[0] : null);
                if (Graph.Root == null)
                {
                    Graph.Root = this;
                    return false;
                }
            }
            if(this == Graph.LastAddedNode)
            {
                Graph.LastAddedNode = HasNeighbors ? this[0] : (_cannotVisitNeighbors.Length > 0 ? _cannotVisitNeighbors[0] : null);
            }
            --Graph.Size;
            ++Graph._removedNodes;
            for (int i = 0; i < _neighbors.Length; ++i)
            {
                MyLib.ArrayRemoveOne(ref this[i]._cannotVisitNeighbors, this);
            }
            for (int i = 0; i < _cannotVisitNeighbors.Length; ++i)
            {
                _cannotVisitNeighbors[i].RemoveNeighbor(this);
            }
            _neighbors = null;
            _cannotVisitNeighbors = null;
            return true;
        }

        #region Iterators
        public IEnumerator<T> GetEnumerator()
        {
            for(int i = 0; i < _neighbors.Length; ++i)
            {
                yield return this[i].Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<Node> IEnumerable<Node>.GetEnumerator()
        {
            for (int i = 0; i < _neighbors.Length; ++i)
            {
                yield return this[i];
            }
        }
        #endregion

        [SerializeField, HideInInspector]
        private int[] _idNeighbor = new int[0], _idCannotVisit = new int[0];

        public void OnBeforeSerialize()
        {
            _idNeighbor = new int[(_neighbors ?? (_neighbors = new NeighborAndDistance[0])).Length];
            for(int i = 0; i < _idNeighbor.Length; ++i)
            {
                _idNeighbor[i] = _neighbors[i].Neighbor.ID;
            }
            _idCannotVisit = new int[(_cannotVisitNeighbors ?? (_cannotVisitNeighbors = new Node[0])).Length];
            for (int i = 0; i < _cannotVisitNeighbors.Length; ++i)
            {
                _idCannotVisit[i] = _cannotVisitNeighbors[i].ID;
            }
        }

        public void OnAfterDeserialize()
        {

        }

        public void TrueOnAfterDeserialize()
        {
            _cannotVisitNeighbors = new Node[_idCannotVisit.Length];
            foreach (var node in Graph._nodesSerialized)
            {
                int i = System.Array.FindIndex(_idNeighbor, (x) => x == node.ID);
                if (i != -1)
                {
                    _neighbors[i].Neighbor = node;
                }
                else if ((i = System.Array.FindIndex(_idCannotVisit, (x) => x == node.ID)) != -1)
                {
                    _cannotVisitNeighbors[i] = node;
                }
            }
        }


        [Serializable]
        protected class NeighborAndDistance
        {
            public float Distance;
            [NonSerialized]
            public Node Neighbor;

            public NeighborAndDistance(Node neighbor, float distance)
            {
                Neighbor = neighbor;
                Distance = distance;
            }
        }
    }
}





