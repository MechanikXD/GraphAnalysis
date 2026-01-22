using Cysharp.Threading.Tasks;
using Other;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Structure.PlayerController
{
    public class BackgroundController : MonoBehaviour, IDragHandler, IScrollHandler
    {
        private const float SCALE_SNAP_DISTANCE = 0.01f;
        private static bool _enabled;
        
        [SerializeField] private bool _preserveBounds = true;
        [SerializeField] private float _dragSpeed;
        
        [SerializeField] private float _scrollSpeed;
        [SerializeField] private float _scaleLerpSpeed;
        [SerializeField] private Vector2 _scrollBounds;
        
        private SpriteRenderer _spriteRenderer;
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
            Vector3 delta = -eventData.delta * _dragSpeed * Time.deltaTime;
            MoveCamera(_camera.transform.position + new Vector3(delta.x, delta.y, 0f));
        }

        public void OnScroll(PointerEventData eventData)
        {
            if (!_enabled) return;
            ZoomCamera(_camera.orthographicSize - eventData.scrollDelta.y * _scrollSpeed);
        }

        private async UniTask ZoomCameraToDesired()
        {
            while (Mathf.Abs(_desiredOrthoSize - _camera.orthographicSize) > SCALE_SNAP_DISTANCE)
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
    }
}