using System;
using System.Collections.Generic;
using OrderElimination;
using OrderElimination.MacroGame;
using RoguelikeMap.UI.Characters;
using StartSessionMenu.ChooseCharacter.CharacterCard;
using UnityEngine;
using VContainer;

namespace RoguelikeMap.UI.PointPanels
{
    public class BattlePanel : Panel
    {
        [SerializeField] 
        private Transform _characterParent;
        [SerializeField]
        private CharacterCard _characterCardPrefab;
        [SerializeField]
        private BattleScenarioVisualiser _scenarioVisualiser;

        private List<CharacterTemplate> _enemies;
        private CharacterInfoPanel _characterInfoPanel;
        public event Action OnStartAttack;
        
        [Inject]
        public void Construct(CharacterInfoPanel characterInfoPanel)
        {
            _characterInfoPanel = characterInfoPanel;
        }
        
        public void Initialize(BattleScenario battleScenario, 
            IReadOnlyList<GameCharacter> enemies, IReadOnlyList<GameCharacter> allies)
        {
            foreach (var enemy in enemies)
            {
                var characterCard = Instantiate(_characterCardPrefab, _characterParent);
                characterCard.InitializeCard(enemy, false);
                characterCard.OnGetInfo += ShowCharacterInfo;
            }
            _scenarioVisualiser.Initialize(battleScenario, enemies, allies);
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
            OnStartAttack?.Invoke();
            _scenarioVisualiser.SetActiveCells(false);
            base.Close();
        }
    }
}