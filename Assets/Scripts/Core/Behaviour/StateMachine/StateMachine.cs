using System;
using System.Collections.Generic;

namespace Core.Behaviour.StateMachine
{
    public class StateMachine<TOwner>
    {
        public State<TOwner> CurrentState { get; private set; }
        private Dictionary<Type, State<TOwner>> _states;

        public void Initialize(State<TOwner> initialState)
        {
            CurrentState = initialState;
            CurrentState.EnterState();
            _states = new Dictionary<Type, State<TOwner>> { { initialState.GetType(), initialState } };
        }

        public void ChangeState(State<TOwner> newState)
        {
            CurrentState.ExitState();
            CurrentState = newState;
            CurrentState.EnterState();
        }
        
        public void ChangeState<TState>() where TState : State<TOwner>
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

        public void AddState(State<TOwner> state) => _states.Add(state.GetType(), state);
        public State<TOwner> GetState<TState>() where TState : State<TOwner> =>  _states[typeof(TState)];
    }
}