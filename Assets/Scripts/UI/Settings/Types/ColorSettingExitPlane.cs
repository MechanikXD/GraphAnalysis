using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Settings.Types
{
    public class ColorSettingExitPlane : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private ColorSettingPrefab _prefab;
        
        public void OnPointerClick(PointerEventData eventData) => _prefab.ConfirmColor();
    }
}