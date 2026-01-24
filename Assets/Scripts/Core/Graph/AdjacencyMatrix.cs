using System.Collections.Generic;
using System.Threading;
using Analysis;
using Core.LoadSystem;
using Core.LoadSystem.Serializable;
using Core.Structure;
using Cysharp.Threading.Tasks;
using UI;
using UI.View;
using UnityEngine;

namespace Core.Graph
{
    public class AdjacencyMatrix : ISerializable<SerializableAdjacencyMatrix>
    {
        private readonly static CancellationTokenSource Cts = new CancellationTokenSource();
        public static string BgFilePath { get; set; }
        public List<Node> Nodes { get; } = new List<Node>();
        private List<List<float>> _matrix = new List<List<float>>();
        private Dictionary<string, float> _globalStats;
        public int Length { get; private set; }
        public bool IsOriented { get; private set; }
        public bool IsWeighted { get; set; }

        public void MakeOriented() => IsOriented = true;
        
        public void AddNode(Node node, bool updateStats=true)
        {
            node.NodeIndex = Nodes.Count;
            Nodes.Add(node);
            Length++;
            
            foreach (var col in _matrix) col.Add(0);
            var emptyList = new List<float>( new float[Length] );
            _matrix.Add(emptyList);
            if (updateStats) ProcessStats().Forget();
        }

        public void RemoveNode(int at, bool updateStats=true)
        {
            Nodes.RemoveAt(at);
            _matrix.RemoveAt(at);
            for (var i = 0; i < _matrix.Count; i++)
            {
                _matrix[i].RemoveAt(at);
                Nodes[i].NodeIndex = i;
            }
            Length--;
            if (updateStats) ProcessStats().Forget();
        }

        public void SetValue(float value, int row, int column, bool updateStats)
        {
            _matrix[row][column] = value;
            if (!IsOriented) _matrix[column][row] = value;
            if (updateStats) ProcessStats().Forget();
        }

        public float this[int row, int column] => _matrix[row][column];

        private async UniTask ProcessStats()
        {
            if (Length <= 0)
            {
                UIManager.Instance.GetHUDCanvas<GlobalStatDisplayView>().Hide();
                _globalStats?.Clear();
                return;
            }
            
            Cts.Cancel();
            var clone = Clone();

            var stats = Length switch
            {
                <= 80 => MetricProvider.ProcessMetrics(clone),
                <= 250 => await MetricProvider.ProcessMatrixAsync(clone),
                > 250 => await MetricProvider.ProcessMatrixParallel(clone)
            };

            _globalStats = stats.global;

            foreach (var node in Nodes)
            {
                node.LoadStats(stats.local);
            }
            
            var hud = UIManager.Instance.GetHUDCanvas<GlobalStatDisplayView>();
            hud.LoadText(_globalStats);
            if (!hud.IsEnabled) UIManager.Instance.ShowHUD<GlobalStatDisplayView>();
        }

        private AdjacencyMatrix Clone()
        {
            var clone = new AdjacencyMatrix();
            foreach (var node in Nodes)
            {
                clone.AddNode(node, false);
            }
            for (var i = 0; i < _matrix.Count; i++)
            {
                for (var j = 0; j < _matrix[i].Count; j++)
                {
                    var value = 0f;
                    if (!IsWeighted) value = _matrix[i][j];
                    else if (!IsWeighted && _matrix[i][j] != 0f) value = 1f;
                    clone.SetValue(value, i, j, false);
                }
            }
            
            return clone;
        }

        public SerializableAdjacencyMatrix SerializeSelf()
        {
            var nodes = new SerializableNode[Nodes.Count];
            for (var i = 0; i < Nodes.Count; i++)
            {
                nodes[i] = Nodes[i].SerializeSelf();
            }

            var allEdges = GameManager.Instance.GetAllEdges();
            var serializableEdges = new SerializableEdge[allEdges.Length];
            for (var i = 0; i < allEdges.Length; i++)
            {
                serializableEdges[i] = allEdges[i].SerializeSelf();
            }
            
            return new SerializableAdjacencyMatrix(_globalStats, nodes, serializableEdges, BgFilePath, Length, IsOriented, IsWeighted);
        }

