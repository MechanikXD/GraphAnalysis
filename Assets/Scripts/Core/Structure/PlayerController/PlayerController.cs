using Core.Behaviour.StateMachine;
using Core.Graph;
using Core.Structure.PlayerController.States;
using UnityEngine;

namespace Core.Structure.PlayerController
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private LayerMask _raycastMask;

        public LayerMask RaycastMask => _raycastMask;
        
        private static PlayerStateMachine _controller;
        public ContextAction[] EmptyContextActions { get; private set; }
        private Vector2 _lastContextActionPosition;

        private void Awake()
        {
            EmptyContextActions = new[]
            {
                new ContextAction("New Node", CreateNode)
            };
        }
        
        private void Start()
        {
            _controller = new PlayerStateMachine();
            var defaultState = new Default(_controller, this);
            var nodeLink = new NodeLink(_controller, this);
            
            _controller.Initialize(defaultState);
            _controller.AddState(nodeLink);
        }
        
        private void CreateNode()
        {
            GameManager.Instance.CreateNodeFromScreenPos(_lastContextActionPosition);
        }

        public static void StartNodeLink(Node source, bool oneSidedLink)
        {
            ((NodeLink)_controller.GetState<NodeLink>()).SetSource(source, oneSidedLink);
            _controller.ChangeState<NodeLink>();
        }

        private void Update() => _controller.CurrentState.FrameUpdate();

        private void FixedUpdate() => _controller.CurrentState.FixedFrameUpdate();
        
        public void OnLeftClick()
        {
            _controller.CurrentState.OnLeftClick();
        }

        public void OnRightClick()
        {
            _lastContextActionPosition = Input.mousePosition;
            _controller.CurrentState.OnRightClick();
        }
    }
}