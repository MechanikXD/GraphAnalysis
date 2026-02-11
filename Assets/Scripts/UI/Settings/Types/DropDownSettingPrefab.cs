using Core.LoadSystem;
using TMPro;
using UnityEngine;

namespace UI.Settings.Types
{
    public class DropDownSettingPrefab : SettingPrefab
    {
        [SerializeField] private TMP_Dropdown _dropdown;
        public string[] Options { get; private set; }
        public int CurrentOption { get; private set; }

        private void OnEnable()
        {
            _dropdown.onValueChanged.AddListener(OnSwitch);
        }
        
        private void OnDisable()
        {
            _dropdown.onValueChanged.RemoveListener(OnSwitch);
        }

        public void SilentSwitch(int option)
        {
            _dropdown.SetValueWithoutNotify(option);
            CurrentOption = option;
        }

        private void OnSwitch(int option)
        {
            CurrentOption = option;
            SettingsChanged();
        }

        public void Load(string entryKey, string[] values)
        {
            Options = values;
            Title = entryKey;
            _lse.SetEntry(entryKey);
            _lse.RefreshString();

            foreach (var value in values)
            {
                _dropdown.options.Add(new TMP_Dropdown.OptionData(value));    
            }

            var optionIndex = 0;
            if (SaveManager.HasSetting(Title))
            {
                var option = SaveManager.GetSetting<int>(Title);
                for (var i = 0; i < Options.Length; i++)
                {
                    if (option != i) continue;

                    optionIndex = i;
                    break;
                }
            }
            
            _dropdown.value = optionIndex;
            CurrentOption = optionIndex;
        }
        
        public override void WriteChangesInStorage() => SaveManager.SetSetting(Title, CurrentOption);
    }
}