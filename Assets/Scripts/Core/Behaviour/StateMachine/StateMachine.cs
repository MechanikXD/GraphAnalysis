using System;
using System.Collections.Generic;

namespace Core.Behaviour.StateMachine
{
    public class PlayerStateMachine
    {
        public PlayerState CurrentState { get; private set; }
        private Dictionary<Type, PlayerState> _states;

        public void Initialize(PlayerState initialState)
        {
            CurrentState = initialState;
            CurrentState.EnterState();
            _states = new Dictionary<Type, PlayerState> { { initialState.GetType(), initialState } };
        }

        public void ChangeState(PlayerState newState)
        {
            CurrentState.ExitState();
            CurrentState = newState;
            CurrentState.EnterState();
        }
        
        public void ChangeState<TState>() where TState : PlayerState
        {
            var newState = _states[typeof(TState)];
            ChangeState(newState);
        }

        public void StopMachine()
        {
            CurrentState.ExitState();
            CurrentState = null;
            _states.Clear();
        }

        public void AddState(PlayerState state) => _states.Add(state.GetType(), state);
        public PlayerState GetState<TState>() where TState : PlayerState =>  _states[typeof(TState)];
    }
}