using System;
using System.Collections.Generic;
using Core.Behaviour;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class UIManager : SingletonBase<UIManager>
    {
        private readonly Dictionary<Type, CanvasView> _uiCanvases = new Dictionary<Type, CanvasView>();
        private readonly Dictionary<Type, CanvasView> _hudCanvases = new Dictionary<Type, CanvasView>();

        [SerializeField] private bool _stopTimeScaleOnPause;
        [SerializeField] private GameObject _sceneInputBlock;
        [SerializeField] private CanvasView[] _sceneUiCanvases;
        [SerializeField] private CanvasView[] _sceneHudCanvases;

        private readonly Stack<CanvasView> _uiStack = new Stack<CanvasView>();
        public bool HasOpenedUI { get; private set; }

        protected override void Initialize() {
            ExitPauseState();
            InitializeCanvases();
        }
        
        public static bool IsPointerOverUI(Vector2 screenPosition)
        {
            var eventSystem = EventSystem.current;
            if (eventSystem == null)
            {
                Debug.LogError("EventSystem is null");
                return false;
            }
            
            var pointerEventData = new PointerEventData(eventSystem) { position = screenPosition };
            var results = new List<RaycastResult>();
            eventSystem.RaycastAll(pointerEventData, results);
            
            foreach (var result in results)
            {
                if (result.module is GraphicRaycaster) return true;
            }

            return false;
        }

        public void ShowUI<T>() where T : CanvasView {
            if (!HasOpenedUI) EnterPauseState();
            
            if (_uiStack.Count > 0) _uiStack.Peek().Hide();
            var canvas = GetUICanvas<T>();
            _uiStack.Push(canvas);
            HasOpenedUI = true;
            canvas.Show();
        }

        public void ExitLastCanvas() {
            if (_uiStack.Count > 0) _uiStack.Pop().Hide();
            
            if (_uiStack.Count > 0) _uiStack.Peek().Show();
            else ExitPauseState();
        }
        
        public void ShowHUD<T>() where T : CanvasView {
            var hud = GetHUDCanvas<T>();
            if (!hud.IsEnabled) hud.Show();
        }

        public void HideHUD<T>() where T : CanvasView {
            if (_hudCanvases.TryGetValue(typeof(T), out var hud) && hud.IsEnabled) hud.Hide();
        }
        
        private void EnterPauseState() {
            HasOpenedUI = true;
            if (_stopTimeScaleOnPause) Time.timeScale = 0f;
            _sceneInputBlock.SetActive(true);
        }

        private void ExitPauseState() {
            HasOpenedUI = false;
            if (_stopTimeScaleOnPause) Time.timeScale = 1f;
            _sceneInputBlock.SetActive(false);
        }

        public T GetUICanvas<T>() where T : CanvasView => (T)_uiCanvases[typeof(T)];
        public T GetHUDCanvas<T>() where T : CanvasView => (T)_hudCanvases[typeof(T)];
        
        private void InitializeCanvases() {
            // Initialize first
            foreach (var hudCanvas in _sceneHudCanvases) {
                hudCanvas.Initialize();
                _hudCanvases.Add(hudCanvas.GetType(), hudCanvas);
            }
            foreach (var uiCanvas in _sceneUiCanvases) {
                uiCanvas.Initialize();
                _uiCanvases.Add(uiCanvas.GetType(), uiCanvas);
            }
            // Then show/hide
            foreach (var hudCanvas in _sceneHudCanvases) {
                if (hudCanvas.HideOnStart) hudCanvas.Hide(true);
                else hudCanvas.Show(true);
            }
            foreach (var uiCanvas in _sceneUiCanvases) {
                if (uiCanvas.HideOnStart) uiCanvas.Hide(true);
                else uiCanvas.Show(true);
            }
        }
    }
}