using System;
using Events;
using OrderElimination.MacroGame;
using RoguelikeMap.SquadInfo;
using RoguelikeMap.UI.PointPanels;
using UnityEngine;

namespace RoguelikeMap.Points.Models
{
    [Serializable]
    public class EventPointModel : PointModel
    {
        [Input] public PointModel entries;
        [Output] public PointModel exits;
        
        [SerializeReference]
        private EventPointGraph _eventGraph;
        [SerializeField]
        private BattleScenario _battleScenario;
        
        protected EventPanel Panel => _panel as EventPanel;
        
        public override PointType Type => PointType.Event;
        public BattleScenario Scenario => _battleScenario;
        
        public override void Visit(Squad squad)
        {
            base.Visit(squad);
            Panel.Open(_eventGraph.IsContainsBattle);
            _eventGraph.Process(Panel);
        }
    }
}