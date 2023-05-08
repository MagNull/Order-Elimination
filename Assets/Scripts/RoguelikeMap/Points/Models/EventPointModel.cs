using System;
using RoguelikeMap.Panels;
using RoguelikeMap.SquadInfo;
using UnityEngine;

namespace RoguelikeMap.Points.Models
{
    [Serializable]
    public class EventPointModel : PointModel
    {
        [SerializeReference]
        private EventInfo _startEventInfo;

        protected EventPanel Panel => _panel as EventPanel;
        public override PointType Type => PointType.Event;
        public EventInfo StartEventInfo => _startEventInfo;

        public override void Visit(Squad squad)
        {
            base.Visit(squad);
            Panel.SetEventInfo(_startEventInfo);
            Panel.Open();
        }
    }
}