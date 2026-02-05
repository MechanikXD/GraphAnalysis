using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

namespace Other
{
    public class LocalizeDrawKeyFromText : MonoBehaviour
    {
        [SerializeField] private LocalizeStringEvent _lse;
        [SerializeField] private TMP_Text _text;
        private bool _wasSubscribed;
        
        private void Start() => SubscribeEvent();

        private void OnEnable()
        {
            if (!_wasSubscribed) SubscribeEvent();
        }

        private void OnDisable()
        {
            if (_wasSubscribed) UnsubscribeEvent();
        }

        private void SubscribeEvent()
        {
            _wasSubscribed = true;
            _lse.StringReference.TableEntryReference = _text.text;
            _lse.RefreshString();
        }

        private void UnsubscribeEvent()
        {
            _wasSubscribed = false;
            _lse.StringReference.TableEntryReference = default;
            _text.SetText(string.Empty);
        }
    }
}