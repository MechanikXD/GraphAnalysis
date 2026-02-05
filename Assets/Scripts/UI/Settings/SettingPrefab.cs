using System;
using UnityEngine;
using UnityEngine.Localization.Components;

namespace UI.Settings
{
    public abstract class SettingPrefab : MonoBehaviour
    {
        [SerializeField] protected LocalizeStringEvent _lse;
        protected string Title;
        public bool WasChanged { get; protected set; }
        public void ClearChanged() => WasChanged = false;
        public abstract void WriteChangesInStorage();
        // Event that invokes all changes.
        public event Action<SettingPrefab> OnSettingChanged;
        // Method to invoke event. Called within settings when value changed.
        internal void SettingsChanged()
        {
            OnSettingChanged?.Invoke(this);
            WasChanged = true;
        }
    }
}