using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Structure.PlayerController
{
    public class TempNodesController : MonoBehaviour, IDragHandler
    {
        [SerializeField] private Transform _tempRoot;
        [SerializeField] private float _dragSpeed;
        private static bool _enabled;

        public static void Enable() => _enabled = true;
        public static void Disable() => _enabled = false;
        
        private void Start()
        {
            _tempRoot = GameManager.Instance.TempRoot;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_enabled) return;
            Vector3 delta = eventData.delta * _dragSpeed * Time.deltaTime;
            _tempRoot.transform.position += new Vector3(delta.x, delta.y, 0f);
        }
    }
}