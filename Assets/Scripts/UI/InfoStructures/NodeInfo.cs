using System.Collections.Generic;
using System.Text;
using Core.Graph;
using TMPro;
using UnityEngine;

namespace UI.InfoStructures
{
    public class NodeInfo : Info
    {
        [SerializeField] private TMP_InputField _nameInput;
        [SerializeField] private TMP_Text _positionField;
        [SerializeField] private TMP_Text _statField;
        private Node _currentNode;

        private void OnEnable() => _nameInput.onEndEdit.AddListener(RenameNode);

        private void OnDisable() => _nameInput.onEndEdit.RemoveListener(RenameNode);

        public void DisplayNode(Node node)
        {
            _currentNode = node;
            _nameInput.SetTextWithoutNotify(_currentNode.NodeName);
            _positionField.SetText(FormatPosition(_currentNode.transform.position));
            _statField.SetText(FormalStats(_currentNode.Stats));
        }

        private void RenameNode(string newName) => _currentNode.NodeName = newName;

        private static string FormatPosition(Vector2 position)
        {
            return $"({position.x:F}; {position.y:F})";
        }

        private static string FormalStats(Dictionary<string, float> stats)
        {
            var sb = new StringBuilder();
            foreach (var stat in stats)
            {
                sb.Append(stat.Key).Append(": ").Append(stat.Value).AppendLine();
            }
            
            // Remove last \n
            if (sb.Length > 0) sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
    }
}