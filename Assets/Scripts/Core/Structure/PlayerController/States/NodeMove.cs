using Core.Behaviour.StateMachine;
using Core.Graph;
using UnityEngine;

namespace Core.Structure.PlayerController.States
{
    public class NodeMove : PlayerState
    {
        private readonly Camera _camera;
        private Vector2 _sourcePosition;
        private Node _node;

        public NodeMove(PlayerStateMachine sm, PlayerController owner) : base(sm, owner)
        {
            _camera = GameManager.Instance.MainCamera;
        }
        
        public void SetNode(Node node)
        {
            _node = node;
            _sourcePosition = node.transform.position;
        }

        public override void EnterState()
        {
            foreach (var edge in _node.Connections) edge.Disable();
        }

        public override void ExitState()
        {
            foreach (var edge in _node.Connections) edge.Enable();
        }

        public override void FrameUpdate()
        {
            var newPos = _camera.ScreenToWorldPoint(Input.mousePosition);
            newPos.z = 0;
            _node.transform.position = newPos;
        }

        public override void FixedFrameUpdate() { }

        public override void OnLeftClick()
        {
            var newPos = _camera.ScreenToWorldPoint(Input.mousePosition);
            newPos.z = 0f;
            _node.transform.position = newPos;
            foreach (var edge in _node.Connections)
            {
                var otherNode = edge.GetOppositeNode(_node);
                if (otherNode != null) edge.AdjustEdge(otherNode.transform.position, newPos);
            }
            StateMachine.ChangeState<Default>();
            
            _node = null;
            _sourcePosition = Vector2.zero;
        }

        public override void OnRightClick()
        {
            _node.transform.position = _sourcePosition;
            StateMachine.ChangeState<Default>();
        }
    }
}