using System.Collections.Generic;
using Core.Structure;
using Core.Structure.PlayerController;
using JetBrains.Annotations;
using UI;
using UI.InfoStructures;
using UI.View;
using UnityEngine;

namespace Core.Graph
{
    public class Node : MonoBehaviour, IInteractable
    {
        private List<Edge> _connections;

        private ContextAction[] _contextAction;
        public List<Edge> Connections => _connections;
        public string NodeName { get; set; }
        public int NodeIndex { get; set; }
        
        private void Awake()
        {
            _connections = new List<Edge>();
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
        
        private void StartLink() => PlayerController.StartNodeLink(this, false);
        private void StartMove() => PlayerController.StartNodeMove(this);
        private void StartOneSidedLink() => PlayerController.StartNodeLink(this, true);

        public void AddLink(Edge link) => _connections.Add(link);
        public void RemoveLink(Edge link) => _connections.Remove(link);

        private void DeleteNode()
        {
            foreach (var edge in _connections) edge.CascadeDestroy(this);
            
            GameManager.Instance.AdjacencyMatrix.RemoveNode(NodeIndex);
            _connections.Clear();
            Destroy(gameObject);
        }
    }
}