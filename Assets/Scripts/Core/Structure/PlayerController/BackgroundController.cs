using System.Collections;
using Other;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Structure.PlayerController
{
    public class BackgroundController : MonoBehaviour, IDragHandler, IScrollHandler
    {
        private const float SCALE_SNAP_DISTANCE = 0.01f;
        
        [SerializeField] private Transform _mapRoot;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        
        [SerializeField] private bool _preserveBounds = true;
        [SerializeField] private float _dragSpeed;
        
        [SerializeField] private float _scrollSpeed;
        [SerializeField] private float _scaleLerpSpeed;
        [SerializeField] private Vector2 _scrollBounds;
        
        private Vector2 _cameraHalfViewSize;
        private Vector2 _xBounds;
        private Vector2 _yBounds;
        private Vector3 _desiredScale;
        private Coroutine _coroutine;

        private void Awake()
        {
            var cam = Camera.main;
            var height = cam!.orthographicSize;
            var width = height * cam.aspect;
            _cameraHalfViewSize = new Vector2(width, height);
            _desiredScale = Vector3.one;
            
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
            _desiredScale = new Vector3(newScale, newScale, 1f);
            _coroutine ??= StartCoroutine(ScaleRootToDesired());
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
        }

        // TODO: Switch to UniTasks
        private IEnumerator ScaleRootToDesired()
        {
            var currentScale = _mapRoot.localScale;
            while (Mathf.Abs(_desiredScale.x - currentScale.x) > SCALE_SNAP_DISTANCE)
            {
                currentScale = Vector3.Lerp(currentScale, _desiredScale, _scaleLerpSpeed * Time.deltaTime);
                _mapRoot.localScale = currentScale;
                UpdateBounds();
                MoveRoot(_mapRoot.position);
                yield return null;
            }
            
            _mapRoot.localScale = _desiredScale;
            _coroutine = null;
        }
    }
}