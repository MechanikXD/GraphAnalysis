using System;
using UnityEngine;

namespace UI.Settings.Scriptable
{
    [Serializable]
    public abstract class SettingData
    {
        [SerializeField] protected SettingPrefab _prefab;
        [SerializeField] private string _settingName;
        public string SettingName => _settingName;

        public abstract SettingPrefab Create();
    }
    
    [Serializable]
    public abstract class SettingData<T> : SettingData where T : SettingPrefab
    {
        public abstract T CreateTyped(T prefab);
        
        public sealed override SettingPrefab Create()
        {
            if (_prefab is not T typedPrefab)
            {
                Debug.LogError($"{SettingName} expected prefab of type {typeof(T).Name}, got {_prefab.GetType().Name}");
                return null;
            }
            
            return CreateTyped(typedPrefab);
        }
    }
}