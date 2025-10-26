using Core.Behaviour.StateMachine;

namespace Core.Structure.PlayerController
{
    public abstract class PlayerControlState : State<PlayerController>
    {
        protected PlayerControlState(StateMachine<PlayerController> sm, PlayerController owner) 
            : base(sm, owner) { }

        public abstract void PrimaryAction();
        public abstract void SecondaryAction();
    }
}