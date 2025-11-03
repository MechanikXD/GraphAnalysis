using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI.View
{
    public class ContextWindow : CanvasView
    {
        [SerializeField] private Vector2 _offset;
        [SerializeField] private Transform _content;
        [SerializeField] private ButtonView _buttonPrefab;
        private bool _hasContent;
        private List<Action> _unsubscribers;

        protected override void Initialize()
        {
            _unsubscribers = new List<Action>();
        }

        public void SetPosition(Vector2 position)
        {
            _content.transform.position = position + _offset;
        }
        
        public void ClearContent()
        {
            foreach(var unsubscriber in _unsubscribers) unsubscriber();
            _unsubscribers.Clear();
            
            for (var i = _content.childCount - 1; i >= 0; i--)
            {
                Destroy(_content.GetChild(i).gameObject);
            }
            _hasContent = false;
        }

        public void LoadContext(ContextAction[] actions)
        {
            if (_hasContent) ClearContent();
            
            foreach (var action in actions)
            {
                var newButton = Instantiate(_buttonPrefab, _content);
                void Listener()
                {
                    action.Action();
                    Hide();
                    ClearContent();
                }

                newButton.Button.onClick.AddListener(Listener);
                newButton.SetTitle(action.Title);
                _unsubscribers.Add(() => newButton.Button.onClick.RemoveListener(Listener));
            }
            _hasContent = true;
        }
    }

    public class ContextAction
    {
        public string Title { get; }
        public Action Action { get; }

        public ContextAction(string title, Action action)
        {
            Title = title;
            Action = action;
        }
    }
}