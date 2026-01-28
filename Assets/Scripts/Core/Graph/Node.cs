using System.Collections.Generic;
using Core.LoadSystem;
using Core.LoadSystem.Serializable;
using Core.Structure;
using Core.Structure.PlayerController;
using Cysharp.Threading.Tasks;
using Other;
using UI;
using UI.InfoStructures;
using UI.View;
using UnityEngine;

namespace Core.Graph
{
    public class Node : MonoBehaviour, IInteractable, ISerializable<SerializableNode>
    {
        private const float COLOR_SNAP_DISTANCE = 0.1f;
        [SerializeField] private float _colorChangeSpeed;
        [SerializeField] private SpriteRenderer _renderer;
        private Color _targetColor;
        private bool _isChangingColor;
        public Color NodeColor => _targetColor;
        public Dictionary<string, float> Stats { get; private set; }
        private ContextAction[] _contextAction;
        public List<Edge> Connections { get; private set; }
        public float Degree => Connections.Count;
        public string NodeName { get; set; }
        public int NodeIndex { get; set; }
        public Vector2 Position => transform.position;
        
        private void Awake()
        {
            Connections = new List<Edge>();
            _contextAction = new[] 
            {
                new ContextAction("Link with", StartLink),
                new ContextAction("Link to", StartOneSidedLink),
                new ContextAction("Move", StartMove),
                new ContextAction("Delete", DeleteNode)
            };
        }
        
        public void OnLeftClick()
        {
            var info = InfoView.GetInfo<NodeInfo>();
            info.DisplayNode(this);
            InfoView.ShowInfo<NodeInfo>();
            UIManager.Instance.ShowHUD<InfoView>();
        }

        public void OnRightClick()
        {
            var contextWind = UIManager.Instance.GetHUDCanvas<ContextWindow>();
            contextWind.LoadContext(_contextAction);
            contextWind.SetPosition(Input.mousePosition);
            contextWind.Show();
        }

        public void LoadStats(Dictionary<string, float[]> localStats)
        {
            Stats ??= new Dictionary<string, float>();
            foreach (var kvp in localStats)
            {
                Stats[kvp.Key] = kvp.Value[NodeIndex];
            }
        }

        public void AssignColor(string targetMetric, float min, float max)
        {
            var metricValue = Stats[targetMetric];
            metricValue -= min;
            metricValue /= max - min;
            _targetColor = Color.Lerp(GameManager.Instance.LowColor, GameManager.Instance.HighColor, metricValue);
            if (!_isChangingColor) ChangeColor().Forget();
        }
        
        private void StartLink() => PlayerController.EnterNodeLink(this, false);
        private void StartMove() => PlayerController.EnterNodeMove(this);
        private void StartOneSidedLink() => PlayerController.EnterNodeLink(this, true);

        public void AddLink(Edge link) => Connections.Add(link);
        public void RemoveLink(Edge link) => Connections.Remove(link);

        public void DeleteNode()
        {
            foreach (var edge in Connections) edge.CascadeDestroy(this);
            
            GameManager.Instance.AdjacencyMatrix.RemoveNode(NodeIndex);
            Connections.Clear();
            Destroy(gameObject);
        }

        public void SilentDestroy()
        {
            foreach (var edge in Connections) edge.CascadeDestroy(this, true);
            Connections.Clear();
            Destroy(gameObject);
        }

        public SerializableNode SerializeSelf() => new SerializableNode(Stats, NodeName, NodeIndex, new Vector2(transform.position.x, transform.position.y));

        public void DeserializeSelf(SerializableNode serialized)
        {
            Stats = serialized.Stats;
            NodeName = serialized._nodeName;
            NodeIndex = serialized._nodeIndex;
        }

        private async UniTask ChangeColor()
        {
            _isChangingColor = true;
            while (_renderer.color.Distance(_targetColor) > COLOR_SNAP_DISTANCE)
            {
                _renderer.color = Color.Lerp(_renderer.color, _targetColor, _colorChangeSpeed * Time.deltaTime);
                await UniTask.NextFrame(destroyCancellationToken);
            }

            _renderer.color = _targetColor;
            _isChangingColor = false;
        }
    }
}