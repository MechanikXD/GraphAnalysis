using Core.Structure;
using Core.Structure.PlayerController;
using UI;
using UnityEngine;

namespace Core.Graph
{
    public class Edge : MonoBehaviour, IInteractable
    {
        private Node _first;
        private Node _second;
        
        private ContextAction[] _contextAction;

        private void Awake()
        {
            _contextAction = new[] 
            {
                new ContextAction("Delete", DeleteEdge)
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

        private void DeleteEdge() => Destroy(gameObject);
    }
}