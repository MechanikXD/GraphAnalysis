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
        private IEnumerator Start()
        {
            var settingKey = GlobalStorage.SettingKeys.Display.LANGUAGE;
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[
                    SettingsManager.GetSetting<DropDownSettingPrefab>(settingKey).CurrentOption];
            SettingsManager.AddEventOnSetting<DropDownSettingPrefab>(
                settingKey, LocaleSelected);
            yield return LocalizationSettings.InitializationOperation;
            // call on start, because LocalizationSettings does not inform about current locale.
            OnLocaleChanged(LocalizationSettings.SelectedLocale);
        }

        private void OnEnable()
        {
            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        }

        private void OnDisable()
        {
            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        }
        
        private static void OnLocaleChanged(Locale newLocale)
        {
            var availableLocalesList = LocalizationSettings.AvailableLocales.Locales;
            var index = availableLocalesList.IndexOf(newLocale);
            SettingsManager.GetSetting<DropDownSettingPrefab>(
                GlobalStorage.SettingKeys.Display.LANGUAGE).SilentSwitch(index);
        }

        private static void LocaleSelected(DropDownSettingPrefab p) => 
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[p.CurrentOption];
    }
}