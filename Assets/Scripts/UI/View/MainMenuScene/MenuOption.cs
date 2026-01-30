using System;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace UI.View.MainMenuScene
{
    public class MenuOption : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _icon;
        [SerializeField] private LocalizeStringEvent _lse;
        private Action _buttonUnsubscribe;
        
        public void Load(Sprite icon, string entryKey, Action onOptionPress)
        {
            _icon.sprite = icon;
            _lse.SetEntry(entryKey);
            _lse.RefreshString();

            void ButtonAction() => onOptionPress();
            _button.onClick.AddListener(ButtonAction);
            _buttonUnsubscribe = () => _button.onClick.RemoveListener(ButtonAction);
        }

        public void Clear()
        {
            _icon.sprite = null;
            _lse.SetEntry(string.Empty);
            _lse.RefreshString();
            _buttonUnsubscribe();
            _buttonUnsubscribe = () => { };
        }
    }
}