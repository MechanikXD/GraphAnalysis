using Core.LoadSystem;
using Core.LoadSystem.Serializable;
using Core.Structure;
using Core.Structure.PlayerController;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Other;
using UI;
using UI.Settings.Types;
using UI.UiStructures.InfoStructures;
using UI.View.GraphScene;
using UnityEngine;

namespace Core.Graph
{
    /*
     *  Second Node                        First Node
     *  Base Color    <------------->      Gradient
     * Forward Arrow                    Backward arrow
     */
    public class Edge : MonoBehaviour, IInteractable, ISerializable<SerializableEdge>
    {
        [SerializeField] private BoxCollider2D _collider;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private SpriteRenderer _gradientRenderer;
        
        [SerializeField] private SpriteRenderer _forwardArrow;
        [SerializeField] private SpriteRenderer _backwardArrow;
        [SerializeField] private float _arrowOffset;
        private float _colorChangeSpeed = 5f;

        private float _weight;
        private bool _isOneSided;
        private bool _wasPlaced;
        private Node _first;
        private Node _second;

        private bool _isChangingColor;
        
        public bool IsCustomWeight { get; private set; }
        public string FromNodeName => _first.NodeName;
        public string ToNodeName => _second.NodeName;

        public float Weight
        {
            get => _weight;
            set
            {
                IsCustomWeight = true;
                SetValueInMatrix(value, true);
                _weight = value;
            }
        }
        
        private ContextAction[] _contextAction;

        public bool IsOneSided
        {
            get => _isOneSided;
            set
            {
                _backwardArrow.gameObject.SetActive(!value);
                _isOneSided = value;
            }
        }

        private void Start()
        {
            void UpdateColorSpeed(SliderSettingPrefab slider) => _colorChangeSpeed = slider.CurrentValue;
            SettingsManager.AddEventOnSetting<SliderSettingPrefab>(
                GlobalStorage.SettingKeys.Controls.COLOR_CHANGE_SPEED, UpdateColorSpeed);
        }

        private void Awake()
        {
            _contextAction = new[] 
            {
                new ContextAction("Delete", DeleteEdge)
            };
        }

        [CanBeNull] public Node GetOppositeNode(Node node)
        {
            if (node == _first) return _second;
            else if (node == _second) return _first;
            else return null;
        }
        
        public void SetLenght(float value)
        {
            if (_isOneSided) value -= GlobalStorage.EdgeData.CROP_WHEN_ONE_SIDED;
            else value -= GlobalStorage.EdgeData.CROP_WHEN_TWO_SIDED;
            if (value < 0) value = 0;
            
            _collider.size = new Vector2(value, _collider.size.y);
            _spriteRenderer.size = new Vector2(value, GlobalStorage.EdgeData.ARROW_WIDTH);
            _gradientRenderer.transform.localScale = new Vector3(GlobalStorage.EdgeData.ARROW_WIDTH, value, 1);

            var halfValue = value / 2;
            _forwardArrow.transform.localPosition = new Vector3(-halfValue + _arrowOffset, 0, 0);
            _backwardArrow.transform.localPosition = new Vector3(halfValue - _arrowOffset, 0, 0);
        }
        
