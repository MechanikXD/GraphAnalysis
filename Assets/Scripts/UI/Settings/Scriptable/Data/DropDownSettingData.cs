using System;
using UI.Settings.Types;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UI.Settings.Scriptable.Data
{
    [Serializable]
    public class DropDownSettingData : SettingData<DropDownSettingPrefab>
    {
        [SerializeField] private string[] _values;
        
        public override DropDownSettingPrefab CreateTyped(DropDownSettingPrefab prefab) 
        {
            var dropDown = Object.Instantiate(prefab);
            dropDown.Load(SettingName, _values);
            return dropDown;
        }
    }
}