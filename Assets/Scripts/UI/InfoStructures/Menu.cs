using System.Collections.Generic;
using System.IO;
using Core.Graph;
using Core.Structure.PlayerController;
using SimpleFileBrowser;
using UI.Prompts;
using UI.View;
using UnityEngine;
using UnityEngine.UI;

namespace UI.InfoStructures
{
    public class Menu : Info
    {
        [SerializeField] private Button _createNodesButton;
        [SerializeField] private Button _generateEdgesButton;
        [SerializeField] private Button _changeBgButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _exitButton;

        [SerializeField] private BackgroundController _bgController;

        private void OnEnable()
        {
            _changeBgButton.onClick.AddListener(ChangeBackground);
            _generateEdgesButton.onClick.AddListener(ShowGenerateEdgesPrompt);
        }

        private void OnDisable()
        {
            _changeBgButton.onClick.RemoveListener(ChangeBackground);
            _generateEdgesButton.onClick.RemoveListener(ShowGenerateEdgesPrompt);
        }

        private void ChangeBackground()
        {
            FileBrowser.SetFilters( true, new FileBrowser.Filter( "Images", ".jpg", ".png" ));
            FileBrowser.ShowLoadDialog(OnImageSelected, null, FileBrowser.PickMode.Files);
        }

        private void ShowGenerateEdgesPrompt() => UIManager.Instance.GetHUDCanvas<GlobalHUD>()
            .GetPrompt<GenerateEdgesPrompt>().ShowPrompt();

        public void SetBackground(string filepath) => OnImageSelected(new[]{filepath});

        private void OnImageSelected(IList<string> paths)
        {
            if (string.IsNullOrWhiteSpace(paths[0])) return;
            if (!File.Exists(paths[0]))
            {
                Debug.LogWarning("Image file not found: " + paths[0]);
                AdjacencyMatrix.BgFilePath = string.Empty;
                return;
            }

            var imageBytes= File.ReadAllBytes(paths[0]);
            var texture = new Texture2D(2, 2); // Size doesn't matter, will auto-resize
            var loaded = texture.LoadImage(imageBytes);

            if (!loaded)
            {
                Debug.LogWarning("File is not a valid PNG/JPG image: " + paths[0]);
                Destroy(texture);
            }
            else
            {
                var sprite = Sprite.Create(texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f)
                );
                AdjacencyMatrix.BgFilePath = paths[0];
                _bgController.ChangeBackground(sprite);
            }
        }
    }
}