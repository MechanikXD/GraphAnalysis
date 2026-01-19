using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace UI.View
{
    public class GlobalStatDisplayView: CanvasView
    {
        [SerializeField] private TMP_Text _textFiled;

        public void LoadText(Dictionary<string, float> stats)
        {
            _textFiled.SetText(FormalStats(stats));
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