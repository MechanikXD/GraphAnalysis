using System;
using Core.Behaviour.StateMachine;
using UI;
using UI.View.GraphScene;
using UnityEngine;

namespace Core.Structure.PlayerController.States
{
    public class Default : PlayerState
    {
        private readonly Camera _camera;

        public Default(PlayerStateMachine sm, PlayerController owner) : base(sm, owner)
        {
            _camera = Camera.main;
        }

        public override void EnterState() { }
        public override void ExitState() => HideHUD();

        public override void FrameUpdate() { }

        public override void OnLeftClick()
        {
            if (!UIManager.IsPointerOverUI(Input.mousePosition))
            {
                if (!InteractWithGameObject(i => i.OnLeftClick()))
                {
                    HideHUD();
                }
            }
        }

        private void HideHUD()
        {
            var contextWindow = UIManager.Instance.GetHUDCanvas<ContextWindow>();
            contextWindow.ClearContent();
            contextWindow.Hide();
            UIManager.Instance.HideHUD<InfoView>();
        }

        public override void OnRightClick()
        {
            if (!InteractWithGameObject(i => i.OnRightClick()))
            {
                var contextWindow = UIManager.Instance.GetHUDCanvas<ContextWindow>();
                contextWindow.LoadContext(StateOwner.EmptyContextActions);
                contextWindow.SetPosition(Input.mousePosition);
                contextWindow.Show();
            }
        }

        private bool InteractWithGameObject(Action<IInteractable> interaction)
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            var hit = Physics2D.Raycast(ray.origin, ray.direction, float.MaxValue, StateOwner.RaycastMask);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.TryGetComponent<IInteractable>(out var interactable))
                {
                    interaction(interactable);
                    return true;
                }
            }
            
            return false;
        }
    }
}