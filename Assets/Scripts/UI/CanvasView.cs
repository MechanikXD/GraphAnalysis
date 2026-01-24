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
        
        public virtual void Show(bool isInitialHide=false)
        {
            ThisCanvas.enabled = true;
            IsEnabled = true;
        }

        public virtual void Hide(bool isInitialHide=false)
        {
            ThisCanvas.enabled = false;
            IsEnabled = false;
        }
    }
}