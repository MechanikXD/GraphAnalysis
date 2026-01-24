using System.Collections.Generic;
using System.IO;
using Core.Structure.PlayerController;
using SimpleFileBrowser;
using UnityEngine;
using UnityEngine.UI;

namespace UI.InfoStructures
{
    public class Menu : Info
    {
        [SerializeField] private Button _createNodesButton;
        [SerializeField] private Button _changeBgButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _exitButton;

        [SerializeField] private BackgroundController _bgController;

        private void OnEnable()
        {
            _changeBgButton.onClick.AddListener(ChangeBackground);
        }

        private void OnDisable()
        {
            _changeBgButton.onClick.RemoveListener(ChangeBackground);
        }

        private void ChangeBackground()
        {
            FileBrowser.SetFilters( true, new FileBrowser.Filter( "Images", ".jpg", ".png" ));
            FileBrowser.ShowLoadDialog(OnImageSelected, null, FileBrowser.PickMode.Files);
        }

        private void OnImageSelected(IList<string> paths)
        {
            var imageBytes = File.ReadAllBytes(paths[0]);
            var texture = new Texture2D(2, 2); // Size doesn't matter, will auto-resize
            var loaded = texture.LoadImage(imageBytes);

            if (!loaded)
            {
                Debug.LogError("Failed to load image data!");
            }
            else
            {
                var sprite = Sprite.Create(texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f)
                );
                _bgController.ChangeBackground(sprite);
            }
        }
    }
}