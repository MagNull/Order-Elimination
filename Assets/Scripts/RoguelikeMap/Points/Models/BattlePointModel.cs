using System;
using System.Collections.Generic;
using System.Linq;
using OrderElimination;
using OrderElimination.MacroGame;
using RoguelikeMap.SquadInfo;
using RoguelikeMap.UI.PointPanels;
using UnityEngine;

namespace RoguelikeMap.Points.Models
{
    [Serializable]
    public class BattlePointModel : PointModel
    {
        [SerializeField]
        private List<CharacterTemplate> _enemies;
        [SerializeField]
        private BattleScenario _battleScenario;

        [SerializeField]
        private int _itemsCount;

        private List<GameCharacter> _enemiesGameCharacter;
        
        protected BattlePanel Panel => _panel as BattlePanel;
        
        public override PointType Type => PointType.Battle;
        public IReadOnlyList<IGameCharacterTemplate> Enemies => _enemies;
        public BattleScenario Scenario => _battleScenario;

        public int ItemsCount => _itemsCount;

        public override void Visit(Squad squad)
        {
            squad.OnUpdateMembers -= Panel.UpdateAlliesOnMap;
            squad.OnUpdateMembers += Panel.UpdateAlliesOnMap;
            base.Visit(squad);
            _enemiesGameCharacter = GameCharactersFactory.CreateGameCharacters(Enemies).ToList();
            Panel.Initialize(_battleScenario, _enemiesGameCharacter, squad.Members);//TODO: Store GameCharacters
            Panel.Open();
        }
    }
}