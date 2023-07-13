using System;
using OrderElimination.MacroGame;
using RoguelikeMap.SquadInfo;
using RoguelikeMap.UI.PointPanels;
using UnityEngine;

namespace RoguelikeMap.Points.Models
{
    [Serializable]
    public class EventPointModel : PointModel
    {
        [SerializeReference]
        private EventPointGraph _eventGraph;
        [SerializeField]
        private BattleScenario _battleScenario;
        //private bool _isContainsBattle => IsContainsBattle();
        
        protected EventPanel Panel => _panel as EventPanel;
        
        public override PointType Type => PointType.Event;
        public BattleScenario Scenario => _battleScenario;
        
        public override void Visit(Squad squad)
        {
            base.Visit(squad);
            Panel.Initialize(_eventGraph);
            Panel.Open();
        }

        // private bool IsContainsBattle()
        // {
        //     return _eventGraph is not null && _eventGraph.IsContainsBattle;
        // }
    }
}