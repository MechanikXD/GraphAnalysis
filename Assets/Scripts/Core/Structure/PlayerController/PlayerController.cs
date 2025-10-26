using Core.Behaviour.StateMachine;
using Core.Graph;
using Core.Structure.PlayerController.States;
using UnityEngine;

namespace Core.Structure.PlayerController
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private LayerMask _nodeOnlyMask;
        [SerializeField] private float _linkRadius = 0.1f;
        [SerializeField] private Node _nodePrefab;
        [SerializeField] private Edge _edgePrefab;
        private Camera _camera;

        public LayerMask NodeOnlyMask => _nodeOnlyMask;
        public float LinkRadius => _linkRadius;
        public Node NodePrefab => _nodePrefab;
        public Edge EdgePrefab => _edgePrefab;

        private static StateMachine<PlayerController> _controller;
        public ContextAction[] EmptyContextActions { get; private set; }

        private void Awake()
        {
            _camera = Camera.main;
            EmptyContextActions = new[]
            {
                new ContextAction("New Node", CreateNode)
            };

            _controller = new StateMachine<PlayerController>();
            var defaultState = new Default(_controller, this);
            var nodeLink = new NodeLink(_controller, this);
            
            _controller.Initialize(defaultState);
            _controller.AddState(nodeLink);
        }

        public static void StartNodeLink(Node source)
        {
            ((NodeLink)_controller.GetState<NodeLink>()).SetSource(source);
            _controller.ChangeState<NodeLink>();
        }

        private void Update() => _controller.CurrentState.FrameUpdate();

        private void FixedUpdate() => _controller.CurrentState.FixedFrameUpdate();

        private void CreateNode()
        {
            CreateNodeFromScreenPos(Input.mousePosition);
        }
        
        private void CreateNodeFromScreenPos(Vector2 screenPos)
        {
            var worldPos = _camera.ScreenToWorldPoint(screenPos);
            worldPos.z = 0;
            Instantiate(_nodePrefab, worldPos, Quaternion.identity);
        }
        
        public void OnPrimary()
        {
            ((PlayerControlState)_controller.CurrentState).PrimaryAction();
        }

        public void OnSecondary()
        {
            ((PlayerControlState)_controller.CurrentState).SecondaryAction();
        }
    }
}