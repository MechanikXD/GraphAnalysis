namespace Core.Behaviour.StateMachine
{
    public abstract class State<TOwner>
    {
        protected readonly TOwner StateOwner; 
        protected readonly StateMachine<TOwner> StateMachine;

        protected State(StateMachine<TOwner> sm, TOwner owner)
        {
            StateOwner = owner;
            StateMachine = sm;
        }
        
        public abstract void EnterState();
        public abstract void ExitState();
        
        public abstract void FrameUpdate();
        public abstract void FixedFrameUpdate();
    }
}