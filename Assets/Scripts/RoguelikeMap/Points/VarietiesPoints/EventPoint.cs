using OrderElimination;

namespace RoguelikeMap.Points
{
    public class EventPoint : Point
    {
        public override void Visit(Squad squad)
        {
            squad.VisitEventPoint(this);
        }
    }
}