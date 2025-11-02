using Core.Structure;
using Core.Structure.PlayerController;
using UI;
using UnityEngine;

namespace Core.Graph
{
    public class Edge : MonoBehaviour, IInteractable
    {
        [SerializeField] private BoxCollider2D _collider;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private bool _wasPlaced;
        private Node _first;
        private Node _second;
        private float _weight;
        
        private ContextAction[] _contextAction;

        private void Awake()
        {
            _contextAction = new[] 
            {
                new ContextAction("Delete", DeleteEdge)
            };
        }

        public void SetLenght(float value)
        {
            _collider.size = new Vector2(value, _collider.size.y);
            _spriteRenderer.size = new Vector2(value, _spriteRenderer.size.y);
        }

        public void OnLeftClick()
        {
            // TODO : Display Stats
        }

        public void OnRightClick()
        {
            var contextWind = UIManager.Instance.GetHUDCanvas<ContextWindow>();
            contextWind.LoadContext(_contextAction);
            contextWind.SetPosition(Input.mousePosition);
            contextWind.Show();
        }

        public void SetNodes(Node first, Node second, out float weight)
        {
            _first = first;
            _second = second;
            _wasPlaced = true;
            
            weight = Vector2.Distance(_first.transform.position, second.transform.position);
            _weight = weight;
            
            _first.AddLink(this);
            _second.AddLink(this);
        }

        public void CascadeDestroy(Node fromNode)
        {
            if (_wasPlaced) GameManager.Instance.AdjacencyMatrix[_first.NodeIndex, _second.NodeIndex] = 0;
                
            if (fromNode != _first) _first.RemoveLink(this);
            if (fromNode != _second) _second.RemoveLink(this);
            
            Destroy(gameObject);
        }

        public void DeleteEdge()
        {
            if (_wasPlaced) GameManager.Instance.AdjacencyMatrix[_first.NodeIndex, _second.NodeIndex] = 0;
            
            if (_first != null) _first.RemoveLink(this);
            if (_second != null) _second.RemoveLink(this);
            
            Destroy(gameObject);
        }
    }
}