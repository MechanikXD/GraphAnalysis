using UnityEngine;
using UnityEngine.UI;

namespace UI.Prompts
{
    public abstract class PromptBase : MonoBehaviour
    {
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _cancelButton;

        protected virtual void OnEnable()
        {
            _confirmButton.onClick.AddListener(OnConfirm);
            _cancelButton.onClick.AddListener(OnCancel);
            
            _confirmButton.onClick.AddListener(HidePrompt);
            _cancelButton.onClick.AddListener(HidePrompt);
        }

        protected virtual void OnDisable()
        {
            _confirmButton.onClick.RemoveListener(OnConfirm);
            _confirmButton.onClick.RemoveListener(OnCancel);
            
            _confirmButton.onClick.RemoveListener(HidePrompt);
            _cancelButton.onClick.RemoveListener(HidePrompt);
        }

        public void ShowPrompt() => gameObject.SetActive(true);
        public void HidePrompt() => gameObject.SetActive(false);

        public abstract void OnConfirm();
        public abstract void OnCancel();
    }
}