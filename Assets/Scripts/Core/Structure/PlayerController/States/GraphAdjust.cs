using Core.Behaviour.StateMachine;
using Cysharp.Threading.Tasks;
using Other;
using UI;
using UI.View.GraphScene;
using UnityEngine;

namespace Core.Structure.PlayerController.States
{
    public class GraphAdjust : PlayerState
    {
        private readonly Camera _camera;
        private const float SNAP_DISTANCE = 0.01f;
        

        public GraphAdjust(PlayerStateMachine sm, PlayerController owner) : base(sm, owner)
        {
            _camera = Camera.main;
        }

        public override void EnterState()
        {
            FocusCamera().Forget();
            TempNodesController.Enable();
            UIManager.Instance.ShowHUD<GraphAdjustView>();
            CoordinateTracker.Instance.Disable();
        }

        public override void ExitState()
        {
            GameManager.Instance.ApplyTempNodes();
            TempNodesController.Disable();
            UIManager.Instance.HideHUD<GraphAdjustView>();
            CoordinateTracker.Instance.Enable();
        }

        public override void FrameUpdate() { }

        public override void FixedFrameUpdate() { }

        public override void OnLeftClick() { }

        public override void OnRightClick() { }

        public async UniTask FocusCamera()
        {
            BackgroundController.Disable();
            var targetPos = new Vector3(0, 0, _camera.transform.position.z);
            while (true)
            {
                var atDestination = Vector3.Distance(_camera.transform.position, targetPos) < SNAP_DISTANCE;
                var atTargetZoom = Mathf.Abs(GlobalStorage.MAX_ZOOM - _camera.orthographicSize) < SNAP_DISTANCE;
                if (atDestination && atTargetZoom)
                {
                    _camera.transform.position = targetPos;
                    _camera.orthographicSize = GlobalStorage.MAX_ZOOM;
                    break;
                }

                if (!atDestination)
                {
                    _camera.transform.position = Vector3.Lerp(
                        _camera.transform.position,
                        targetPos,
                        GlobalStorage.MOVE_SPEED * Time.deltaTime);
                }

                if (!atTargetZoom)
                {
                    _camera.orthographicSize = Mathf.Lerp(
                        _camera.orthographicSize,
                        GlobalStorage.MAX_ZOOM,
                        GlobalStorage.MOVE_SPEED * Time.deltaTime
                    );
                }
                
                await UniTask.NextFrame(StateOwner.destroyCancellationToken);
            }
            BackgroundController.Enable();
        }
    }
}