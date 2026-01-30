using System.Collections.Generic;
using Core.Structure;
using UI.Settings.Scriptable;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace UI.Settings
{
    public class SettingGroupTab : MonoBehaviour
    {
        [SerializeField] private Button _groupButton;
        [SerializeField] private LocalizeStringEvent _lse;
        private Transform _content;
        private readonly List<SettingPrefab> _loadedSettings = new List<SettingPrefab>();
        public int GroupIndex { get; private set; }

        private void OnEnable()
        {
            _groupButton.onClick.AddListener(SwitchToThisGroup);
        }

        private void OnDisable()
        {
            _groupButton.onClick.RemoveListener(SwitchToThisGroup);
        }

        private void SwitchToThisGroup() => SettingsManager.Instance.SwitchGroup(GroupIndex);

        public void SetRoot(Transform root, int groupIndex)
        {
            _content = root;
            GroupIndex = groupIndex;
        }

        public Dictionary<string, SettingPrefab> LoadSettings(SettingGroup settingGroup)
        {
            _lse.SetEntry(settingGroup.GroupTitle);
            _lse.RefreshString();
            var settings = settingGroup.GetSettings();
            foreach (var setting in settings)
            {
                _loadedSettings.Add(setting.Value);
                setting.Value.transform.SetParent(_content);
                setting.Value.transform.localScale = Vector3.one;
            }

            return settings;
        }

        public List<SettingPrefab> GetChangedSettings()
        {
            var changed = new List<SettingPrefab>();
            foreach (var setting in _loadedSettings)
            {
                if (!setting.WasChanged) continue;

                changed.Add(setting);
                setting.ClearChanged();
            }
            
            return changed;
        }

        public void ShowGroup()
        {
            foreach (var setting in _loadedSettings)
            {
                setting.gameObject.SetActive(true);
            }
        }

        public void HideGroup()
        {
            foreach (var setting in _loadedSettings)
            {
                setting.gameObject.SetActive(false);
            }
        }
    }
}