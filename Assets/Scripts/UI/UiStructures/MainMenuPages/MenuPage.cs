using UnityEngine;

namespace UI.UiStructures.MainMenuPages
{
    public abstract class MenuPage : MonoBehaviour
    {
        [SerializeField] private bool _displayOnStart;
        public bool DisplayOnStart => _displayOnStart;
        
        public virtual void Initialize() {}

        public virtual void Hide() => gameObject.SetActive(false);
        public virtual void Show() => gameObject.SetActive(true);
    }
}