using Core.LoadSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Settings.Types
{
    public class BooleanSettingPrefab : SettingPrefab
    {
        [SerializeField] private Toggle _switch;
        private string _name;
        private bool _defaultValue;
        public bool IsOn { get; private set; }

        private void OnEnable() => _switch.onValueChanged.AddListener(OnSwitch);

        private void OnDisable() => _switch.onValueChanged.RemoveListener(OnSwitch);
        
        private void OnSwitch(bool isOn)
        {
            IsOn = isOn;
            SettingsChanged();
        }

        public void Load(string settingName, bool value)
        {
            _titleField.SetText(settingName);
            Title = settingName;
            _defaultValue = value;
            IsOn = SaveManager.HasSetting(Title) ? SaveManager.GetSetting<bool>(Title) : _defaultValue;
            _switch.isOn = IsOn;
        }
        
        public override void WriteChangesInStorage() => SaveManager.SetSetting(Title, IsOn);
    }
}