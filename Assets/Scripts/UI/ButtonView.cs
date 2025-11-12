using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ButtonView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _title;

        public Button Button => _button;
        
        public void SetTitle(string title) => _title.SetText(title);
    }
}