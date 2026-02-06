using Core.Behaviour;
using Core.Structure;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

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

        public void ClearMessage() => Log(GlobalStorage.EMPTY_ENTRY_KEY, _logColor, 0f);
        public void LogInfo(string entryKey, float feedTime = 5f) => Log(entryKey, _logColor, feedTime);
        public void LogWarning(string entryKey, float feedTime = 5f) => Log(entryKey, _warningColor, feedTime);
        public void LogError(string entryKey, float feedTime = 5f) => Log(entryKey, _errorColor, feedTime);

        protected override void Awake()
        {
            base.Awake();
            ClearMessage();
        }

        private void Log(string entryKey, Color color, float feedTime = 5f)
        {
            _feed.SetText(LocalizationSettings.StringDatabase.GetLocalizedString(entryKey));
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

            ClearMessage();
            _feedIsDisplayed = false;
        }
    }
}