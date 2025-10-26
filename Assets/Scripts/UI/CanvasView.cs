using UnityEngine;

namespace UI
{
    public abstract class CanvasView : MonoBehaviour
    {
        [SerializeField] private bool _hideOnStart;
        protected Canvas ThisCanvas;
        public bool HideOnStart => _hideOnStart;
        public bool IsEnabled => ThisCanvas.enabled;

        private void Awake()
        {
            ThisCanvas = GetComponent<Canvas>();
            Initialize();
        }
        protected virtual void Initialize() {}
        
        public void Show() => ThisCanvas.enabled = true;
        public void Hide() => ThisCanvas.enabled = false;
    }
}