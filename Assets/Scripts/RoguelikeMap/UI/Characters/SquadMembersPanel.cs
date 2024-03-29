using System;
using System.Collections.Generic;
using System.Linq;
using OrderElimination;
using OrderElimination.MacroGame;
using Sirenix.Serialization;
using StartSessionMenu.ChooseCharacter.CharacterCard;
using UnityEngine;

namespace RoguelikeMap.UI.Characters
{
    public class SquadMembersPanel : ChoosingSquadMembersPanel
    {
        [SerializeField] 
        private int MaxSquadSize = 3;
        
        private int _selectedCount = -1;
        
        public event Action<List<GameCharacter>, int> OnSelected;

        public void UpdateMembers(IReadOnlyList<GameCharacter> activeMembers, IReadOnlyList<GameCharacter> inactiveMembers)
        {
            if (_characterCards is not null)
            {
                Unsubscribe();
                ResetCharactersCard();
            }
            Subscribe();
            _selectedCount = activeMembers.Count;
            InitializeCharactersCard(activeMembers, _selectedDropZone.transform, true);
            InitializeCharactersCard(inactiveMembers, _unselectedDropZone.transform);
        }

        protected override void TrySelectCard(DropZone dropZone, CharacterCard card)
        {
            if (card is CharacterCardWithHealthBar characterCardWithHealthBar)
                TrySelectCard(dropZone, characterCardWithHealthBar);
            else
                Logging.LogException( new ArgumentException());
        }

        private void TrySelectCard(DropZone dropZone, CharacterCardWithHealthBar card)
        {
            if (_selectedDropZone == dropZone)
            {
                if (card.IsSelected || _selectedCount >= MaxSquadSize) return;
                SelectCard(card, _selectedDropZone.transform);
                _selectedCount++;
            }
            else
            {
                if (!card.IsSelected)
                    return;
                SelectCard(card, _unselectedDropZone.transform);
                _selectedCount--;
            }
        }

        public override void Close()
        {
            SaveCharacters();
            base.Close();
        }

        private void SaveCharacters()
        {
            var characters = _characterCards
                .Where(x => x.IsSelected)
                .Select(x => x.Character)
                .ToList();
            var countActiveCharacters = characters.Count;
            characters
                .AddRange(_characterCards
                    .Where(x => !x.IsSelected)
                    .Select(x => x.Character));
            OnSelected?.Invoke(characters, countActiveCharacters);
        }

        private void ResetCharactersCard()
        {
            foreach (var card in _characterCards)
                Destroy(card.gameObject);
            _characterCards.Clear();
        }
    }
}