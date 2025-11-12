using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Graph
{
    public class AdjacencyMatrix
    {
        public List<Node> Nodes { get; } = new List<Node>();
        private readonly List<List<float>> _matrix = new List<List<float>>();
        public int Length { get; private set; }
        public bool IsOriented { get; private set; }
        public bool IsWeighted { get; set; }

        public void MakeOriented() => IsOriented = true;
        
        public void AddNode(Node node)
        {
            node.NodeIndex = Nodes.Count;
            Nodes.Add(node);
            Length++;
            
            foreach (var col in _matrix) col.Add(0);
            var emptyList = new List<float>( new float[Length] );
            _matrix.Add(emptyList);
        }

        public void RemoveNode(string node)
        {
            var nodeIndex = -1;
            for (var i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].NodeName != node) continue;

                nodeIndex = i;
                break;
            }

            if (nodeIndex == -1) Debug.LogError($"Node with name {node} was not found");
            else RemoveNode(nodeIndex);
        }

        public void RemoveNode(int at)
        {
            Nodes.RemoveAt(at);
            _matrix.RemoveAt(at);
            for (var i = 0; i < _matrix.Count; i++)
            {
                _matrix[i].RemoveAt(at);
                Nodes[i].NodeIndex = i;
            }
            Length--;
        }

        public float this[int row, int column]
        {
            get => _matrix[row][column];
            set
            {
                _matrix[row][column] = value;
                if (!IsOriented) _matrix[column][row] = value;
            }
        }

        public float[,] AsArray()
        {
            var matrix = new float[Length, Length];
            for (var i = 0; i < Length; i++)
            {
                for (var j = 0; j < Length; j++)
                {
                    matrix[i, j] = IsWeighted ? _matrix[i][j] : 1f;
                }
            }
            
            return matrix;
        }
        
        public float[] Bfs(int start)
        {
            var dist = Enumerable.Repeat(float.PositiveInfinity, Length).ToArray();
            var q = new Queue<int>();
            dist[start] = 0;
            q.Enqueue(start);

            while (q.Count > 0)
            {
                var u = q.Dequeue();
                for (var v = 0; v < Length; v++)
                {
                    if (_matrix[u][v] <= 0 || !float.IsPositiveInfinity(dist[v])) continue;

                    dist[v] = dist[u] + 1;
                    q.Enqueue(v);
                }
            }
            return dist;
        }

        public float[] Dijkstra(int start)
        {
            var dist = Enumerable.Repeat(float.PositiveInfinity, Length).ToArray();
            var visited = new bool[Length];
            dist[start] = 0;

            for (var i = 0; i < Length; i++)
            {
                var u = -1;
                var minDist = float.PositiveInfinity;
                for (var j = 0; j < Length; j++)
                    if (!visited[j] && dist[j] < minDist) { u = j; minDist = dist[j]; }

                if (u == -1) break;
                visited[u] = true;

                for (var v = 0; v < Length; v++)
                {
                    if (_matrix[u][v] <= 0) continue;

                    var alt = dist[u] + _matrix[u][v];
                    if (alt < dist[v]) dist[v] = alt;
                }
            }
            return dist;
        }
        
        public float LargestComponentSize()
        {
            var visited = new bool[Length];
            var maxSize = 0;

            for (var i = 0; i < Length; i++)
            {
                if (visited[i]) continue;
                var q = new Queue<int>();
                q.Enqueue(i);
                visited[i] = true;
                var size = 0;
                while (q.Count > 0)
                {
                    var u = q.Dequeue();
                    size++;
                    for (var v = 0; v < Length; v++)
                    {
                        if (!(_matrix[u][v] > 0) || visited[v]) continue;

                        visited[v] = true;
                        q.Enqueue(v);
                    }
                }
                maxSize = Mathf.Max(maxSize, size);
            }
            return maxSize;
        }

        public AdjacencyMatrix TakeSnapshot()
        {
            var snapshot = new AdjacencyMatrix();
            foreach (var node in Nodes)
            {
                snapshot.AddNode(node);
            }
            for (var i = 0; i < _matrix.Count; i++)
            {
                for (var j = 0; j < _matrix[i].Count; j++)
                {
                    snapshot[i, j] = IsWeighted ? _matrix[i][j] : 1f;
                }
            }
            
            return snapshot;
        }
    }
}