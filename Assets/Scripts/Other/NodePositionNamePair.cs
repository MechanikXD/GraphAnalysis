using TMPro;
using UnityEngine;
using static System.String;

namespace Other
{
    public class NodePositionNamePair : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _xInput;
        [SerializeField] private TMP_InputField _yInput;
        [SerializeField] private TMP_InputField _nameInput;

        [SerializeField] private Vector2 _defaultPos;
        private float _lastXPos;
        private float _lastYPos;

        private void OnEnable()
        {
            _xInput.onEndEdit.AddListener(ValidateXPos);
            _yInput.onEndEdit.AddListener(ValidateYPos);
        }

        private void OnDisable()
        {
            _xInput.onEndEdit.RemoveListener(ValidateXPos);
            _yInput.onEndEdit.RemoveListener(ValidateYPos);
        }

        public void Clear()
        {
            _xInput.SetTextWithoutNotify(Empty);
            _yInput.SetTextWithoutNotify(Empty);
            _lastXPos = _defaultPos.x;
            _lastYPos = _defaultPos.y;
        }

        public (Vector2 pos, string name) GetData() => 
            (new Vector2(_lastXPos, _lastYPos), _nameInput.text);

        private void ValidateXPos(string input) => ValidateInputNumber(ref _lastXPos, _xInput, input);
        private void ValidateYPos(string input) => ValidateInputNumber(ref _lastYPos, _yInput, input);

        private void ValidateInputNumber(ref float lastPos, TMP_InputField field, string input)
        {
            if (float.TryParse(input, out var number)) lastPos = number;
            field.SetTextWithoutNotify(lastPos.ToString("0.###"));
        }
    }
}