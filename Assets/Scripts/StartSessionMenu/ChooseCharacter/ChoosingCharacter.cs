using System;
using System.Collections.Generic;
using System.Linq;
using OrderElimination;
using RoguelikeMap.UI.Characters;
using StartSessionMenu.ChooseCharacter.CharacterCard;
using UnityEngine;
using VContainer;

namespace StartSessionMenu.ChooseCharacter
{
    public class ChoosingCharacter : ChoosingSquadMembersPanel
    {
        [SerializeField] 
        private MoneyCounter _uiCounter;
        [SerializeField]
        private List<Character> _characters;
        [SerializeField]
        private int MaxSquadSize = 3;
        
        private Wallet _wallet;
        private int _selectedCount = 0;

        [Inject]
        public void Configure(Wallet wallet)
        {
            _wallet = wallet;
        }
        
        private void Start()
        {
            InitializeCharactersCard();
            _selectedDropZone.OnTrySelect += TrySelectCard;
        }

        private void InitializeCharactersCard()
        {
            _uiCounter.Initialize(_wallet);
            base.InitializeCharactersCard(_characters, _unselectedDropZone.transform);
        }
        
        protected override void TrySelectCard(DropZone dropZone, CharacterCard.CharacterCard card)
        {
            if (card is CharacterCardWithCost characterCardWithCost)
                TrySelectCard(dropZone, characterCardWithCost);
            else
                throw new ArgumentException();
        }

        private void TrySelectCard(DropZone dropZone, CharacterCardWithCost card)
        {
            if (dropZone == _selectedDropZone)
            {
                if (card.IsSelected || _wallet.Money - card.Cost < 0
                                    || _selectedCount >= MaxSquadSize) 
                    return;
                _wallet.SubtractMoney(card.Cost);
                SelectCard(card);
                _selectedCount++;
            }
            else
            {
                if (!card.IsSelected) 
                    return;
                _wallet.AddMoney(card.Cost);
                UnselectCard(card);
                _selectedCount--;
            }
        }

        public void SaveCharacters()
        {
            if (_selectedCount <= 0)
                return;

            var characters = _characterCards
                .Where(x => x.IsSelected)
                .Select(x => x.Character)
                .ToList();
            SquadMediator.SetCharacters(characters);
        }
    }
}