using System;
using System.Collections.Generic;
using OrderElimination;
using RoguelikeMap.Panels;
using StartSessionMenu.ChooseCharacter.CharacterCard;
using UnityEngine;
using UnityEngine.Events;
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
        
        protected override void SelectCharacter(CharacterCard.CharacterCard card)
        {
            if (card is CharacterCardWithCost characterCardWithCost)
                SelectCharacter(characterCardWithCost);
            else
                throw new ArgumentException();
        }

        private void SelectCharacter(CharacterCardWithCost card)
        {
            if (!card.IsSelected && _wallet.Money - card.Cost >= 0
                && _selectedCharacters.Count < MaxSquadSize)
            {
                _wallet.SubtractMoney(card.Cost);
                SelectCard(card);
            }
            else if (card.IsSelected)
            {
                _wallet.AddMoney(card.Cost);
                UnselectCard(card);
            }
        }

        public void SaveCharacters()
        {
            if (_selectedCharacters.Count <= 0)
                return;

            SquadMediator.SetCharacters(_selectedCharacters);
        }
    }
}