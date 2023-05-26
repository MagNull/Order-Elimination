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
        
        [Title("Drop Zone")]
        [SerializeField]
        protected DropZone _selectedDropZone;
        [SerializeField]
        protected DropZone _unselectedDropZone;

        [Title("Card")]
        [SerializeField]
        protected CharacterCard _characterButtonPref;

        [ShowInInspector]
        protected List<CharacterCard> _characterCards = new ();

        protected void InitializeCharactersCard(IReadOnlyList<Character> characterToSelect, Transform parent, bool isSelected = false)
        {
            _selectedDropZone.OnTrySelect += TrySelectCard;
            _unselectedDropZone.OnTrySelect += TrySelectCard;
            
            foreach (var info in characterToSelect)
            {
                var characterCard = Instantiate(_characterButtonPref, parent);
                characterCard.InitializeCard(info, isSelected);
                characterCard.OnGetInfo += ShowCharacterInfo;
                _characterCards.Add(characterCard);
            }
        }

        protected void SelectCard(CharacterCard card)
        {
            card.transform.SetParent(_selectedDropZone.transform);
            card.Select();
        }

        protected void UnselectCard(CharacterCard card)
        {
            card.transform.SetParent(_unselectedDropZone.transform);
            card.Select();
        }
        
        protected virtual void TrySelectCard(DropZone dropZone, CharacterCard card)
        {
            throw new NotImplementedException();
        }

        protected virtual void ShowCharacterInfo(CharacterCard card)
        {
            Debug.Log("ShowCharacterInfo");
        }
    }
}