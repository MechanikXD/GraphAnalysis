using Core.Behaviour.StateMachine;

namespace Core.Structure.PlayerController.States
{
    public class GraphAdjust : PlayerState
    {
        public GraphAdjust(PlayerStateMachine sm, PlayerController owner) : base(sm, owner) { }

        public override void EnterState()
        {
            // Generate nodes
            // Zoom out
            // Disable background controller
            // Enable temp root controller
            // Enable respective UI
        }

        public override void ExitState()
        {
            // Clear up (inverse of enter state)
        }

        public override void FrameUpdate() { }

        public override void FixedFrameUpdate() { }

        public override void OnLeftClick() { }

        public override void OnRightClick() { }
    }
}