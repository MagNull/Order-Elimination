using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace OrderElimination
{
    public class ChooseCharacterScreen : MonoBehaviour
    {
        [SerializeField]
        private Transform _selected;
        [SerializeField]
        private Transform _notSelected;

        [SerializeField] 
        private GameObject _characterButtonPref;
        [SerializeField] 
        private GameObject _emptyButtonPref;
        [SerializeField]
        private int _amountAvailable = 1000;
        [SerializeField] 
        private int _countOfCharacters = 2;
        [SerializeField] 
        private Text _amountTextUI;

        private List<Character> _selectedCharacters;
        private List<GameObject> _emptyCards;
        private void Start()
        {
            CreatePanel();
        }
        public void CreatePanel()
        {
            _selectedCharacters = new List<Character>();
            _amountTextUI.text = _amountAvailable.ToString() + "$";
            
            var infos = Resources.LoadAll<Character>("Characters");
            foreach (var info in infos)
            {
                var characterCard = Instantiate(_characterButtonPref, _notSelected);
                var characterCardInfo = characterCard.GetComponent<CharacterCard>();
                characterCardInfo.InitializeCard(info, 400);
                UnityAction act = () => SelectCharacter(characterCardInfo);
                characterCardInfo.Button.onClick.AddListener(act);
            }

            _emptyCards = new List<GameObject>();
            for (int i = 0; i < _countOfCharacters; i++)
            {
                var card = Instantiate(_emptyButtonPref, _selected);
                _emptyCards.Add(card);
            }
                
        }

        public void SelectCharacter(CharacterCard card)
        {
            if (!card._isSelected && _amountAvailable - card.Cost >= 0)
            {
                _amountAvailable -= card.Cost;
                card.transform.SetParent(_selected);
                _selectedCharacters.Add(card.Character);
                card.transform.SetSiblingIndex(_selectedCharacters.Count - 1);
                _emptyCards[_selectedCharacters.Count - 1].SetActive(false);
                card.Select();
            }
            else if (card._isSelected)
            {
                _amountAvailable += card.Cost;
                card.transform.SetParent(_notSelected);
                _selectedCharacters.Remove(card.Character);
                _emptyCards[_selectedCharacters.Count].SetActive(true);
                card.Select();
            }

            _amountTextUI.text = _amountAvailable.ToString() + "$";
        }
        
    }
}
