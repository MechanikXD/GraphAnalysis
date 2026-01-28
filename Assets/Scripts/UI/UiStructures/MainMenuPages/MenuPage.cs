using UnityEngine;

namespace UI.UiStructures.MainMenuPages
{
    public abstract class MenuPage : MonoBehaviour
    {
        [SerializeField] private bool _displayOnStart;
        public bool DisplayOnStart => _displayOnStart;
        [SerializeField] private bool _initializeOnAwake;
        private bool _wasInitialized;
        
        protected void Awake()
        {
            if (_initializeOnAwake) Initialize();
        }
        protected virtual void Initialize() => _wasInitialized = true;

        public virtual void Hide() => gameObject.SetActive(false);
        public virtual void Show()
        {
            if (!_wasInitialized) Initialize();
            gameObject.SetActive(true);
        }
    }
}