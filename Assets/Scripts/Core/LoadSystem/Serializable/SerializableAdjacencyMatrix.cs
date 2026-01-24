using System;
using System.Collections.Generic;

namespace Core.LoadSystem.Serializable
{
    [Serializable]
    public class SerializableAdjacencyMatrix
    {
        public string _bgFilePath;
        public Dictionary<string, float> GlobalStats;
        public SerializableNode[] _nodes;
        public SerializableEdge[] _edges;
        public int _length;
        public bool _isOriented;
        public bool _isWeighted;

        public SerializableAdjacencyMatrix(Dictionary<string, float> globalStats, 
            SerializableNode[] nodes, SerializableEdge[] edges, string bgFilePath, int length, bool isOriented, bool isWeighted)
        {
            _bgFilePath = bgFilePath;
            GlobalStats = globalStats;
            _nodes = nodes;
            _edges = edges;
            _length = length;
            _isOriented = isOriented;
            _isWeighted = isWeighted;
        }
    }
}