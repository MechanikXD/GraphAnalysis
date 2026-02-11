using UI.Settings.Types;
using UnityEngine;

namespace UI.Settings.Scriptable.Data
{
    public class ColorSettingData : SettingData<ColorSettingPrefab>
    {
        [SerializeField] private Color _defaultColor;
        
        public override ColorSettingPrefab CreateTyped(ColorSettingPrefab prefab)
        {
            var slider = Object.Instantiate(prefab);
            slider.Load(SettingNameEntry, _defaultColor);
            return slider;
        }
    }
}