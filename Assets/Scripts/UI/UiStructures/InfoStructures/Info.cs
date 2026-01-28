using UnityEngine;

namespace UI.UiStructures.InfoStructures
{
    public abstract class Info : MonoBehaviour
    {
        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);
    }
}