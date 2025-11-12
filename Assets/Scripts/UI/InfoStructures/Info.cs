using UnityEngine;

namespace UI.InfoStructures
{
    public abstract class Info : MonoBehaviour
    {
        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);
    }
}