using System;
using System.Collections.Generic;
using OrderElimination;
using OrderElimination.MacroGame;
using RoguelikeMap.UI.Characters;
using StartSessionMenu.ChooseCharacter.CharacterCard;
using Unity.VisualScripting;
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

        private List<CharacterTemplate> _enemies;
        private CharacterInfoPanel _characterInfoPanel;
        public event Action OnStartAttack;
        
        [Inject]
        public void Initialize(CharacterInfoPanel characterInfoPanel)
        {
            _characterInfoPanel = characterInfoPanel;
        }
        
        public void UpdateEnemies(IEnumerable<GameCharacter> enemies)
        {
            foreach (var enemy in enemies)
            {
                var characterCard = Instantiate(_characterCardPrefab, _characterParent);
                characterCard.InitializeCard(enemy, false);
                characterCard.OnClicked += ShowCharacterInfo;
            }
        }

        private void ShowCharacterInfo(CharacterCard card)
        {
            _characterInfoPanel.InitializeCharacterInfo(card.Character);
            _characterInfoPanel.Open();
        }

        public void OnClickAttackButton()
        {
            OnStartAttack?.Invoke();
            // foreach (var character in _enemies)
            // {
            //     Destroy(character);
            // }
            base.Close();
        }
    }
}