        public void DeserializeSelf(SerializableAdjacencyMatrix serialized)
        {
            // Graph was empty prior, no need to deserialize
            if (serialized.GlobalStats == null) return;

            BgFilePath = serialized._bgFilePath;
            _globalStats = serialized.GlobalStats;
            IsOriented = serialized._isOriented;
            IsWeighted = serialized._isWeighted;
            Length = serialized._length;
            var gm = GameManager.Instance;

            _matrix = new List<List<float>>();
            for (var i = 0; i < Length; i++)
            {
                var emptyList = new List<float>(new float[Length]);
                _matrix.Add(emptyList);
            }
            
            foreach (var node in serialized._nodes)
            {
                var newNode = gm.CreateNode(new Vector2(node._positionX, node._positionY), node._nodeName, false);
                newNode.DeserializeSelf(node);
                Nodes.Add(newNode);
            }

            foreach (var edge in serialized._edges)
            {
                var newEdge = gm.CreateEdge(Vector2.zero, edge._isOneSided);
                var firstNode = Nodes[edge._firstNodeIndex];
                var secondNode = Nodes[edge._secondNodeIndex];
                newEdge.SetNodes(firstNode, secondNode, edge._weight, edge._isOneSided, false);
                newEdge.AdjustEdge(firstNode.transform.position, secondNode.transform.position);
                newEdge.DeserializeSelf(edge);
            }
        }

        public void GenerateFromNodes(float lengthCutOff, bool preserveConnectivity)
        {
            // Expected to have an empty matrix by now
            for (var i = 0; i < Nodes.Count; i++)
            {
                for (var j = 0; j < Nodes.Count; j++)
                {
                    if (i == j) _matrix[i][j] = 0;
                    else _matrix[i][j] = Vector2.Distance(Nodes[i].transform.position, Nodes[j].transform.position);
                }
            }
            // Get all edges that the graph will contain
            var edges = preserveConnectivity
                ? BuildMstWithThreshold(lengthCutOff)
                : FilteredEdges(lengthCutOff);
            // Instantiate edges
            foreach (var edge in edges)
            {
                var newEdge = GameManager.Instance.CreateEdge(Vector2.zero, false);
                var firstNode = Nodes[edge.from];
                var secondNode = Nodes[edge.to];
                newEdge.SetNodes(firstNode, secondNode, edge.weight, false, false);
                newEdge.AdjustEdge(firstNode.transform.position, secondNode.transform.position);
            }
            // Update entire graph at once
            ProcessStats().Forget();
        }
        
        // !!! Auto-generated
        private List<(int from, int to, float weight)> BuildMstWithThreshold(float threshold)
        {
            var result = new List<(int from, int to, float weight)>();
            var edgeSet = new HashSet<(int, int)>(); // avoid duplicates
            if (Length <= 1) return result;

            // Build MST (Prim)
            var inTree = new bool[Length];
            var minCost = new float[Length];
            var parent = new int[Length];

            for (var i = 0; i < Length; i++)
            {
                minCost[i] = float.PositiveInfinity;
                parent[i] = -1;
            }

            minCost[0] = 0f;
            for (var step = 0; step < Length; step++)
            {
                var u = -1;
                var best = float.PositiveInfinity;

                for (var i = 0; i < Length; i++)
                {
                    if (!inTree[i] && minCost[i] < best)
                    {
                        best = minCost[i];
                        u = i;
                    }
                }

                if (u == -1) break;

                inTree[u] = true;
                if (parent[u] != -1)
                {
                    var a = Mathf.Min(parent[u], u);
                    var b = Mathf.Max(parent[u], u);

                    edgeSet.Add((a, b));
                    result.Add((a, b, minCost[u]));
                }

                for (var v = 0; v < Length; v++)
                {
                    var w = _matrix[u][v];
                    if (!inTree[v] && w > 0f && w < minCost[v])
                    {
                        minCost[v] = w;
                        parent[v] = u;
                    }
                }
            }

            // Add all edges below threshold
            for (var i = 0; i < Length; i++)
            {
                for (var j = i + 1; j < Length; j++)
                {
                    var w = _matrix[i][j];
                    if (w > 0f && w <= threshold)
                    {
                        if (edgeSet.Add((i, j)))
                        {
                            result.Add((i, j, w));
                        }
                    }
                }
            }

            return result;
        }

        private List<(int from, int to, float weight)> FilteredEdges(float threshold)
        {
            var result = new List<(int from, int to, float weight)>();
            for (var i = 0; i < Length; i++)
            {
                for (var j = i + 1; j < Length; j++)
                {
                    var w = _matrix[i][j];
                    if (w > 0f && w <= threshold)
                    {
                        result.Add((i, j, w));
                    }
                }
            }

            return result;
        }
    }
}