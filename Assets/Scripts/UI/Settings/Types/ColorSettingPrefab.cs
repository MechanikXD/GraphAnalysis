using Core.LoadSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Settings.Types
{
    public class ColorSettingPrefab : SettingPrefab
    {
        [SerializeField] private Button _colorPickButton;
        [SerializeField] private Image _colorImage;
        [SerializeField] private GameObject _pickerRoot;
        [SerializeField] private FlexibleColorPicker _fcp;
        public Color CurrentColor { get; private set; }

        private void OnEnable()
        {
            _colorPickButton.onClick.AddListener(ShowRoot);
            _fcp.onColorChange.AddListener(OnColorChanged);
        }
        
        private void OnDisable()
        {
            _colorPickButton.onClick.RemoveListener(ShowRoot);
            _fcp.onColorChange.RemoveListener(OnColorChanged);
        }

        private void OnColorChanged(Color newColor)
        {
            CurrentColor = newColor;
            _colorImage.color = newColor;
            WasChanged = true;
        }

        public void ConfirmColor()
        {
            if (WasChanged) SettingsChanged();
            HideRoot();
        }

        private void ShowRoot() => _pickerRoot.SetActive(true);
        private void HideRoot() => _pickerRoot.SetActive(false);

        public void Load(string entryKey, Color defaultColor)
        {
            _lse.SetEntry(entryKey);
            _lse.RefreshString();
            Title = entryKey;
            CurrentColor = SaveManager.HasSetting(Title) 
                ? StringToColor(SaveManager.GetSetting<string>(Title)) : defaultColor;

            _fcp.color = CurrentColor;
            _colorImage.color = CurrentColor;
            HideRoot();
        }

        private string ColorToString() =>
            $"{CurrentColor.r}:{CurrentColor.g}:{CurrentColor.b}:{CurrentColor.a}";

        private static Color StringToColor(string color)
        {
            var values = color.Split(':');
            return new Color(float.Parse(values[0]), float.Parse(values[1]), 
                float.Parse(values[2]), float.Parse(values[3]));
        }

        public override void WriteChangesInStorage() => SaveManager.SetSetting(Title, ColorToString());
    }
}