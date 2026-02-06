using Core.Structure;
using UnityEngine;
using UnityEngine.UI;

namespace UI.View.GraphScene
{
    public class SettingsView : CanvasView
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Transform _contentRoot;
        [SerializeField] private Transform _groupTabRoot;

        internal override void Initialize()
        {
            base.Initialize();
            SettingsManager.Instance.CreateGroups(_contentRoot, _groupTabRoot);
        }
        
        private void OnEnable() => _closeButton.onClick.AddListener(ExitUICanvas);

        private void OnDisable() => _closeButton.onClick.RemoveListener(ExitUICanvas);

        private static void ExitUICanvas()
        {
            SettingsManager.Instance.SaveChangedSettings();
            if (UIManager.Instance != null) UIManager.Instance.ExitLastCanvas();
        }
    }
}