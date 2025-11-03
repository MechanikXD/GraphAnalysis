using UnityEngine;

namespace UI
{
    public abstract class CanvasView : MonoBehaviour
    {
        [SerializeField] private bool _hideOnStart;
        protected Canvas ThisCanvas;
        public bool HideOnStart => _hideOnStart;
        public bool IsEnabled { get; protected set; }

        private void Awake()
        {
            ThisCanvas = GetComponent<Canvas>();
            Initialize();
        }
        protected virtual void Initialize() {}
        
        public virtual void Show()
        {
            ThisCanvas.enabled = true;
            IsEnabled = true;
        }

        public virtual void Hide()
        {
            ThisCanvas.enabled = false;
            IsEnabled = false;
        }
    }
}