using OrderElimination;
using UnityEngine;

namespace RoguelikeMap.Point
{
    public class SafeZonePoint : OrderElimination.Point
    {
        public override void Visit(Squad squad)
        {
            squad.VisitSafeZonePoint(this);
        }
    }
}