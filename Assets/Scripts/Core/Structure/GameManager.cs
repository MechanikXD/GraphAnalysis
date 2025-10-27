using UnityEngine;
using Core.Behaviour;
using Core.Graph;

namespace Core.Structure
{
    public class  GameManager : SingletonBase<GameManager>
    {
        [SerializeField] private Node _nodePrefab;
        [SerializeField] private Edge _edgePrefab;
        
        [SerializeField] private Transform _mapRoot;
        [SerializeField] private Transform _nodeRoot;
        [SerializeField] private Transform _edgeRoot;
        
        public Camera MainCamera { get; private set; }

        protected override void Initialize()
        {
            MainCamera = Camera.main;
            _nodeRoot.SetParent(_mapRoot);
            _edgeRoot.SetParent(_mapRoot);
        }

        public void MoveRoot(Vector3 newPos) => _nodeRoot.position = newPos;
        public void ScaleRoot(Vector3 newScale) => _nodeRoot.localScale = newScale;

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