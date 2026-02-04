using System;
using System.Collections.Generic;
using Core.Behaviour;
using Core.LoadSystem;
using Other;
using UI.Settings;
using UI.Settings.Scriptable;
using UnityEngine;

namespace Core.Structure
{
    public class SettingsManager : SingletonBase<SettingsManager>
    {
        [SerializeField] private bool _clearSettingOnStartup;
        [SerializeField] private SettingGroupTab _groupTabPrefab;
        [SerializeField] private SettingsConfig _settingConfig;
        private static Dictionary<string, SettingPrefab> _settings = new Dictionary<string, SettingPrefab>();
        
        private List<SettingGroupTab> _groupTabs = new List<SettingGroupTab>();
        private int _currentGroupIndex;

        protected override void Awake()
        {
            ToSingleton(true);
            if (_clearSettingOnStartup) SaveManager.DeleteAll();
            Initialize();
        }

        public static T GetSetting<T>(string settingName) where T : SettingPrefab => (T)_settings[settingName];

        public static void AddEventOnSetting<T>(string settingName, Action<T> action) where T : SettingPrefab
        {
            if (_settings.TryGetValue(settingName, out var setting))
            {
                setting.OnSettingChanged += prefab =>
                {
                    if (prefab is T typed) action(typed);
                };
            }
            else Debug.LogError($"{settingName} does not exist in setting");
        }

        public void SwitchGroup(int index)
        {
            if (index == _currentGroupIndex) return;
            
            _groupTabs[_currentGroupIndex].HideGroup();
            _currentGroupIndex = index;
            _groupTabs[_currentGroupIndex].ShowGroup();
        }

        public void SaveChangedSettings()
        {
            foreach (var settingGroupTab in _groupTabs)
            {
                foreach (var changedSetting in settingGroupTab.GetChangedSettings())
                {
                    changedSetting.WriteChangesInStorage();
                }
            }
            
            SaveManager.SaveSettings();
            InfoFeed.Instance.LogInfo(GlobalStorage.InfoKeys.LOG_SETTINGS_UPDATED);
        }

        public void CreateGroups(Transform contentRoot, Transform groupTabRoot)
        {
            _groupTabs = new List<SettingGroupTab>();
            _settings = new Dictionary<string, SettingPrefab>();
            _currentGroupIndex = 0;
            var firstDisplayed = false;
            for (var i = 0; i < _settingConfig.SettingGroups.Length; i++)
            {
                // Create new group tab
                var newGroup = Instantiate(_groupTabPrefab, groupTabRoot);
                newGroup.SetRoot(contentRoot, i);
                _groupTabs.Add(newGroup);

                // Create settings for this group
                var createdSettings = newGroup.LoadSettings(_settingConfig.SettingGroups[i]);
                foreach (var setting in createdSettings)
                {
                    _settings.Add(setting.Key, setting.Value);
                }
                
                // Enable only first tab
                if (!firstDisplayed)
                {
                    newGroup.ShowGroup();
                    _currentGroupIndex = i;
                    firstDisplayed = true;
                }
                else newGroup.HideGroup();
            }
            
            foreach (var kvp in _settings) kvp.Value.SettingsChanged();
        }
    }
}