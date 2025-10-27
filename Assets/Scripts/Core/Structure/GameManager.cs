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
        
        public Camera MainCamera { get; private set; }

        protected override void Initialize()
        {
            MainCamera = Camera.main;
        }

        public void CreateNodeFromScreenPos(Vector2 screenPos)
        {
            var worldPos = MainCamera.ScreenToWorldPoint(screenPos);
            worldPos.z = 0;
            Instantiate(_nodePrefab, worldPos, Quaternion.identity, _nodeRoot);
        }

        public Edge CreateEdge(Vector2 worldPos)
        {
            return Instantiate(_edgePrefab, worldPos, Quaternion.identity, _edgeRoot);
        }
    }
}