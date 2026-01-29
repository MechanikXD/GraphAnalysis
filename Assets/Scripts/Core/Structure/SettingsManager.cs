using System;
using System.Collections.Generic;
using Core.Behaviour;
using Core.LoadSystem;
using UI.Settings;
using UI.Settings.Scriptable;
using UnityEngine;

namespace Core.Structure
{
    public class SettingsManager : SingletonBase<SettingsManager>
    {
        [SerializeField] private SettingGroupTab _groupTabPrefab;
        [SerializeField] private SettingsConfig _settingConfig;
        private readonly static Dictionary<string, SettingPrefab> Settings = new Dictionary<string, SettingPrefab>();
        
        private readonly List<SettingGroupTab> _groupTabs = new List<SettingGroupTab>();
        private int _currentGroupIndex;

        protected override void Awake()
        {
            ToSingleton(true);
            Initialize();
        }
        
        public static void AddEventOnSetting(string settingName, Action<SettingPrefab> action)
        {
            if (Settings.TryGetValue(settingName, out var setting))
            {
                setting.OnSettingChanged += action;
            }
            else
            {
                Debug.LogError($"{settingName} does not exist in setting");
            }
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
        }

        public void CreateGroups(Transform contentRoot, Transform groupTabRoot)
        {
            _groupTabs.Clear();
            _currentGroupIndex = 0;
            var firstDisplayed = false;
            var loadSettings = Settings.Count <= 0;
            for (var i = 0; i < _settingConfig.SettingGroups.Length; i++)
            {
                // Create new group tab
                var newGroup = Instantiate(_groupTabPrefab, groupTabRoot);
                newGroup.SetRoot(contentRoot, i);
                _groupTabs.Add(newGroup);

                // Create settings for this group
                var createdSettings = newGroup.LoadSettings(_settingConfig.SettingGroups[i]);
                // Load settings into dictionary if it's empty
                if (loadSettings)
                    foreach (var setting in createdSettings)
                        Settings.Add(setting.Key, setting.Value);
                
                // Enable only first tab
                if (!firstDisplayed)
                {
                    newGroup.ShowGroup();
                    _currentGroupIndex = i;
                    firstDisplayed = true;
                }
                else newGroup.HideGroup();
            }
            
            foreach (var kvp in Settings) kvp.Value.SettingsChanged();
        }
    }
}