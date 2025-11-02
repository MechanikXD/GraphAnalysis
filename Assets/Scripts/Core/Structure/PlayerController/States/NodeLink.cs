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

        public NodeLink(PlayerStateMachine sm, PlayerController owner) : base(sm, owner)
        {
            _camera = GameManager.Instance.MainCamera;
        }

        public void SetSource(Node source) => _source = source;

        public override void EnterState()
        {
            _sourcePosition = _source.transform.position;
            _edge = GameManager.Instance.CreateEdge(_sourcePosition);
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
                _edge.SetNodes(_source, node, out var weight);
                GameManager.Instance.AdjacencyMatrix[_source.NodeIndex, node.NodeIndex] = weight;
                Debug.Log("New Matrix:\n" + GameManager.Instance.MatrixToString());
                StateMachine.ChangeState<Default>();
            }
        }

        private void UpdateEdgePosition(Vector2 edgeEnd)
        {
            var newPosition = (edgeEnd + _sourcePosition) / 2;
            _edge.transform.position = newPosition;
            
            var lenght = Vector2.Distance(edgeEnd, _sourcePosition);
            _edge.SetLenght(lenght);
            
            Vector3 dir = _sourcePosition - (Vector2)_edge.transform.position;
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