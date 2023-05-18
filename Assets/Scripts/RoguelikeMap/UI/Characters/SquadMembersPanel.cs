using System;
using System.Collections.Generic;
using System.Linq;
using OrderElimination;
using StartSessionMenu.ChooseCharacter.CharacterCard;

namespace RoguelikeMap.UI.Characters
{
    public class SquadMembersPanel : ChoosingSquadMembersPanel
    {
        public event Action<List<Character>> OnSelected;
        
        public void UpdateMembers(List<Character> members)
        {
            InitializeCharactersCard(members, _selectedTransform);
        }

        protected override void TrySelectCard(CharacterCard card)
        {
            if (card is CharacterCardWithHealthBar characterCardWithCost)
                SelectCharacter(characterCardWithCost);
            else
                throw new ArgumentException();
        }

        private void SelectCharacter(CharacterCardWithHealthBar card)
        {
            if (card.IsSelected)
                SelectCard(card);
            else if (!card.IsSelected)
                UnselectCard(card);
        }

        protected override void ShowCharacterInfo(CharacterCard card)
        {
            _characterInfoPanel.InitializeCharacterInfo(card.Character);
            _characterInfoPanel.Open();
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
            OnSelected?.Invoke(characters);
        }
    }
}