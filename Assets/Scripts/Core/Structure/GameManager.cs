using UnityEngine;
using Core.Behaviour;
using Core.Graph;

namespace Core.Structure
{
    public class  GameManager : SingletonBase<GameManager>
    {
        [SerializeField] private Transform _nodeRoot;
        [SerializeField] private Transform _edgeRoot;
        
        [SerializeField] private Node _nodePrefab;
        [SerializeField] private Edge _edgePrefab;

        private int _totalNodesCreated;
        public AdjacencyMatrix AdjacencyMatrix { get; private set; }
        public Camera MainCamera { get; private set; }

        protected override void Initialize()
        {
            MainCamera = Camera.main;
            AdjacencyMatrix = new AdjacencyMatrix();
        }

        public void CreateNodeFromScreenPos(Vector2 screenPos)
        {
            var worldPos = MainCamera.ScreenToWorldPoint(screenPos);
            worldPos.z = 0;
            
            var newNode = Instantiate(_nodePrefab, worldPos, Quaternion.identity, _nodeRoot);
            newNode.NodeName = "Node " + _totalNodesCreated;
            _totalNodesCreated++;
            
            AdjacencyMatrix.AddNode(newNode);
        }

        public Edge CreateEdge(Vector2 worldPos, bool oneSided)
        {
            var newEdge = Instantiate(_edgePrefab, worldPos, Quaternion.identity, _edgeRoot);
            if (oneSided) newEdge.IsOneSided = true;
            return newEdge;
        }
    }
}