using System.Collections;
using Core.Structure;
using UI.Settings.Types;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Other
{
    public class LocalizationLoader : MonoBehaviour
    {
        [SerializeField] private string _localizationSettingKey = "Language";

        private IEnumerator Start()
        {
            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
            SettingsManager.AddEventOnSetting<DropDownSettingPrefab>(_localizationSettingKey, LocaleSelected);
            yield return LocalizationSettings.InitializationOperation;
            // call on start, because LocalizationSettings does not inform about current locale.
            OnLocaleChanged(LocalizationSettings.SelectedLocale);
        }

        private void OnDisable()
        {
            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
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