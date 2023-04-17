using System;
using System.Collections.Generic;
using OrderElimination;
using StartSessionMenu.ChooseCharacter.CharacterCard;

namespace RoguelikeMap.Panels
{
    public class SquadMembersPanel : ChoosingSquadMembersPanel
    {
        public event Action<List<Character>> OnSelected;
        
        public void UpdateMembers(List<Character> members)
        {
            _selectedCharacters = members;
            InitializeCharactersCard(members, _selectedTransform);
        }

        protected override void SelectCharacter(CharacterCard card)
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

        public override void Close()
        {
            SaveCharacters();
            base.Close();
        }

        private void SaveCharacters()
        {
            OnSelected?.Invoke(_selectedCharacters);
        }
    }
}