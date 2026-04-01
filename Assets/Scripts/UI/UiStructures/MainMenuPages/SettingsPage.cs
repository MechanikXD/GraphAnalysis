using Core.Structure;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UI.UiStructures.MainMenuPages
{
    public class SettingsPage : MenuPage
    {
        [SerializeField] private Transform _contentRoot;
        [SerializeField] private Transform _groupTabRoot;

        public override void Initialize()
        {
            SettingsManager.Instance.CreateGroups(_contentRoot, _groupTabRoot);
        }

        private void Start() => SettingsManager.Instance.InvokeSettingsAfterStart().Forget();

        public override void Hide()
        {
            SettingsManager.Instance.SaveChangedSettings();
            base.Hide();
        }
    }
}