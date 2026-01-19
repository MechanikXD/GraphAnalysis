using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.LoadSystem.Serializable
{
    [Serializable]
    public class SerializableNode
    {
        public Dictionary<string, float> Stats;
        public string _nodeName;
        public int _nodeIndex;
        public Vector2 _position;

        public SerializableNode(Dictionary<string, float> stats, string nodeName, int nodeIndex, Vector2 position)
        {
            Stats = stats;
            _nodeName = nodeName;
            _nodeIndex = nodeIndex;
            _position = position;
        }
    }
}