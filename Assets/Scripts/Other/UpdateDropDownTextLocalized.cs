using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

namespace Other
{
    public class UpdateDropDownTextLocalized : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _dropdown;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private LocalizeStringEvent _lse;
        private bool _wasSubscribed;
        
        private void Start()
        {
            _lse.StringReference.TableEntryReference = _label.text;
        }

        private void OnEnable()
        {
            _dropdown.onValueChanged.AddListener(OnDropdownChanged);
            _lse.RefreshString();
        }

        private void OnDisable()
        {
            _dropdown.onValueChanged.RemoveListener(OnDropdownChanged);
        }

        private void OnDropdownChanged(int index)
        {
            // Let dropdown update first, then force localization
            StartCoroutine(RefreshLocalizationNextFrame(index));
        }

        private IEnumerator RefreshLocalizationNextFrame(int index)
        {
            yield return null; // Wait for dropdown to finish its update
            _lse.StringReference.TableEntryReference = _dropdown.options[index].text;
            _lse.RefreshString();
        }
    }
}