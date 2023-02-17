using OrderElimination;

namespace RoguelikeMap.Point
{
    public class SafeZonePoint : OrderElimination.Point
    {
        public void Visit(Squad squad)
        {
            squad.VisitSafeZonePoint();
        }
    }
}