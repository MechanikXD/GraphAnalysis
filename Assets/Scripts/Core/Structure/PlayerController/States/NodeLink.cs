using Core.Behaviour.StateMachine;
using Core.Graph;
using UnityEngine;

namespace Core.Structure.PlayerController.States
{
    public class NodeLink : PlayerState
    {
        private readonly Camera _camera;
        private Vector2 _sourcePosition;
        private Node _source;
        private Edge _edge;
        private static bool _isOneSidedLink;

        public NodeLink(PlayerStateMachine sm, PlayerController owner) : base(sm, owner)
        {
            _camera = GameManager.Instance.MainCamera;
        }

        public void SetSource(Node source, bool oneSided)
        {
            _isOneSidedLink = oneSided;
            _source = source;
        }

        public override void EnterState()
        {
            _sourcePosition = _source.transform.position;
            _edge = GameManager.Instance.CreateEdge(_sourcePosition, _isOneSidedLink);
        }

        public override void ExitState()
        {
            _edge = null;
            _source = null;
            _sourcePosition = Vector2.negativeInfinity;
        }

        public override void FrameUpdate() => 
            _edge.AdjustEdge(_sourcePosition, _camera.ScreenToWorldPoint(Input.mousePosition));

        public override void OnLeftClick()
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            var hit = Physics2D.Raycast(ray.origin, ray.direction, float.PositiveInfinity,
                StateOwner.RaycastMask);

            if (hit.collider != null && hit.transform.gameObject.TryGetComponent<Node>(out var node) && node != _source)
            {
                _edge.AdjustEdge(_sourcePosition, node.transform.position);
                _edge.SetNodes(_source, node, _isOneSidedLink);
                StateMachine.ChangeState<Default>();
            }
        }

        public override void OnRightClick()
        {
            _edge.DeleteEdge();
            StateMachine.ChangeState<Default>();
        }
    }
}