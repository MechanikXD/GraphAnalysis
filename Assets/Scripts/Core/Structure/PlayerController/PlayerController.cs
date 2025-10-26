using System;
using Core.Graph;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Structure.PlayerController
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerInput _input;
        [SerializeField] private Node _nodePrefab;
        [SerializeField] private Edge _edgePrefab;
        private Camera _camera;
        
        private ContextAction[] _emptyContextActions;

        private void Awake()
        {
            _camera = Camera.main;
            _emptyContextActions = new[]
            {
                new ContextAction("New Node", CreateNode)
            };
        }

        private void CreateNode()
        {
            Vector2 pos = Input.mousePosition;
            var newNode = Instantiate(_nodePrefab);
            newNode.transform.position = pos;
        }

        private bool TryGetObjectAt(Vector2 position, out GameObject objectHit)
        {
            _camera.ScreenToWorldPoint(position);
            var hit = Physics2D.Raycast(_camera.transform.position, _camera.transform.forward);
            if (hit.collider != null)
            {
                objectHit = hit.collider.gameObject;
                return true;
            }

            objectHit = null;
            return false;
        }

        private bool TryInteractWithGameObject(Action<IInteractable> interactionType)
        {
            Vector2 mousePos = Input.mousePosition;
            if (TryGetObjectAt(mousePos, out var hit) &&
                hit.TryGetComponent<IInteractable>(out var interactable))
            {
                interactionType(interactable);
                return true;
            }
            
            return false;
        }
        
        public void OnPrimary()
        {
            TryInteractWithGameObject(i => i.Primary());
            // var contextWind = UIManager.Instance.GetHUDCanvas<ContextWindow>();
            // if (contextWind.IsEnabled) contextWind.Hide();
        }

        public void OnSecondary()
        {
            if (!TryInteractWithGameObject(i => i.Secondary()))
            {
                var contextWind = UIManager.Instance.GetHUDCanvas<ContextWindow>();
                contextWind.LoadContext(_emptyContextActions);
                contextWind.SetPosition(Input.mousePosition);
                contextWind.Show();
            }
        }
    }
}