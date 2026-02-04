using System;
using System.Collections.Generic;
using UI.UiStructures.MainMenuPages;
using UnityEngine;

namespace UI.View.MainMenuScene
{
    public class MainView : CanvasView
    {
        [SerializeField] private Transform _optionsRoot;
        [SerializeField] private MenuOption _optionPrefab;

        [SerializeField] private Sprite _projectsIcon;
        [SerializeField] private Sprite _settingsIcon;
        [SerializeField] private Sprite _exitIcon;

        [SerializeField] private MenuPage[] _pages;
        private static MenuPage _currentPage;
        private static Dictionary<Type, MenuPage> _pagesDict;
        
        protected override void Initialize()
        {
            OrderInfos();
            LoadOptions();
        }

        private void LoadOptions()
        {
            CrateOption("Projects", _projectsIcon, ShowPage<ProjectList>);
            CrateOption("Settings", _settingsIcon, ShowPage<SettingsPage>);
            CrateOption("Exit", _exitIcon, Application.Quit);
        }

        private void CrateOption(string title, Sprite image, Action action)
        {
            var newOption = Instantiate(_optionPrefab, _optionsRoot);
            newOption.Load(image, title, action);
        }
        
        public static T GetPage<T>() where T : MenuPage => _pagesDict[typeof(T)] as T;
        
        public static void ShowPage<T>() where T : MenuPage
        {
            if (_currentPage != null) _currentPage.Hide();
            _currentPage = _pagesDict[typeof(T)] as T;
            if (_currentPage != null) _currentPage.Show();
        }

        private void OrderInfos()
        {
            _pagesDict = new Dictionary<Type, MenuPage>();
            foreach (var page in _pages)
            {
                _pagesDict.Add(page.GetType(), page);
                page.Initialize();
                if (!page.DisplayOnStart) page.Hide();
                else
                {
                    if (_currentPage != null)
                    {
                        Debug.LogWarning("Several Pages are displayed on start!");
                        _currentPage.Hide();
                    }

                    _currentPage = page;
                }
            }
        }
    }
}