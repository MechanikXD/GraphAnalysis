using System.Collections.Generic;
using UnityEngine;

namespace Core.Graph
{
    public class AdjacencyMatrix
    {
        public List<Node> Nodes { get; } = new List<Node>();
        private readonly List<List<float>> _matrix = new List<List<float>>();
        public int Length { get; private set; }
        private bool _isOriented;

        public void MakeOriented() => _isOriented = true;
        public int Dim => _matrix.Count;

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
            foreach (var col in _matrix)
            {
                col.RemoveAt(at);
            }
            Length--;
        }

        public float this[int row, int column]
        {
            get => _matrix[row][column];
            set
            {
                _matrix[row][column] = value;
                if (!_isOriented) _matrix[column][row] = value;
            }
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
                    snapshot[i, j] = _matrix[i][j];
                }
            }
            
            return snapshot;
        }
    }
}