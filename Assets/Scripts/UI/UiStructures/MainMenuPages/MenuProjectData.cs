using Core.LoadSystem;
using Core.Structure;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.UiStructures.MainMenuPages
{
    public class MenuProjectData : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _title;
        private string _currentTitle;
        [SerializeField] private Button _openButton;
        [SerializeField] private Button _deleteButton;

        public void Load(string title)
        {
            _title.SetTextWithoutNotify(title);
            _currentTitle = title;
            _openButton.onClick.AddListener(LoadGraphScene);
            _deleteButton.onClick.AddListener(DeleteProject);
            _title.onEndEdit.AddListener(UpdateProjectTitle);
        }

        public void Clear()
        {
            _title.SetTextWithoutNotify(string.Empty);
            _currentTitle = string.Empty;
            _openButton.onClick.RemoveListener(LoadGraphScene);
            _deleteButton.onClick.RemoveListener(DeleteProject);
            _title.onEndEdit.RemoveListener(UpdateProjectTitle);
        }

        private void UpdateProjectTitle(string newTitle)
        {
            SaveManager.RenameSession(_currentTitle, newTitle);
            _currentTitle = newTitle;
        }

        private void DeleteProject()
        {
            SaveManager.DeleteSession(_currentTitle);
            Clear();
            Destroy(gameObject);
        }

        private void LoadGraphScene()
        {
            SceneManager.LoadScene("MainScene");
            SceneManager.sceneLoaded += ConfigureGameManager;
        }

        private void ConfigureGameManager(Scene scene, LoadSceneMode loadSceneMode)
        {
            GameManager.Instance.ConfigureLoadOptions(_currentTitle);
            SceneManager.sceneLoaded -= ConfigureGameManager;
        }
    }
}