using Core.Behaviour.StateMachine;
using Core.Graph;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Structure.PlayerController.States
{
    public class NodeLink : PlayerControlState
    {
        private readonly Camera _camera;
        private Vector2 _sourcePosition;
        private Node _source;
        private Edge _edge;

        public NodeLink(StateMachine<PlayerController> sm, PlayerController owner) : base(sm, owner)
        {
            _camera = Camera.main;
        }

        public void SetSource(Node source) => _source = source;

        public override void EnterState()
        {
            _sourcePosition = _source.transform.position;
            _edge = Object.Instantiate(StateOwner.EdgePrefab, _sourcePosition, Quaternion.identity);
        }

        public override void ExitState()
        {
            _edge = null;
            _source = null;
            _sourcePosition = Vector2.negativeInfinity;
        }

        public override void FrameUpdate() => UpdateEdgePosition( _camera.ScreenToWorldPoint(Input.mousePosition));

        public override void FixedFrameUpdate() { }

        public override void PrimaryAction()
        {
            Vector2 inputPoint = Input.mousePosition;
            var inputPosition = _camera.ScreenToWorldPoint(inputPoint);
            var hit = Physics2D.OverlapCircle(inputPosition, StateOwner.LinkRadius, StateOwner.NodeOnlyMask);

            if (hit != null && hit.gameObject.TryGetComponent<Node>(out var node))
            {
                UpdateEdgePosition(node.transform.position);
                _edge.SetNodes(_source, node);
                StateMachine.ChangeState<Default>();
            }
        }

        private void UpdateEdgePosition(Vector2 position)
        {
            var newPosition = (position + _sourcePosition) / 2;
            _edge.transform.position = newPosition;
            
            var lenght = Vector2.Distance(position, _sourcePosition);
            _edge.SetLenght(lenght);
            
            Vector3 dir = _sourcePosition - (Vector2)_edge.transform.position;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            _edge.transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        public override void SecondaryAction()
        {
            Debug.Log("Secondary Action");
            _edge.DeleteEdge();
            StateMachine.ChangeState<Default>();
        }
    }
}