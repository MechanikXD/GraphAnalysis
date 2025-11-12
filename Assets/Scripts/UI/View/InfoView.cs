using System;
using System.Collections;
using System.Collections.Generic;
using UI.InfoStructures;
using UnityEngine;

namespace UI.View
{
    public class InfoView : CanvasView
    {
        private const float SNAP_DISTANCE = 5f;
        [SerializeField] private Info[] _infos;
        [SerializeField] private Transform _content;
        [SerializeField] private Vector2 _hiddenPosition;
        [SerializeField] private float _lerpSpeed;
        private readonly Vector2 _shownPosition = new  Vector2(540f, 0f);
        private Coroutine _coroutine;
        private static Info _currentInfo;

        private static Dictionary<Type, Info> _infoDict;
        public static T GetInfo<T>() where T : Info => _infoDict[typeof(T)] as T;
        public static void ShowInfo<T>() where T : Info
        {
            if (_currentInfo != null) _currentInfo.Hide();
            _currentInfo = _infoDict[typeof(T)] as T;
            if (_currentInfo != null) _currentInfo.Show();
        }

        protected override void Initialize()
        {
            OrderInfos();
        }

        private void OrderInfos()
        {
            _infoDict = new Dictionary<Type, Info>();
            foreach (var info in _infos)
            {
                _infoDict.Add(info.GetType(), info);
                info.Hide();
            }
        }

        private IEnumerator MoveView(Vector2 destination, bool hideAfter)
        {
            if (_coroutine != null) StopCoroutine(_coroutine);
            ThisCanvas.enabled = true;
            while (Vector2.Distance(destination, _content.localPosition) > SNAP_DISTANCE)
            {
                _content.localPosition = Vector2.Lerp(_content.localPosition, destination,
                    _lerpSpeed * Time.deltaTime);

                yield return null;
            }

            _content.localPosition = destination;
            _coroutine = null;
            if (hideAfter) ThisCanvas.enabled = false;
        }

        public override void Show()
        {
            IsEnabled = true;
            _coroutine = StartCoroutine(MoveView(_shownPosition, false));
        }

        public override void Hide()
        {
            IsEnabled = false;
            _coroutine = StartCoroutine(MoveView(_hiddenPosition, true));
        }
    }
}