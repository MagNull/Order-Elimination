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
using VContainer;

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
        private ScenesMediator _mediator;

        [Inject]
        private void Construct(ScenesMediator scenesMediator)
        {
            _mediator = scenesMediator;
        }
        
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
            var gameCharacters = GameCharactersFactory.CreateGameCharacters(_characters);
            InitializeCharactersCard(gameCharacters, _unselectedDropZone.transform);
            foreach (var card in _characterCards)
                card.GetComponent<DraggableObject>().OnDestroy += AddMoneyByDestroy;
        }

        protected override void TrySelectCard(DropZone dropZone, CharacterCard.CharacterCard card)
        {
            if (card is CharacterCardWithCost characterCardWithCost)
                TrySelectCard(dropZone, characterCardWithCost);
            else
                Logging.LogException(new ArgumentException());
        }

        private void TrySelectCard(DropZone dropZone, CharacterCardWithCost card)
        {
            if(!_selectedDropZones.Contains(dropZone))
                return;
            if (dropZone.IsEmpty)
            {
                if (card.IsSelected
                    || _wallet.Money - card.Cost < 0
                    || _selectedCount >= MaxSquadSize) 
                    return;
                _wallet.SubtractMoney(card.Cost);
                SelectCard(card, dropZone.transform);
                _selectedCount++;
                dropZone.Select(card);
            }
            else
            {
                var cost = dropZone.TryGetCost();
                if (cost < 0 || _wallet.Money + cost < card.Cost)
                    return;
                SelectCard(card, dropZone.transform);
                _wallet.SubtractMoney(card.Cost - cost);
                dropZone.Select(card);
            }
            SetActiveStartButton();
        }

        private void SetActiveStartButton()
        {
            _startGameButton.DOInterectable(_selectedCount != 0);
        }

        private void AddMoneyByDestroy(CharacterCard.CharacterCard characterCard)
        {
            if (!characterCard.IsSelected)
                return;
            _wallet.AddMoney(characterCard.Character.CharacterData.Price);
            _selectedCount--;
            SetActiveStartButton();
        }

        public bool SaveCharacters()
        {
            if (_selectedCount <= 0)
                return false;

            var characters = 
                (from zone in _selectedDropZones 
                where zone.CharacterCard != null 
                select GameCharactersFactory.CreateGameCharacter(zone.CharacterCard.Character.CharacterData))
                .ToArray();
            

            _mediator.Register("player characters", characters);
            return true;
        }

        public void ClickShift(float shift)
        {
            _tweener?.Kill();
            _tweener = DOVirtual.Float(_scrollRect.horizontalNormalizedPosition,
                _scrollRect.horizontalNormalizedPosition + shift, 0.1f, 
                x => _scrollRect.horizontalNormalizedPosition = x);
        }
    }
}