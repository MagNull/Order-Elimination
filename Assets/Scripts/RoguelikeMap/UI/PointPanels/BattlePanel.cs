using System;
using System.Collections.Generic;
using DG.Tweening;
using OrderElimination;
using OrderElimination.MacroGame;
using RoguelikeMap.UI.Characters;
using StartSessionMenu.ChooseCharacter.CharacterCard;
using TMPro;
using UnityEngine;
using VContainer;

namespace RoguelikeMap.UI.PointPanels
{
    public class BattlePanel : Panel
    {
        [SerializeField]
        private TMP_Text _text;
        [SerializeField]
        private Transform _characterParent;
        [SerializeField]
        private CharacterCard _characterCardPrefab;
        [SerializeField]
        private BattleScenarioVisualiser _scenarioVisualiser;

        private List<CharacterTemplate> _enemies;
        private readonly List<CharacterCard> _cards = new();
        private CharacterInfoPanel _characterInfoPanel;
        private int _pointIndex;

        public event Action<int> OnAccepted;

        [Inject]
        public void Construct(CharacterInfoPanel characterInfoPanel)
        {
            _characterInfoPanel = characterInfoPanel;
        }

        public void Initialize(string pointName,
            BattleScenario battleScenario, IReadOnlyList<GameCharacter> enemies,
            IReadOnlyList<GameCharacter> allies, int pointIndex)
        {
            _text.text = pointName;
            if (_cards.Count != 0)
                Clear();
            foreach (var enemy in enemies)
            {
                var characterCard = Instantiate(_characterCardPrefab, _characterParent);
                characterCard.InitializeCard(enemy, false);
                characterCard.OnClicked += ShowCharacterInfo;
                _cards.Add(characterCard);
            }

            _scenarioVisualiser.Initialize(battleScenario, enemies, allies);
            _pointIndex = pointIndex;
        }

        private void Clear()
        {
            foreach (var card in _cards)
                Destroy(card.gameObject);
            _cards.Clear();
            _scenarioVisualiser.SetActiveCells(false);
        }

        private void ShowCharacterInfo(CharacterCard card)
        {
            _characterInfoPanel.InitializeCharacterInfo(card.Character);
            _characterInfoPanel.Open();
        }

        public void UpdateAlliesOnMap(IReadOnlyList<GameCharacter> allies)
        {
            _scenarioVisualiser.SetActiveAlliesCells(false);
            _scenarioVisualiser.UpdateCharactersCells(allies);
        }

        public void OnClickAttackButton()
        {
            OnAccepted?.Invoke(_pointIndex);
            _scenarioVisualiser.SetActiveCells(false);
            base.Close();
        }

        protected override void OpenWithShift()
        {
            gameObject.SetActive(true);
            if (_scaleRatio is default(float))
                InitializeCanvasSettings();

            transform.DOMoveX(Screen.width - _shift * _scaleRatio - 25f, _windowOpeningTime);
            if (_isHaveCameraShift)
                DoCameraShift();
        }

        protected override void CloseWithShift()
        {
            if (_scaleRatio is default(float))
                InitializeCanvasSettings();

            transform.DOMoveX(Screen.width + _shift * _scaleRatio + 30f, _windowOpeningTime)
                .OnComplete(() => gameObject.SetActive(false));
            if (_isHaveCameraShift)
                DoCameraShift();
        }
    }
}