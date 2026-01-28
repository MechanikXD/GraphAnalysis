using System;
using System.Collections.Generic;
using UI.InfoStructures;
using UI.Prompts;
using UnityEngine;
using UnityEngine.UI;

namespace UI.View
{
    public class GlobalHUD : CanvasView
    {
        [SerializeField] private Button _menuButton;
        [SerializeField] private PromptBase[] _prompts;
        private Dictionary<Type, PromptBase> _promptGetters;

        protected override void Initialize()
        {
            base.Initialize();
            TypePrompt();
        }

        private void OnEnable()
        {
            _menuButton.onClick.AddListener(ShowMenu);
        }

        private void OnDisable()
        {
            _menuButton.onClick.RemoveListener(ShowMenu);
        }

        public T GetPrompt<T>() where T : PromptBase => (T)_promptGetters[typeof(T)];
        
        private static void ShowMenu()
        {
            InfoView.ShowInfo<Menu>();
            UIManager.Instance.ShowHUD<InfoView>();
        }

        public void ActivateMenuButton(bool active) => _menuButton.gameObject.SetActive(active);

        private void TypePrompt()
        {
            _promptGetters = new Dictionary<Type, PromptBase>();
            foreach (var prompt in _prompts)
            {
                _promptGetters.Add(prompt.GetType(), prompt);
                if (prompt.gameObject.activeInHierarchy) prompt.HidePrompt();
            }
        }
    }
}