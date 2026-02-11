using Core.Behaviour;
using TMPro;
using UnityEngine;

namespace Other
{
    public class CoordinateTracker : SingletonBase<CoordinateTracker>
    {
        private Camera _cam;
        [SerializeField] private TMP_Text _coordinateField;
    
        protected override void Initialize()
        {
            base.Initialize();
            _cam = Camera.main;
        }

        private void Update()
        {
            _coordinateField.SetText(FormatCoordinates(GetMouseWorldCoordinates()));
        }

        private Vector2 GetMouseWorldCoordinates()
        {
            var mouseWorldPosition = _cam.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0f;
            return mouseWorldPosition;
        }

        private static string FormatCoordinates(Vector2 coordinates) => 
            $"({coordinates.x:F3}; {coordinates.y:F3})";

        public void Disable() => gameObject.SetActive(false);
        public void Enable() => gameObject.SetActive(true);
    }
}
