using Cysharp.Threading.Tasks;
using UI.Settings.Types;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace Core.Structure
{
    public class GlobalSettingsManager : MonoBehaviour
    {
        private readonly int[] _frameRateValues = { -1, 30, 60, 144 };

        private void Start()
        {
            void UpdateScreenMode(DropDownSettingPrefab dropdown)
            {
                switch (dropdown.CurrentOption)
                {
                    case 0: // fullscreen
                        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                        Screen.fullScreen = true;
                        break;
                    case 1: // windowed
                        Screen.fullScreenMode = FullScreenMode.Windowed;
                        Screen.fullScreen = false;
                        break;
                }
            }
            
            void UpdateFrameRateLimit(DropDownSettingPrefab dropdown) => 
                Application.targetFrameRate = _frameRateValues[dropdown.CurrentOption];

            void UpdateVsync(BooleanSettingPrefab boolean) => 
                QualitySettings.vSyncCount = boolean.IsOn ? 1 : 0;
            
            SettingsManager.AddEventOnSetting<DropDownSettingPrefab>(
                GlobalStorage.SettingKeys.Display.SCREEN_MODE, UpdateScreenMode);
            SettingsManager.AddEventOnSetting<DropDownSettingPrefab>(
                GlobalStorage.SettingKeys.Display.FRAME_RATE_LIMIT, UpdateFrameRateLimit);
            SettingsManager.AddEventOnSetting<DropDownSettingPrefab>(
                GlobalStorage.SettingKeys.Display.ANTI_ALIASING, UpdateAntiAliasing);
            SettingsManager.AddEventOnSetting<BooleanSettingPrefab>(
                GlobalStorage.SettingKeys.Display.V_SYNC, UpdateVsync);
            SceneManager.sceneLoaded += (_,_) => UpdateAntiAliasingOnStart().Forget();
        }

        private async UniTask UpdateAntiAliasingOnStart()
        {
            await UniTask.Yield(PlayerLoopTiming.Update, destroyCancellationToken);
            UpdateAntiAliasing(SettingsManager.GetSetting<DropDownSettingPrefab>(
                GlobalStorage.SettingKeys.Display.ANTI_ALIASING));
        }

        // This will be called on start once. Game object is DontDestroyOnLoad, so upper method should 
        // be called on sceneChange to re-initialize camera on scene.
        private static void UpdateAntiAliasing(DropDownSettingPrefab dropdown)
        {
            var mainCam = Camera.main;
            if (mainCam != null)
            {
                var uacd = mainCam.gameObject.GetComponent<UniversalAdditionalCameraData>();
                // Because option indexes align with AntialiasingMode's
                if (uacd != null) uacd.antialiasing = (AntialiasingMode)dropdown.CurrentOption;
            }
        }
    }
}