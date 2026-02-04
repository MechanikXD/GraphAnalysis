using System.Collections.Generic;
using System.Text;
using Core.Graph;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace UI.UiStructures.InfoStructures
{
    public class NodeInfo : Info
    {
        [SerializeField] private TMP_InputField _nameInput;
        [SerializeField] private TMP_Text _positionField;
        [SerializeField] private TMP_Text _statField;
        private Node _currentNode;

        private void OnEnable()
        {
            _nameInput.onEndEdit.AddListener(RenameNode);
            LocalizationSettings.SelectedLocaleChanged += UpdateNodeStats;
        }

        private void OnDisable()
        {
            _nameInput.onEndEdit.RemoveListener(RenameNode);
            LocalizationSettings.SelectedLocaleChanged -= UpdateNodeStats;
        }

        private void UpdateNodeStats(Locale locale) => _statField.SetText(FormatStats(_currentNode.Stats));
        
        public void DisplayNode(Node node)
        {
            _currentNode = node;
            _nameInput.SetTextWithoutNotify(_currentNode.NodeName);
            _positionField.SetText(FormatPosition(_currentNode.transform.position));
            _statField.SetText(FormatStats(_currentNode.Stats));
        }

        private void RenameNode(string newName) => _currentNode.NodeName = newName;

        private static string FormatPosition(Vector2 position)
        {
            return $"({position.x:F}; {position.y:F})";
        }

        private static string FormatStats(Dictionary<string, float> stats)
        {
            var sb = new StringBuilder();
            foreach (var stat in stats)
            {
                var localizedMetric = LocalizationSettings.StringDatabase.GetLocalizedString(stat.Key);
                sb.Append(localizedMetric).Append(": ").Append(stat.Value.ToString("F3")).AppendLine();
            }
            
            // Remove last \n
            if (sb.Length > 0) sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
    }
}