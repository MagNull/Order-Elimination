using OrderElimination;
using UnityEngine;

namespace RoguelikeMap.Point
{
    public class SafeZonePoint : OrderElimination.Point
    {
        public override void Visit(Squad squad)
        {
            squad.VisitSafeZonePoint(this, 
                new DialogWindowData(WindowFormat.Small, new Vector3(), Vector3.one, PointInfo.Text));
        }
    }
}