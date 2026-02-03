using System.Collections.Generic;
using System.IO;
using Core.Graph;
using Core.Structure;
using Core.Structure.PlayerController;
using SimpleFileBrowser;
using UI.UiStructures.Prompts;
using UI.View.GraphScene;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.UiStructures.InfoStructures
{
    public class Menu : Info
    {
        [SerializeField] private Button _createNodesButton;
        [SerializeField] private Button _generateEdgesButton;
        [SerializeField] private Button _changeBgButton;

        [SerializeField] private Button _exportGraphButton;
        [SerializeField] private Button _exportAdjMatrixButton;
        [SerializeField] private Button _exportStatsButton;
        
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _exitButton;

        [SerializeField] private BackgroundController _bgController;

        private void OnEnable()
        {
            _changeBgButton.onClick.AddListener(ChangeBackground);
            _generateEdgesButton.onClick.AddListener(ShowGenerateEdgesPrompt);
            _createNodesButton.onClick.AddListener(ShowGenerateNodesPrompt);
            _settingsButton.onClick.AddListener(OpenSettings);
            _exitButton.onClick.AddListener(SaveAndExit);
            
            _exportGraphButton.onClick.AddListener(() => Export(ExportGraph, "newGraph"));
            _exportAdjMatrixButton.onClick.AddListener(() => Export(ExportAdjMatrix, "newAdjacencyMatrix"));
            _exportStatsButton.onClick.AddListener(() => Export(ExportStats, "newStats"));
        }

        private void OnDisable()
        {
            _changeBgButton.onClick.RemoveListener(ChangeBackground);
            _generateEdgesButton.onClick.RemoveListener(ShowGenerateEdgesPrompt);
            _createNodesButton.onClick.RemoveListener(ShowGenerateNodesPrompt);
            _settingsButton.onClick.RemoveListener(OpenSettings);
            _exitButton.onClick.RemoveListener(SaveAndExit);
            
            _exportGraphButton.onClick.RemoveAllListeners();
            _exportAdjMatrixButton.onClick.RemoveAllListeners();
            _exportStatsButton.onClick.RemoveAllListeners();
        }

        private void OpenSettings() => UIManager.Instance.ShowUI<SettingsView>();

        private void SaveAndExit()
        {
            GameManager.Instance.ForceSave();
            SceneManager.LoadScene("MainMenu");
        }
        
        private void ChangeBackground()
        {
            FileBrowser.SetFilters( true, new FileBrowser.Filter( "Images", ".jpg", ".png" ));
            FileBrowser.ShowLoadDialog(OnImageSelected, null, FileBrowser.PickMode.Files);
        }

        private static void ShowGenerateEdgesPrompt() => UIManager.Instance.GetHUDCanvas<GlobalHUD>()
            .GetPrompt<GenerateEdgesPrompt>().ShowPrompt();
        
        private static void ShowGenerateNodesPrompt() => UIManager.Instance.GetHUDCanvas<GlobalHUD>()
            .GetPrompt<GenerateNodesPrompt>().ShowPrompt();

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

        private void Export(FileBrowser.OnSuccess exportMethod, string fileName) => 
            FileBrowser.ShowSaveDialog(exportMethod, null, FileBrowser.PickMode.Files, initialFilename:fileName);

        private void ExportGraph(string[] path) => 
            File.WriteAllText(path[0] + ".json", GameManager.Instance.GetGraphJson());
        
        private void ExportAdjMatrix(string[] path) => 
            File.WriteAllText(path[0] + ".csv", GameManager.Instance.GetGraphAsCsv());
        
        private void ExportStats(string[] path) => 
            File.WriteAllText(path[0] + ".csv", GameManager.Instance.GetNodeStatsAsCsv());
    }
}