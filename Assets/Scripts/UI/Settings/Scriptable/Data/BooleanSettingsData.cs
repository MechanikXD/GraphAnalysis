using System;
using UI.Settings.Types;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UI.Settings.Scriptable.Data
{
    [Serializable]
    public class BooleanSettingsData : SettingData<BooleanSettingPrefab>
    {
        [SerializeField] private bool _isOnByDefault;
        
        public override BooleanSettingPrefab CreateTyped(BooleanSettingPrefab prefab)
        {
            var boolean = Object.Instantiate(prefab);
            boolean.Load(SettingName, _isOnByDefault);
            return boolean;
        }
    }
}