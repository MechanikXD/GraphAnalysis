using System.Collections.Generic;
using Core.Structure;
using Core.Structure.PlayerController;
using UI;
using UI.InfoStructures;
using UI.View;
using UnityEngine;

namespace Core.Graph
{
    public class Node : MonoBehaviour, IInteractable
    {
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
    }
}