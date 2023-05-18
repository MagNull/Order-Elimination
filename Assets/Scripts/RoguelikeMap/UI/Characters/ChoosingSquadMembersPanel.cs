using System;
using System.Collections.Generic;
using OrderElimination;
using Sirenix.OdinInspector;
using StartSessionMenu.ChooseCharacter.CharacterCard;
using UnityEngine;

namespace RoguelikeMap.UI.Characters
{
    public abstract class ChoosingSquadMembersPanel : Panel
    {
        [SerializeField] 
        protected CharacterInfoPanel _characterInfoPanel;

        [Title("Transforms")]
        [SerializeField]
        protected Transform _selectedTransform;
        [SerializeField]
        protected Transform _notSelectedTransform;
        [SerializeField]
        protected Transform _defaultParent;

        [Title("Card")]
        [SerializeField]
        protected CharacterCard _characterButtonPref;
        
        protected List<CharacterCard> _characterCards = new ();
        
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
            _characterCards.Clear();
            base.Close();
        }
    }
}