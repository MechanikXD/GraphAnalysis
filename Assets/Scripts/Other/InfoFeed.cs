using Core.Behaviour;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Other
{
    public class InfoFeed : SingletonBase<InfoFeed>
    {
        [SerializeField] private TMP_Text _feed;
        [SerializeField] private Color _logColor;
        [SerializeField] private Color _warningColor = Color.yellow;
        [SerializeField] private Color _errorColor = Color.red;
        private float _remainingFeedTime;
        private bool _feedIsDisplayed;

        public void ClearMessage() => Log(string.Empty, _logColor, 0f);
        public void LogInfo(string msg, float feedTime = 5f) => Log(msg, _logColor, feedTime);
        public void LogWarning(string msg, float feedTime = 5f) => Log(msg, _warningColor, feedTime);
        public void LogError(string msg, float feedTime = 5f) => Log(msg, _errorColor, feedTime);

        protected override void Awake()
        {
            base.Awake();
            ClearMessage();
        }

        private void Log(string msg, Color color, float feedTime = 5f)
        {
            _feed.SetText(msg);
            _feed.color = color;
            _remainingFeedTime = feedTime;
            if (!_feedIsDisplayed) HideFeedLater().Forget();
        }

        private async UniTask HideFeedLater()
        {
            _feedIsDisplayed = true;
            while (_remainingFeedTime > 0f)
            {
                _remainingFeedTime -= Time.deltaTime;
                await UniTask.NextFrame(destroyCancellationToken);
            }

            _remainingFeedTime = 0f;
            _feedIsDisplayed = false;
        }
    }
}