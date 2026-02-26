using System.Collections.Generic;
using System.Text;
using Core.Structure;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace UI.View.GraphScene
{
    public class GlobalStatDisplayView : CanvasView
    {
        [SerializeField] private TMP_Text _textFiled;

        private void Start()
        {
            _textFiled.SetText(string.Empty);
        }

        private void OnEnable()
        {
            LocalizationSettings.SelectedLocaleChanged += UpdateStats;
        }

        private void OnDisable()
        {
            LocalizationSettings.SelectedLocaleChanged -= UpdateStats;
        }

        private static void UpdateStats(Locale locale)
        {
            GameManager.Instance.AdjacencyMatrix.UpdateGlobalStatView();
        }

        public void LoadText(Dictionary<string, float> stats)
        {
            _textFiled.SetText(FormatStats(stats));
        }

        public void AppendTextLine(string text)
        {
            _textFiled.SetText(_textFiled.text + '\n' + text);
        }

        private static string FormatStats(Dictionary<string, float> stats)
        {
            var sb = new StringBuilder();
            foreach (var stat in stats)
            {
                var localizedMetric =
                    LocalizationSettings.StringDatabase.GetLocalizedString(stat.Key);
                sb.Append(localizedMetric).Append(": ").Append(stat.Value.ToString("F3"))
                    .AppendLine();
            }

            // Remove last \n
            if (sb.Length > 0) sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
    }
}