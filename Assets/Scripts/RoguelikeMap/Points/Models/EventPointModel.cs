using System;
using System.Threading.Tasks;
using Events;
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
        
        protected EventPanel Panel => _panel as EventPanel;
        
        public override PointType Type => PointType.Event;
        
        public override async Task Visit(Squad squad)
        {
            await base.Visit(squad);
            Panel.Open(_eventGraph.IsContainsBattle);
            _eventGraph.Process(Panel);
        }
    }
}