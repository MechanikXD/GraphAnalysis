using Core.Structure;
using Core.Structure.PlayerController;
using UI;
using UI.InfoStructures;
using UI.View;
using UnityEngine;

namespace Core.Graph
{
    public class Edge : MonoBehaviour, IInteractable
    {
        private const float CROP_WHEN_TWO_SIDED = 0.6f;
        private const float CROP_WHEN_ONE_SIDED = 0.4f;
        private const float OFFSET_WHEN_ONE_SIDED = 0.1f;
        
        [SerializeField] private BoxCollider2D _collider;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        
        [SerializeField] private GameObject _forwardArrow;
        [SerializeField] private GameObject _backwardArrow;
        [SerializeField] private float _arrowOffset;

        private float _weight;
        private bool _isOneSided;
        private bool _wasPlaced;
        private Node _first;
        private Node _second;
        
        public string FromNodeName => _first.NodeName;
        public string ToNodeName => _second.NodeName;

        public float Weight
        {
            get => _weight;
            set
            {
                SetValueInMatrix(value);
                _weight = value;
            }
        }

        public float OffsetWhenOneSided => OFFSET_WHEN_ONE_SIDED;
        
        private ContextAction[] _contextAction;

        public bool IsOneSided
        {
            get => _isOneSided;
            set
            {
                _backwardArrow.SetActive(!value);
                _isOneSided = value;
            }
        }
        
        private void Awake()
        {
            _contextAction = new[] 
            {
                new ContextAction("Delete", DeleteEdge)
            };
        }

        public void SetLenght(float value)
        {
            if (_isOneSided) value -= CROP_WHEN_ONE_SIDED;
            else value -= CROP_WHEN_TWO_SIDED;
            if (value < 0) value = 0;
            
            _collider.size = new Vector2(value, _collider.size.y);
            _spriteRenderer.size = new Vector2(value, _spriteRenderer.size.y);

            var halfValue = value / 2;
            _forwardArrow.transform.localPosition = new Vector3(-halfValue + _arrowOffset, 0, 0);
            _backwardArrow.transform.localPosition = new Vector3(halfValue - _arrowOffset, 0, 0);
        }

        public void OnLeftClick()
        {
            var info = InfoView.GetInfo<EdgeInfo>();
            info.DisplayEdge(this);
            InfoView.ShowInfo<EdgeInfo>();
            UIManager.Instance.ShowHUD<InfoView>();
        }

        public void OnRightClick()
        {
            var contextWind = UIManager.Instance.GetHUDCanvas<ContextWindow>();
            contextWind.LoadContext(_contextAction);
            contextWind.SetPosition(Input.mousePosition);
            contextWind.Show();
        }

        /// Sets value in the Adjacency Matrix accounting orientation and graph type
        private void SetValueInMatrix(float value)
        {
            var matrix = GameManager.Instance.AdjacencyMatrix;
            if (IsOneSided)
            {
                matrix.MakeOriented();
                matrix[_first.NodeIndex, _second.NodeIndex] = value;
            }
            else if (matrix.IsOriented)
            {
                matrix[_first.NodeIndex, _second.NodeIndex] = value;
                matrix[_second.NodeIndex, _first.NodeIndex] = value;
            }
            else
            {
                matrix[_first.NodeIndex, _second.NodeIndex] = value;
            }
        }

        /// Sets all necessary values like nodes, weight etc.
        public void SetNodes(Node first, Node second, bool oneSided)
        {
            _first = first;
            _second = second;
            IsOneSided = oneSided;
            _wasPlaced = true;

            SetFallBackWeight();
            
            _first.AddLink(this);
            _second.AddLink(this);
        }

        public void SetFallBackWeight()
        {
            var weight = Vector2.Distance(_first.transform.position, _second.transform.position);
            Weight = weight;
            SetValueInMatrix(Weight);
        }

        /// <summary>
        /// Destroys itself but ignores given node, so node itself can delete all edges safely
        /// </summary>
        /// <param name="fromNode"> Node that deleted this edge </param>
        public void CascadeDestroy(Node fromNode)
        {
            if (_wasPlaced) SetValueInMatrix(0);
                
            if (fromNode != _first) _first.RemoveLink(this);
            if (fromNode != _second) _second.RemoveLink(this);
            
            Destroy(gameObject);
        }

        /// <summary>
        /// Properly destroys itself, removing all links between edges
        /// </summary>
        public void DeleteEdge()
        {
            if (_wasPlaced) SetValueInMatrix(0);
            
            if (_first != null) _first.RemoveLink(this);
            if (_second != null) _second.RemoveLink(this);
            
            Destroy(gameObject);
        }
    }
}