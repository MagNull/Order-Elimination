using System;
using System.Collections.Generic;
using OrderElimination;
using OrderElimination.MacroGame;
using Sirenix.OdinInspector;
using StartSessionMenu.ChooseCharacter.CharacterCard;
using UnityEngine;
using VContainer;

namespace RoguelikeMap.UI.Characters
{
    public abstract class ChoosingSquadMembersPanel : Panel
    {
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

        private CharacterInfoPanel _characterInfoPanel;
        
        [Inject]
        public void Construct(CharacterInfoPanel characterInfoPanel)
        {
            _characterInfoPanel = characterInfoPanel;
        }
        
        protected void InitializeCharactersCard(IEnumerable<GameCharacter> characterToSelect, Transform parent, bool isSelected = false)
        {
            _selectedDropZone.OnTrySelect += TrySelectCard;
            _unselectedDropZone.OnTrySelect += TrySelectCard;
            
            foreach (var gameCharacter in characterToSelect)
            {
                var characterCard = Instantiate(_characterButtonPref, parent);
                characterCard.InitializeCard(gameCharacter, isSelected);
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
            Logging.LogException( new NotImplementedException());
        }

        private void ShowCharacterInfo(CharacterCard card)
        {
            _characterInfoPanel.InitializeCharacterInfo(card.Character);
            _characterInfoPanel.Open();
        }
    }
}