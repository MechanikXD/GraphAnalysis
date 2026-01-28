using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.View.MainMenuScene
{
    public class MenuOption : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private UnityEvent _action;
        private Action _buttonUnsubscribe;
        
        public void Load(Sprite icon, string title, Action onOptionPress)
        {
            _icon.sprite = icon;
            _title.SetText(title);

            void ButtonAction() => onOptionPress();
            _button.onClick.AddListener(ButtonAction);
            _buttonUnsubscribe = () => _button.onClick.RemoveListener(ButtonAction);
        }

        public void Clear()
        {
            _icon.sprite = null;
            _title.SetText(string.Empty);
            _buttonUnsubscribe();
            _buttonUnsubscribe = () => { };
        }
    }
}