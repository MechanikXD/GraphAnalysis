using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Other;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Structure.PlayerController
{
    public class BackgroundController : MonoBehaviour, IDragHandler, IScrollHandler
    {
        private static bool _enabled = true;
        
        [SerializeField] private bool _preserveBounds = true;
        [SerializeField] private Vector2 _dragSpeed;
        
        [SerializeField] private float _scrollSpeed;
        [SerializeField] private float _scaleLerpSpeed;
        [SerializeField] private Vector2 _scrollBounds;
        
        private SpriteRenderer _spriteRenderer;
        private BoxCollider2D _collider2D;
        private Camera _camera;
        private Vector2 _xBounds;
        private Vector2 _yBounds;
        private float _desiredOrthoSize;

        private void Awake()
        {
            _camera = Camera.main;
            if (_camera == null)
            {
                Debug.LogError("Main camera not found!");
                enabled = false;
                return;
            }
            
            _desiredOrthoSize = _camera.orthographicSize;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _collider2D = GetComponent<BoxCollider2D>();
            _collider2D.size = _spriteRenderer.size;
            UpdateBounds();
        }

        public static void Enable() => _enabled = true;
        public static void Disable() => _enabled = false;
        
        private void MoveCamera(Vector2 newPos)
        {
            if (_preserveBounds)
            {
                newPos.x = _xBounds.Clamp(newPos.x);
                newPos.y = _yBounds.Clamp(newPos.y);    
            }
            
            _camera.transform.position = new Vector3(newPos.x, newPos.y, _camera.transform.position.z);
        }

        private void ZoomCamera(float newScale)
        {
            newScale = _scrollBounds.Clamp(newScale);
            _desiredOrthoSize = newScale;
            ZoomCameraToDesired().Forget();
        }

        private float ScaledDragSpeed()
        {
            var relativeZoom = _desiredOrthoSize;
            relativeZoom -= _scrollBounds.x;
            relativeZoom /= _scrollBounds.y;

            return _dragSpeed.x + _dragSpeed.y * relativeZoom;
        }

        private void UpdateBounds()
        {
            var camHeight = _camera.orthographicSize;
            var camWidth = camHeight * _camera.aspect;

            var halfMapWidth = _spriteRenderer.size.x / 2f;
            var halfMapHeight = _spriteRenderer.size.y / 2f;

            _xBounds = new Vector2(-halfMapWidth + camWidth, halfMapWidth - camWidth);
            _yBounds = new Vector2(-halfMapHeight + camHeight, halfMapHeight - camHeight);
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if (!_enabled) return; 
            Vector3 delta = -eventData.delta * ScaledDragSpeed() * Time.deltaTime;
            MoveCamera(_camera.transform.position + new Vector3(delta.x, delta.y, 0f));
        }

        public void OnScroll(PointerEventData eventData)
        {
            if (!_enabled) return;
            ZoomCamera(_camera.orthographicSize - eventData.scrollDelta.y * _scrollSpeed);
        }

        private async UniTask ZoomCameraToDesired()
        {
            while (Mathf.Abs(_desiredOrthoSize - _camera.orthographicSize) > GlobalStorage.SCALE_SNAP_DISTANCE)
            {
                _camera.orthographicSize = Mathf.Lerp(
                    _camera.orthographicSize,
                    _desiredOrthoSize,
                    _scaleLerpSpeed * Time.deltaTime
                );

                UpdateBounds();
                MoveCamera(_camera.transform.position);
                await UniTask.NextFrame(destroyCancellationToken);
            }
            
            _camera.orthographicSize = _desiredOrthoSize;
        }

        public void ChangeBackground(Sprite newSprite)
        {
            var newSize = new Vector2
            {
                x = newSprite.texture.width / GlobalStorage.PIXEL_TO_UNIT_RATIO,
                y = newSprite.texture.height / GlobalStorage.PIXEL_TO_UNIT_RATIO
            };

            _spriteRenderer.sprite = newSprite;
            _spriteRenderer.size = newSize;
            _collider2D.size = newSize;
            UpdateBounds();
            MoveCamera(_camera.transform.position);
            
            // Delete nodes outside new boundaries
            var adjMatrix = GameManager.Instance.AdjacencyMatrix;
            var nodesToRemove = new List<int>();
            foreach (var node in adjMatrix.Nodes)
            {
                if (!WithinBounds(node.Position))
                {
                    nodesToRemove.Add(node.NodeIndex);
                    node.SilentDestroy(); // Will be marked for destroy but not destroyed yet
                }
            }
            adjMatrix.RemoveRange(nodesToRemove);
            if (nodesToRemove.Count > 0) InfoFeed.Instance.LogWarning(GlobalStorage.InfoKeys.WARNING_NODE_REMOVED);
            else InfoFeed.Instance.LogInfo(GlobalStorage.InfoKeys.LOG_BACKGROUND_UPDATED);
        }

        private bool WithinBounds(Vector2 pos)
        {
            var size = _spriteRenderer.size;
            var halfWidth = size.x / 2f;
            var halfHeight = size.y / 2f;
            return pos.x >= -halfWidth && pos.x <= halfWidth && pos.y >= -halfHeight &&
                   pos.y <= halfHeight;
        }
    }
}