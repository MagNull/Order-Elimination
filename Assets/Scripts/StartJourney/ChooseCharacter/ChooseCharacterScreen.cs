using System;
using System.Collections.Generic;
using DG.Tweening;
using UIManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace OrderElimination
{
    public class ChooseCharacterScreen : UIPanel
    {
        [SerializeField]
        private Transform _selected;
        [SerializeField]
        private Transform _notSelected;

        [SerializeField]
        private GameObject _characterButtonPref;
        [SerializeField]
        private int _amountAvailable = 1000;
        [SerializeField] 
        private int _countOfCharacters = 2;
        [SerializeField] 
        private Text _amountTextUI;
        [SerializeField] 
        private List<Character> _characters;

        public override PanelType PanelType => PanelType.SquadMembers;
        public event Action<List<Character>> OnSelected;
        
        private List<Character> _selectedCharacters;
        private List<Character> _unselectedCharacters;
        
        private void Start()
        {
            CreatePanel();
        }
        
        public void CreatePanel()
        {
            _selectedCharacters = new List<Character>();
            if(_amountTextUI != null)
                _amountTextUI.text = _amountAvailable.ToString() + "$";
            
            foreach (var info in _characters)
            {
                var characterCard = Instantiate(_characterButtonPref, _notSelected);
                var characterCardInfo = characterCard.GetComponent<CharacterCard>();
                characterCardInfo.InitializeCard(info, 400);
                UnityAction act = () => SelectCharacter(characterCardInfo);
                characterCardInfo.Button.onClick.AddListener(act);
            }
        }

        public void SelectCharacter(CharacterCard card)
        {
            if (!card._isSelected && _amountAvailable - card.Cost >= 0)
            {
                _amountAvailable -= card.Cost;
                card.transform.SetParent(_selected);
                _selectedCharacters.Add(card.Character);
                _unselectedCharacters.Remove(card.Character);
                card.transform.SetSiblingIndex(_selectedCharacters.Count - 1);
                card.Select();
            }
            else if (card._isSelected)
            {
                _amountAvailable += card.Cost;
                card.transform.SetParent(_notSelected);
                _unselectedCharacters.Add(card.Character);
                _selectedCharacters.Remove(card.Character);
                card.Select();
            }

            if(_amountTextUI != null)
                _amountTextUI.text = _amountAvailable.ToString() + "$";
        }

        public void UpdateCharacterInfo(List<Character> characters)
        {
            _characters = characters;
            _unselectedCharacters = characters;
        }

        public override void Close()
        {
            OnSelected?.Invoke(_unselectedCharacters);
            transform.DOMoveX(UIController.StartPosition, 0.1f).OnStepComplete(() => base.Close());
        }

        public void LoadGame()
        {
            if (_selectedCharacters.Count <= 0)
                return;
            
            SquadMediator.SetCharacters(_selectedCharacters);
            SceneManager.LoadScene("RougelikeMap");
        }
    }
}