        public void AdjustEdge(Vector2 start, Vector2 end)
        {
            Vector3 dir = start - end;
            var newPosition = (end + start) / 2;
            if (_isOneSided) newPosition += (Vector2)dir.normalized * GlobalStorage.EdgeData.OFFSET_WHEN_ONE_SIDED;
            transform.position = newPosition;
            
            var lenght = Vector2.Distance(end, start);
            SetLenght(lenght);
            
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        public void UpdateColor()
        {
            if (!_isChangingColor) ChangeColor().Forget();
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
        private void SetValueInMatrix(float value, bool updateStats)
        {
            var matrix = GameManager.Instance.AdjacencyMatrix;
            if (IsOneSided)
            {
                matrix.MakeOriented();
                matrix.SetValue(value, _first.NodeIndex, _second.NodeIndex, updateStats);
            }
            else if (matrix.IsOriented)
            {
                matrix.SetValue(value, _first.NodeIndex, _second.NodeIndex, false);
                matrix.SetValue(value, _second.NodeIndex, _first.NodeIndex, updateStats);
            }
            else
            {
                matrix.SetValue(value, _first.NodeIndex, _second.NodeIndex, updateStats);
            }
        }

        /// Sets all necessary values like nodes, weight etc.
        public void SetNodes(Node first, Node second, float weight, bool oneSided, bool updateStats=true)
        {
            _first = first;
            _second = second;
            IsOneSided = oneSided;
            _wasPlaced = true;

            IsCustomWeight = true;
            _weight = weight;
            SetValueInMatrix(_weight, updateStats);
            
            _first.AddLink(this);
            _second.AddLink(this);
        }
        
        public void SetNodes(Node first, Node second, bool oneSided, bool updateStats=true)
        {
            _first = first;
            _second = second;
            IsOneSided = oneSided;
            _wasPlaced = true;

            SetFallBackWeight(updateStats);
            
            _first.AddLink(this);
            _second.AddLink(this);
        }

        public void SetFallBackWeight(bool updateStats=true)
        {
            IsCustomWeight = false;
            var weight = Vector2.Distance(_first.transform.position, _second.transform.position);
            _weight = weight;
            SetValueInMatrix(_weight, updateStats);
        }

        public void Enable()
        {
            _collider.enabled = true;
            var c = _spriteRenderer.color;
            _spriteRenderer.color = new  Color(c.r, c.g, c.b, 1f);
        }
        
        public void Disable()
        {
            _collider.enabled = false;
            var c = _spriteRenderer.color;
            _spriteRenderer.color = new  Color(c.r, c.g, c.b, .5f);
        }

        /// <summary>
        /// Destroys itself but ignores given node, so node itself can delete all edges safely
        /// </summary>
        /// <param name="fromNode"> Node that deleted this edge </param>
        /// <param name="isSilent"> Will change value in Adjacency matrix if set true </param>
        public void CascadeDestroy(Node fromNode, bool isSilent=false)
        {
            if (_wasPlaced && !isSilent) SetValueInMatrix(0, true);
                
            if (fromNode != _first) _first.RemoveLink(this);
            if (fromNode != _second) _second.RemoveLink(this);
            
            GameManager.Instance.RemoveEdge(this);
            Destroy(gameObject);
        }

        /// <summary>
        /// Properly destroys itself, removing all links between edges
        /// </summary>
        public void DeleteEdge()
        {
            if (_wasPlaced) SetValueInMatrix(0, true);
            
            if (_first != null) _first.RemoveLink(this);
            if (_second != null) _second.RemoveLink(this);
            
            GameManager.Instance.RemoveEdge(this);
            Destroy(gameObject);
        }

        public SerializableEdge SerializeSelf() => new SerializableEdge(_weight, IsOneSided, 
            _first.NodeIndex, _second.NodeIndex, IsCustomWeight);
        
        public void DeserializeSelf(SerializableEdge serialized)
        {
            _weight = serialized._weight;
            _isOneSided = serialized._isOneSided;
            IsCustomWeight = serialized._isCustomWeight;
        }

        private async UniTask ChangeColor()
        {
            _isChangingColor = true;
            while (true)
            {
                var firstNodeColor = _first.NodeColor;
                var secondNodeColor = _second.NodeColor;
                var thisFrameChangeSpeed = _colorChangeSpeed * Time.deltaTime;
                var forwardColorStatus = _spriteRenderer.color.Distance(secondNodeColor) < GlobalStorage.SNAP_DISTANCE;
                var backwardColorStatus = _gradientRenderer.color.Distance(firstNodeColor) < GlobalStorage.SNAP_DISTANCE;

                if (!forwardColorStatus)
                {
                    _spriteRenderer.color = Color.Lerp(_spriteRenderer.color, secondNodeColor, thisFrameChangeSpeed);
                    _forwardArrow.color = Color.Lerp(_forwardArrow.color, secondNodeColor, thisFrameChangeSpeed);
                }
                
                if (!backwardColorStatus)
                {
                    _gradientRenderer.color = Color.Lerp(_gradientRenderer.color, firstNodeColor, thisFrameChangeSpeed);
                    _backwardArrow.color = Color.Lerp(_backwardArrow.color, firstNodeColor, thisFrameChangeSpeed);
                }
                
                if (forwardColorStatus && backwardColorStatus) break;
                await UniTask.NextFrame(destroyCancellationToken);
            }

            _spriteRenderer.color = _second.NodeColor;
            _forwardArrow.color = _second.NodeColor;
            _gradientRenderer.color = _first.NodeColor;
            _backwardArrow.color = _first.NodeColor;
            _isChangingColor = false;
        }
    }
}