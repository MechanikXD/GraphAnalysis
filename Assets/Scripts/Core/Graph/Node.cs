using System.Collections.Generic;
using Core.LoadSystem;
using Core.LoadSystem.Serializable;
using Core.Structure;
using Core.Structure.PlayerController;
using UI;
using UI.InfoStructures;
using UI.View;
using UnityEngine;

namespace Core.Graph
{
    public class Node : MonoBehaviour, IInteractable, ISerializable<SerializableNode>
    {
        public Dictionary<string, float> Stats { get; private set; }
        private ContextAction[] _contextAction;
        public List<Edge> Connections { get; private set; }
        public float Degree => Connections.Count;
        public string NodeName { get; set; }
        public int NodeIndex { get; set; }
        
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
        
        private void StartLink() => PlayerController.StartNodeLink(this, false);
        private void StartMove() => PlayerController.StartNodeMove(this);
        private void StartOneSidedLink() => PlayerController.StartNodeLink(this, true);

        public void AddLink(Edge link) => Connections.Add(link);
        public void RemoveLink(Edge link) => Connections.Remove(link);

        private void DeleteNode()
        {
            foreach (var edge in Connections) edge.CascadeDestroy(this);
            
            GameManager.Instance.AdjacencyMatrix.RemoveNode(NodeIndex);
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
    }
}