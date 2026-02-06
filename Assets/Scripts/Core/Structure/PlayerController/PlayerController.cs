using Core.Behaviour.StateMachine;
using Core.Graph;
using Core.Structure.PlayerController.States;
using UI.View.GraphScene;
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
            var nodeMove = new NodeMove(_controller, this);
            var graphAdjust = new GraphAdjust(_controller, this);

            _controller.Initialize(defaultState);
            _controller.AddState(nodeLink);
            _controller.AddState(nodeMove);
            _controller.AddState(graphAdjust);
        }

        private void CreateNode()
        {
            GameManager.Instance.CreateNodeFromScreenPos(_lastContextActionPosition, null);
        }

        public static void EnterNodeLink(Node source, bool oneSidedLink)
        {
            ((NodeLink)_controller.GetState<NodeLink>()).SetSource(source, oneSidedLink);
            _controller.ChangeState<NodeLink>();
        }

        public static void EnterNodeMove(Node node)
        {
            ((NodeMove)_controller.GetState<NodeMove>()).SetNode(node);
            _controller.ChangeState<NodeMove>();
        }

        public static void EnterGraphAdjust() => _controller.ChangeState<GraphAdjust>();
        public static void EnterDefault() => _controller.ChangeState<Default>();

        private void Update() => _controller.CurrentState.FrameUpdate();

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