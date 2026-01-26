using System.Collections.Generic;
using UnityEngine;
using Core.Behaviour;
using Core.Graph;
using Core.LoadSystem;
using Core.LoadSystem.Serializable;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UI.InfoStructures;
using UI.View;

namespace Core.Structure
{
    public class  GameManager : SingletonBase<GameManager>
    {
        // For manual testing:
        [SerializeField] private string _sessionKey;
        [SerializeField] private bool _deserializeData;
        [SerializeField] private bool _serializeData;
        [SerializeField] private bool _loadGraphData;
        [SerializeField] private string[] _nodeNames;
        [SerializeField] private Vector2[] _nodePos;

        [SerializeField] private Transform _tempRoot;
        private readonly List<Node> _tempNodes = new List<Node>();
        public Transform TempRoot => _tempRoot;
        
        [SerializeField] private Transform _nodeRoot;
        [SerializeField] private Transform _edgeRoot;
        
        [SerializeField] private Node _nodePrefab;
        [SerializeField] private Edge _edgePrefab;
        private bool _initialized;
        
        public List<Edge> CreatedEdges { get; } = new List<Edge>();
        private int _totalNodesCreated;
        public AdjacencyMatrix AdjacencyMatrix { get; private set; }
        public Camera MainCamera { get; private set; }

        private async void Start()
        {
            InfoView.GetInfo<Menu>().SetBackground(AdjacencyMatrix.BgFilePath);
            // To prevent OnApplicationFocus early calls
            await UniTask.Yield(PlayerLoopTiming.PostLateUpdate, destroyCancellationToken);
            _initialized = true;
            if (_loadGraphData) PlayerController.PlayerController.EnterGraphAdjust();
            if (AdjacencyMatrix.Length > 0) AdjacencyMatrix.UpdateGlobalStatView();
        }
        
        protected override void Initialize()
        {
            MainCamera = Camera.main;
            AdjacencyMatrix = new AdjacencyMatrix();
            
            if (_deserializeData && SaveManager.HaveSession(_sessionKey))
            {
                var json = SaveManager.GetSession(_sessionKey);
                var serialized = JsonConvert.DeserializeObject<SerializableAdjacencyMatrix>(json);
                AdjacencyMatrix.DeserializeSelf(serialized);
            }
        }
        
        private void OnApplicationFocus(bool isFocus)
        {
            if (!_initialized) return;
            
            if (_serializeData && !isFocus)
            {
                var serializable = AdjacencyMatrix.SerializeSelf();
                var json = JsonConvert.SerializeObject(serializable);
                SaveManager.StoreSession(_sessionKey, json);
            }
        }

        public Node CreateNode(Vector2 worldPos, [CanBeNull] string nodeName, bool addNodeToMatrix = true)
        {
            var newNode = Instantiate(_nodePrefab, worldPos, Quaternion.identity, _nodeRoot);
            if (string.IsNullOrEmpty(nodeName))
            {
                newNode.NodeName = "Node " + _totalNodesCreated;
                _totalNodesCreated++;
            }
            else
            {
                newNode.NodeName = nodeName;
            }
            
            if (addNodeToMatrix) AdjacencyMatrix.AddNode(newNode);
            return newNode;
        }

        public Node CreateNodeFromScreenPos(Vector2 screenPos, [CanBeNull] string nodeName, bool addNodeToMatrix = true)
        {
            var worldPos = MainCamera.ScreenToWorldPoint(screenPos);
            worldPos.z = 0;
            
            return CreateNode(worldPos, nodeName, addNodeToMatrix);
        }

        public Edge CreateEdge(Vector2 worldPos, bool oneSided)
        {
            var newEdge = Instantiate(_edgePrefab, worldPos, Quaternion.identity, _edgeRoot);
            if (oneSided) newEdge.IsOneSided = true;
            CreatedEdges.Add(newEdge);
            return newEdge;
        }

        public void GenerateTempNodes() => GenerateTempNodes(_nodePos, _nodeNames);

        public void GenerateTempNodes(Vector2[] pos, [CanBeNull] string[] names)
        {
            for (var i = 0; i < pos.Length; i++)
            {
                var newNode = Instantiate(_nodePrefab, pos[i], Quaternion.identity, _tempRoot);
                if (names == null || string.IsNullOrEmpty(names[i]))
                {
                    newNode.NodeName = "Node " + _totalNodesCreated;
                    _totalNodesCreated++;
                }
                else
                {
                    newNode.NodeName = names[i];
                }
                _tempNodes.Add(newNode);
            }
        }

        public void ApplyTempNodes()
        {
            foreach (var node in _tempNodes)
            {
                AdjacencyMatrix.AddNode(node, false);
                node.transform.SetParent(_nodeRoot, true);
            }
            _tempNodes.Clear();
            _tempRoot.position = Vector3.zero;
        }

        public void DestroyTempNodes()
        {
            foreach (var node in _tempNodes) Destroy(node.gameObject);
            
            _tempNodes.Clear();
            _tempRoot.position = Vector3.zero;
        }

        public void RemoveEdge(Edge removed) => CreatedEdges.Remove(removed);

        public void RemoveEdgesAfter(int index) => 
            CreatedEdges.RemoveRange(index, CreatedEdges.Count - index);
    }
}