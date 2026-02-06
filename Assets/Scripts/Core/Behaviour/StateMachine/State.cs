using Core.Structure.PlayerController;
using UnityEngine;

namespace Core.Behaviour.StateMachine
{
    public abstract class PlayerState
    {
        protected readonly PlayerController StateOwner; 
        protected readonly PlayerStateMachine StateMachine;

        protected PlayerState(PlayerStateMachine sm, PlayerController owner)
        {
            StateOwner = owner;
            StateMachine = sm;
        }
        
        public abstract void EnterState();
        public abstract void ExitState();
        
        public abstract void FrameUpdate();
        
        public abstract void OnLeftClick();
        public abstract void OnRightClick();
    }
}