using Core.Structure;
using Core.Structure.PlayerController;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.View
{
    public class GraphAdjustView : CanvasView
    {
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Toggle _keepConnectedToggle;
        [SerializeField] private TMP_InputField _edgeLenghtField;
        private float _lastEdgeLenght;
        [SerializeField] private float _defaultEdgeLenght = 10f;

        public override void Show(bool isInitial = false)
        {
            base.Show(isInitial);
            UIManager.Instance.HideHUD<GlobalStatDisplayView>();
            _keepConnectedToggle.isOn = true;
            _lastEdgeLenght = _defaultEdgeLenght;
            _edgeLenghtField.text = string.Empty;

            _confirmButton.onClick.AddListener(OnConfirm);
            _cancelButton.onClick.AddListener(OnCancel);
            _edgeLenghtField.onEndEdit.AddListener(UpdateEdgeLength);
        }

        public override void Hide(bool isInitial = false)
        {
            base.Hide(isInitial);
            if (!isInitial) UIManager.Instance.ShowHUD<GlobalStatDisplayView>();
            _confirmButton.onClick.RemoveListener(OnConfirm);
            _cancelButton.onClick.RemoveListener(OnCancel);
            _edgeLenghtField.onEndEdit.RemoveListener(UpdateEdgeLength);
        }

        private void UpdateEdgeLength(string value)
        {
            if (float.TryParse(value, out var number))
            {
                _lastEdgeLenght = number;
            }
            
            _edgeLenghtField.SetTextWithoutNotify(_lastEdgeLenght.ToString("0.###"));
        }

        private void OnConfirm()
        {
            GameManager.Instance.ApplyTempNodes();
            GameManager.Instance.AdjacencyMatrix.GenerateFromNodes(_lastEdgeLenght, _keepConnectedToggle.isOn);
            PlayerController.EnterDefault();
        }

        private void OnCancel()
        {
            GameManager.Instance.DestroyTempNodes();
            PlayerController.EnterDefault();
        }
    }
}