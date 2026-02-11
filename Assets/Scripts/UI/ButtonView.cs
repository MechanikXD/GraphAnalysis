using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace UI
{
    public class ButtonView : MonoBehaviour
    {
        [SerializeField] private LocalizeStringEvent _lse;
        [SerializeField] private Button _button;

        public Button Button => _button;
        
        public void SetTitle(string entryKey)
        {
            _lse.StringReference.TableEntryReference = entryKey;
            _lse.RefreshString();
        }
    }
}