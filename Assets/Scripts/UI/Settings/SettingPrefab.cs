using System;
using TMPro;
using UnityEngine;

namespace UI.Settings
{
    public abstract class SettingPrefab : MonoBehaviour
    {
        [SerializeField] protected TMP_Text _titleField;
        protected string Title;
        public bool WasChanged { get; private set; }
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