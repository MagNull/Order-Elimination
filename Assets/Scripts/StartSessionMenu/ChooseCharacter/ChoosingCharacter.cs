using System.Collections.Generic;
using OrderElimination;
using StartSessionMenu.ChooseCharacter.CharacterCard;
using UnityEngine;
using UnityEngine.Events;

namespace StartSessionMenu.ChooseCharacter
{
    public class ChoosingCharacter : MonoBehaviour
    {
        [SerializeField]
        private Transform _selected;
        [SerializeField]
        private Transform _notSelected;

        [SerializeField]
        private GameObject _characterButtonPref;
        [SerializeField] 
        private MoneyCounter _uiCounter;
        
        private Wallet _wallet;
        
        [SerializeField] 
        private List<Character> _characters;
        
        private List<Character> _selectedCharacters = new List<Character>();
        private List<Character> _unselectedCharacters = new List<Character>();
        
        private void Start()
        {
            CreatePanel();
        }

        public void SetWallet(Wallet wallet)
        {
            _wallet = wallet;
        }
        
        public void CreatePanel()
        {
            _uiCounter.Initialize(_wallet);

            _selectedCharacters = new List<Character>();
            _unselectedCharacters = new List<Character>();

            foreach (var info in _characters)
            {
                var characterCard = Instantiate(_characterButtonPref, _notSelected);
                var characterCardInfo = characterCard.GetComponent<CharacterCardWithCost>();
                characterCardInfo.InitializeCard(info, 400);
                UnityAction act = () => SelectCharacter(characterCardInfo);
                characterCardInfo.Button.onClick.AddListener(act);
            }
        }

        public void SelectCharacter(CharacterCardWithCost card)
        {
            if (!card._isSelected && _wallet.Money - card.Cost >= 0)
            {
                _wallet.SubtractMoney(card.Cost);
                card.transform.SetParent(_selected);
                _selectedCharacters.Add(card.Character);
                _unselectedCharacters.Remove(card.Character);
                card.transform.SetSiblingIndex(_selectedCharacters.Count - 1);
                card.Select();
            }
            else if (card._isSelected)
            {
                _wallet.AddMoney(card.Cost);
                card.transform.SetParent(_notSelected);
                _unselectedCharacters.Add(card.Character);
                _selectedCharacters.Remove(card.Character);
                card.Select();
            }
        }

        public void SaveCharacters()
        {
            if (_selectedCharacters.Count <= 0)
                return;
            
            SquadMediator.SetCharacters(_selectedCharacters);
        }
    }
}
