using System;
using System.Collections.Generic;
using System.Linq;
using OrderElimination;
using RoguelikeMap.Panels;
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
        
        private Wallet _wallet;
        private int _selectedCount = 0;
        
        public int MaxSquadSize { get; private set; } = 3;

        [Inject]
        public void Configure(Wallet wallet)
        {
            _wallet = wallet;
        }
        
        private void Start()
        {
            InitializeCharactersCard();
        }

        private void InitializeCharactersCard()
        {
            _uiCounter.Initialize(_wallet);
            base.InitializeCharactersCard(_characters, _notSelectedTransform);
        }
        
        protected override void TrySelectCard(CharacterCard.CharacterCard card)
        {
            if (card is CharacterCardWithCost characterCardWithCost)
                SelectCharacter(characterCardWithCost);
            else
                throw new ArgumentException();
        }

        private void SelectCharacter(CharacterCardWithCost card)
        {
            if (!card.IsSelected && _wallet.Money - card.Cost >= 0
                && _selectedCount < MaxSquadSize)
            {
                _wallet.SubtractMoney(card.Cost);
                SelectCard(card);
                _selectedCount++;
            }
            else if (card.IsSelected)
            {
                _wallet.AddMoney(card.Cost);
                UnselectCard(card);
                _selectedCount--;
            }
            else
            {
                card.SetInitialParent();
            }
        }

        protected override void ShowCharacterInfo(CharacterCard.CharacterCard card)
        {
            
            _characterInfoPanel.Open();
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