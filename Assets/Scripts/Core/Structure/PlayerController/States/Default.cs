using System;
using Core.Behaviour.StateMachine;
using UI;
using UnityEngine;

namespace Core.Structure.PlayerController.States
{
    public class Default : PlayerControlState
    {
        private readonly Camera _camera;

        public Default(StateMachine<PlayerController> sm, PlayerController owner) : base(sm, owner)
        {
            _camera = Camera.main;
        }

        public override void EnterState() { }
        public override void ExitState() { }

        public override void FrameUpdate() { }
        public override void FixedFrameUpdate() { }

        public override void PrimaryAction()
        {
            TryInteractWithGameObject(i => i.Primary());
        }

        public override void SecondaryAction()
        {
            if (!TryInteractWithGameObject(i => i.Secondary()))
            {
                var contextWind = UIManager.Instance.GetHUDCanvas<ContextWindow>();
                contextWind.LoadContext(StateOwner.EmptyContextActions);
                contextWind.SetPosition(Input.mousePosition);
                contextWind.Show();
            }
        }
        
        private bool TryGetObjectAt(Vector2 position, out GameObject objectHit)
        {
            var ray = _camera.ScreenPointToRay(position);
            var hit = Physics2D.Raycast(ray.origin, ray.direction);
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
    }
}