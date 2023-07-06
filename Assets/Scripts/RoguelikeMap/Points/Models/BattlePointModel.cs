using System;
using System.Collections.Generic;
using System.Linq;
using OrderElimination;
using OrderElimination.MacroGame;
using RoguelikeMap.Panels;
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
        
        protected BattlePanel Panel => _panel as BattlePanel;
        
        public override PointType Type => PointType.Battle;
        public IReadOnlyList<IGameCharacterTemplate> Enemies => _enemies;
        public BattleScenario Scenario => _battleScenario;

        public override void Visit(Squad squad)
        {
            base.Visit(squad);
            var enemies = GameCharactersFactory.CreateGameEntities(Enemies).ToList();
            Panel.Initialize(_battleScenario, enemies, squad.Members);//TODO: Store GameCharacters
            Panel.Open();
        }
    }
}