using Core.Structure;
using UnityEngine;

namespace UI.UiStructures.MainMenuPages
{
    public class SettingsPage : MenuPage
    {
        [SerializeField] private Transform _contentRoot;
        [SerializeField] private Transform _groupTabRoot;

        protected override void Initialize()
        {
            base.Initialize();
            SettingsManager.Instance.CreateGroups(_contentRoot, _groupTabRoot);
        }

        public override void Hide()
        {
            SettingsManager.Instance.SaveChangedSettings();
            base.Hide();
        }
    }
}