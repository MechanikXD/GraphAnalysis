using System.Collections;
using Core.Structure;
using UI.Settings.Types;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

namespace Other
{
    public class LocalizationController : MonoBehaviour
    {
        [SerializeField] private string _localizationSettingKey = "Language";

        private IEnumerator Start()
        {
            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
            SceneManager.activeSceneChanged += (_, _) => StartCoroutine(InitializeSetting());
            yield return InitializeSetting();
        }

        private IEnumerator InitializeSetting()
        {
            SettingsManager.AddEventOnSetting<DropDownSettingPrefab>(_localizationSettingKey, LocaleSelected);
            yield return LocalizationSettings.InitializationOperation;
            OnLocaleChanged(LocalizationSettings.SelectedLocale);
        }

        private void OnLocaleChanged(Locale newLocale)
        {
            var availableLocalesList = LocalizationSettings.AvailableLocales.Locales;
            var index = availableLocalesList.IndexOf(newLocale);
            SettingsManager.GetSetting<DropDownSettingPrefab>(_localizationSettingKey).SilentSwitch(index);
        }

        private static void LocaleSelected(DropDownSettingPrefab p) => 
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[p.CurrentOption];
    }
}