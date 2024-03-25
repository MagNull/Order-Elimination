using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using OrderElimination;
using OrderElimination.MacroGame;
using OrderElimination.SavesManagement;
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
        private List<DropZone> _selectedDropZones;
        [SerializeField]
        private ScrollRect _scrollRect;
        private Tweener _tweener;
        private HashSet<CharacterCardWithCost> _selectedCards;
        private IPlayerProgressManager _playerProgressManager;

        private IPlayerProgress PlayerProgress => _playerProgressManager.GetPlayerProgress();
        private int MaxSquadSize => PlayerProgress.MetaProgress.MaxSquadSize;
        private int HireCurrencyLimit => PlayerProgress.MetaProgress.HireCurrencyLimit;
        private int SelectedCharactersTotalCost => _selectedCards.Sum(c => c.Cost);
        private int LeftHireCurrency => HireCurrencyLimit - SelectedCharactersTotalCost;
        private int SelectedCharactersCount => _selectedCards.Count;

        [Inject]
        private void Construct(ScenesMediator scenesMediator)
        {
            _playerProgressManager = scenesMediator
                .Get<IPlayerProgressManager>(MediatorRegistration.ProgressManager);
        }

        private void Start()
        {
            InitializeCharactersCard();
            foreach (var zone in _selectedDropZones)
                zone.OnTrySelect += TrySelectCard;
            OnUpdateSelection();
        }

        private void InitializeCharactersCard()
        {
            var availableCharacters = PlayerProgress.MetaProgress.UnlockedCharacters;
            _selectedCards = new();
            _uiCounter.UpdateValue(HireCurrencyLimit);
            var gameCharacters = GameCharactersFactory.CreateGameCharacters(availableCharacters);
            Subscribe();
            InitializeCharactersCard(gameCharacters, _unselectedDropZone.transform);
            foreach (var card in _characterCards)
                card.GetComponent<DraggableObject>().Dropped += DeselectCard;
        }

        protected override void TrySelectCard(DropZone dropZone, CharacterCard.CharacterCard card)
        {
            if (card is not CharacterCardWithCost characterCardWithCost)
                throw new ArgumentException("Card is invalid");
            TrySelectCard(dropZone, characterCardWithCost);
        }

        private void TrySelectCard(DropZone dropZone, CharacterCardWithCost card)
        {
            if(!_selectedDropZones.Contains(dropZone))
                return;
            if (!dropZone.IsEmpty)
            {
                var existingCard = (CharacterCardWithCost)dropZone.CharacterCard;
                if (LeftHireCurrency + existingCard.Cost < card.Cost)
                    return;
                _selectedCards.Remove(existingCard);
            }
            if (card.IsSelected
                || LeftHireCurrency < card.Cost
                || SelectedCharactersCount >= MaxSquadSize)
                return;
            SelectCard(card, dropZone.transform);
            dropZone.Select(card);
            _selectedCards.Add(card);
            OnUpdateSelection();
        }

        private void OnUpdateSelection()
        {
            _uiCounter.UpdateValue(LeftHireCurrency);
            _startGameButton.DOInterectable(SelectedCharactersCount > 0);
        }

        private void DeselectCard(CharacterCard.CharacterCard characterCard)
        {
            if (!characterCard.IsSelected
                || characterCard is not CharacterCardWithCost cardWithCost)
                return;
            _selectedCards.Remove(cardWithCost);
            OnUpdateSelection();
        }

        public GameCharacter[] GetSelectedCharacters()
        {
            if (SelectedCharactersCount <= 0)
                return new GameCharacter[0];

            return _selectedCards
                .Select(card => GameCharactersFactory.CreateGameCharacter(
                    card.Character.CharacterData))
                .ToArray();
        }

        public void ClickShift(float shift)
        {
            _tweener?.Kill();
            _tweener = DOVirtual.Float(_scrollRect.horizontalNormalizedPosition,
                _scrollRect.horizontalNormalizedPosition + shift, 0.1f, 
                x => _scrollRect.horizontalNormalizedPosition = x);
        }

        private void OnDestroy()
        {
            foreach (var zone in _selectedDropZones)
                zone.OnTrySelect -= TrySelectCard;
            foreach (var card in _characterCards)
                card.GetComponent<DraggableObject>().Dropped -= DeselectCard;
            Unsubscribe();
        }
    }
}