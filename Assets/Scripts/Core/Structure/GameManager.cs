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

        public Node CreateNode(Vector2 worldPos, bool addNodeToMatrix = true)
        {
            var newNode = Instantiate(_nodePrefab, worldPos, Quaternion.identity, _nodeRoot);
            newNode.NodeName = "Node " + _totalNodesCreated;
            _totalNodesCreated++;
            
            if (addNodeToMatrix) AdjacencyMatrix.AddNode(newNode);
            return newNode;
        }

        public Node CreateNodeFromScreenPos(Vector2 screenPos, bool addNodeToMatrix = true)
        {
            var worldPos = MainCamera.ScreenToWorldPoint(screenPos);
            worldPos.z = 0;
            
            return CreateNode(worldPos, addNodeToMatrix);
        }

        public Edge CreateEdge(Vector2 worldPos, bool oneSided)
        {
            var newEdge = Instantiate(_edgePrefab, worldPos, Quaternion.identity, _edgeRoot);
            if (oneSided) newEdge.IsOneSided = true;
            return newEdge;
        }

        public Edge[] GetAllEdges() => _edgeRoot.GetComponentsInChildren<Edge>();
    }
}