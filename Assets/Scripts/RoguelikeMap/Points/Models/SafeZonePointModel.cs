using System;

namespace RoguelikeMap.Points.Models
{
    [Serializable]
    public class SafeZonePointModel : PointModel
    {
        public override PointType Type => PointType.SafeZone;
    }
}