using System;
using System.Collections.Generic;
using OrderElimination;
using StartSessionMenu.ChooseCharacter.CharacterCard;
using UnityEngine;
using UnityEngine.Events;

namespace RoguelikeMap.Panels
{
    public abstract class ChoosingSquadMembersPanel : Panel
    {
        [SerializeField]
        protected Transform _selectedTransform;
        [SerializeField]
        protected Transform _notSelectedTransform;
        [SerializeField]
        protected GameObject _characterButtonPref;
        
        protected List<Character> _selectedCharacters = new List<Character>();
        protected List<Character> _unselectedCharacters = new List<Character>();
        
        protected void InitializeCharactersCard(List<Character> characterToSelect, Transform parent)
        {
            foreach (var info in characterToSelect)
            {
                var characterCard = Instantiate(_characterButtonPref, parent);
                var characterCardInfo = characterCard.GetComponent<CharacterCard>();
                characterCardInfo.InitializeCard(info);
                UnityAction act = () => SelectCharacter(characterCardInfo);
                characterCardInfo.Button.onClick.AddListener(act);
            }
        }

        protected void SelectCard(CharacterCard card)
        {
            card.transform.SetParent(_selectedTransform);
            _selectedCharacters.Add(card.Character);
            _unselectedCharacters.Remove(card.Character);
            card.transform.SetSiblingIndex(_selectedCharacters.Count - 1);
            card.Select();
        }

        protected void UnselectCard(CharacterCard card)
        {
            card.transform.SetParent(_notSelectedTransform);
            _unselectedCharacters.Add(card.Character);
            _selectedCharacters.Remove(card.Character);
            card.Select();
        }
        
        protected virtual void SelectCharacter(CharacterCard card)
        {
            throw new System.NotImplementedException();
        }
    }
}