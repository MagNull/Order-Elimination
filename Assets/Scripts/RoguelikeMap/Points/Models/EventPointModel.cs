using System;
using UnityEngine;

namespace RoguelikeMap.Points.Models
{
    [Serializable]
    public class EventPointModel : PointModel
    {
        [SerializeReference]
        private EventInfo _startEventInfo;

        public override PointType Type => PointType.Event;
        public EventInfo StartEventInfo => _startEventInfo;
    }
}