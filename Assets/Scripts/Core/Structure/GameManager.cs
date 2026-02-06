using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Core.Behaviour;
using Core.Graph;
using Core.LoadSystem;
using Core.LoadSystem.Serializable;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Other;
using UI.Settings.Types;
using UI.UiStructures.InfoStructures;
using UI.View.GraphScene;
using UnityEngine.Localization.Settings;

namespace Core.Structure
{
    public class  GameManager : SingletonBase<GameManager>
    {
        private string _sessionKey;
        // Pre-assign, because events that change those values are not called at once. Expect 3 updates on the start.
        public string TargetMetric { get; private set; } = "Node Degree";
        public Color LowColor { get; private set; } = Color.red;
        public Color HighColor { get; private set; } = Color.green;

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
            if (SaveManager.HaveSession(_sessionKey))
            {
                var json = SaveManager.GetSession(_sessionKey);
                var serialized = JsonConvert.DeserializeObject<SerializableAdjacencyMatrix>(json);
                AdjacencyMatrix.DeserializeSelf(serialized);
            }

            SubscribeToSettings();
            InfoView.GetInfo<Menu>().SetBackground(AdjacencyMatrix.BgFilePath);
            InfoFeed.Instance.LogInfo(GlobalStorage.InfoKeys.LOG_DATA_LOAD_SUCCESS);
            // To prevent OnApplicationFocus early calls
            await UniTask.Yield(PlayerLoopTiming.PostLateUpdate, destroyCancellationToken);
            _initialized = true;

            if (AdjacencyMatrix.Length > 0) AdjacencyMatrix.UpdateGlobalStatView();
        }

        private void SubscribeToSettings()
        {
            void UpdateTargetMetric(DropDownSettingPrefab dropDown)
            {
                TargetMetric = dropDown.Options[dropDown.CurrentOption];
                AdjacencyMatrix.UpdateNodeColors(TargetMetric);
            }
            void UpdateHighColor(ColorSettingPrefab color)
            {
                HighColor = color.CurrentColor;
                AdjacencyMatrix.UpdateNodeColors(TargetMetric);
            }
            void UpdateLowColor(ColorSettingPrefab color)
            {
                LowColor = color.CurrentColor;
                AdjacencyMatrix.UpdateNodeColors(TargetMetric);
            }
            
            SettingsManager.AddEventOnSetting<DropDownSettingPrefab>(
                GlobalStorage.SettingKeys.Graph.TARGET_METRIC, UpdateTargetMetric);
            SettingsManager.AddEventOnSetting<ColorSettingPrefab>(
                GlobalStorage.SettingKeys.Graph.HIGH_VALUE_COLOR, UpdateHighColor);
            SettingsManager.AddEventOnSetting<ColorSettingPrefab>(
                GlobalStorage.SettingKeys.Graph.LOW_VALUE_COLOR, UpdateLowColor);
        }

        public void ConfigureLoadOptions(string key) => _sessionKey = key;

        protected override void Initialize()
        {
            SaveManager.DeserializeSessionKeys();
            MainCamera = Camera.main;
            AdjacencyMatrix = new AdjacencyMatrix();
        }

        public void ForceSave() => OnApplicationFocus(false);
        
        private void OnApplicationFocus(bool isFocus)
        {
            if (!_initialized) return;
            
            if (!isFocus)
            {
                SaveManager.StoreSession(_sessionKey, GetGraphJson());
                SaveManager.SerializeSessionKeys();
            }
        }

        public string GetGraphJson() => JsonConvert.SerializeObject(AdjacencyMatrix.SerializeSelf());

        public string GetGraphAsCsv(bool createHeader=true)
        {
            if (AdjacencyMatrix.Length == 0) return string.Empty;
            
            var sb = new StringBuilder();
            var rowSb = new StringBuilder();
            if (createHeader) sb.Append(GetConcatNodes());
            // Build csv
            for (var i = 0; i < AdjacencyMatrix.Length; i++)
            {
                for (var j = 0; j < AdjacencyMatrix.Length; j++)
                {
                    rowSb.Append(AdjacencyMatrix[i, j]).Append(',');
                }
                if (rowSb.Length > 0) rowSb.Remove(rowSb.Length - 1, 1);
                sb.AppendLine(rowSb.ToString());
                rowSb.Clear();
            }

            return sb.ToString();
        }
        
        public string GetNodeStatsAsCsv(bool createHeaders=true)
        {
            if (AdjacencyMatrix.Length == 0) return string.Empty;
            
            var sb = new StringBuilder();
            var rowSb = new StringBuilder();
            if (createHeaders) sb.Append(',').AppendLine(GetConcatNodes());

            var statOrder = AdjacencyMatrix.Nodes[0].Stats.Keys.ToArray();
            // Build csv
            foreach (var currentStat in statOrder)
            {
                rowSb.Append(LocalizationSettings.StringDatabase.GetLocalizedString(currentStat)).Append(',');
                for (var i = 0; i < AdjacencyMatrix.Length; i++)
                {
                    rowSb.Append(AdjacencyMatrix.Nodes[i].Stats[currentStat]).Append(',');
                }
                if (rowSb.Length > 0) rowSb.Remove(rowSb.Length - 1, 1);
                sb.AppendLine(rowSb.ToString());
                rowSb.Clear();
            }

            return sb.ToString();
        }

        private string GetConcatNodes()
        {
            var sb = new StringBuilder();
            // Concat Node Names
            foreach (var node in AdjacencyMatrix.Nodes)
            {
                sb.Append(node.NodeName).Append(',');
            }
            // Remove last comma
            if (sb.Length > 0) sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
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

        public void UpdateEdgeColors()
        {
            foreach (var edge in CreatedEdges) edge.UpdateColor();
        }

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