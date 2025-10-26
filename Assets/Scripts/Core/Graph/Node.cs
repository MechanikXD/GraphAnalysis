using System.Collections.Generic;
using Core.Structure;
using Core.Structure.PlayerController;
using UI;
using UnityEngine;

namespace Core.Graph
{
    public class Node : MonoBehaviour, IInteractable
    {
        private List<Edge> _connections;

        private ContextAction[] _contextAction;

        private void Awake()
        {
            _connections = new List<Edge>();
            _contextAction = new[] 
            {
                new ContextAction("Link with", StartLink),
                new ContextAction("Delete", DeleteNode)
            };
        }

        public void Primary()
        {
            // TODO : Display Stats
        }

        public void Secondary()
        {
            var contextWind = UIManager.Instance.GetHUDCanvas<ContextWindow>();
            contextWind.LoadContext(_contextAction);
            contextWind.SetPosition(Input.mousePosition);
            contextWind.Show();
        }

        private void StartLink() => PlayerController.StartNodeLink(this);

        public void AddLink(Edge link) => _connections.Add(link);
        public void RemoveLink(Edge link) => _connections.Remove(link);

        private void DeleteNode()
        {
            foreach (var edge in _connections)
            {
                edge.DeleteEdge();
            }
            _connections.Clear();
            Destroy(gameObject);
        }
    }
}