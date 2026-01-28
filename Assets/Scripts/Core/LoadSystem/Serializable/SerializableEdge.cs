using System;

namespace Core.LoadSystem.Serializable
{
    [Serializable]
    public class SerializableEdge
    {
        public float _weight;
        public bool _isOneSided;
        public int _firstNodeIndex;
        public int _secondNodeIndex;
        public bool _isCustomWeight;

        public SerializableEdge(float weight, bool isOneSided, int firstNodeIndex, int secondNodeIndex, bool isCustomWeight)
        {
            _weight = weight;
            _isOneSided = isOneSided;
            _firstNodeIndex = firstNodeIndex;
            _secondNodeIndex = secondNodeIndex;
            _isCustomWeight = isCustomWeight;
        }
    }
}