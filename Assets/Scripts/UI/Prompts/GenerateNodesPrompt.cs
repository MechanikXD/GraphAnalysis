using System.Collections.Generic;
using Core.Structure;
using Core.Structure.PlayerController;
using Other;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Prompts
{
    public class GenerateNodesPrompt : PromptBase
    {
        [SerializeField] private Button _addButton;
        [SerializeField] private Button _deleteButton;
        [SerializeField] private Transform _pairsRoot;
        [SerializeField] private NodePositionNamePair _prefab;
        private readonly List<NodePositionNamePair> _pairs = new List<NodePositionNamePair>();

        protected override void OnEnable()
        {
            base.OnEnable();
            _addButton.onClick.AddListener(AddNewPair);
            _deleteButton.onClick.AddListener(DeleteLastPair);
            _confirmButton.interactable = false;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _addButton.onClick.RemoveListener(AddNewPair);
            _deleteButton.onClick.RemoveListener(DeleteLastPair);
        }

        private void AddNewPair()
        {
            var newPair = Instantiate(_prefab, _pairsRoot);
            newPair.Clear();
            _pairs.Add(newPair);
            if (!_deleteButton.interactable) _deleteButton.interactable = true;
            if (!_confirmButton.interactable) _confirmButton.interactable = true;
        }
        
        private void DeleteLastPair()
        {
            Destroy(_pairs[^1].gameObject);
            _pairs.RemoveAt(_pairs.Count - 1);
            if (_pairs.Count == 0)
            {
                _deleteButton.interactable = false;
                _confirmButton.interactable = false;
            }
        }
        
        public override void OnConfirm()
        {
            var names = new string[_pairs.Count];
            var pos = new Vector2[_pairs.Count];
            for (var i = 0; i < _pairs.Count; i++)
            {
                var currentPair = _pairs[i].GetData();
                names[i] = currentPair.name;
                pos[i] = currentPair.pos;
            }
            foreach (var pair in _pairs) Destroy(pair.gameObject);
            
            GameManager.Instance.GenerateTempNodes(pos, names);
            PlayerController.EnterGraphAdjust();
        }

        public override void OnCancel()
        {
            foreach (var pair in _pairs) Destroy(pair.gameObject);
        }
    }
}