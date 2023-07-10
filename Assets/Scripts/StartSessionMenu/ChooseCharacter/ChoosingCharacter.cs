using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using OrderElimination;
using OrderElimination.MacroGame;
using OrderElimination.UI;
using RoguelikeMap.UI.Characters;
using StartSessionMenu.ChooseCharacter.CharacterCard;
using UnityEngine;
using UnityEngine.UI;

namespace StartSessionMenu.ChooseCharacter
{
    public class ChoosingCharacter : ChoosingSquadMembersPanel
    {
        [SerializeField]
        private Button _startGameButton;
        [SerializeField]
        private MoneyCounter _uiCounter;
        [SerializeField]
        private List<CharacterTemplate> _characters;
        [SerializeField]
        private List<DropZone> _selectedDropZones;
        [SerializeField]
        private int MaxSquadSize = 3;
        [SerializeField]
        private ScrollRect _scrollRect;
        [SerializeField]
        private int StartMoney = 1200;
        
        private Wallet _wallet;
        private int _selectedCount = 0;
        private Tweener _tweener;

        private void Start()
        {
            _wallet = new Wallet(StartMoney);
            InitializeCharactersCard();
            foreach (var zone in _selectedDropZones)
                zone.OnTrySelect += TrySelectCard;
            SetActiveStartButton();
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
            if(_selectedDropZones.Contains(dropZone))
            {
                if (card.IsSelected
                    || _wallet.Money - card.Cost < 0
                    || _selectedCount >= MaxSquadSize) 
                    return;
                if (dropZone.IsSelected)
                    return;
                _wallet.SubtractMoney(card.Cost);
                SelectCard(card, dropZone.transform);
                _selectedCount++;
            }
            else
            {
                if (!card.IsSelected) 
                    return;
                _wallet.AddMoney(card.Cost);
                SelectCard(card, _unselectedDropZone.transform);
                _selectedCount--;
            }
            card.SetDropZone(dropZone);
            SetActiveStartButton();
        }

        private void SetActiveStartButton()
        {
            _startGameButton.DOInterectable(_selectedCount != 0);
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