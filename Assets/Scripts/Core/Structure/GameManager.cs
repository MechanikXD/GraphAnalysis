using System.Text;
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
            newNode.NodeName = "Node " + AdjacencyMatrix.Nodes.Count;
            AdjacencyMatrix.AddNode(newNode);
            Debug.Log("New Matrix:\n" + MatrixToString());
        }

        public Edge CreateEdge(Vector2 worldPos)
        {
            return Instantiate(_edgePrefab, worldPos, Quaternion.identity, _edgeRoot);
        }

        public string MatrixToString()
        {
            var stringBuilder = new StringBuilder();
            for (var i = 0; i < AdjacencyMatrix.Length; i++)
            {
                for (var j = 0; j < AdjacencyMatrix.Length; j++)
                {
                    stringBuilder.Append(AdjacencyMatrix[i, j]).Append(' ');
                }
                stringBuilder.AppendLine();
            }
            
            return stringBuilder.ToString();
        }
    }
}