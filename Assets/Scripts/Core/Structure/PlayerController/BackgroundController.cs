using Other;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Structure.PlayerController
{
    public class BackgroundController : MonoBehaviour, IDragHandler, IScrollHandler
    {
        [SerializeField] private Transform _mapRoot;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        
        [SerializeField] private bool _preserveBounds = true;
        [SerializeField] private float _dragSpeed;
        
        [SerializeField] private float _scrollSpeed;
        [SerializeField] private Vector2 _scrollBounds;
        
        private Vector2 _cameraHalfViewSize;
        private Vector2 _xBounds;
        private Vector2 _yBounds;

        private void Awake()
        {
            var cam = Camera.main;
            var height = cam!.orthographicSize;
            var width = height * cam.aspect;
            _cameraHalfViewSize = new Vector2(width, height);
            
            _spriteRenderer = GetComponent<SpriteRenderer>();
            UpdateBounds();
        }

        private void MoveRoot(Vector2 newPos)
        {
            newPos.x = _xBounds.Clamp(newPos.x);
            newPos.y = _yBounds.Clamp(newPos.y);
            
            _mapRoot.position = newPos;
        }

        private void ScaleRoot(float newScale)
        {
            newScale = _scrollBounds.Clamp(newScale);
            _mapRoot.localScale = new Vector3(newScale, newScale, 1f);
        }

        private void UpdateBounds()
        {
            var scale = _mapRoot.localScale.x;

            var halfWidth = _spriteRenderer.size.x * scale / 2f;
            var halfHeight = _spriteRenderer.size.y * scale / 2f;

            _xBounds = new Vector2(-halfWidth + _cameraHalfViewSize.x, halfWidth - _cameraHalfViewSize.x);
            _yBounds = new Vector2(-halfHeight + _cameraHalfViewSize.y, halfHeight - _cameraHalfViewSize.y);
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            Vector3 delta = -eventData.delta;
            MoveRoot(_mapRoot.transform.position - new Vector3(delta.x, delta.y, 0) * _dragSpeed);
        }

        public void OnScroll(PointerEventData eventData)
        {
            ScaleRoot(_mapRoot.localScale.x + eventData.scrollDelta.y * _scrollSpeed);
            UpdateBounds();
            MoveRoot(_mapRoot.position);
        }
    }
}