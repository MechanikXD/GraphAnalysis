using Core.Structure;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UiStructures.Prompts
{
    public class GenerateEdgesPrompt : PromptBase
    {
        [SerializeField] private Toggle _keepConnected;
        [SerializeField] private TMP_InputField _lengthField;
        private float _lastLengthValue;
        [SerializeField] private float _defaultEdgeLenght = 10f;

        protected override void OnEnable()
        {
            base.OnEnable();
            _lengthField.onEndEdit.AddListener(UpdateEdgeLength);
            _keepConnected.isOn = true;
            _lastLengthValue = _defaultEdgeLenght;
            _lengthField.text = string.Empty;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _lengthField.onEndEdit.RemoveListener(UpdateEdgeLength);
        }

        private void UpdateEdgeLength(string value)
        {
            if (float.TryParse(value, out var number))
            {
                _lastLengthValue = number;
            }
            
            _lengthField.SetTextWithoutNotify(_lastLengthValue.ToString("0.###"));
        }
        
        public override void OnConfirm() => 
            GameManager.Instance.AdjacencyMatrix.GenerateFromNodes(_lastLengthValue, _keepConnected.isOn);

        public override void OnCancel() { }
    }
}