using Core.LoadSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UiStructures.MainMenuPages
{
    public class ProjectList : MenuPage
    {
        [SerializeField] private Transform _projectRoot;
        [SerializeField] private MenuProjectData _projectDataPrefab;
        [SerializeField] private Button _newProjButton;
        private const string PROJECT_NAME_TEMPLATE = "New Project ";
        private int _projectCounter;
        private bool _initialized;
        
        protected override void Initialize()
        {
            LoadSessions();
            base.Initialize();
        }

        private async void Start()
        {
            // To prevent OnApplicationFocus early calls
            await UniTask.Yield(PlayerLoopTiming.PostLateUpdate, destroyCancellationToken);
            _initialized = true;
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            if (_initialized && !hasFocus) SaveManager.SerializeSessionKeys();
        }

        private void OnEnable()
        {
            _newProjButton.onClick.AddListener(CreateNewProject);
        }

        private void OnDisable()
        {
            OnApplicationFocus(false);
            _newProjButton.onClick.RemoveListener(CreateNewProject);
        }

        private void CreateNewProject()
        {
            var newProject = Instantiate(_projectDataPrefab, _projectRoot);
            newProject.Load(PROJECT_NAME_TEMPLATE + _projectCounter);
            _projectCounter++;
        }

        private void LoadSessions()
        {
            SaveManager.DeserializeSessionKeys();
            var allSessions = SaveManager.SessionList;
            foreach (var session in allSessions)
            {
                var newProject = Instantiate(_projectDataPrefab, _projectRoot);
                newProject.Load(session);
            }

            _projectCounter = allSessions.Count;
        }
    }
}