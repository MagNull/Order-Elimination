using System;
using System.Collections.Generic;
using OrderElimination;
using StartSessionMenu.ChooseCharacter.CharacterCard;
using UnityEngine;

namespace RoguelikeMap.Panels
{
    public abstract class ChoosingSquadMembersPanel : Panel
    {
        [SerializeField]
        protected Transform _selectedTransform;
        [SerializeField]
        protected Transform _notSelectedTransform;
        [SerializeField]
        protected Transform _defaultParent;
        [SerializeField]
        protected CharacterCard _characterButtonPref;
        [SerializeField]
        protected Panel _characterInfoPanel;
        
        protected List<CharacterCard> _characterCards = new List<CharacterCard>();
        
        protected void InitializeCharactersCard(List<Character> characterToSelect, Transform parent)
        {
            foreach (var info in characterToSelect)
            {
                var characterCard = Instantiate(_characterButtonPref, parent);
                characterCard.InitializeCard(info, _defaultParent);
                characterCard.OnTrySelect += TrySelectCard;
                characterCard.OnUnselect += UnselectCard;
                characterCard.OnGetInfo += ShowCharacterInfo;
                _characterCards.Add(characterCard);
            }
        }

        protected void SelectCard(CharacterCard card)
        {
            card.transform.SetParent(_selectedTransform);
            card.Select();
        }

        protected void UnselectCard(CharacterCard card)
        {
            card.transform.SetParent(_notSelectedTransform);
            card.Select();
        }
        
        protected virtual void TrySelectCard(CharacterCard card)
        {
            throw new NotImplementedException();
        }

        protected virtual void ShowCharacterInfo(CharacterCard card)
        {
            Debug.Log("ShowCharacterInfo");
        }
        
        public override void Close()
        {
            foreach (var card in _characterCards)
            {
                card.OnTrySelect -= TrySelectCard;
                card.OnUnselect -= UnselectCard;
                card.OnGetInfo -= ShowCharacterInfo;
            }
            _characterCards.Clear();
            base.Close();
        }
    }
}