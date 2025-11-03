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
            UpdateEdgePosition( _camera.ScreenToWorldPoint(Input.mousePosition));

        public override void FixedFrameUpdate() { }

        public override void OnLeftClick()
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            var hit = Physics2D.Raycast(ray.origin, ray.direction, float.PositiveInfinity,
                StateOwner.RaycastMask);

            if (hit.collider != null && hit.transform.gameObject.TryGetComponent<Node>(out var node) && node != _source)
            {
                UpdateEdgePosition(node.transform.position);
                _edge.SetNodes(_source, node, _isOneSidedLink);
                StateMachine.ChangeState<Default>();
            }
        }

        private void UpdateEdgePosition(Vector2 edgeEnd)
        {
            Vector3 dir = _sourcePosition - (Vector2)_edge.transform.position;
            var newPosition = (edgeEnd + _sourcePosition) / 2;
            if (_isOneSidedLink) newPosition += (Vector2)dir.normalized * _edge.OffsetWhenOneSided;
            _edge.transform.position = newPosition;
            
            var lenght = Vector2.Distance(edgeEnd, _sourcePosition);
            _edge.SetLenght(lenght);
            
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            _edge.transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        public override void OnRightClick()
        {
            _edge.DeleteEdge();
            StateMachine.ChangeState<Default>();
        }
    }
}