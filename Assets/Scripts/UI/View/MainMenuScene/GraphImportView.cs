using System;
using System.IO;
using Core.LoadSystem;
using Core.LoadSystem.Serializable;
using Newtonsoft.Json;
using SimpleFileBrowser;
using TMPro;
using UI.UiStructures.MainMenuPages;
using UnityEngine;
using UnityEngine.UI;

namespace UI.View.MainMenuScene
{
    public class GraphImportView : CanvasView
    {
        [SerializeField] private Button _selectPathButton;
        [SerializeField] private TMP_InputField _sessionName;
        [SerializeField] private TMP_InputField _pathField;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _cancelButton;
        private string _json;

        public override void Show(bool isInitialHide = false)
        {
            base.Show(isInitialHide);
            _confirmButton.interactable = false;
            _pathField.SetTextWithoutNotify(string.Empty);
            _json = string.Empty;
            _sessionName.SetTextWithoutNotify(string.Empty);
        }

        private void OnEnable()
        {
            _selectPathButton.onClick.AddListener(StartImportFile);
            _confirmButton.onClick.AddListener(CreateNewSession);
            _cancelButton.onClick.AddListener(CloseThisView);
        }

        private void OnDisable()
        {
            _selectPathButton.onClick.AddListener(StartImportFile);
            _confirmButton.onClick.AddListener(CreateNewSession);
            _cancelButton.onClick.AddListener(CloseThisView);
        }

        private void CreateNewSession()
        {
            var projList = MainView.GetPage<ProjectList>();
            var key = string.IsNullOrEmpty(_sessionName.text)
                ? projList.GetTemplateName()
                : _sessionName.text;
            SaveManager.StoreSession(key, _json);
            projList.CreateNewProject(key);
            CloseThisView();
        }

        private static void CloseThisView() => UIManager.Instance.HideHUD<GraphImportView>();

        private void StartImportFile()
        {
            FileBrowser.SetFilters(true, "json");
            FileBrowser.ShowLoadDialog(OnImportFileSelected, null, FileBrowser.PickMode.Files);
        }
        
        private void OnImportFileSelected(string[] paths)
        {
            var path = paths[0];
            try
            {
                var json = File.ReadAllText(path);
                JsonConvert.DeserializeObject<SerializableAdjacencyMatrix>(json);
                
                _json = json;
                _pathField.SetTextWithoutNotify(path);
                _confirmButton.interactable = true;
            }
            // Any error while deserializing/reading file
            catch (Exception)
            {
                // TODO: Display error message to user
                _confirmButton.interactable = false;
            }
        }
    }
}