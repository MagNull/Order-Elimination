using System;
using System.Collections.Generic;
using OrderElimination;
using StartSessionMenu.ChooseCharacter.CharacterCard;
using UnityEngine;
using UnityEngine.Events;

namespace RoguelikeMap.Panels
{
    //TODO(coder): Merge with ChoosingCharacter.cs
    public class SquadMembersPanel : Panel
    {
        [SerializeField]
        private Transform _selected;
        [SerializeField]
        private Transform _notSelected;
        [SerializeField]
        private GameObject _characterButtonPref;

        private List<Character> _selectedCharacters;
        private List<Character> _unselectedCharacters;
        
        public event Action<List<Character>> OnSelected;
        
        public void UpdateMembers(List<Character> members)
        {
            _selectedCharacters = members;
            InitializeCharactersCard();
        }

        public void InitializeCharactersCard()
        {
            _unselectedCharacters = new List<Character>();

            foreach (var info in _selectedCharacters)
            {
                var characterCard = Instantiate(_characterButtonPref, _selected);
                var characterCardInfo = characterCard.GetComponent<CharacterCardWithHealthBar>();
                characterCardInfo.InitializeCard(info);
                UnityAction act = () => SelectCharacter(characterCardInfo);
                characterCardInfo.Button.onClick.AddListener(act);
            }
        }

        public void SelectCharacter(CharacterCardWithHealthBar card)
        {
            if (!card._isSelected)
            {
                card.transform.SetParent(_selected);
                _selectedCharacters.Remove(card.Character);
                _unselectedCharacters.Add(card.Character);
                card.transform.SetSiblingIndex(_selectedCharacters.Count - 1);
                card.Select();
            }
            else if (card._isSelected)
            {
                card.transform.SetParent(_notSelected);
                _unselectedCharacters.Remove(card.Character);
                _selectedCharacters.Add(card.Character);
                card.Select();
            }
        }

        public override void Close()
        {
            SaveCharacters();
            base.Close();
        }

        public void SaveCharacters()
        {
            OnSelected?.Invoke(_selectedCharacters);
        }
    }
}