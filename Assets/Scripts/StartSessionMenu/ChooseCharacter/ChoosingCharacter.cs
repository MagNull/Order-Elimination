using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using OrderElimination;
using OrderElimination.MetaGame;
using RoguelikeMap.UI.Characters;
using StartSessionMenu.ChooseCharacter.CharacterCard;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace StartSessionMenu.ChooseCharacter
{
    public class ChoosingCharacter : ChoosingSquadMembersPanel
    {
        [SerializeField] 
        private MoneyCounter _uiCounter;
        [SerializeField]
        private List<CharacterTemplate> _characters;
        [SerializeField]
        private int MaxSquadSize = 3;
        [SerializeField] 
        private ScrollRect _scrollRect;
        
        private Wallet _wallet;
        private int _selectedCount = 0;
        private Tweener _tweener;

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
            var gameCharacters = GameCharactersFactory.CreateGameEntities(_characters);
            InitializeCharactersCard(gameCharacters, _unselectedDropZone.transform);
        }
        
        protected override void TrySelectCard(DropZone dropZone, CharacterCard.CharacterCard card)
        {
            if (card is CharacterCardWithCost characterCardWithCost)
                TrySelectCard(dropZone, characterCardWithCost);
            else
                Logging.LogException( new ArgumentException());
        }

        private void TrySelectCard(DropZone dropZone, CharacterCardWithCost card)
        {
            if (dropZone == _selectedDropZone)
            {
                if (card.IsSelected
                    || _wallet.Money - card.Cost < 0
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

        public bool SaveCharacters()
        {
            if (_selectedCount <= 0)
                return false;

            var characters = _characterCards
                .Where(x => x.IsSelected)
                .Select(x => x.Character)
                .ToList();
            SquadMediator.SetCharacters(characters);
            return true;
        }

        public void ClickShift(float shift)
        {
            _tweener?.Kill();
            _tweener = DOVirtual.Float(_scrollRect.horizontalNormalizedPosition, _scrollRect.horizontalNormalizedPosition + shift, 0.1f, (x) => _scrollRect.horizontalNormalizedPosition = x);
        }
    }
}