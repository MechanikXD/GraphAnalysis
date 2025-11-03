using Core.Graph;
using TMPro;
using UnityEngine;

namespace UI.InfoStructures
{
    public class EdgeInfo : Info
    {
        [SerializeField] private TMP_InputField _weightInput;
        [SerializeField] private TMP_Text _fromNode;
        [SerializeField] private GameObject _fromNodePrefix;
        [SerializeField] private TMP_Text _toNode;
        [SerializeField] private GameObject _toNodePrefix;
        private Edge _currentEdge;

        private void OnEnable() => _weightInput.onEndEdit.AddListener(ChangeWeight);

        private void OnDisable() => _weightInput.onEndEdit.RemoveListener(ChangeWeight);

        public void DisplayEdge(Edge edge)
        {
            _currentEdge = edge;
            _weightInput.SetTextWithoutNotify(_currentEdge.Weight.ToString("F"));
            _fromNode.SetText(_currentEdge.FromNodeName);
            _toNode.SetText(_currentEdge.ToNodeName);
            
            if (_currentEdge.IsOneSided)
            {
                _fromNodePrefix.SetActive(true);
                _toNodePrefix.SetActive(true);
            }
            else
            {
                _fromNodePrefix.SetActive(false);
                _toNodePrefix.SetActive(false);
            }
        }

        private void ChangeWeight(string newWeight)
        {
            if (float.TryParse(newWeight, out var weight))
            {
                _currentEdge.Weight = weight;
            }
            else
            {
                _currentEdge.SetFallBackWeight();
            }
        }
    }
}