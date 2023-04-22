using System;

namespace RoguelikeMap.Points.VarietiesPoints.Infos
{
    [Serializable]
    public class SafeZonePointInfo : VarietiesPoint
    {
        public override PointType PointType => PointType.SafeZone;
    }
}