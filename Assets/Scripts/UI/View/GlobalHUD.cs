using UI.InfoStructures;
using UnityEngine;
using UnityEngine.UI;

namespace UI.View
{
    public class GlobalHUD : CanvasView
    {
        [SerializeField] private Button _menuButton;

        private void OnEnable()
        {
            _menuButton.onClick.AddListener(ShowMenu);
        }

        private void OnDisable()
        {
            _menuButton.onClick.RemoveListener(ShowMenu);
        }

        private static void ShowMenu()
        {
            InfoView.ShowInfo<Menu>();
            UIManager.Instance.ShowHUD<InfoView>();
        }

        public void ActivateMenuButton(bool active) => _menuButton.gameObject.SetActive(active);
    }
}