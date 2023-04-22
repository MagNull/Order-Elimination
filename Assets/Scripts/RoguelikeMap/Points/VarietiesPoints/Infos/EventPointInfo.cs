using System;
using UnityEngine;

namespace RoguelikeMap.Points.VarietiesPoints.Infos
{
    [Serializable]
    public class EventPointInfo : VarietiesPoint
    {
        [SerializeReference]
        private EventInfo _startEventInfo;

        public override PointType PointType => PointType.Event;
        public EventInfo StartEventInfo => _startEventInfo;
    }
}