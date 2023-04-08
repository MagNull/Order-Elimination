using OrderElimination;

namespace RoguelikeMap.Points
{
    public class SafeZonePoint : Points.Point
    {
        public override void Visit(Squad squad)
        {
            squad.VisitSafeZonePoint(this);
        }
    }
}