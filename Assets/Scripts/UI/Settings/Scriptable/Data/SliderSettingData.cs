using System;
using UI.Settings.Types;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UI.Settings.Scriptable.Data
{
    [Serializable]
    public class SliderSettingData : SettingData<SliderSettingPrefab>
    {
        [SerializeField] private float _defaultValue;
        [SerializeField] private Vector2 _bounds;
        [SerializeField] private bool _wholeNumbers;

        public override SliderSettingPrefab CreateTyped(SliderSettingPrefab prefab)
        {
            var slider = Object.Instantiate(prefab);
            slider.Load(SettingName, _defaultValue, _bounds, _wholeNumbers);
            return slider;
        }
    }
}