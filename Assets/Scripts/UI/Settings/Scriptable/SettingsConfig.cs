using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Settings.Scriptable
{
    [CreateAssetMenu(fileName = "Settings Config", menuName = "ScriptableObjects/Settings Config")]
    public class SettingsConfig : ScriptableObject
    {
        [SerializeField] private SettingGroup[] _settingGroups;
        public SettingGroup[] SettingGroups =>  _settingGroups;
    }

    [Serializable] 
    public class SettingGroup
    {
        [SerializeField] private string _groupTitle;
        [SerializeReference] private List<SettingData> _settings = new List<SettingData>();
        
        public string GroupTitle => _groupTitle;

        public Dictionary<string, SettingPrefab> GetSettings()
        {
            var created = new Dictionary<string, SettingPrefab>();

            foreach (var settingData in _settings)
            {
                var newSetting = settingData.Create();
                created.Add(settingData.SettingName, newSetting);
            }
            
            return created;
        }
    }
}