using System;

namespace RoguelikeMap.Points.VarietiesPoints.Infos
{
    [Serializable]
    public class SafeZonePointInfo : VarietiesPointInfo
    {
        public override PointType PointType => PointType.SafeZone;
    }
}