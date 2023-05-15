using System;
using System.Collections.Generic;
using System.Linq;
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
        public bool IsContainsBattle => CheckContainsBattle();
        public override void Visit(Squad squad)
        {
            base.Visit(squad);
            Panel.SetEventInfo(_startEventInfo, IsContainsBattle);
            Panel.Open();
        }

        public bool CheckContainsBattle()
        {
            return CheckContainsBattle(_startEventInfo);
        }
        
        private bool CheckContainsBattle(IReadOnlyList<EventInfo> eventInfos)
        {
            return eventInfos.Any(CheckContainsBattle);
        }

        private bool CheckContainsBattle(EventInfo eventInfo)
        {
            if (eventInfo.IsEnd)
                return false;
            if (eventInfo.IsFork)
                return CheckContainsBattle(eventInfo.NextStages);
            
            return eventInfo.NextStage is null ? eventInfo.IsBattle : CheckContainsBattle(eventInfo.NextStage);
        }
    }
}